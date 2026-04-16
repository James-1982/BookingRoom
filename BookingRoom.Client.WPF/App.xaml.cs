using BookingRoom.Client.WPF.Services;
using BookingRoom.Client.WPF.Services.Themes;
using BookingRoom.Client.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace BookingRoom.Client.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        public App()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            Services = services.BuildServiceProvider();

            InitializeComponent();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Usa la tua classe Client
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ITimeService, TimeService>();
            services.AddSingleton(new BookingClient("127.0.0.1", 5000));
            services.AddSingleton<IBookingService, BookingService>();
            // Registrazione MainWindow con DI
            services.AddSingleton<MainViewModel>();
            services.AddTransient<MainWindow>(provider =>
            {
                var vm = provider.GetRequiredService<MainViewModel>();
                return new MainWindow { DataContext = vm };
            });
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            if (mainWindow.DataContext is MainViewModel vm)
                await vm.LoadRoomsAsync();
        }
    }
}