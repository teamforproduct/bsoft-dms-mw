using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using System.Collections.Generic;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Linq;
using BL.CrossCutting.Helpers;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryContactPersonCommand : BaseDictionaryCommand
    {
        private ModifyAgentContactPerson Model { get { return GetModel<ModifyAgentContactPerson>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            return true;
        }

        public override object Execute()
        {
            using (var transaction = Transactions.GetTransaction())
            {
                var person = _dictDb.GetInternalAgentPersons(_context, new FilterDictionaryAgentPerson { IDs = new List<int> { Model.Id } }).FirstOrDefault();

                if (person == null) throw new DictionaryRecordWasNotFound();

                person.AgentCompanyId = Model.CompanyId;
                person.Name = Model.Name;
                person.FirstName = Model.FirstName;
                person.LastName = Model.LastName;
                person.MiddleName = Model.MiddleName;
                person.IsMale = Model.IsMale;
                person.Description = Model.Description;
                person.IsActive = Model.IsActive;

                CommonDocumentUtilities.SetLastChange(_context, person);

                _dictService.ExecuteAction(BL.Model.Enums.EnumDictionaryActions.ModifyAgentPerson, _context, person);

                transaction.Complete();

            }
            return null;
        }

        private void AddContact(AddAgentContact model)
        {
            _dictService.ExecuteAction(BL.Model.Enums.EnumDictionaryActions.AddAgentContact, _context, model);
        }

    }
}
