using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryAddressTypeCommand : BaseDictionaryAddressTypeCommand
    {
        private AddAddressType Model { get { return GetModel<AddAddressType>(); } }

        public override object Execute()
        {
            try
            {
                var newAddrType = new InternalDictionaryAddressType(Model);
                CommonDocumentUtilities.SetLastChange(_context, newAddrType);
                return _dictDb.AddAddressType(_context, newAddrType);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}