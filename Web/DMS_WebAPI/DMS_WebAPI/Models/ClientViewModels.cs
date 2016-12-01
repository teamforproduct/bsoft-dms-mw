using BL.Model.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DMS_WebAPI.Models
{
    public class ClientViewModel
    {
        #region Client
        [Required]
        [Display(Name = "Название компании")]
        public string ClientName { get; set; }
        //public string Code { get; set; }

        #endregion Client

        #region Admin
        [Required]
        [Display(Name = "Емайл администратора компании")]
        public string AdminEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должно быть по крайней мере {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль администратора")]
        public string AdminPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль администратора")]
        [Compare("AdminPassword", ErrorMessage = "Пароль и подтверждение пароля не совпадают.")]
        public string AdminConfirmPassword { get; set; }
        #endregion Admin

        #region Server
        [Required]
        [Display(Name = "Адрес сервера базы данных")]
        public string ServerAddress { get; set; }

        [Required]
        [Display(Name = "Название сервера базы данных")]
        public string ServerName { get; set; }

        [Required]
        [Display(Name = "Тип сервера базы данных")]
        public EnumDatabaseType ServerServerType { get; set; }

        [Required]
        [Display(Name = "Название базы данных")]
        public string ServerDefaultDatabase { get; set; }

        [Required]
        [Display(Name = "Использовать интегрированную система безопасности")]
        public bool ServerIntegrateSecurity { get; set; }

        [Required]
        [Display(Name = "Логин пользователя для подключения к базе")]
        public string ServerUserName { get; set; }

        [Required]
        [Display(Name = "Пароль пользователя для подключения к базе")]
        [DataType(DataType.Password)]
        public string ServerUserPassword { get; set; }

        [Display(Name = "Строка подключения к базе")]
        public string ServerConnectionString { get; set; }

        [Required]
        [Display(Name = "Название схемы базы")]
        public string ServerDefaultSchema { get; set; }
        #endregion Server
    }
}
