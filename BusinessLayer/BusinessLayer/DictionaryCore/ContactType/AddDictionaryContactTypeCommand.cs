using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore.ContactType
{
    public class AddDictionaryContactTypeCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryContactType Model
        {
            get
            {
                if (!(_param is ModifyDictionaryContactType))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryContactType)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            var spr = _dictDb.GetInternalDictionaryContactType(_context, 
                   new FilterDictionaryContactType { Name = Model.Name });
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
                var newContactType = new InternalDictionaryContactType
                {
                    Id = Model.Id,
                    Name = Model.Name,
                    InputMask=Model.InputMask
                };
                CommonDocumentUtilities.SetLastChange(_context, newContactType);
                return _dictDb.AddDictionaryContactType(_context, newContactType);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
