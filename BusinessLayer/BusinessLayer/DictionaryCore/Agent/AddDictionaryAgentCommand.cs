using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using System.Collections.Generic;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;


namespace BL.Logic.DictionaryCore
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

            if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent() { NameExact = Model.Name }))
            {
                throw new DictionaryAgentNameNotUnique();
            }

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newAgent = new InternalDictionaryAgent(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAgent);
                return _dictDb.AddAgent(_context, newAgent);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
