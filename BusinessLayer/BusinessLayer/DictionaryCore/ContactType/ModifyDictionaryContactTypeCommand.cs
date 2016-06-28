using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryContactTypeCommand : BaseDictionaryCommand
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
            var spr = _dictDb.GetInternalDictionaryContactType(_context, new FilterDictionaryContactType
                { NameExact = Model.Name, Code=Model.Code,IsActive=Model.IsActive,NotContainsIDs = new List<int> {Model.Id} });
            if (spr != null)
            {
                throw new DictionaryRecordNotUnique();
            }

            _admin.VerifyAccess(_context, CommandType,false,true);

            return true;
        }

        public override object Execute()
        {
            try
            {
                var newContactType = new InternalDictionaryContactType(Model);
                CommonDocumentUtilities.SetLastChange(_context, newContactType);
                _dictDb.UpdateContactType(_context, newContactType);
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
