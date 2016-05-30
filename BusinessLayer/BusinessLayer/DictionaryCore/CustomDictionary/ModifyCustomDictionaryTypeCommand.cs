using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyCustomDictionaryTypeCommand : BaseDictionaryCommand
    {
        private ModifyCustomDictionaryType Model
        {
            get
            {
                if (!(_param is ModifyCustomDictionaryType))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyCustomDictionaryType)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            var cdt = _dictDb.GetInternalCustomDictionaryType(_context, new FilterCustomDictionaryType { Code = Model.Code });
            if (cdt != null && cdt.Id != Model.Id)
            {
                throw new DictionaryRecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newItem = new InternalCustomDictionaryType(Model);
                CommonDocumentUtilities.SetLastChange(_context, newItem);
                _dictDb.UpdateCustomDictionaryType(_context, newItem);
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