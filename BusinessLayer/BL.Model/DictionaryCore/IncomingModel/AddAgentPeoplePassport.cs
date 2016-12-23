using BL.Model.Extensions;
using System;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Паспортные данные
    /// </summary>
    public class AddAgentPeoplePassport
    {
        /// <summary>
        /// Серия паспорта
        /// </summary>
        public string PassportSerial { get; set; }
        /// <summary>
        /// Номер паспорта
        /// </summary>
        public int? PassportNumber { get; set; }
        /// <summary>
        /// Дата выдачи паспорта
        /// </summary>
        public DateTime? PassportDate { get { return _PassportDate; } set { _PassportDate = value.ToUTC(); } }
        private DateTime? _PassportDate;
        /// <summary>
        /// Кем выдан паспорт
        /// </summary>
        public string PassportText { get; set; }
    }
}
