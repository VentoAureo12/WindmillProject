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
    /// Логика взаимодействия для ManagerOrderDetailsWindow.xaml
    /// </summary>
    public partial class ManagerOrderDetailsWindow : Window
    {
        public ManagerOrderDetailsWindow(Заказ selectedOrder)
        {
            if (selectedOrder== null)
            {
                MessageBox.Show("Данное окно не работает по причине отсутствия данных");
                return;
            }
            InitializeComponent();
            OrdersData.ItemsSource = ElectroShopBDEntities.GetContext().Заказ_Товар.Where(u => u.ID_заказа == selectedOrder.ID).ToList();

        }

        private void NameSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
