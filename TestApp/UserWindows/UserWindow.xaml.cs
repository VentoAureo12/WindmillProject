using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

    namespace TestApp
    {
        /// <summary>
        /// Логика взаимодействия для UserWindow.xaml
        /// </summary>
        public partial class UserWindow : Window
        {
            // Идентификатор пользователя
            int userID;

            // Конструктор класса
            public UserWindow()
            {
                InitializeComponent();

                // Загрузка списка товаров в ListView из базы данных
                ProductsListView.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();

                // Получение идентификатора пользователя из DataTransfer
                userID = DataTransfer.userID;

                // Отображение имени пользователя в Label
                userNameBox.Content = DataTransfer.Name + " " + DataTransfer.Surname + " " + DataTransfer.Patronic;

                // Получение всех видов товара и добавление "Все типы" в список
                var allTypes = ElectroShopBDEntities.GetContext().Вид_товара.ToList();
                allTypes.Insert(0, new Вид_товара
                {
                    Название = "Все типы"
                });

                // Установка источника данных для ComboBox
                ComboElectronicType.ItemsSource = allTypes;

                // Установка "Все типы" в качестве выбранного элемента
                ComboElectronicType.SelectedIndex = 0;
            }

            // Обработчик события клика по пункту контекстного меню (добавление в корзину)
            private void MenuItem_Click(object sender, RoutedEventArgs e)
            {
                // Получение выбранного товара
                var current = (Товар)ProductsListView.SelectedItem;

                // Добавление товара в корзину и обновление видимости кнопок
                BasketClass.addProduct((int)current.ID);
                this.checkBasketCount();
            }

            // Метод для проверки количества товаров в корзине и обновления видимости кнопок
            private void checkBasketCount()
            {
                btnClearBasket.Visibility = Visibility.Hidden;
                btnEnterBasket.Visibility = Visibility.Hidden;

                // Если в корзине есть товары, отображаются кнопки
                if (BasketClass.getBasket().Count > 0)
                {
                    btnClearBasket.Visibility = Visibility.Visible;
                    btnEnterBasket.Visibility = Visibility.Visible;
                }
            }

            // Обработчик двойного клика по метке с именем пользователя
            private void userNameBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                // Открытие окна с заказами пользователя и закрытие текущего окна
                UserOrdersWindow userOrders = new UserOrdersWindow();
                userOrders.Show();
                Close();
            }

            // Обработчик клика по кнопке "Очистить корзину"
            private void Button_Click_1(object sender, RoutedEventArgs e)
            {
                // Очистка корзины и обновление видимости кнопок
                BasketClass.ClearBasket();
                this.checkBasketCount();
            }

            // Обработчик клика по кнопке "Корзина"
            private void Button_Click(object sender, RoutedEventArgs e)
            {
                // Отображение содержимого корзины в MessageBox
                string msg = "";
                foreach (var product in BasketClass.getBasket())
                {
                    msg += $" {product}";
                }
                MessageBox.Show($"{msg}");

                // Открытие окна с корзиной и закрытие текущего окна
                BasketView basketView = new BasketView();
                basketView.Show();
                this.Close();
            }

            // Обработчик события загрузки окна
            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                // Пустой обработчик события
            }

            // Обработчик изменения выбора в ComboBox "Выберите категорию"
            private void ComboElectronicType_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                // Фильтрация товаров в зависимости от выбранной категории
                switch (ComboElectronicType.SelectedIndex)
                {
                    case 0:
                        ProductsListView.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();
                        break;
                    case 1:
                        ProductsListView.ItemsSource = ElectroShopBDEntities.GetContext().Товар.Where(u => u.Вид_товара == 1).ToList();
                        break;
                    case 2:
                        ProductsListView.ItemsSource = ElectroShopBDEntities.GetContext().Товар.Where(u => u.Вид_товара == 2).ToList();
                        break;
                    case 3:
                        ProductsListView.ItemsSource = ElectroShopBDEntities.GetContext().Товар.Where(u => u.Вид_товара == 3).ToList();
                        break;
                    case 4:
                        ProductsListView.ItemsSource = ElectroShopBDEntities.GetContext().Товар.Where(u => u.Вид_товара == 4).ToList();
                        break;
                    case 5:
                        ProductsListView.ItemsSource = ElectroShopBDEntities.GetContext().Товар.Where(u => u.Вид_товара == 5).ToList();
                        break;
                }
            }

            // Обработчик изменения текста в TextBox "Введите поисковой запрос"
            private void ElectroSearch_TextChanged(object sender, TextChangedEventArgs e)
            {
                // Фильтрация товаров по введенному тексту
                ProductsListView.ItemsSource = ElectroShopBDEntities.GetContext().Товар.Where(u => u.Наименование.Contains(ElectroSearch.Text)).ToList();
            }
        }
    }

