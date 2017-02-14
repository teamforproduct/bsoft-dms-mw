using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentPersonCommand : BaseDictionaryAgentPersonCommand
    {
        private AddAgentPerson Model { get { return GetModel<AddAgentPerson>(); } }

        public override object Execute()
        {
            var newPerson = new InternalDictionaryAgentPerson(Model);
            CommonDocumentUtilities.SetLastChange(_context, newPerson);
            return _dictDb.AddAgentPerson(_context, newPerson);
        }

    }
}
