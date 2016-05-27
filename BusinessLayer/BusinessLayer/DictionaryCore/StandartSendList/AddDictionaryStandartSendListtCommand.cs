using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;


namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryStandartSendListCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryStandartSendList Model
        {
            get
            {
                if (!(_param is ModifyDictionaryStandartSendList))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryStandartSendList)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            var spr = _dictDb.GetStandartSendLists(_context, new FilterDictionaryStandartSendList
            {
               Name = Model.Name,
               PositionID = Model.PositionId,
               
            });
            if (spr != null)
            {
                throw new DictionaryRecordNotUnique();
            }

            return true;
        }

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
