using System;
using tinyrowgame.SocketCommands;
using TinyWebSockets;
using TinyWebSockets.Interfaces;
using Xamarin.Forms;

namespace tinyrowgame
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class tinyrowgamePageContext : IMessageReceiver
    {
        private MessageHandler _stateService;

        public bool IsActive => true;

        public void HandleAction(IMessage action)
        {
            LastMessage = "Got message";
            switch(action) {
                case Init init:
                    LastMessage = "UserNr:"+init.UserData.Nr;
                    break;
            }
           
        }

        public string LastMessage
        {
            get;
            set;
        } = "Tiny row";

        public void SetStateService(MessageHandler stateService)
        {
            this._stateService = stateService;
        }

        internal void Appear()
        {
            Service.MessageHandler.PopulateActions(this);
            Service.MessageHandler.RegisterActionReceiver(this);
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
