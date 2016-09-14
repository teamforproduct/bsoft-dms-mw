using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryAddressTypeCommand : BaseDictionaryCommand
    {
       
        private ModifyDictionaryAddressType Model
        {
            get
            {
                if (!(_param is ModifyDictionaryAddressType))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryAddressType)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            var spr = _dictDb.GetInternalDictionaryAddressType(_context, new FilterDictionaryAddressType
            {
                Name = Model.Name,
                IsActive = Model.IsActive,
                NotContainsIDs = new List<int> { Model.Id }
            });
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
                var newAddrType = new InternalDictionaryAddressType(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAddrType);
                _dictDb.UpdateAddressType(_context, newAddrType);
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