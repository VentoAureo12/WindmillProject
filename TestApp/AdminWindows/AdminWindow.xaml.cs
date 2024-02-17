﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private bool isDirty = true;
        public AdminWindow()
        {
            InitializeComponent();
            GoodsData.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();
            TypesOfGoodsComboBox.ItemsSource = ElectroShopBDEntities.GetContext().Вид_товара.ToList();
            var typesOfGoods = ElectroShopBDEntities.GetContext().Вид_товара.ToList();

            typesOfGoods.Insert(0, new Вид_товара { Название = "Все" });

            TypeSearchComboBox.ItemsSource = typesOfGoods;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var goodsForRemoving = GoodsData.SelectedItems.Cast<Товар>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить следующие {goodsForRemoving.Count()} элементов?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    ElectroShopBDEntities.GetContext().Товар.RemoveRange(goodsForRemoving);
                    ElectroShopBDEntities.GetContext().SaveChanges();
                    MessageBox.Show("Удалено");
                    GoodsData.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Edit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GoodsData.IsReadOnly = false;
            GoodsData.BeginEdit();
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
            GoodsData.IsReadOnly = true;
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
            GoodsData.ItemsSource = null;
            GoodsData.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();
            MessageBox.Show("Отмена действия");
            isDirty = true;
            GoodsData.IsReadOnly = true;
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GoodsData.IsReadOnly = false;
            Товар goods = new Товар()
            {
                Наименование = "Вставьте название",
                Вид_товара = 1,
                Количество_на_складе = 0,
                Цена = 0,
                Изображение = null
            };
            ElectroShopBDEntities.GetContext().Товар.Add(goods);
            try
            {
                ElectroShopBDEntities.GetContext().SaveChanges();
                GoodsData.ItemsSource = null;
                GoodsData.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();
                MessageBox.Show("Данные готовы к добавлению");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            isDirty = false;
            GoodsData.IsReadOnly = true;
        }

        private void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BorderFind.Visibility = Visibility.Visible;
            isDirty = false;
        }

        private void Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = isDirty;
        }

        private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TypeSearchComboBox.SelectedIndex = -1;
            NameSearchField.Text = "";
            GoodsData.ItemsSource = null;
            GoodsData.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();
            isDirty = false;
        }

        private void NameSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
           if(NameSearchField.Text != null)
            {
                try
                {
                    GoodsData.ItemsSource = ElectroShopBDEntities.GetContext().Товар.Where(u => u.Наименование.Contains(NameSearchField.Text)).ToList();
                }
                catch
                {

                }
                TypeSearchComboBox.SelectedIndex = -1;
            }
        }

        private void TypeSearchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Получаем выбранный вид товара
                Вид_товара selectedType = TypeSearchComboBox.SelectedItem as Вид_товара;

                if (selectedType != null && selectedType.Название != "Все")
                {
                    // Фильтруем товары по выбранному виду
                    List<Товар> filteredGoods = ElectroShopBDEntities.GetContext().Товар
                        .Where(t => t.Вид_товара1.Название == selectedType.Название)
                        .ToList();

                    // Обновляем источник данных для GoodsData
                    GoodsData.ItemsSource = filteredGoods;
                }
                else
                {
                    // Если выбран "Все", обновляем источник данных без фильтрации
                    GoodsData.ItemsSource = ElectroShopBDEntities.GetContext().Товар.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            isDirty = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png)|*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                Товар currentItem = ((FrameworkElement)sender).DataContext as Товар;

                // Считайте выбранное изображение в байтовый массив
                byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);

                // Обновите свойство ImageData текущего элемента
                currentItem.Изображение = imageBytes;
            }
        }

        public class BytesToImageConverter: IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is byte[] bytes)
                {
                    BitmapImage image = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(bytes))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                    }
                    return image;
                }
                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
