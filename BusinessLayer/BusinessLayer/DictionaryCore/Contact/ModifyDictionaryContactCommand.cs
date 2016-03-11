using System;
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
            var spr = _dictDb.GetInternalDictionaryContact(_context,
                new FilterDictionaryContact { Value = Model.Value, ContactTypeId = Model.ContactType, AgentId = Model.AgentId });
            if (spr != null)
            {
                throw new DictionaryRecordNotUnique();
            }

            _admin.VerifyAccess(_context, CommandType);

            return true;
        }

        public override object Execute()
        {
            try
            {
                 
                var newContact = new InternalDictionaryContact
                {
                    AgentId=Model.AgentId,
                    ContactType=Model.ContactType,
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
