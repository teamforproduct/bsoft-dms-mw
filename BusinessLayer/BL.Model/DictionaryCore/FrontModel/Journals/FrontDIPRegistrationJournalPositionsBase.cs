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
    public class FrontDIPRegistrationJournalPositionsBase: TreeItem
    {
  
        /// <summary>
        /// Для просмотра документов
        /// </summary>
        public int? IsViewing { get; set; }

        /// <summary>
        /// Для регистрации документов
        /// </summary>
        public int? IsRegistration { get; set; }

    }
}