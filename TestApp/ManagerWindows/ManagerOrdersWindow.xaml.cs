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
    /// Логика взаимодействия для ManagerOrdersWindow.xaml
    /// </summary>
    public partial class ManagerOrdersWindow : Window
    {
        private bool isDirty = true;
        List<Заказ> itemSource;
        public ManagerOrdersWindow(Пользователь selectedUser)
        {
            if(selectedUser == null)
            {
                MessageBox.Show("Данное окно не работает по причине отсутствия данных");
                return;
            }
            InitializeComponent();
            OrdersData.ItemsSource = ElectroShopBDEntities.GetContext().Заказ.Where(u => u.ID_пользователя == selectedUser.ID).ToList();
            itemSource = ElectroShopBDEntities.GetContext().Заказ.Where(u => u.ID_пользователя == selectedUser.ID).ToList();
        }

        private void NameSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            OrdersData.ItemsSource = ElectroShopBDEntities.GetContext().Заказ.Where(u => u.ID.ToString().Contains(NameSearchField.Text)).ToList();
        }

        private void OrdersData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrdersData.SelectedItem != null)
            {
                Заказ selectedOrder = (Заказ)OrdersData.SelectedItem;
                ManagerOrderDetailsWindow detailWindow = new ManagerOrderDetailsWindow(selectedOrder);
                detailWindow.Show();
            }
        }


        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OrdersData.IsReadOnly = false;
            OrdersData.BeginEdit();
            isDirty = false;
        }

        private void Edit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

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

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isDirty;
        }

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
            OrdersData.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();
            MessageBox.Show("Отмена действия");
            isDirty = true;
            OrdersData.IsReadOnly = true;
        }
        private void Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OrdersData.ItemsSource = null;
            OrdersData.ItemsSource = itemSource;
            isDirty = false;
        }
    }
}
