using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DocumentCore.InternalModel;
using System.Collections.Generic;

namespace BL.Model.DocumentCore.FrontModel
{
    public class FrontDocumentAction
    {
        public InternalDocument Document { get; set; }
        public List<InternalDictionaryPositionWithActions> PositionWithActions { get; set; }


    }
}
