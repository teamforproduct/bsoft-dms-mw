namespace BL.Model.Users
{
    /// <summary>
    /// класс сотрудника
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Токен из авторизации
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// ИД веб пользователя
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// ИД сотрудника
        /// </summary>
        public int? AgentId { get; set; }
        /// <summary>
        /// ФИО
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Код языка
        /// </summary>
        public int LanguageId { get; set; }
        ///// <summary>
        ///// ЛОгин
        ///// </summary>
        //public string Login { get; set; }
        ///// <summary>
        ///// Пароль
        ///// </summary>
        //public string Password { get; set; }
    }
}