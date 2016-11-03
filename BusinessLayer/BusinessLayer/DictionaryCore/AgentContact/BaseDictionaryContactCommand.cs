using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryContactCommand : BaseDictionaryCommand
    {
        protected ModifyDictionaryContact Model
        {
            get
            {
                if (!(_param is ModifyDictionaryContact))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryContact)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType,false,true);

            Model.Value?.Trim();

            // У одного агента не должно быть два контакта одинакового типа
            var spr = _dictDb.GetContacts(_context,
                   new FilterDictionaryContact
                   {
                       NotContainsIDs = new List<int> { Model.Id },
                       ContactTypeIDs = new List<int> { Model.ContactTypeId },
                       AgentIDs = new List<int> { Model.AgentId }
                   });

            if (spr.Count() != 0) throw new DictionaryAgentContactTypeNotUnique(Model.AgentId.ToString(), Model.Value);

            // У одного агента не должно быть два контакта с одинаковыми значениями
            spr = _dictDb.GetContacts(_context, 
                   new FilterDictionaryContact
                   {
                       NotContainsIDs = new List<int> { Model.Id },
                       ContactExact = Model.Value,
                       AgentIDs = new List<int> { Model.AgentId }
                   });

            if (spr.Count() != 0) throw new DictionaryAgentContactNotUnique(Model.Value);
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}
