namespace BL.Model.Context
{
    /// <summary>
    /// класс сотрудника
    /// </summary>
    public class Employee
    {
        
        /// <summary>
        /// ИД сотрудника
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Фамилия И.О.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// деактивированный сотрудник(помеченный на удаление) не может войти в систему + не участвует в выборках
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// сотрудник заблокирован администратором - не может войти в систему
        /// </summary>
        public bool IsLockout { get; set; }

        /// <summary>
        /// Количество актуальных назначений на данный момент
        /// </summary>
        public int AssigmentsCount { get; set; }

    }
}