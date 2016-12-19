using System;
using System.Collections.Generic;
using System.Linq;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryTagCommand : BaseDictionaryTagCommand
    {
        private ModifyTag Model { get { return GetModel<ModifyTag>(); } }
        public override object Execute()
        {
            try
            {
                var item = new InternalDictionaryTag(Model);
                CommonDocumentUtilities.SetLastChange(_context, item);
                _dictDb.UpdateTag(_context, item);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (DictionaryTagNotFoundOrUserHasNoAccess)
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