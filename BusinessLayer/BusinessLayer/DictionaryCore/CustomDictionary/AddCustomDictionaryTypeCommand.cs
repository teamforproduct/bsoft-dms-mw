using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore.CustomDictionary
{
    public class AddCustomDictionaryTypeCommand : BaseDictionaryCommand
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
            if (cdt != null)
            {
                throw new DictionaryRecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newItem = new InternalCustomDictionaryType
                {
                    Code = Model.Code,
                    Description = Model.Description
                };
                CommonDocumentUtilities.SetLastChange(_context, newItem);
                return _dictDb.AddCustomDictionaryType(_context, newItem);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}