using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;

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

        public int? ParentId
        {
            get { return null; }
        }

        public int ObjectId
        {
            get { return (int)EnumObjects.DictionaryAgentClientCompanies; }
        }

        public ITreeItem Parent
        {
            get { return null; }
        }

        public IEnumerable<ITreeItem> Childs { get; set; }
        //{
        //    get { return ChildPositions; }
        //    set { ChildPositions = (IEnumerable<FrontDictionaryPosition>)value; }
        //}


        #endregion

    }
}