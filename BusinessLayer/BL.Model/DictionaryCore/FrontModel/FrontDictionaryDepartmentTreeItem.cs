using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Tree;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Структура предприятия"
    /// </summary>
    // 
    public class FrontDictionaryDepartmentTreeItem: TreeItem
    {
  
        /// <summary>
        /// Код подразделения
        /// </summary>
        //[IgnoreDataMember]
        public string Code { get; set; }

        /// <summary>
        /// Id компании
        /// </summary>
        public int CompanyId { get; set; }
        
    }
}