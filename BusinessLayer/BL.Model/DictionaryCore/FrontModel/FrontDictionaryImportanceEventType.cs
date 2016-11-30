using BL.Model.Extensions;
using System;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// важность событий
    /// </summary>
    public class FrontDictionaryImportanceEventType
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        private DateTime _LastChangeDate; 
        public DateTime LastChangeDate { get { return _LastChangeDate; } set { _LastChangeDate=value.ToUTC(); } }
    }
}