﻿using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

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
            var agents = _dictDb.GetDictionaryAgentBanks(_context, new FilterDictionaryAgentBank
            {
                MFOCode = Model.MFOCode,
                IsActive=Model.IsActive,
                NotContainsId=new List<int> { Model.Id }
            },null);

            if (agents.Count() > 0)
            {
                throw new DictionaryRecordNotUnique();
            }
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
