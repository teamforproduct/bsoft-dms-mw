using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore
{
    public class AddCustomDictionaryTypeCommand : BaseCustomDictionaryTypeCommand
    {
        private AddCustomDictionaryType Model { get { return GetModel<AddCustomDictionaryType>(); } }

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