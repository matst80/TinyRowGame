using System;
using tinyrowgame.SocketCommands;
using TinyWebSockets;
using TinyWebSockets.Interfaces;
using Xamarin.Forms;

namespace tinyrowgame
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class tinyrowgamePageContext 
    {
        internal void Appear()
        {
            Service.MessageHandler.PopulateActions(this);
        }
    }

    public partial class tinyrowgamePage : ContentPage
    {
        private tinyrowgamePageContext ContextModel = new tinyrowgamePageContext();

        public tinyrowgamePage()
        {
            BindingContext = ContextModel;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ContextModel.Appear();
        }
    }
}
