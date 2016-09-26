using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore
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
            //pss А где _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyCostomDictionaryType(_context, _dictDb, Model);

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newItem = new InternalCustomDictionaryType(Model);
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