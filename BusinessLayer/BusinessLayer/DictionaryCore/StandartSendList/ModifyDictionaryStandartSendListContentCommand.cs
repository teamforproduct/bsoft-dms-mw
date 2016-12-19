using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryStandartSendListContentCommand : BaseDictionaryStandartSendListContentCommand
    {
        private ModifyStandartSendListContent Model { get { return GetModel<ModifyStandartSendListContent>(); } }

        public override object Execute()
        {
            try
            {
                var model = new InternalDictionaryStandartSendListContent(Model);
                CommonDocumentUtilities.SetLastChange(_context, model);
                _dictDb.UpdateStandartSendListContent(_context, model);
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
