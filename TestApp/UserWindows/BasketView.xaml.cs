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
        int price;
        public BasketView()
        {
            InitializeComponent();
            DeliveryInput.ItemsSource = ElectroShopBDEntities.GetContext().Пункт_Выдачи.ToList();
            BasketListViewItems = new List<BasketItemViewModel>();
            foreach (int id in BasketClass.getBasket())
            {
                Товар product = ElectroShopBDEntities.GetContext().Товар.Find(id);
                if (product != null)
                {
                    var existingItem = BasketListViewItems.FirstOrDefault(item => item.Product == product.ID);
                    if (existingItem != null)
                    {
                        existingItem.Quantity++;
                    }
                    else
                    {
                        BasketItemViewModel newItem = new BasketItemViewModel
                        {
                            Изображение = product.Изображение,
                            Наименование = product.Наименование,
                            Цена = (int)product.Цена,
                            Product = product.ID,
                            Quantity = 1
                        };
                        newItem.QuantityChanged += (sender, args) => updateResultSum();

                        BasketListViewItems.Add(newItem);
                    }
                }
            }
            BasketListView.ItemsSource = BasketListViewItems;
            updateResultSum();
        }

        private void updateResultSum()
        {
            int totalSum = 0;

            foreach (var basketItem in BasketListViewItems)
            {
                totalSum += basketItem.Quantity * basketItem.Цена;
            }

            resultSum.Content = $"Итого: {totalSum} руб";
            price = totalSum;
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

                // Создаем заказ
                Заказ orderData = new Заказ
                {
                    ID = orderID,
                    ID_пользователя = DataTransfer.userID,
                    Статус_заказа = 2,
                    ID_пункта_выдачи = int.Parse(DeliveryInput.SelectedValue.ToString()),
                    Сумма_заказа = price
                };

                // Добавляем заказ в контекст данных
                ElectroShopBDEntities.GetContext().Заказ.Add(orderData);

                // Обрабатываем каждый товар в корзине
                foreach (var basketItem in BasketListViewItems)
                {
                    // Создаем запись о товаре в заказе
                    Заказ_Товар orderItemData = new Заказ_Товар
                    {
                        ID_заказа = orderID,
                        ID_товара = basketItem.Product,
                        Количество_товара = basketItem.Quantity
                    };

                    // Добавляем запись о товаре в заказе в контекст данных
                    ElectroShopBDEntities.GetContext().Заказ_Товар.Add(orderItemData);
                }

                try
                {
                    // Сохраняем изменения в базе данных
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
