using BL.Model.SystemCore.InternalModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.InternalModel
{
    public class InternalDictionaryPositionWithActions
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public int? ExecutorAgentId { get; set; }
        public string DepartmentName { get; set; }
        public string ExecutorAgentName { get; set; }
        [IgnoreDataMember]
        public List<InternalSystemActionForDocument> Actions { get; set; }
        public List<InternalSystemActionCategoryForDocument> Categories { get; set; }

    }
}