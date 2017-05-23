using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAgentPersonExistingCommand : BaseDictionaryCommand
    {
        private AddAgentPersonExisting Model { get { return GetModel<AddAgentPersonExisting>(); } }

        public override object Execute()
        {
            var newPerson = new InternalDictionaryAgentPerson(Model);
            CommonDocumentUtilities.SetLastChange(_context, newPerson);
            _dictDb.AddAgentPersonToCompany(_context, newPerson);
            return null;
        }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            var person = _dictDb.GetInternalAgentPersons(_context, new FilterDictionaryAgentPerson { IDs = new List<int> { Model.PersonId } }).FirstOrDefault();

            if (person == null) throw new DictionaryRecordWasNotFound();

            if (person.AgentCompanyId.HasValue)
            {
                var company = _dictDb.GetAgentCompanies(_context, new FilterDictionaryAgentCompany { IDs = new List<int> { person.AgentCompanyId.Value } }, null).FirstOrDefault();
                throw new DictionaryAgentPersonCompanyExists(company.Name);
            }

            return true;
        }

    }
}
