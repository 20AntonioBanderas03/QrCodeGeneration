using Lesson.data;
using Microsoft.Win32;
using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Lesson
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private User _currentUser;
        private UsersListWindow _owner;

        public UserWindow(UsersListWindow owner, User user = null)
        {
            InitializeComponent();

            _currentUser = user == null ? new User() : user;
            _owner = owner;

            PushFields(_currentUser);
            PushImage(_currentUser.UserImage);
        }

        private void PushFields(User user)
        {
            //заполнение полей
            UserIdTB.Text = user.user_id.ToString().TrimEnd();
            UserNameTB.Text = !string.IsNullOrWhiteSpace(user.user_name) ? user.user_name.TrimEnd() : "";
            UserEmailTB.Text = !string.IsNullOrWhiteSpace(user.user_email) ? user.user_email.TrimEnd() : "";

            UserStatusCB.ItemsSource = new LessonDatabaseEntities().Privileges.Select(x => x.privilege_name).ToArray();
            UserStatusCB.SelectedIndex = _currentUser.privilege_id - 1;

            if (user.user_password == null)
                OldPasswordPB.IsEnabled = false;
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog() { Filter = "Photo files (*.png, *.bmp)|*.png; *.bmp" };
            if (ofd.ShowDialog() == true)
            {
                byte[] photoBytes = File.ReadAllBytes(ofd.FileName);

                PushImage(photoBytes);
            }
        }

        private void PushImage(byte[] photoBytes)
        {
            if (photoBytes != null)
            {
                //обновление фото в бд
                _currentUser.user_image = photoBytes;

                //отображение фото из MemoryStream
                using (MemoryStream ms = new MemoryStream(photoBytes))
                {
                    var bi = new BitmapImage();

                    ms.Position = 0;
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();

                    UserImage.Source = bi;
                }
            }
        }

        private void PushImage(BitmapImage bi) => UserImage.Source = bi;

        private void RemoveImageButton_Click(object sender, RoutedEventArgs e)
        {
            //прекращение отображения фото и удаление из бд
            UserImage.Source = null;
            _currentUser.user_image = null;

            PushImage(_currentUser.UserImage);
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserStatusCB.SelectedIndex == -1)
            {
                MessageBox.Show("Вы не выбрали привилегию...", "Ошибка");
                return;
            }


            if (_currentUser.user_password == null && string.IsNullOrWhiteSpace(NewPasswordPB.Password))
            {
                MessageBox.Show("Вы не ввели пароль...", "Ошибка");
                return;
            }

            LessonDatabaseEntities entities1 = new LessonDatabaseEntities();

            if (!string.IsNullOrEmpty(NewPasswordPB.Password.Trim()))
            {
                if (!PasswordValid())
                {
                    MessageBox.Show("Неправильный старый пароль", "Ошибка");
                    return;
                }

                _currentUser.user_password = NewPasswordPB.Password.Trim();
            }

            _currentUser.user_name = UserNameTB.Text;
            _currentUser.user_email = UserEmailTB.Text;
            _currentUser.user_image = _currentUser.user_image;
            _currentUser.privilege_id = entities1.Privileges.ToArray()[UserStatusCB.SelectedIndex].privilege_id;

            entities1.Users.AddOrUpdate(_currentUser);

            bool IsSaved = false;
            try
            {
                IsSaved = true;
                entities1.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                return;
            }

            MessageBox.Show("Изменения вступили в силу...", "Успешно");
        }

        private bool PasswordValid()
        {
            if (_currentUser.user_password == null)
                return true;

            return OldPasswordPB.Password.Trim().Equals(_currentUser.user_password.TrimEnd());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите удалить текущего пользователя?", "Удалить", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            if (_currentUser.user_id != 0)//пользователь добавлен в бд
            {
                LessonDatabaseEntities entities1 = new LessonDatabaseEntities();
                entities1.Users.Remove(entities1.Users.First(x => x.user_id == _currentUser.user_id));
                entities1.SaveChanges();
            }

            MessageBox.Show("Удаление завершено...", "Успешно");
            Close();
        }
    }
}
