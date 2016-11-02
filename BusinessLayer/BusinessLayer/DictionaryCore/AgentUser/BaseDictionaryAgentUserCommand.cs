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
    public class BaseDictionaryAgentUserCommand : BaseDictionaryCommand
    {
        protected ModifyDictionaryAgentUser Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentUser))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentUser)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
