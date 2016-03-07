using System.Collections.Generic;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.SystemCore.InternalModel;

namespace BL.Model.AdminCore
{
    public class DocumentActionsModel
    {
        public InternalDocument Document { get; set; }
        public List<InternalDictionaryPositionWithActions> PositionWithActions { get; set; }
        public Dictionary<int, List<InternalSystemAction>> ActionsList { get; set; }

    }
}
