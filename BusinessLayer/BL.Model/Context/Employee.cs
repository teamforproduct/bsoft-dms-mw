namespace BL.Model.Context
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

        //public int ClientId { get; set; }
        //public string ClientCode { get; set; }

        /// <summary>
        /// деактивированный сотрудник не может войти в систему
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Количество должностей исполняемых на данный момент
        /// </summary>
        public int PositionExecutorsCount { get; set; }

    }
}