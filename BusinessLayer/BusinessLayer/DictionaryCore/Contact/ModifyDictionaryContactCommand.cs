using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Linq;

namespace BL.Logic.DictionaryCore.Contact
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
            var spr = _dictDb.GetDictionaryContacts(_context, Model.AgentId,
                   new FilterDictionaryContact
                   {
                       Contact = Model.Value,
                       ContactTypeId = new List<int> { Model.ContactTypeId },
                       AgentId = new List<int> { Model.AgentId },
                       IsActive=Model.IsActive
                   });

            if (spr.Count() != 0)
            {
                throw new DictionaryRecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            try
            {

                var newContact = new InternalDictionaryContact(Model);
                CommonDocumentUtilities.SetLastChange(_context, newContact);
                _dictDb.UpdateDictionaryContact(_context, newContact);
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
