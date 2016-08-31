using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Штатное расписание". 
    /// </summary>
    public class FrontDictionaryPosition : ModifyDictionaryPosition, ITreeItem
    {

        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Имя сотрудника на должности по штатному рассписанию
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// Ранг
        /// </summary>
        public int? Rang { get; set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public string ExecutorAgentName { get; set; }


        public string MainExecutorAgentName { get; set; }

        /// <summary>
        /// Наименование вышестоящей должности
        /// </summary>
        public string ParentPositionName { get; set; }

        public int? MaxSubordinationTypeId { get; set; }
        public string PositionPhone { get; set; }

        public virtual IEnumerable<FrontDictionaryPosition> ChildPositions { get; set; }
        public virtual IEnumerable<FrontDictionaryDepartment> ChiefDepartments { get; set; }
        public virtual IEnumerable<FrontDictionaryStandartSendList> StandartSendLists { get; set; }

        #region ITreeItem

        [IgnoreDataMember]
        public int? ParentItemId
        {
           get { return DepartmentId; }
        }

        public int? ObjectId
        {
            get { return (int)EnumObjects.DictionaryPositions; }
        }

        [IgnoreDataMember]
        public bool IsUsed { get; set; }

        public IEnumerable<ITreeItem> Childs { get; set; }
        
        #endregion

    }
}