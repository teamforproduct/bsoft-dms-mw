using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore.AgentBank
{
    public class ModifyDictionaryAgentBankCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgentBank Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentBank))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentBank)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false, true);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newBank = new InternalDictionaryAgentBank
                {
                    Id = Model.Id,
                    MFOCode=Model.MFOCode,
                    Swift=Model.Swift,
                    Name=Model.Name,
                    IsActive = Model.IsActive,
                    Description = Model.Description
                };
                CommonDocumentUtilities.SetLastChange(_context, newBank);
                _dictDb.UpdateDictionaryAgentBank(_context, newBank);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}
