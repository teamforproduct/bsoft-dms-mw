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
    public class ModifyDictionaryContactCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryContact Model
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType,false,true);

            // У одного агента не должно быть два контакта одинакового типа
            var spr = _dictDb.GetContacts(_context, Model.AgentId,
                   new FilterDictionaryContact
                   {
                       NotContainsIDs = new List<int> { Model.Id },
                       ContactTypeIDs = new List<int> { Model.ContactTypeId },
                       AgentIDs = new List<int> { Model.AgentId }
                   });

            if (spr.Count() != 0)
            {
                throw new DictionaryRecordNotUnique(new System.Exception("Два контакта одинакового типа"));
            }

            // У одного агента не должно быть два контакта с одинаковыми значениями
            spr = _dictDb.GetContacts(_context, Model.AgentId,
                   new FilterDictionaryContact
                   {
                       NotContainsIDs = new List<int> { Model.Id },
                       ContactExact = Model.Value,
                       AgentIDs = new List<int> { Model.AgentId }
                   });

            if (spr.Count() != 0)
            {
                throw new DictionaryRecordNotUnique(new System.Exception("Два контакта с одинаковыми значениями"));
            }

            return true;
        }

        public override object Execute()
        {
            try
            {

                var newContact = new InternalDictionaryContact(Model);
                CommonDocumentUtilities.SetLastChange(_context, newContact);
                _dictDb.UpdateContact(_context, newContact);
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
