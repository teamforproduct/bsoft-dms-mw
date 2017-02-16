namespace BL.Model.Users
{
    public class FrontPermission
    {
        /// <summary>
        /// Модуль
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Фича
        /// </summary>
        public string Feature { get; set; }

        /// <summary>
        /// Доступ по CRUD
        /// </summary>
        public string AccessType { get; set; }

    }
}
