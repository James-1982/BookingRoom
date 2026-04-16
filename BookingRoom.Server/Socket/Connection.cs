using BookingRoom.Server.BusinessLogic;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace BookingRoom.Server.Socket;

public class Connection
{
    private readonly ConcurrentQueue<byte[]> _sendQueue = new();
    private readonly CommandHandler _handler;
    private bool _sending = false;

    public System.Net.Sockets.Socket Socket { get; private set; }
    public byte[] Buffer { get; private set; }
    public int BytesBuffered { get; set; }
    public SocketAsyncEventArgs EventArgs { get; private set; }
    public DateTime LastActivity { get; private set; }

    public Connection(int bufferSize, CommandHandler handler)
    {
        _handler = handler;
        Buffer = new byte[bufferSize];
        EventArgs = new SocketAsyncEventArgs();
        EventArgs.SetBuffer(new byte[bufferSize], 0, bufferSize);
        EventArgs.Completed += OnIOCompleted;
        BytesBuffered = 0;
        LastActivity = DateTime.UtcNow;
    }

    public void Attach(System.Net.Sockets.Socket socket)
    {
        Socket = socket;
        StartReceive();
    }

    public void CloseConnection()
    {
        if (Socket == null)
            return;

        try
        {
            if (Socket.Connected)
                Socket.Shutdown(SocketShutdown.Both);
        }
        catch { /* ignora eccezioni su socket già chiuso */ }

        try
        {
            Socket.Close();
        }
        catch { }

        Console.WriteLine("Connection closed");
    }

    private void StartReceive()
    {
        EventArgs.SetBuffer(BytesBuffered, Buffer.Length - BytesBuffered);

        bool willRaiseEvent = Socket.ReceiveAsync(EventArgs);
        if (!willRaiseEvent)
        {
            // evita ricorsione immediata
            Task.Run(() => ProcessReceiveAsync());
        }
    }

    private async void OnIOCompleted(object sender, SocketAsyncEventArgs e)
    {
        await ProcessReceiveAsync();
    }

    private async Task ProcessReceiveAsync()
    {
        int bytesRead = EventArgs.BytesTransferred;
        if (bytesRead > 0)
        {
            LastActivity = DateTime.UtcNow;
            Array.Copy(EventArgs.Buffer, EventArgs.Offset, Buffer, BytesBuffered, bytesRead);
            BytesBuffered += bytesRead;

            int offset = 0;

            while (BytesBuffered - offset >= 4) // almeno 4 byte per length
            {
                int messageLength = BitConverter.ToInt32(Buffer, offset);
                if (BytesBuffered - offset - 4 < messageLength)
                    break; // messaggio incompleto

                byte[] payload = new byte[messageLength];
                Array.Copy(Buffer, offset + 4, payload, 0, messageLength);

                await _handler.HandleMessage(this, payload);

                offset += 4 + messageLength;
            }

            // Mantieni frammento incompleto
            int remaining = BytesBuffered - offset;
            if (remaining > 0)
                Array.Copy(Buffer, offset, Buffer, 0, remaining);

            BytesBuffered = remaining;
        }

        // Rilancia receive
        StartReceive();
    }

    public void EnqueueSend(byte[] data)
    {
        _sendQueue.Enqueue(data);

        // Se non stiamo già inviando, avvia send
        if (!_sending)
        {
            _sending = true;
            StartSend();
        }
    }

    private void StartSend()
    {
        if (_sendQueue.TryDequeue(out var data))
        {
            var sendEvent = new SocketAsyncEventArgs();
            sendEvent.SetBuffer(data, 0, data.Length);
            sendEvent.Completed += (s, e) => OnSendCompleted(e);

            bool willRaiseEvent = Socket.SendAsync(sendEvent);
            if (!willRaiseEvent)
            {
                OnSendCompleted(sendEvent);
            }
        }
        else
        {
            _sending = false; // coda vuota, stop invio
        }
    }

    private void OnSendCompleted(SocketAsyncEventArgs e)
    {
        LastActivity = DateTime.UtcNow;
        e.Dispose(); // rilascia EventArgs temporaneo
        StartSend(); // invia prossimo pacchetto se presente
    }
}