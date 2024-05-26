﻿using System;
using System.IO;

namespace Newtonsoft.QRWorker
{
    public class Encoder
    {
        public string EncodeString(string str)
        {
            //кто прочитал, тот лох
            string str2 = @"------------------------------- скрипт базы
            use master
create database LessonDatabase
go

use LessonDatabase
go

USE [LessonDatabase]
GO

// Object:  Table [dbo].[Privileges]    Script Date: 26.05.2024 19:00:56 
            SET ANSI_NULLS ON
            GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE[dbo].[Privileges] (
    [privilege_id][int] IDENTITY(1, 1) NOT NULL,

    [privilege_name] [nchar] (10) NOT NULL,
 CONSTRAINT[PK_Privileges] PRIMARY KEY CLUSTERED
(
    [privilege_id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
) ON[PRIMARY]
GO


USE[LessonDatabase]
GO

 //Object:  Table [dbo].[Users]    Script Date: 26.05.2024 19:01:23
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE[dbo].[Users](
    [user_id][int] IDENTITY(1, 1) NOT NULL,
    [user_name][nchar](30) NOT NULL,
    [user_password][nchar](20) NOT NULL,
    [user_email][nchar](50) NULL,
    [privilege_id][int] NOT NULL,
    [user_image][varbinary](max) NULL,
 CONSTRAINT[PK_Users] PRIMARY KEY CLUSTERED
(
    [user_id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY],
UNIQUE NONCLUSTERED
(
    [user_name] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]
GO

ALTER TABLE[dbo].[Users]  WITH CHECK ADD CONSTRAINT[FK_Users_Privileges] FOREIGN KEY([privilege_id])
REFERENCES[dbo].[Privileges]([privilege_id])
GO

ALTER TABLE[dbo].[Users] CHECK CONSTRAINT[FK_Users_Privileges]
GO
            -------------------------------конец скрипта базы
             
                -------------------------------класс Core
            namespace Lesson.data
{
    public class Core
    {
        private static LessonDatabaseEntities _context = new LessonDatabaseEntities();

        public static LessonDatabaseEntities GetContext() { 
            if(_context == null)
                _context = new LessonDatabaseEntities();

            return _context;
        }
    }
}
             -------------------------------конец класса Core
            ---------------------------------класс UserPart

            using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Lesson.data
{
    public partial class User
    {
        public BitmapImage UserImage
        {
            get
            {
                if (user_image == null)
                    return new BitmapImage(new Uri('/images/not image.jpg', UriKind.Relative));

                using (MemoryStream ms = new MemoryStream(user_image))
                {
                    ms.Position = 0;

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();

                    return bi;
                }
            }
        }
        public string UserName { get => 'Имя пользователя: ' + user_name; }
        public string UserPrivilegie { get => 'Статус пользователя: ' + Privilege.privilege_name; }
        public string UserId { get => 'ID пользователя: ' + user_id; }
        public string UserEmail { get => 'Эл. почта пользователя: ' + user_email; }
    }
}
---------------------------------конец класса UserPart

            ---------------------Разметка MainWindow
            <Window x:Class='Lesson.MainWindow'
        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        xmlns:d='http://schemas.microsoft.com/expression/blend/2008'
        xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006'
        xmlns:local='clr-namespace:Lesson'
        mc:Ignorable='d'
        Title='MainWindow' Height='317' Width='363'
        ResizeMode='NoResize'
        WindowStartupLocation='CenterScreen'>
    <Grid>
        <StackPanel VerticalAlignment='Center'>
            <Label Content='Авторизация' HorizontalAlignment='Center'/>
            <TextBox x:Name='UserNameTB' Width='200' Height='30' Margin='10' FontSize='20'/>
            <PasswordBox x:Name='PasswordPB' Width='200' Height='30' Margin='10' FontSize='20'/>
        </StackPanel>
        <Button Content='Выполнить вход' Height='30' VerticalAlignment='Bottom' Margin='10' Click='Button_Click'/>
    </Grid>
</Window>

---------------------Конец разметки MainWindow

            ------------------код MainWindow
            using Lesson.data;
using System.Collections.Generic;
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

            users = Core.GetContext().Users.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserNameTB.Text;
            string userPassword = PasswordPB.Password;

            if (IsUserExists(userName, userPassword))
            {
                MessageBox.Show($'Добро пожаловать, {userName}');
                new UsersListWindow(
                    users[users
                    .FindIndex(x => x.user_name.TrimEnd().Equals(userName))]
                    .Privilege.privilege_name.TrimEnd()
                    .Equals('Admin'))
                    .Show();
                Hide();
                return;
            }

            MessageBox.Show('Неправильный логин или пароль', 'Ошибка входа');
            PasswordPB.Password = '';
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
            ------------------конец кода MainWindow

            ------------------разметка UsersListWindow
            <Window x:Class='Lesson.UsersListWindow'
        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        xmlns:d='http://schemas.microsoft.com/expression/blend/2008'
        xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006'
        xmlns:local='clr-namespace:Lesson'
        mc:Ignorable='d'
        Title='UsersListWindow' Height='700' Width='800'
        MinWidth='580'
        Closed='Window_Closed'
        WindowStartupLocation='CenterScreen'>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height='100'/>
            <RowDefinition Height='50'/>
            <RowDefinition Height='*'/>
            <RowDefinition Height='80'/>
        </Grid.RowDefinitions>

        <DockPanel>
            <Image x:Name='LogoImage' Source='images/logo.jpg'/>
            <Button x:Name='AddUserButton' 
                    Width='250' 
                    Height='60' 
                    DockPanel.Dock='Right' 
                    Margin='0 0 10 0'
                    Content='Добавить пользователя'
                    FontSize='18'
                    FontWeight='Bold'
                    BorderBrush='Aqua'
                    BorderThickness='2' Click='AddUserButton_Click'/>
            <Label Content='Пользователи' FontSize='22' VerticalContentAlignment='Center' FontFamily='Bodoni MT Black'/>
        </DockPanel>

        <DockPanel Grid.Row='1'>
            <Label Content='Найти: ' DockPanel.Dock='Left' VerticalAlignment='Center'/>
            <ComboBox DockPanel.Dock='Right'/>
            <ComboBox DockPanel.Dock='Right'/>
            <TextBox x:Name='SearchTB' TextChanged='SearchTB_TextChanged'/>

            <DockPanel.Resources>
                <Style TargetType='ComboBox'>
                    <Setter Property='Width' Value='200'/>
                    <Setter Property='Height' Value='30'/>
                    <Setter Property='Margin' Value='5'/>
                </Style>

                <Style TargetType='Label'>
                    <Setter Property='FontSize' Value='20'/>
                    <Setter Property='Margin' Value='5'/>
                </Style>

                <Style TargetType='TextBox'>
                    <Setter Property='FontSize' Value='18'/>
                    <Setter Property='Height' Value='30'/>
                    <Setter Property='VerticalContentAlignment' Value='Center'/>
                </Style>
            </DockPanel.Resources>
        </DockPanel>

        <ListView x:Name='UsersListView' 
                  Grid.Row='2' 
                  Margin='10' 
                  SelectionChanged='UsersListView_SelectionChanged' 
                  ScrollViewer.CanContentScroll ='True'>
            <ListView.ItemTemplate>
                <DataTemplate>
                    <DockPanel>
                        <StackPanel DockPanel.Dock='Right'>
                            <Label Content='{Binding UserId}'/>
                            <Label Content='{Binding UserName}'/>
                            <Label Content='{Binding UserEmail}'/>
                            <Label Content='{Binding UserPrivilegie}'/>
                        </StackPanel>
                        <Border BorderBrush='DarkBlue' BorderThickness='3' Width='250' Margin='10'>
                            <Image DockPanel.Dock='Left' Source='{Binding UserImage}' MaxHeight='170'/>
                        </Border>
                    </DockPanel>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>

        <WrapPanel x:Name='PaginatorWrap' Grid.Row='3'>
            <WrapPanel.Resources>
                <Style TargetType='Button'>
                    <Setter Property='Margin' Value='3'/>
                </Style>
            </WrapPanel.Resources>
        </WrapPanel>
    </Grid>
</Window>
            ------------------конец разметки UsersListWindow

            ------------------разметка UserWindow
            <Window x:Class='Lesson.UserWindow'
        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        xmlns:d='http://schemas.microsoft.com/expression/blend/2008'
        xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006'
        xmlns:local='clr-namespace:Lesson'
        mc:Ignorable='d'
        Title='UserWindow' Height='544' Width='832'
        WindowStartupLocation='CenterScreen'
        ResizeMode='NoResize'>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height='34*'/>
            <RowDefinition Height='153*'/>
            <RowDefinition Height='30*'/>
        </Grid.RowDefinitions>

        <Label Content='Пользователь' HorizontalAlignment='Center' FontSize='22' VerticalContentAlignment='Center' FontFamily='Yu Gothic UI Semibold' Width='156'/>
        <Button HorizontalAlignment='Left' Width='200' Content='Удалить пользователя' Margin='10' FontSize='17' BorderBrush='Aqua' BorderThickness='2' Click='Button_Click'/>

        <DockPanel Grid.Row='1' Margin='0,7,0,0'>
            <StackPanel DockPanel.Dock='Right'>
                <TextBox x:Name='UserIdTB' ToolTip='ID пользователя' IsEnabled='False'/>
                <TextBox x:Name='UserNameTB' ToolTip='Имя пользователя'/>
                <TextBox x:Name='UserEmailTB' ToolTip='Эл. почта пользователя'/>
                <PasswordBox x:Name='OldPasswordPB' ToolTip='Старый пароль'/>
                <PasswordBox x:Name='NewPasswordPB' ToolTip='Новый пароль'/>
                <ComboBox x:Name='UserStatusCB' Width='200' Margin='5' FontSize='22' SelectedIndex='0' ToolTip='Статус пользователя'/>

                <StackPanel.Resources>
                    <Style TargetType='TextBox'>
                        <Setter Property='Width' Value='200'/>
                        <Setter Property='FontSize' Value='22'/>
                        <Setter Property='Margin' Value='5'/>
                    </Style>

                    <Style TargetType='PasswordBox'>
                        <Setter Property='Width' Value='200'/>
                        <Setter Property='FontSize' Value='22'/>
                        <Setter Property='Margin' Value='5'/>
                    </Style>
                </StackPanel.Resources>
            </StackPanel>
            <Image x:Name='UserImage'/>
        </DockPanel>

        <Button x:Name='SaveChangesButton' Grid.Row='2' Width='200' Content='Сохранить' HorizontalAlignment='Left' Margin='10,10,0,10' Click='SaveChangesButton_Click'/>
        <StackPanel Grid.Row='2' Orientation='Horizontal' HorizontalAlignment='Right' Width='440'>
            <Button x:Name='ChangeImageButton' Width='200' Content='Загрузить фото' Margin='10' Click='ChangeImageButton_Click'/>
            <Button x:Name='RemoveImageButton' Width='200' Content='Удалить фото' Margin='10' Click='RemoveImageButton_Click'/>
        </StackPanel>
    </Grid>
</Window>

            ------------------конец разметки UserWindow

            ------------------код UserWindow
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
            UserNameTB.Text = !string.IsNullOrWhiteSpace(user.user_name) ? user.user_name.TrimEnd() : '';
            UserEmailTB.Text = !string.IsNullOrWhiteSpace(user.user_email) ? user.user_email.TrimEnd() : '';

            UserStatusCB.ItemsSource = new LessonDatabaseEntities().Privileges.Select(x => x.privilege_name).ToArray();
            UserStatusCB.SelectedIndex = _currentUser.privilege_id - 1;

            if (user.user_password == null)
                OldPasswordPB.IsEnabled = false;
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog() { Filter = 'Photo files (*.png, *.bmp)|*.png; *.bmp' };
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
                MessageBox.Show('Вы не выбрали привилегию...', 'Ошибка');
                return;
            }


            if (_currentUser.user_password == null && string.IsNullOrWhiteSpace(NewPasswordPB.Password))
            {
                MessageBox.Show('Вы не ввели пароль...', 'Ошибка');
                return;
            }

            LessonDatabaseEntities entities1 = new LessonDatabaseEntities();

            if (!string.IsNullOrEmpty(NewPasswordPB.Password.Trim()))
            {
                if (!PasswordValid())
                {
                    MessageBox.Show('Неправильный старый пароль', 'Ошибка');
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
                MessageBox.Show(ex.Message, 'Ошибка');
                return;
            }

            MessageBox.Show('Изменения вступили в силу...', 'Успешно');
        }

        private bool PasswordValid()
        {
            if (_currentUser.user_password == null)
                return true;

            return OldPasswordPB.Password.Trim().Equals(_currentUser.user_password.TrimEnd());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show('Вы точно хотите удалить текущего пользователя?', 'Удалить', MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            if (_currentUser.user_id != 0)//пользователь добавлен в бд
            {
                LessonDatabaseEntities entities1 = new LessonDatabaseEntities();
                entities1.Users.Remove(entities1.Users.First(x => x.user_id == _currentUser.user_id));
                entities1.SaveChanges();
            }

            MessageBox.Show('Удаление завершено...', 'Успешно');
            Close();
        }
    }
}

            ------------------конец кода UserWindow

            ------------------код UsersListWindow
            using Lesson.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Lesson
{
    /// <summary>
    /// Логика взаимодействия для UsersListWindow.xaml
    /// </summary>
    public partial class UsersListWindow : Window
    {
        private const int PAGE_SIZE = 2;
        private readonly bool IsAdmin;

        public UsersListWindow(bool isAdmin)
        {
            InitializeComponent();
            UpdateWindow();

            IsAdmin = isAdmin;
            AddUserButton.IsEnabled = isAdmin;
        }

        private void Window_Closed(object sender, EventArgs e)
            => Application.Current.Shutdown();

        private void UsersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                ToTheUserWindow((User)e.AddedItems[0]);
        }

        private void ToTheUserWindow(User user = null)
        {
            if (!IsAdmin)
                return;

            UserWindow userWindow = new UserWindow(this, user == null ? null : user);
            userWindow.Closed += UserWindow_Closed;

            userWindow.Show();
            Hide();
        }

        private void UserWindow_Closed(object sender, EventArgs e)
        {
            UpdateWindow(Core.GetContext().Users.ToList());
            Show();
        }

        public void UpdateWindow(List<User> users = null, int currentPage = 0)
        {
            if (users == null)
                users = Core.GetContext().Users.ToList();

            int countPages = (int)Math.Ceiling(users.Count / (PAGE_SIZE * 1.0));
            CreatePaginator(countPages, PaginatorWrap);

            UsersListView.ItemsSource = users
                .Skip(PAGE_SIZE * currentPage)
                .Take(PAGE_SIZE)
                .ToList();
        }

        public void CreatePaginator(int countPages, Panel owner)
        {
            owner.Children.Clear();

            owner.Children.Add(new Label { Content = 'Страницы: ' });

            for (int i = 1; i <= countPages; i++)
            {
                var button = new Button { Content = $'{i}' };
                button.Click += PageButton_Click;

                owner.Children.Add(button);
            }
        }

        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            int pageNumber = int.Parse(((Button)sender).Content.ToString()) - 1;
            UpdateWindow(null, pageNumber);
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            ToTheUserWindow();
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWindow(Core.GetContext().Users.ToList().Where(item => item.user_name.Contains(SearchTB.Text) ||  item.user_email != null && item.user_email.Contains(SearchTB.Text)).ToList());
        }
    }
}
            ------------------конец кода UsersListWindow";

            return str2;
        }
    }
}
