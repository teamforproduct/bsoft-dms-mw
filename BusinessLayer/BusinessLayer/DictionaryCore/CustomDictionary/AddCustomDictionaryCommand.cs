using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class AddCustomDictionaryCommand : BaseCustomDictionaryCommand
    {
        private AddCustomDictionary Model { get { return GetModel<AddCustomDictionary>(); } }

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