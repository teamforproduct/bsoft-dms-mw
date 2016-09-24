using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddCustomDictionaryCommand : BaseDictionaryCommand
    {
        private ModifyCustomDictionary Model
        {
            get
            {
                if (!(_param is ModifyCustomDictionary))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyCustomDictionary)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            //pss А где _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyCostomDictionary(_context, _dictDb, Model);
            
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newItem = new InternalCustomDictionary(Model);
                CommonDocumentUtilities.SetLastChange(_context, newItem);
                return _dictDb.AddCustomDictionary(_context, newItem);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}