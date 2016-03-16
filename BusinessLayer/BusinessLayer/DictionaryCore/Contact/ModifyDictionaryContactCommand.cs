using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;


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

            return true;
        }

        public override object Execute()
        {
            try
            {
                 
                var newContact = new InternalDictionaryContact
                {
                    Id =Model.Id,
                    AgentId=Model.AgentId,
                    ContactTypeId=Model.ContactTypeId,
                    Value = Model.Value,
                    IsActive = Model.IsActive,
                    Description=Model.Description
                };
            
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
