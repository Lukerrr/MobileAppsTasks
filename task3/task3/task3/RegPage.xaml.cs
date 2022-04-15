using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace task3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegPage : ContentPage
    {
        public RegPage()
        {
            InitializeComponent();
        }

        private async void RequestRegister(string username, string password)
        {
            ServerStatus result = await ChatClient.Instance.Register(username, password);

            switch (result)
            {
                case ServerStatus.SUCCESS:
                    await DisplayAlert("Congratulations!", "Registration successful", "Login");
                    await Navigation.PopAsync();
                    break;
                case ServerStatus.ERROR_REG_LOGIN_EXISTS:
                    await DisplayAlert("Error", "Login already exists", "Close");
                    break;
                case ServerStatus.ERROR_REG_BAD_LOGIN:
                    await DisplayAlert("Error", "Bad login or password", "Close");
                    break;
                case ServerStatus.ERROR_UNKNOWN:
                    await DisplayAlert("Error", "Unknown error", "Close");
                    break;
                default:
                    break;
            }
        }

        private void OnButtonConfirmClicked(object sender, EventArgs e)
        {
            if (usernameEntry.Text == null || usernameEntry.Text.Length == 0)
            {
                DisplayAlert("Error", "Please enter Username", "Close");
                return;
            }

            if (passwordEntry.Text == null || passwordEntry.Text.Length == 0)
            {
                DisplayAlert("Error", "Please enter Password", "Close");
                return;
            }

            if (passwordEntry.Text != passwordCheckEntry.Text)
            {
                DisplayAlert("Error", "Passwords do not match!", "Close");
                return;
            }

            RequestRegister(usernameEntry.Text, passwordEntry.Text);
        }
    }
}