﻿using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore.Agent
{
    public class ModifyDictionaryAgentCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgent Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgent))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgent)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType,false,true);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newAgent = new InternalDictionaryAgent(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAgent);
                _dictDb.UpdateDictionaryAgent(_context, newAgent);
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