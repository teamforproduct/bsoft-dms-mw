using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.Extensions;
using System;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Типы рассылки
    /// </summary>
    public class FrontDictionarySendType : ListItem
    {
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Важность
        /// </summary>
        public bool IsImportant { get; set; }
        /// <summary>
        /// Тип субординации
        /// </summary>
        public EnumSubordinationTypes SubordinationType { get; set; }

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