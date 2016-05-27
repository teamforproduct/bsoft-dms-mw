using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAgentPersonCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryAgentPerson Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAgentPerson))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAgentPerson)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
             _admin.VerifyAccess(_context, CommandType,false,true);
            var agents = _dictDb.GetAgentPersons(_context, new FilterDictionaryAgentPerson
            {
                TaxCode = Model.TaxCode,
                IsActive=Model.IsActive,
                NotContainsIDs = new List<int> { Model.Id}
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
                var newPerson = new InternalDictionaryAgentPerson(Model);
                CommonDocumentUtilities.SetLastChange(_context, newPerson);
                _dictDb.UpdateAgentPerson(_context, newPerson);
                

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
