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
    public class ModifyDictionaryAddressTypeCommand : BaseDictionaryAddressTypeCommand
    {
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