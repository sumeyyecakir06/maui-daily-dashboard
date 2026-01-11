using odev3son.Models;
using odev3son.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace odev3son;

public partial class TodoPage : ContentPage
{
    public ObservableCollection<ToDoItem> TodoList { get; } = new();
    public ICommand RefreshCommand { get; }

    private bool _isRefreshing;

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            if (_isRefreshing != value)
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
    }

    public TodoPage()
    {
        InitializeComponent();

        RefreshCommand = new Command(async () => await RefreshTodos());

        BindingContext = this;

        // LoadTodos'u async method olarak deðiþtirdik
        Task.Run(async () => await LoadTodosAsync());
    }

    // LoadTodosAsync olarak deðiþtirildi - async Task döndürüyor
    private async Task LoadTodosAsync()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() => IsRefreshing = true);
            var todos = await FirebaseService.GetTodosAsync();

            // Tarihe göre sýrala (yakýn tarihler önce)
            var sortedTodos = todos
                .OrderBy(t => t.Date)
                .ThenBy(t => t.Time)
                .ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TodoList.Clear();
                foreach (var todo in sortedTodos)
                {
                    TodoList.Add(todo);
                }
                IsRefreshing = false;
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Hata", $"Görevler yüklenemedi: {ex.Message}", "Tamam");
                IsRefreshing = false;
            });
        }
    }

    private async Task RefreshTodos()
    {
        await LoadTodosAsync();
    }

    private async void AddNewTodo_Clicked(object sender, EventArgs e)
    {
        // Yeni görev sayfasýna git
        await Navigation.PushAsync(new AddToDoPage());
    }

    private async void OnTodoSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ToDoItem selectedTodo)
        {
            // Seçimi temizle
            if (sender is CollectionView collectionView)
                collectionView.SelectedItem = null;

            // Düzenleme sayfasýna git
            await Navigation.PushAsync(new AddToDoPage(selectedTodo));
        }
    }

    private async void OnCheckBoxChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is ToDoItem todo)
        {
            try
            {
                // Firebase'de güncelle
                await FirebaseService.UpdateTodoAsync(todo);

                // UI'yý güncelle
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnPropertyChanged(nameof(TodoList));
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Güncelleme hatasý: {ex.Message}", "Tamam");
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Sayfa her göründüðünde listeyi yenile
        Task.Run(async () => await LoadTodosAsync());
    }
}