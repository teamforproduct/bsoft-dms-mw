using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryContactPersonCommand : BaseDictionaryCommand
    {
        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }


        public override bool CanExecute()
        {

           // var agent = _dictDb.GetAgent(_context, Model);
           // if (agent.IsEmployee )
           // {
           //     throw new DictionaryRecordCouldNotBeDeleted();
           // }

            _adminService.VerifyAccess(_context, CommandType,false,true);


            return true;
        }

        public override object Execute()
        {
            try
            {
                var newPers = new InternalDictionaryAgentPerson
                {
                    Id = Model,
                    AgentCompanyId = null
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
