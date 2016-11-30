using BL.Model.Enums;
using BL.Model.Extensions;
using System;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Типы рассылки
    /// </summary>
    public class FrontDictionarySendType
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
        /// Важность
        /// </summary>
        public bool IsImportant { get; set; }
        /// <summary>
        /// Тип субординации
        /// </summary>
        public EnumSubordinationTypes SubordinationType { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTime LastChangeDate { get { return _LastChangeDate; } set { _LastChangeDate=value.ToUTC(); } }
        private DateTime  _LastChangeDate; 

        /// <summary>
        /// Название типа субординации
        /// </summary>
        public string SubordinationTypeName { get; set; }
        /// <summary>
        /// Рассылка за пределы орагнизации
        /// </summary>
        public bool IsExternal { get; set; }
    }
}