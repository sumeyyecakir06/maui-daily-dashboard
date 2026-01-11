using odev3son.Models;
using odev3son.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace odev3son;

public partial class AddToDoPage : ContentPage, INotifyPropertyChanged
{
    private ToDoItem _todo;
    private bool _isEditing;

    public ToDoItem Todo
    {
        get => _todo;
        set
        {
            _todo = value;
            OnPropertyChanged();
        }
    }

    public bool IsNotNew => _isEditing;

    public AddToDoPage(ToDoItem? item = null)
    {
        InitializeComponent();

        if (item == null)
        {
            Todo = new ToDoItem
            {
                Date = DateTime.Now,
                Time = DateTime.Now.TimeOfDay
            };
            _isEditing = false;
        }
        else
        {
            Todo = item;
            _isEditing = true;
        }

        BindingContext = this;
    }

    //  KAYDET butonu için event handler
    private async void OnSaveTodoClicked(object sender, EventArgs e)
    {
        // Validasyon
        if (string.IsNullOrWhiteSpace(Todo.Title))
        {
            await DisplayAlert("Hata", "Başlık boş olamaz.", "Tamam");
            return;
        }

        // Butonu devre dışı bırak
        if (sender is Button btn) btn.IsEnabled = false;

        try
        {
            var (ok, msg) = _isEditing
                ? await FirebaseService.UpdateTodoAsync(Todo)
                : await FirebaseService.AddTodoAsync(Todo);

            if (ok)
            {
                await DisplayAlert("Başarılı", msg, "Tamam");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Hata", msg, "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            // Butonu tekrar aktif et
            if (sender is Button btn2) btn2.IsEnabled = true;
        }
    }

    //  SİL butonu için event handler
    private async void OnDeleteTodoClicked(object sender, EventArgs e)
    {
        // Silme onayı
        bool answer = await DisplayAlert("Silme Onayı",
            "Bu görevi silmek istediğinizden emin misiniz?",
            "Evet", "Hayır");

        if (!answer)
            return;

        // Butonu devre dışı bırak
        if (sender is Button btn) btn.IsEnabled = false;

        try
        {
            var (ok, msg) = await FirebaseService.DeleteTodoAsync(Todo.Id);

            if (ok)
            {
                await DisplayAlert("Başarılı", msg, "Tamam");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Hata", msg, "Tamam");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
        }
        finally
        {
            // Butonu tekrar aktif et
            if (sender is Button btn2) btn2.IsEnabled = true;
        }
    }

    // INotifyPropertyChanged implementasyonu
    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}