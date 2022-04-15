using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace task3
{
    public class MessageListEntry
    {
        public MessageListEntry(ChatMessage message, Color color)
        {
            UserName = message.Name;
            MessageText = message.Text;
            Color = color;
        }
        public string UserName { get; private set; }
        public string MessageText { get; private set; }
        public Color Color { get; private set; }
    }
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            InitializeComponent();
            MessagesList.ItemsSource = messages;
            ChatClient.Instance.Subscribe(OnMessageReceived);
        }

        protected override void OnDisappearing()
        {
            ChatClient.Instance.Unsubscribe();
            base.OnDisappearing();
        }

        private async void RequestSend(string message)
        {
            ServerStatus result = await ChatClient.Instance.SendMessage(messageEditor.Text);
            messageEditor.Text = string.Empty;

            switch (result)
            {
                case ServerStatus.ERROR_UNKNOWN:
                    await DisplayAlert("Error", "Sending has failed!", "Close");
                    break;
                default:
                    break;
            }
        }

        private void OnMessageReceived(ChatMessage message)
        {
            Color color = Color.FromRgb(random.Next(256), random.Next(256), random.Next(256));
            if (usersMap.ContainsKey(message.Name))
            {
                color = usersMap[message.Name];
            }
            else
            {
                usersMap[message.Name] = color;
            }

            messages.Insert(0, new MessageListEntry(message, color));
        }

        private void OnButtonSendClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(messageEditor.Text))
            {
                return;
            }

            RequestSend(messageEditor.Text);
        }

        private Random random = new Random();
        private Dictionary<string, Color> usersMap = new Dictionary<string, Color>();
        private ObservableCollection<MessageListEntry> messages = new ObservableCollection<MessageListEntry>();
    }
}
