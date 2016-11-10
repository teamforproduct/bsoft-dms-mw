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
    public class FrontDIPRegistrationJournalPositionsClientCompany : FrontDIPRegistrationJournalPositionsBase
    {
  
        /// <summary>
        /// Должность
        /// </summary>
        public int? PositionId { get; set; }

        /// <summary>
        /// Компания
        /// </summary>
        public int? CompanyId { get; set; }
        
    }
}