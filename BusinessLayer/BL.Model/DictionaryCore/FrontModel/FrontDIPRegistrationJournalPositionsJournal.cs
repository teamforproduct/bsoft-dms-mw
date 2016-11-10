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
    public class FrontDIPRegistrationJournalPositionsJournal : FrontDIPRegistrationJournalPositionsBase
    {

        /// <summary>
        /// Индекс журнала
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public int? PositionId { get; set; }

        /// <summary>
        /// Журтнал
        /// </summary>
        public int? RegistrationJournalId { get; set; }
        
    }
}