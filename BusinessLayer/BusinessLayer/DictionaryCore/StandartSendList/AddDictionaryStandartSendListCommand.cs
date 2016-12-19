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
    public class AddDictionaryStandartSendListCommand : BaseDictionaryStandartSendListCommand
    {
        private AddStandartSendList Model { get { return GetModel<AddStandartSendList>(); } }

        public override object Execute()
        {
            try
            {
                var newList = new InternalDictionaryStandartSendList()
                {
                    Name = Model.Name,
                    PositionId = Model.PositionId
                    
                };
                CommonDocumentUtilities.SetLastChange(_context, newList);
                return _dictDb.AddStandartSendList(_context, newList);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
