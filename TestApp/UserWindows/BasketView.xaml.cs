using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TestApp.Classes;
using TestApp;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для BasketView.xaml
    /// </summary>
    public partial class BasketView : Window
    {
        private List<BasketItemViewModel> BasketListViewItems;
        private Dictionary<Товар, int> basketItemsCount;
        int price;
        public BasketView()
        {
            InitializeComponent();
            DeliveryInput.ItemsSource = ElectroShopBDEntities.GetContext().Пункт_Выдачи.ToList();
            BasketListViewItems = new List<BasketItemViewModel>();
            basketItemsCount = new Dictionary<Товар, int>();
            foreach (int id in BasketClass.getBasket())
            {
                Товар product = ElectroShopBDEntities.GetContext().Товар.Find(id);
                if (product != null)
                {
                    if (basketItemsCount.ContainsKey(product))
                    {
                        basketItemsCount[product]++;
                    }
                    else
                    {
                        basketItemsCount.Add(product, 1);
                    }
                }
            }
            BasketListView.ItemsSource = basketItemsCount.Select(pair => new BasketItemViewModel
            {
                Изображение = pair.Key.Изображение,
                Наименование = pair.Key.Наименование,
                Цена = (int)pair.Key.Цена,
                Product = pair.Key.ID,
                Quantity = pair.Value
            });
            updateResultSum();
        }

        private void updateResultSum()
        {
            basketItemsCount.Clear();

            foreach (var basketItem in BasketListView.Items)
            {
                if (basketItem is BasketItemViewModel item)
                {
                    basketItemsCount[item.Product] = item.Quantity;
                }
            }

            int totalSum = 0;

            foreach (var pair in basketItemsCount)
            {
                int productId = pair.Key;
                int quantity = pair.Value;

                Товар product = ElectroShopBDEntities.GetContext().Товар.Find(productId);

                if (product != null)
                {
                    totalSum += quantity * product.Цена;
                }
            }

            resultSum.Content = $"Итого: {totalSum} руб";
            price = (int)basketItemsCount.Sum(pair => pair.Value * pair.Key.Цена);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.ComponentModel.IEditableCollectionView items = BasketListView.Items; //Cast to interface

            Товар selectedItem = BasketListView.SelectedItems[0] as Товар; // cast item to product

            BasketClass.Delete((int)selectedItem.ID); // remove product from basket


            items.Remove(BasketListView.SelectedItem); // remove product from listView



            updateResultSum();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                BasketItemViewModel selectedItem = button.DataContext as BasketItemViewModel;
                if (selectedItem != null)
                {
                    selectedItem.Quantity++;
                    updateResultSum();
                }
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                BasketItemViewModel selectedItem = button.DataContext as BasketItemViewModel;
                if (selectedItem != null && selectedItem.Quantity > 1)
                {
                    selectedItem.Quantity--;
                    updateResultSum();
                }
            }
        }
        private void SnapBackButton_Click(object sender, RoutedEventArgs e)
        {
            UserWindow window = new UserWindow();
            window.Show();
            this.Close();
        }
        
        private void MakeOrderButton_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show($"Вы точно хотите оформить заказ", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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



                Заказ orderData = new Заказ();

                int deliveryID = int.Parse(DeliveryInput.SelectedValue.ToString());

                orderData.ID = orderID;
                orderData.ID_пользователя = DataTransfer.userID;
                orderData.Статус_заказа = 2;
                orderData.ID_пункта_выдачи = deliveryID;
                orderData.Сумма_заказа = price;

                ElectroShopBDEntities.GetContext().Заказ.Add(orderData);

                foreach (int id in BasketClass.getBasket())
                {
                    Заказ_Товар orderItemData = new Заказ_Товар();

                    Товар productID = new Товар();

                    productID = ElectroShopBDEntities.GetContext().Товар.Where(u => u.ID == id).FirstOrDefault();

                    orderItemData.ID_товара = productID.ID;
                    orderItemData.ID_заказа = orderID;
                    //orderItemData.Количество_товара = ????

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
