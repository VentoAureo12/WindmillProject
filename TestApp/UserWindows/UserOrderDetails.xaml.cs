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

namespace TestApp.UserWindows
{
    /// <summary>
    /// Логика взаимодействия для UserOrderDetails.xaml
    /// </summary>
    public partial class UserOrderDetails : Window
    {
        public UserOrderDetails(Заказ selectedOrder)
        {
            InitializeComponent();
            userOrders.ItemsSource = ElectroShopBDEntities.GetContext().Заказ_Товар.Where(u => u.ID_заказа == selectedOrder.ID).ToList();
        }

        private void SnapBackButton_Click(object sender, RoutedEventArgs e)
        {
            UserOrdersWindow window = new UserOrdersWindow();
            window.Show();
            this.Close();
        }
    }
}
