using BL.Model.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BL.Model.SystemCore.FrontModel
{
    /// <summary>
    /// Настройки системы
    /// </summary>
    public class FrontSystemSetting
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Имя настройки
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание настройки
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Тип значения
        /// </summary>
        public string ValueTypeCode { get; set; }
        /// <summary>
        /// Название группы настройки
        /// </summary>
        public string SettingTypeName { get; set; }
        /// <summary>
        /// Порядок следования настройки
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Порядок следования группы
        /// </summary>
        public int OrderSettingType { get; set; }

        //public int? AgentId { get; set; }
    }
}
