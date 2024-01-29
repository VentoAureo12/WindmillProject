using System;
using System.Collections.Generic;
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
using TestApp.ManagerWindows;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ManagerWindow()
        {
            InitializeComponent();
            ClientDataGrid.ItemsSource = ElectroShopBDEntities.GetContext().Пользователь.ToList().Where(u => u.Роль == 1);
        }

        private void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void NameSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameSearchField.Text != null)
            {
                try
                {
                    var DataSource = ElectroShopBDEntities.GetContext().Пользователь.Where
                        (user => user.Фамилия.ToLower().Contains(NameSearchField.Text) ||
                        user.Имя.ToLower().Contains(NameSearchField.Text) ||
                        user.Отчество.ToLower().Contains(NameSearchField.Text)).ToList();

                    ClientDataGrid.ItemsSource = DataSource.Where(u => u.Роль == 1).ToList();
                }
                catch
                {

                }
            }
        }

        private void ClientDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientDataGrid.SelectedItem != null)
            {
                Пользователь selectedUser = (Пользователь)ClientDataGrid.SelectedItem;
                ManagerOrdersWindow userOrdersWindow = new ManagerOrdersWindow(selectedUser);
                userOrdersWindow.Show();
            }
        }
    }
}
