using BL.Model.DictionaryCore.FrontModel;
using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryPosition
    {

        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public int? ExecutorAgentId { get; set; }
        public int? LastChangeUserId { get; set; }
        public DateTime? LastChangeDate { get; set; }

        public string ParentPositionName { get; set; }
        public string DepartmentName { get; set; }
        public string ExecutorAgentName { get; set; }
        public int? MaxSubordinationTypeId { get; set; }
        public string PositionPhone { get; set; }

        public virtual IEnumerable<BaseDictionaryPosition> ChildPositions { get; set; }
        public virtual IEnumerable<FrontDictionaryDepartment> ChiefDepartments { get; set; }
        public virtual IEnumerable<BaseDictionaryStandartSendList> StandartSendLists { get; set; }
    }
}