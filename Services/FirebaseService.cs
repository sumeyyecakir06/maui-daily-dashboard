using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Query;
using odev3son.Models;
using System.Collections.ObjectModel;

namespace odev3son.Services;

public static class FirebaseService
{
    // Firebase yapılandırması
    private static readonly string ApiKey = "AIzaSyAN-nPwUiDNfbckeSn1jIHqCSEqoLyNyBM";
    private static readonly string AuthDomain = "sumeyyecakir23010310021.firebaseapp.com";
    private static readonly string DatabaseUrl = "https://sumeyyecakir23010310021-default-rtdb.firebaseio.com/";

    private static FirebaseAuthConfig _config = new()
    {
        ApiKey = ApiKey,
        AuthDomain = AuthDomain,
        Providers = new FirebaseAuthProvider[]
        {
            new EmailProvider()
        }
    };

    private static FirebaseAuthClient _authClient = new(_config);
    private static FirebaseClient _firebaseClient = new(DatabaseUrl);

    // Kayıt olma
    public static async Task<(bool Success, string Message)> RegisterAsync(string username, string email, string password)
    {
        try
        {
            //---------------------------------------------
            var result = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password, username);

            if (result?.User != null)
            {
                // Kullanıcı bilgilerini kaydet
                Preferences.Default.Set("UserEmail", email);
                Preferences.Default.Set("UserId", result.User.Uid);
                Preferences.Default.Set("Username", username);
                return (true, "Kayıt başarılı!");
            }

            return (false, "Kayıt başarısız.");
        }
        catch (FirebaseAuthHttpException ex)
        {
            return (false, $"Firebase hatası: {ex.Reason}");
        }
        catch (Exception ex)
        {
            return (false, $"Hata: {ex.Message}");
        }
    }

    // Giriş yapma
    public static async Task<(bool Success, string Message)> LoginAsync(string email, string password)
    {
        try
        {
            //------------------------------------
            var result = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

            if (result?.User != null)
            {
                // Kullanıcı bilgilerini kaydet
                Preferences.Default.Set("UserEmail", email);
                Preferences.Default.Set("UserId", result.User.Uid);
                Preferences.Default.Set("IsLoggedIn", true);
                return (true, "Giriş başarılı!");
            }

            return (false, "Giriş başarısız.");
        }
        catch (FirebaseAuthHttpException ex)
        {
            return (false, $"Firebase hatası: {ex.Reason}");
        }
        catch (Exception ex)
        {
            return (false, $"Hata: {ex.Message}");
        }
    }

    // Çıkış yapma
    public static void Logout()
    {
        Preferences.Default.Remove("UserEmail");
        Preferences.Default.Remove("UserId");
        Preferences.Default.Remove("IsLoggedIn");
        Preferences.Default.Remove("Username");
    }

    // Todo işlemleri - USER BAZLI
    public static async Task<(bool Success, string Message)> AddTodoAsync(ToDoItem item)
    {
        try
        {
            var userId = Preferences.Default.Get("UserId", string.Empty);
            if (string.IsNullOrEmpty(userId))
                return (false, "Kullanıcı oturumu bulunamadı.");

            if (string.IsNullOrEmpty(item.Id))
                item.Id = Guid.NewGuid().ToString();

            await _firebaseClient
                .Child("todos")
                .Child(userId)
                .Child(item.Id)
                .PutAsync(item);

            return (true, "Görev başarıyla eklendi.");
        }
        catch (Exception ex)
        {
            return (false, $"Hata: {ex.Message}");
        }
    }

    public static async Task<(bool Success, string Message)> UpdateTodoAsync(ToDoItem item)
    {
        try
        {
            //-----------------
            var userId = Preferences.Default.Get("UserId", string.Empty);
            if (string.IsNullOrEmpty(userId))
                return (false, "Kullanıcı oturumu bulunamadı.");

            await _firebaseClient
                .Child("todos")
                .Child(userId)
                .Child(item.Id)
                .PutAsync(item);

            return (true, "Görev başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return (false, $"Hata: {ex.Message}");
        }
    }

    public static async Task<(bool Success, string Message)> DeleteTodoAsync(string todoId)
    {
        try
        {
            var userId = Preferences.Default.Get("UserId", string.Empty);
            if (string.IsNullOrEmpty(userId))
                return (false, "Kullanıcı oturumu bulunamadı.");

            await _firebaseClient
                .Child("todos")
                .Child(userId)
                .Child(todoId)
                .DeleteAsync();

            return (true, "Görev başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return (false, $"Hata: {ex.Message}");
        }
    }

    public static async Task<ObservableCollection<ToDoItem>> GetTodosAsync()
    {
        var todos = new ObservableCollection<ToDoItem>();

        try
        {
            var userId = Preferences.Default.Get("UserId", string.Empty);
            if (string.IsNullOrEmpty(userId))
                return todos;

            var items = await _firebaseClient
                .Child("todos")
                .Child(userId)
                .OnceAsync<ToDoItem>();

            foreach (var item in items)
            {
                if (item.Object != null)
                {
                    item.Object.Id = item.Key;
                    todos.Add(item.Object);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Todo yükleme hatası: {ex.Message}");
        }

        return todos;
    }
}