//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lesson.data
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_password { get; set; }
        public string user_email { get; set; }
        public int privilege_id { get; set; }
        public byte[] user_image { get; set; }
    
        public virtual Privilege Privilege { get; set; }
    }
}
