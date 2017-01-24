using BL.Model.Enums;
using BL.Model.Extensions;
using System;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Типы этапов
    /// </summary>
    public class FrontDictionaryStageType
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
    }
}