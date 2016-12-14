using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class LinkDictionaryContactPersonCommand : BaseDictionaryCommand
    {
        private LinkDictionaryAgentContactPerson Model { get { return GetModel<LinkDictionaryAgentContactPerson>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType,false,true);


            return true;
        }

        public override object Execute()
        {
            try
            {
                var newPers = new InternalDictionaryAgentPerson
                {
                    Id = Model.PersonId,
                    AgentCompanyId = Model.CompanyId,
                };
                _dictDb.UpdateAgentPersonCompanyId(_context, newPers);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }
}
