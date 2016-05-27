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
    public class AddDictionaryAgentBankCommand : BaseDictionaryCommand
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
            var agents = _dictDb.GetAgentBanks(_context, new FilterDictionaryAgentBank
            {
                MFOCode = Model.MFOCode
            },null);

            if (agents.Count() > 0)
            {
                throw new DictionaryRecordNotUnique();
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newBank = new InternalDictionaryAgentBank(Model);
                CommonDocumentUtilities.SetLastChange(_context, newBank);
                return _dictDb.AddAgentBank(_context, newBank);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
