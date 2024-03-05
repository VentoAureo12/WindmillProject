using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int tries;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == null)
            {
                MessageBox.Show("Введите логин");
                return;
            }
            if (PasswordBox.Password == null)

            {
                MessageBox.Show("Введите пароль");
                return;
            }
            var passwordMD5 = GetHash(PasswordBox.Password);

            Данные_пользователя userData = new Данные_пользователя();
            userData = ElectroShopBDEntities.GetContext().Данные_пользователя.Where(u => u.Логин == LoginBox.Text && u.Пароль == passwordMD5).FirstOrDefault();
            if (userData == null && tries <= 3)
            {
                MessageBox.Show("Данные введены неверно");
                tries++;
                return;
            }
            if (tries >= 3 && userData == null)
            {
                DateTime date = DateTime.Now;
                MessageBox.Show($"Вы ввели неправильные данные больше трёх раз. Система заблокирована на {10} секунд");
                while (date.AddSeconds(10) > DateTime.Now)
                {
                    LoginBox.IsEnabled = false;
                    PasswordBox.IsEnabled = false;
                    EnterButton.IsEnabled = false;
                }
                LoginBox.IsEnabled = true;
                PasswordBox.IsEnabled = true;
                EnterButton.IsEnabled = true;
                return;
            }

            DataTransfer.userID = userData.Пользователь.ID;
            DataTransfer.Name = userData.Пользователь.Имя;
            DataTransfer.Surname = userData.Пользователь.Фамилия;
            DataTransfer.Patronic = userData.Пользователь.Отчество;



            if (userData.Пользователь.Роль == 2)
            {
                if (userData == null)
                {
                    MessageBox.Show("Данные введены неверно");
                    return;
                }
                else
                {
                    AdminWindow window = new AdminWindow();
                    userData.Последняя_дата_входа = DateTime.Now;
                    window.Show();
                    MessageBox.Show("Добро пожаловать, администратор!");
                }
            }
            else
                if (userData.Пользователь.Роль == 3)
            {
                if (userData == null)
                {
                    MessageBox.Show("Данные введены неверно");
                    return;
                }
                userData.Последняя_дата_входа = DateTime.Now;
                ManagerWindow window = new ManagerWindow();
                window.Show();
                MessageBox.Show("Добро пожаловать, менеджер!");
            }
            else

                if (userData.Пользователь.Роль == 1)
            {
                if (userData == null)
                {
                    MessageBox.Show("Данные введены неверно");
                    return;
                }
                else
                {
                    userData.Последняя_дата_входа = DateTime.Now;
                    UserWindow window = new UserWindow();
                    window.Show();
                    MessageBox.Show("Добро пожаловать!");
                }

            }
        }

        public void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow window = new RegistrationWindow();
            window.Show();
            this.Close();

        }

        public static string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}
