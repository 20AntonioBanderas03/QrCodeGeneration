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

            owner.Children.Add(new Label { Content = "Страницы: " });

            for (int i = 1; i <= countPages; i++)
            {
                var button = new Button { Content = $"{i}" };
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
