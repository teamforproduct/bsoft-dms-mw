using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetDefaultSubordinationsCommand : BaseSubordinationCommand
    {
        private ModifyAdminDefaultSubordination Model
        {
            get
            {
                if (!(_param is ModifyAdminDefaultSubordination)) throw new WrongParameterTypeError();
                return (ModifyAdminDefaultSubordination)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                // Добавляю рассылку во все направления для сведения и исполнения для указанной должности
                var positions = _dictDb.GetPositions(_context, new BL.Model.DictionaryCore.FilterModel.FilterDictionaryPosition()
                { IsActive = true });

                foreach (var position in positions)
                {
                    // разрешаю рассылку на самого себя (самоконтроль)

                    // разрешаю выполнять рыссылку от указанной должности для исполнения
                    SetSubordination(new ModifyAdminSubordination()
                    {
                        SourcePositionId = Model.PositionId,
                        TargetPositionId = position.Id,
                        SubordinationTypeId = BL.Model.Enums.EnumSubordinationTypes.Execution,
                        IsChecked = true,
                    });
                    // разрешаю выполнять рыссылку от указанной должности для сведения
                    SetSubordination(new ModifyAdminSubordination()
                    {
                        SourcePositionId = Model.PositionId,
                        TargetPositionId = position.Id,
                        SubordinationTypeId = BL.Model.Enums.EnumSubordinationTypes.Informing,
                        IsChecked = true,
                    });

                    // разрешаю выполнять рыссылку на указанную должность для исполнения
                    SetSubordination(new ModifyAdminSubordination()
                    {
                        SourcePositionId = position.Id,
                        TargetPositionId = Model.PositionId,
                        SubordinationTypeId = BL.Model.Enums.EnumSubordinationTypes.Execution,
                        IsChecked = true,
                    });

                    // разрешаю выполнять рыссылку на указанную должность для сведения
                    SetSubordination(new ModifyAdminSubordination()
                    {
                        SourcePositionId = position.Id,
                        TargetPositionId = Model.PositionId,
                        SubordinationTypeId = BL.Model.Enums.EnumSubordinationTypes.Informing,
                        IsChecked = true,
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }

        }

        private void SetSubordination(ModifyAdminSubordination model)
        {
            _adminService.ExecuteAction(BL.Model.Enums.EnumAdminActions.SetSubordination, _context, model);
        }
    }
}