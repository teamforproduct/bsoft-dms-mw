using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;
using System;
using BL.Model.Enums;

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

        public int? ParentId
        {
            get { return PositionId; }
        }

        public int ObjectId
        {
            get { return (int)EnumObjects.DictionaryPositionExecutors; }
        }

        public ITreeItem Parent
        { 
            get { return null; }
        }

        public IEnumerable<ITreeItem> Childs { get; set; }
       
        public string Name
        {
            get { return AgentName; }
            set { AgentName = value; }
        }
        #endregion
    }
}