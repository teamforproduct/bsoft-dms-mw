using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Штатное расписание". 
    /// </summary>
    public class FrontShortListPosition : ListItem
    {

        /// <summary>
        /// Код подразделения
        /// </summary>
        public string DepartmentCodePath { get; set; }
        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// Наименование компании
        /// </summary>
        public string CompanyName { get; set; }

    }
}