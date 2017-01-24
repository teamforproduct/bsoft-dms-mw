using System;
using System.Collections.Generic;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DocumentCore.FrontModel;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalAdditinalLinkedDocumentSendListsPrepare
    {
        public IEnumerable<FrontDocumentAccess> Accesses { get; set; }
        public IEnumerable<FrontDictionaryPosition> Positions { get; set; }
        public IEnumerable<FrontDocument> Documents { get; set; }
        public string SendTypeName { get; set; }
    }
}
