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
    public class AddDictionaryContactCommand : BaseDictionaryCommand
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
            _admin.VerifyAccess(_context, CommandType, false);

            // У одного агента не должно быть два контакта одинакового типа
            var spr = _dictDb.GetContacts(_context,Model.AgentId, 
                   new FilterDictionaryContact {
                       ContactTypeIDs = new List<int> { Model.ContactTypeId },
                       AgentIDs =new List<int> { Model.AgentId }
                   });

            if (spr.Count() !=0)
            {
                throw new DictionaryRecordNotUnique();
            }

            // У одного агента не должно быть два контакта с одинаковыми значениями
            spr = _dictDb.GetContacts(_context, Model.AgentId,
                   new FilterDictionaryContact
                   {
                       ContactExact = Model.Value,
                       ContactTypeIDs = new List<int> { Model.ContactTypeId },
                       AgentIDs = new List<int> { Model.AgentId }
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

                newContact.Value.Replace(" ",string.Empty);

                return _dictDb.AddContact(_context, newContact);
            }
     
            catch (Exception ex)
            {
                
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
