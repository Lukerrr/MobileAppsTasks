using Xamarin.Forms;

namespace task3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            ChatClient.Instance.Initialize();
            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
