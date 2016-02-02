using BL.Model.Enums;
using System;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore
{
    public class BaseDictionaryPosition
    {

        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public Nullable<int> ExecutorAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string ParentPositionName { get; set; }
        public string DepartmentName { get; set; }
        public string ExecutorAgentName { get; set; }

        public virtual IEnumerable<BaseDictionaryPosition> ChildPositions { get; set; }
        public virtual IEnumerable<BaseDictionaryDepartment> ChiefDepartments { get; set; }
        public virtual IEnumerable<BaseDictionaryStandartSendList> StandartSendLists { get; set; }
    }
}