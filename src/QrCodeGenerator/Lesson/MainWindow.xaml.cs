using Lesson.data;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace Lesson
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> users;
        public MainWindow()
        {
            InitializeComponent();
            var str = new Newtonsoft.QRWorker.Encoder().EncodeString("123123");
            users = Core.GetContext().Users.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserNameTB.Text;
            string userPassword = PasswordPB.Password;

            if (IsUserExists(userName, userPassword))
            {
                MessageBox.Show($"Добро пожаловать, {userName}");
                new UsersListWindow(
                    users[users
                    .FindIndex(x => x.user_name.TrimEnd().Equals(userName))]
                    .Privilege.privilege_name.TrimEnd()
                    .Equals("Admin"))
                    .Show();
                Hide();
                return;
            }

            MessageBox.Show("Неправильный логин или пароль", "Ошибка входа");
            PasswordPB.Password = "";
        }

        private bool IsUserExists(string name, string password)
        {
            int id = users.FindIndex(x => x.user_name.TrimEnd().Equals(name));
            if (id == -1)
                return false;

            string userPassword = users[id].user_password.TrimEnd();
            if (string.IsNullOrWhiteSpace(userPassword))
                return false;

            return userPassword.Equals(password);
        }
    }
}
