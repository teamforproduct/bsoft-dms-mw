using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using BL.Model.Tree;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// 
    /// </summary>
    // 
    public class FrontDIPSubordinationsBase: TreeItem
    {
  
        /// <summary>
        /// Для сведения
        /// </summary>
        public int? IsInforming { get; set; }

        /// <summary>
        /// Для исполнения
        /// </summary>
        public int? IsExecution { get; set; }

    }
}