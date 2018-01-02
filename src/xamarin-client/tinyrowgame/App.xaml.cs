using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace tinyrowgame
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new tinyrowgamePage();

            Task.Run(async () => await Service.SocketService.StartListening(new Uri("ws://fw.knatofs.se:8001")));

        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
