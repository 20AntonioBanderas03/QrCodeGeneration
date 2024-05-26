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
                    return new BitmapImage(new Uri("/images/not image.jpg", UriKind.Relative));

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
        public string UserName { get => "Имя пользователя: " + user_name; }
        public string UserPrivilegie { get => "Статус пользователя: " + Privilege.privilege_name; }
        public string UserId { get => "ID пользователя: " + user_id; }
        public string UserEmail { get => "Эл. почта пользователя: " + user_email; }
    }
}
