﻿using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using System.Collections.Generic;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;


namespace BL.Logic.DictionaryCore.Agent
{
    public class AddDictionaryAgentCommand : BaseDictionaryCommand
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
                var newAgent = new InternalDictionaryAgent
                {
                    Name = Model.Name,
                    IsBank=Model.IsBank,
                    IsIndividual=Model.IsIndividual,
                    IsCompany=Model.IsCompany,
                    IsEmployee=Model.IsEmployee,
                    Description=Model.Description,
                    IsActive = Model.IsActive,
                    ResidentTypeId=Model.ResidentTypeId ?? 0
                };
                CommonDocumentUtilities.SetLastChange(_context, newAgent);
                return _dictDb.AddDictionaryAgent(_context, newAgent);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
