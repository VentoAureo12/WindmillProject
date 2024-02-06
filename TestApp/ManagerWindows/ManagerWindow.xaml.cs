using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;
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
using System.Xml.Linq;
using TestApp.Classes;
using TestApp.ManagerWindows;
using Windows.UI.Text;
using Paragraph = Microsoft.Office.Interop.Word.Paragraph;
using Microsoft.Win32;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : System.Windows.Window
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

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            if (ClientDataGrid.SelectedItem != null)
            {
                if (ClientDataGrid.SelectedItem is Пользователь selectedUser)
                {
                    Пользователь user = ElectroShopBDEntities.GetContext().Пользователь.Find(selectedUser.ID);
                    if (user != null)
                    {
                        GenerateReport(user);
                    }
                }
            }
        }

        private void GenerateReport(Пользователь user)
        {
            // Создаем приложение Word
            Application wordApp = new Application();

            // Создаем новый документ
            Document doc = wordApp.Documents.Add();

            // Добавляем заголовок
            Paragraph title = doc.Paragraphs.Add();
            title.Range.Text = "Отчет по заказам пользователя";
            title.Range.Font.Bold = 1;
            title.Range.Font.Size = 16;

            // Выравниваем заголовок по центру
            title.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            doc.Paragraphs.Add(); // Добавляем пустой параграф

            // Добавляем информацию о пользователе
            Paragraph userInfo = doc.Paragraphs.Add();
            userInfo.Range.Text = $"ФИО пользователя: {user.Фамилия} {user.Имя} {user.Отчество}";
            userInfo.Range.Font.Size = 12;
            userInfo.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; // Выравниваем влево
            doc.Paragraphs.Add(); // Добавляем пустой параграф

            // Получаем все заказы пользователя
            IEnumerable<Заказ> userOrders = user.Заказ;

            // Добавляем количество сделанных заказов
            Paragraph orderCountInfo = doc.Paragraphs.Add();
            orderCountInfo.Range.Text = $"Количество сделанных заказов: {userOrders.Count()}";
            orderCountInfo.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; // Выравниваем влево
            doc.Paragraphs.Add(); // Добавляем пустой параграф

            // Добавляем общую сумму заказов пользователя
            Paragraph totalSumInfo = doc.Paragraphs.Add();
            totalSumInfo.Range.Text = $"Общая сумма заказов: {userOrders.Sum(o => o.Сумма_заказа)} рублей";
            totalSumInfo.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; // Выравниваем влево
            doc.Paragraphs.Add(); // Добавляем пустой параграф

            foreach (var order in userOrders)
            {
                // Добавляем информацию о заказе
                Paragraph orderInfo = doc.Paragraphs.Add();
                orderInfo.Range.Text = $"Заказ № {order.ID}, Сумма: {order.Сумма_заказа} рублей";
                orderInfo.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; // Выравниваем влево
                doc.Paragraphs.Add(); // Добавляем пустой параграф

                // Добавляем уникальный номер заказа
                Paragraph orderIdInfo = doc.Paragraphs.Add();
                orderIdInfo.Range.Text = $"Уникальный номер заказа: {order.ID}";
                orderIdInfo.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; // Выравниваем влево
                doc.Paragraphs.Add(); // Добавляем пустой параграф

                // Получаем товары в заказе
                IEnumerable<Заказ_Товар> orderItems = order.Заказ_Товар;

                foreach (var orderItem in orderItems)
                {
                    // Добавляем информацию о товаре в заказе
                    Paragraph itemInfo = doc.Paragraphs.Add();
                    itemInfo.Range.Text = $"Товар: {orderItem.Товар.Наименование}, Количество: {orderItem.Количество_товара}";
                    itemInfo.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; // Выравниваем влево
                    doc.Paragraphs.Add(); // Добавляем пустой параграф
                }

                // Добавляем пробел после каждого заказа
                doc.Paragraphs.Add(); // Добавляем пустой параграф
            }

            // Отображаем диалоговое окно сохранения файла
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Word Documents (*.docx)|*.docx";
            saveFileDialog.FileName = $"Отчет_{user.ID}_{DateTime.Now:yyyyMMdd_HHmmss}";

            if (saveFileDialog.ShowDialog() == true)
            {
                // Сохраняем документ
                object fileName = saveFileDialog.FileName;
                doc.SaveAs2(ref fileName);
                doc.Close();
                wordApp.Quit();

                MessageBox.Show($"Отчет сохранен: {fileName}");
            }
            else
            {
                // Если пользователь отменил выбор файла, закрываем документ
                doc.Close(WdSaveOptions.wdDoNotSaveChanges);
                wordApp.Quit();
            }
        }
    }
}
