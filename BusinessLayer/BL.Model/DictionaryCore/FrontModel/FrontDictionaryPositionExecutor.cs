using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;
using System;
using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Исполнители". 
    /// </summary>
    public class FrontDictionaryPositionExecutor : ModifyDictionaryPositionExecutor, ITreeItem
    {

        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Агент
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string PositionFullName { get; set; }

        /// <summary>
        /// Тип исполнителя
        /// </summary>
        public string PositionExecutorTypeName { get; set; }

        /// <summary>
        /// Уровень доступа: лично, референт, ио
        /// </summary>
        public string AccessLevelName { get; set; }

        #region ITreeItem
        [IgnoreDataMember]
        public int? ParentItemId 
        {
            get { return PositionId; }
            
        }

        public int? ObjectId
        {
            get { return (int)EnumObjects.DictionaryPositionExecutors; }
        }

        [IgnoreDataMember]
        public bool IsUsed{ get; set; }

        public IEnumerable<ITreeItem> Childs { get; set; }
       
        public string Name
        {
            get { return AgentName; }
            set { AgentName = value; }
        }
        #endregion
    }
}