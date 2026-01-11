using odev3son.Services;

namespace odev3son;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var isUsernameValid = !string.IsNullOrWhiteSpace(UsernameEntry.Text) &&
                             UsernameEntry.Text.Length >= 3;
        var isEmailValid = !string.IsNullOrWhiteSpace(EmailEntry.Text) &&
                          EmailEntry.Text.Contains("@");
        var isPasswordValid = !string.IsNullOrWhiteSpace(PasswordEntry.Text) &&
                             PasswordEntry.Text.Length >= 6;

        RegisterButton.IsEnabled = isUsernameValid && isEmailValid && isPasswordValid;
    }

    private async void CancelButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnGotoLoginPageButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text?.Trim() ?? "";
        var email = EmailEntry.Text?.Trim() ?? "";
        var pass = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(pass))
        {
            await DisplayAlert("Kayıt başarısız",
                "Kullanıcı adı, email ve şifre boş olamaz.", "Tamam");
            return;
        }

        if (pass.Length < 6)
        {
            await DisplayAlert("Kayıt başarısız",
                "Şifre en az 6 karakter olmalıdır.", "Tamam");
            return;
        }

        RegisterButton.IsEnabled = false;

        try
        {
            var (ok, message) = await FirebaseService.RegisterAsync(username, email, pass);

            if (ok)
            {
                await DisplayAlert("Kayıt başarılı", "Hesabınız oluşturuldu.", "Tamam");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Kayıt başarısız", message, "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Kayıt sırasında hata: {ex.Message}", "Tamam");
        }
        finally
        {
            RegisterButton.IsEnabled = true;
        }
    }
}