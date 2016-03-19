using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Штатное расписание". 
    /// </summary>
    public class FrontDictionaryPosition : ModifyDictionaryPosition
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
        /// Наименование вышестоящей должности
        /// </summary>
        public string ParentPositionName { get; set; }

        public string ExecutorAgentName { get; set; }

        public int? MaxSubordinationTypeId { get; set; }
        public string PositionPhone { get; set; }

        public virtual IEnumerable<FrontDictionaryPosition> ChildPositions { get; set; }
        public virtual IEnumerable<FrontDictionaryDepartment> ChiefDepartments { get; set; }
        public virtual IEnumerable<BaseDictionaryStandartSendList> StandartSendLists { get; set; }

    }
}