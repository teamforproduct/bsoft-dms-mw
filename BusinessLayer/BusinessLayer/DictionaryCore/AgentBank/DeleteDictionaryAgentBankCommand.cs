﻿using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.DictionaryCore.AgentBank
{
    public class DeleteDictionaryAgentBankCommand : BaseDictionaryCommand
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

            _admin.VerifyAccess(_context, CommandType, false, true);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newBank = new InternalDictionaryAgentBank
                {
                    Id = Model

                };
                _dictDb.DeleteDictionaryAgentBank(_context, newBank);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }
}