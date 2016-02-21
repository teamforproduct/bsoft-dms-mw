using BL.Model.SystemCore;
using System.Collections.Generic;

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
        public IEnumerable<InternalSystemAction> Actions { get; set; }
    }
}