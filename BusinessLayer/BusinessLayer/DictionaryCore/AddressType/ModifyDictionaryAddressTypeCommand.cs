using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;

namespace BL.Logic.DictionaryCore.DocumentType
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
            var spr = _dictDb.GetInternalDictionaryAddressType(_context, new FilterDictionaryAddressType { Name = Model.Name });
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
                var newAddrType = new InternalDictionaryAddressType
                {
                    Id = Model.Id,
                    Name = Model.Name,
                    IsActive=Model.IsActive
                };
                CommonDocumentUtilities.SetLastChange(_context, newAddrType);
                _dictDb.UpdateDictionaryAddressType(_context, newAddrType);
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