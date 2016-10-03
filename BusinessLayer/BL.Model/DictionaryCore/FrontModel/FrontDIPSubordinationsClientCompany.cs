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
    public class FrontDIPSubordinationsClientCompany: FrontDIPSubordinationsBase
    {
  
        /// <summary>
        /// Руководитель
        /// </summary>
        public int? SourcePositionId { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public int? CompanyId { get; set; }
        
    }
}