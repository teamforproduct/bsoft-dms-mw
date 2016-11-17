using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryAgentCommand : BaseDictionaryCommand
    {
        protected ModifyDictionaryAgent Model
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

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType,false,true);

            Model.Name?.Trim();

            //if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent()
            //{
            //    NameExact = Model.Name,
            //    NotContainsIDs = new List<int> { Model.Id }
            //}))
            //{
            //    throw new DictionaryAgentNameNotUnique(Model.Name);
            //}

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
