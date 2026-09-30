using System;
using System.Configuration;
using System.Windows;
using WpfApp12.Data;
using WpfApp12.Services;
using WpfApp12.ViewModels;

namespace WpfApp12
{
    /// <summary>Точка входа и «корень композиции»: здесь собираются все слои.</summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var connectionString = ConfigurationManager.ConnectionStrings["LibraryDb"].ConnectionString;
            Func<AppDbContext> contextFactory = () => new AppDbContext(connectionString);

            try
            {
                using (var db = contextFactory())
                {
                    await db.Database.EnsureCreatedAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к базе данных:\n" + ex.Message,
                    "LibraryManager", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            var repository = new BookRepository(contextFactory);
            var service = new LibraryService(repository);
            var viewModel = new MainViewModel(service);
            var window = new Views.MainWindow { DataContext = viewModel };

            viewModel.ShowEditDialog = editVm =>
            {
                var dialog = new Views.BookEditView { DataContext = editVm, Owner = window };
                editVm.CloseRequested += result => dialog.DialogResult = result;
                return dialog.ShowDialog();
            };
            viewModel.Confirm = message =>
                MessageBox.Show(window, message, "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

            MainWindow = window;
            window.Show();
            await viewModel.RefreshAsync();
        }
    }
}
