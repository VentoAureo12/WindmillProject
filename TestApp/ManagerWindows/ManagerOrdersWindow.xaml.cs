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

namespace TestApp.ManagerWindows
{
    /// <summary>
    /// Логика взаимодействия для ManagerOrdersWindow.xaml
    /// </summary>
    public partial class ManagerOrdersWindow : Window
    {
        public ManagerOrdersWindow(Пользователь selectedUser)
        {
            if(selectedUser == null)
            {
                MessageBox.Show("Данное окно не работает по причине отсутствия данных");
                return;
            }
            InitializeComponent();
            OrdersData.ItemsSource = ElectroShopBDEntities.GetContext().Заказ.Where(u => u.ID_пользователя == selectedUser.ID).ToList();
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
    }
}
