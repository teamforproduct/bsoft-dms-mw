using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

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
            var spr = _dictDb.GetInternalDictionaryContact(_context, 
                   new FilterDictionaryContact { Value = Model.Value, ContactTypeId=Model.ContactType, AgentId=Model.AgentId });
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
                return _dictDb.AddDictionaryContact(_context, newContact);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
