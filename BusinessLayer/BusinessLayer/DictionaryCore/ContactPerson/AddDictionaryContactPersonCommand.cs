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
    public class AddDictionaryContactPersonCommand : BaseDictionaryCommand
    {
        private AddAgentContactPerson Model { get { return GetModel<AddAgentContactPerson>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            return true;
        }

        public override object Execute()
        {
            int person = -1;

            using (var transaction = Transactions.GetTransaction())
            {
                var newPerson = new InternalDictionaryAgentPerson(Model);

                CommonDocumentUtilities.SetLastChange(_context, newPerson);

                person = _dictDb.AddAgentPerson(_context, newPerson);

                transaction.Complete();

            }

            return person;
        }

        private void AddContact(AddAgentContact model)
        {
            _dictService.ExecuteAction(BL.Model.Enums.EnumDictionaryActions.AddAgentContact, _context, model);
        }

    }
}
