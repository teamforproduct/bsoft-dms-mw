using System;
using System.Collections.Generic;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;


namespace BL.Logic.DictionaryCore.StandartSendList
{
    public class AddDictionaryStandartSendListContentCommand : BaseDictionaryCommand
    {
        private ModifyDictionaryStandartSendListContent Model
        {
            get
            {
                if (!(_param is ModifyDictionaryStandartSendListContent))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryStandartSendListContent)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            var spr = _dictDb.GetDictionaryStandartSendListContents(_context, new FilterDictionaryStandartSendListContent
            {
                TargetAgentId = Model.TargetAgentId,
                TargetPositionId = Model.TargetPositionId,
                SendTypeId = new List<EnumSendTypes> { Model.SendTypeId},
                IsActive = Model.IsActive
            
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
                var newCont = new InternalDictionaryStandartSendListContent()
                {
                    StandartSendListId=Model.StandartSendListId,
                    Stage = Model.Stage,
                    SendType = Model.SendTypeId,
                    TargetPositionId = Model.TargetPositionId,
                    TargetAgentId = Model.TargetAgentId,
                    Task = Model.Task,
                    Description = Model.Description,
                    DueDate = Model.DueDate,
                    DueDay = Model.DueDay,
                    AccessLevel = Model.AccessLevelId

                };
                CommonDocumentUtilities.SetLastChange(_context, newCont);
                return _dictDb.AddDictionaryStandartSendListContent(_context, newCont);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}
