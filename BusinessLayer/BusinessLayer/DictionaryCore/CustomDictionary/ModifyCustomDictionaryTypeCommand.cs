using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class ModifyCustomDictionaryTypeCommand : BaseCustomDictionaryTypeCommand
    {
        private ModifyCustomDictionaryType Model { get { return GetModel<ModifyCustomDictionaryType>(); } }

        public override object Execute()
        {
            try
            {
                var newItem = new InternalCustomDictionaryType(Model);
                CommonDocumentUtilities.SetLastChange(_context, newItem);
                _dictDb.UpdateCustomDictionaryType(_context, newItem);
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