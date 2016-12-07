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

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryContactPersonCommand : BaseDictionaryCommand
    {
        private AddContactPerson Model { get { return GetModel<AddContactPerson>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newPerson = new InternalDictionaryAgentPerson
                {
                    Id = Model.PersonId,
                    AgentCompanyId = Model.CompanyId
                };
                CommonDocumentUtilities.SetLastChange(_context, newPerson);
                _dictDb.UpdateAgentPersonCompanyId(_context, newPerson);

                return Model.PersonId;

            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

    }
}
