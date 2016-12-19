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
    public class ModifyDictionaryStandartSendListCommand : BaseDictionaryStandartSendListCommand
    {
        private ModifyStandartSendList Model { get { return GetModel<ModifyStandartSendList>(); } }

        public override object Execute()
        {
            try
            {
                var newList = new InternalDictionaryStandartSendList(Model);
                CommonDocumentUtilities.SetLastChange(_context, newList);
                _dictDb.UpdateStandartSendList(_context, newList);


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
