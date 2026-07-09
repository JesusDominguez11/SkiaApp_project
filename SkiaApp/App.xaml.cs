namespace SkiaApp
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();

            _services = services;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var page = _services.GetRequiredService<DashboardPage>();

            return new Window(page) { Title = "SkiaApp" };
        }
    }
}
