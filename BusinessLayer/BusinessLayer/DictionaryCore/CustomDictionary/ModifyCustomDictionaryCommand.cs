using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class ModifyCustomDictionaryCommand : BaseDictionaryCommand
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
            var cd = _dictDb.GetInternalCustomDictionary(_context, new FilterCustomDictionary { IDs = new List<int> { Model.DictionaryTypeId }, Code = Model.Code });
            if (cd != null && cd.Id != Model.Id)
            {
                throw new DictionaryRecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newItem = new InternalCustomDictionary(Model);
                CommonDocumentUtilities.SetLastChange(_context, newItem);
                _dictDb.UpdateCustomDictionary(_context, newItem);
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