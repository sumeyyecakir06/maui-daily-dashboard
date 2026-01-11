using odev3son.Services;
using Microsoft.Maui.Controls;

namespace odev3son;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var isEmailValid = !string.IsNullOrWhiteSpace(EmailEntry.Text) &&
                          EmailEntry.Text.Contains("@");
        var isPasswordValid = !string.IsNullOrWhiteSpace(PasswordEntry.Text) &&
                             PasswordEntry.Text.Length >= 6;

        LoginButton.IsEnabled = isEmailValid && isPasswordValid;
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        try
        {
            LoginButton.IsEnabled = false;
            var email = EmailEntry.Text?.Trim() ?? "";
            var password = PasswordEntry.Text ?? "";

            var (success, message) = await FirebaseService.LoginAsync(email, password);

            if (success)
            {
                Preferences.Default.Set("IsLoggedIn", true);
                Application.Current!.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("Giriþ Baþarýsýz", message, "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Giriþ sýrasýnda hata: {ex.Message}", "Tamam");
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }

    private async void CancelButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

    private async void OnGotoLoginPageButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}