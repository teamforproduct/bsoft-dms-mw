using System;
using System.Collections.Generic;
using System.Linq;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;


namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryStandartSendListContentCommand : BaseDictionaryStandartSendListContentCommand
    {
        private AddStandartSendListContent Model { get { return GetModel<AddStandartSendListContent>(); } }

        public override object Execute()
        {
            try
            {
                var newCont = new InternalDictionaryStandartSendListContent(Model);
                CommonDocumentUtilities.SetLastChange(_context, newCont);
                return _dictDb.AddStandartSendListContent(_context, newCont);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
