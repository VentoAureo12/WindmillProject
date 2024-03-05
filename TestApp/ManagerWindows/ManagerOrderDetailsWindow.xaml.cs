using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestApp.ManagerWindows
{
    /// <summary>
    /// Класс окна для отображения и редактирования деталей заказа менеджера
    /// </summary>
    public partial class ManagerOrderDetailsWindow : Window
    {
        private bool isDirty = true; // Флаг изменений в данных
        List<Заказ_Товар> itemSource; // Источник данных для DataGrid

        /// <summary>
        /// Конструктор окна, принимающий выбранный заказ
        /// </summary>
        /// <param name="selectedOrder">Выбранный заказ</param>
        public ManagerOrderDetailsWindow(Заказ selectedOrder)
        {
            if (selectedOrder == null)
            {
                MessageBox.Show("Данное окно не работает по причине отсутствия данных");
                return;
            }
            InitializeComponent();

            // Инициализация источника данных для DataGrid
            itemSource = ElectroShopBDEntities.GetContext().Заказ_Товар.Where(u => u.ID_заказа == selectedOrder.ID).ToList();
            OrdersData.ItemsSource = itemSource;
        }

        /// <summary>
        /// Обработчик события изменения текста в поле поиска
        /// </summary>
        private void NameSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Реализация поиска (отсутствует в данном коде)
        }

        /// <summary>
        /// Обработчик выполнения команды редактирования
        /// </summary>
        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OrdersData.IsReadOnly = false;
            OrdersData.BeginEdit();
            isDirty = false;
        }

        /// <summary>
        /// Обработчик проверки возможности выполнения команды редактирования
        /// </summary>
        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        /// <summary>
        /// Обработчик проверки возможности выполнения команды сохранения
        /// </summary>
        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

        /// <summary>
        /// Обработчик выполнения команды сохранения
        /// </summary>
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ElectroShopBDEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            isDirty = true;
            OrdersData.IsReadOnly = true;
        }

        /// <summary>
        /// Обработчик проверки возможности выполнения команды отмены
        /// </summary>
        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

        /// <summary>
        /// Обработчик выполнения команды отмены
        /// </summary>
        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var context = ElectroShopBDEntities.GetContext();
            var changedEntries = context.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.CurrentValues.SetValues(entry.OriginalValues);
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
            OrdersData.ItemsSource = null;
            OrdersData.ItemsSource = itemSource;
            MessageBox.Show("Отмена действия");
            isDirty = true;
            OrdersData.IsReadOnly = true;
        }

        /// <summary>
        /// Обработчик проверки возможности выполнения команды обновления
        /// </summary>
        private void Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        /// <summary>
        /// Обработчик выполнения команды обновления
        /// </summary>
        private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Обновление данных в DataGrid (в данном коде используется неверный источник данных)
            OrdersData.ItemsSource = null;
            OrdersData.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();
            isDirty = false;
        }
    }
}
