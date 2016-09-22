using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
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
            if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent() { NameExact = Model.Name, NotContainsIDs = new List<int> { Model.Id } }))
            {
                throw new DictionaryAgentNameNotUnique();
            }

            if (!string.IsNullOrEmpty(Model.MFOCode))
            {
                if (_dictDb.ExistsAgentBanks(_context, new FilterDictionaryAgentBank { MFOCodeExact = Model.MFOCode, NotContainsIDs = new List<int> { Model.Id } }))
                {
                    throw new DictionaryAgentBankMFOCodeNotUnique();
                }
            }

            _admin.VerifyAccess(_context, CommandType, false, true);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newBank = new InternalDictionaryAgentBank(Model);
                CommonDocumentUtilities.SetLastChange(_context, newBank);
                _dictDb.UpdateAgentBank(_context, newBank);
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
