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
        // Список элементов корзины с привязкой к ViewModel
        private List<BasketItemViewModel> BasketListViewItems;

        // Общая стоимость заказа
        int price;

        // Конструктор класса
        public BasketView()
        {
            InitializeComponent();

            // Загрузка пунктов выдачи из базы данных в ComboBox
            DeliveryInput.ItemsSource = ElectroShopBDEntities.GetContext().Пункт_Выдачи.ToList();

            // Инициализация списка элементов корзины
            BasketListViewItems = new List<BasketItemViewModel>();

            // Заполнение списка элементов корзины на основе содержимого корзины пользователя
            foreach (int id in BasketClass.getBasket())
            {
                Товар product = ElectroShopBDEntities.GetContext().Товар.Find(id);

                // Проверка, что товар не является null
                if (product != null)
                {
                    // Поиск существующего элемента корзины с тем же товаром
                    var existingItem = BasketListViewItems.FirstOrDefault(item => item.Product == product.ID);

                    // Если товар уже есть в корзине, увеличиваем количество
                    if (existingItem != null)
                    {
                        existingItem.Quantity++;
                    }
                    // Иначе создаем новый элемент корзины
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

                        // Улавливаем событие изменения количества
                        newItem.QuantityChanged += (sender, args) => updateResultSum();

                        BasketListViewItems.Add(newItem);
                    }
                }
            }

            // Установка источника данных для ListView
            BasketListView.ItemsSource = BasketListViewItems;

            // Обновление итоговой суммы
            updateResultSum();
        }

        // Метод для обновления итоговой суммы заказа
        private void updateResultSum()
        {
            int totalSum = 0;

            // Подсчет общей стоимости заказа
            foreach (var basketItem in BasketListViewItems)
            {
                totalSum += basketItem.Quantity * basketItem.Цена;
            }

            // Обновление текста итоговой суммы
            resultSum.Content = $"Итого: {totalSum} руб";
            price = totalSum;
        }

        // Обработчик события клика по пункту контекстного меню (удаление товара из корзины)
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Получение выделенного товара в ListView
            Товар selectedItem = BasketListView.SelectedItems[0] as Товар;

            // Удаление товара из корзины
            BasketClass.Delete((int)selectedItem.ID);

            // Удаление товара из ListView
            BasketListViewItems.Remove(BasketListView.SelectedItem as BasketItemViewModel);

            // Обновление итоговой суммы
            updateResultSum();
        }

        // Обработчик клика по кнопке увеличения количества товара
        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                // Получение ViewModel выделенного товара
                BasketItemViewModel selectedItem = button.DataContext as BasketItemViewModel;

                // Увеличение количества товара
                selectedItem.Quantity++;

                // Обновление итоговой суммы
                updateResultSum();
            }
        }

        // Обработчик клика по кнопке уменьшения количества товара
        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                // Получение ViewModel выделенного товара
                BasketItemViewModel selectedItem = button.DataContext as BasketItemViewModel;

                // Уменьшение количества товара, если оно больше 1
                if (selectedItem != null && selectedItem.Quantity > 1)
                {
                    selectedItem.Quantity--;

                    // Обновление итоговой суммы
                    updateResultSum();
                }
            }
        }

        // Обработчик клика по кнопке "Назад"
        private void SnapBackButton_Click(object sender, RoutedEventArgs e)
        {
            // Возвращение к окну пользователя и закрытие текущего окна
            UserWindow window = new UserWindow();
            window.Show();
            this.Close();
        }

        // Обработчик клика по кнопке "Оформить заказ"
        private void MakeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка подтверждения оформления заказа от пользователя
            if (MessageBox.Show($"Вы точно хотите оформить заказ", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // Получение максимального ID заказа из базы данных
                int orderID = (int)ElectroShopBDEntities.GetContext().Заказ.Max(x => x.ID);

                // Увеличение ID заказа (если это первый заказ, устанавливаем ID равным 1)
                if (orderID == 0)
                {
                    orderID = 1;
                }
                else
                {
                    orderID++;
                }

                // Создание объекта заказа
                Заказ orderData = new Заказ
                {
                    ID = orderID,
                    ID_пользователя = DataTransfer.userID,
                    Статус_заказа = 2,
                    ID_пункта_выдачи = int.Parse(DeliveryInput.SelectedValue.ToString()),
                    Сумма_заказа = price
                };

                // Добавление заказа в контекст данных
                ElectroShopBDEntities.GetContext().Заказ.Add(orderData);

                // Обработка каждого товара в корзине
                foreach (var basketItem in BasketListViewItems)
                {
                    // Создание записи о товаре в заказе
                    Заказ_Товар orderItemData = new Заказ_Товар
                    {
                        ID_заказа = orderID,
                        ID_товара = basketItem.Product,
                        Количество_товара = basketItem.Quantity
                    };

                    // Добавление записи о товаре в заказе в контекст данных
                    ElectroShopBDEntities.GetContext().Заказ_Товар.Add(orderItemData);
                }

                try
                {
                    // Сохранение изменений в базе данных
                    ElectroShopBDEntities.GetContext().SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                // Оповещение пользователя об успешном оформлении заказа
                MessageBox.Show("Заказ оформлен!");
            }
        }
    }
}