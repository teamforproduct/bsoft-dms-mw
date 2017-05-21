using System;

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
        /// ФИО
        /// </summary>
        public string Name { get; set; }

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