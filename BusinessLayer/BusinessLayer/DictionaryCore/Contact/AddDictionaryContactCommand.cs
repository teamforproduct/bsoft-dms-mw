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
            var spr = _dictDb.GetContacts(_context,Model.AgentId, 
                   new FilterDictionaryContact {
                       Contact = Model.Value,
                       ContactTypeId = new List<int> { Model.ContactTypeId },
                       AgentId =new List<int> { Model.AgentId }
                   });

            if (spr.Count() !=0)
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
                return _dictDb.AddContact(_context, newContact);
            }
     
            catch (Exception ex)
            {
                
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
