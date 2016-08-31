using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Компании". 
    /// </summary>
    public class FrontDictionaryAgentClientCompany : ModifyDictionaryAgentClientCompany, ITreeItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }


        #region ITreeItem
        [IgnoreDataMember]
        public int? ParentItemId
        {
            get { return null; }
        }

        public int? ObjectId
        {
            get { return (int)EnumObjects.DictionaryAgentClientCompanies; }
        }

        [IgnoreDataMember]
        public bool IsUsed { get; set; }

        public IEnumerable<ITreeItem> Childs { get; set; }

        #endregion

    }
}