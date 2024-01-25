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

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для BasketView.xaml
    /// </summary>
    public partial class BasketView : Window
    {
        private List<Товар> basketItems = new List<Товар>();
        public BasketView()
        {
            InitializeComponent();
            DeliveryInput.ItemsSource = ElectroShopBDEntities.GetContext().Пункт_Выдачи.ToList();
            basketItems = new List<Товар>();
            foreach (int id in BasketClass.getBasket())
            {
                basketItems.Add(ElectroShopBDEntities.GetContext().Товар.Find(id));
            }
            BasketListView.ItemsSource = basketItems;
            updateResultSum();
        }
        private void updateResultSum()
        {
            resultSum.Content = $"Итого:{basketItems.Sum(product => product.Цена)}";

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.ComponentModel.IEditableCollectionView items = BasketListView.Items; //Cast to interface

            Товар selectedItem = BasketListView.SelectedItems[0] as Товар; // cast item to product

            BasketClass.Delete((int)selectedItem.ID); // remove product from basket


            items.Remove(BasketListView.SelectedItem); // remove product from listView



            updateResultSum();
        }

        private void SnapBackButton_Click(object sender, RoutedEventArgs e)
        {
            UserWindow window = new UserWindow();
            window.Show();
            this.Close();
        }

        private void MakeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show($"Вы точно хотите оформить заказ", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int orderID = (int)ElectroShopBDEntities.GetContext().Заказ.Max(x => x.ID);
                if (orderID == 0)
                {
                    orderID = 1;
                }
                else
                {
                    orderID++;
                }

                foreach (int id in BasketClass.getBasket())
                {
                    Заказ orderData = new Заказ();

                    Заказ_Товар orderItemData = new Заказ_Товар();

                    Товар productID = new Товар();


                    int deliveryID = int.Parse(DeliveryInput.SelectedValue.ToString());

                    orderData.ID = orderID;
                    orderData.ID_пользователя = DataTransfer.userID;
                    orderData.ID_пункта_выдачи = deliveryID;

                    productID = ElectroShopBDEntities.GetContext().Товар.Where(u => u.ID == id).FirstOrDefault();

                    orderItemData.ID_товара = productID.ID;
                    orderItemData.ID_заказа = orderID;

                    ElectroShopBDEntities.GetContext().Заказ.Add(orderData);
                    ElectroShopBDEntities.GetContext().Заказ_Товар.Add(orderItemData);
                }
                try
                {
                    ElectroShopBDEntities.GetContext().SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                MessageBox.Show("Заказ оформлен!");
            }
        }
    }
}
