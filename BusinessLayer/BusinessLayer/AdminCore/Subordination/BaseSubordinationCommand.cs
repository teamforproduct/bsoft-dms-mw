using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.AdminCore
{
    public class BaseSubordinationCommand : BaseAdminCommand
    {
        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }

        public void SetAllSubordinations(int positionId, bool IsChecked, bool IsExecution, bool IsInforming)
        {
            // Добавляю рассылку во все направления для сведения и исполнения для указанной должности
            try
            {
                if (IsChecked)
                {
                    // все должности которые и на которые может писать positionId
                    var existsSubordinarions = _adminDb.GetSubordinations(_context, new FilterAdminSubordination() { PositionIDs = new List<int> { positionId } });

                    // все должности
                    var positions = _dictDb.GetPositions(_context, new FilterDictionaryPosition()
                    { IsActive = true, NotContainsIDs = new List<int> { positionId } });

                    var subordinations = new List<InternalAdminSubordination>();

                    if (IsExecution)
                    {
                        // разрешаю выполнять рыссылку от указанной должности для исполнения
                        subordinations.AddRange(positions.Select(x => new InternalAdminSubordination()
                        {
                            SourcePositionId = positionId,
                            TargetPositionId = x.Id,
                            SubordinationTypeId = (int)EnumSubordinationTypes.Execution
                        }));
                    }

                    if (IsInforming)
                    {
                        // разрешаю выполнять рыссылку от указанной должности для сведения
                        subordinations.AddRange(positions.Select(x => new InternalAdminSubordination()
                        {
                            SourcePositionId = positionId,
                            TargetPositionId = x.Id,
                            SubordinationTypeId = (int)EnumSubordinationTypes.Informing
                        }));
                    }

                    if (IsExecution)
                    {
                        // разрешаю выполнять рыссылку на указанную должность для исполнения
                        subordinations.AddRange(positions.Select(x => new InternalAdminSubordination()
                        {
                            SourcePositionId = x.Id,
                            TargetPositionId = positionId,
                            SubordinationTypeId = (int)EnumSubordinationTypes.Execution
                        }));
                    }

                    if (IsInforming)
                    {
                        // разрешаю выполнять рыссылку на указанную должность для сведения
                        subordinations.AddRange(positions.Select(x => new InternalAdminSubordination()
                        {
                            SourcePositionId = x.Id,
                            TargetPositionId = positionId,
                            SubordinationTypeId = (int)EnumSubordinationTypes.Informing
                        }));
                    }

                    if (IsExecution)
                    {
                        subordinations.Add(new InternalAdminSubordination()
                        {
                            SourcePositionId = positionId,
                            TargetPositionId = positionId,
                            SubordinationTypeId = (int)EnumSubordinationTypes.Execution
                        });
                    }

                    if (IsInforming)
                    {
                        subordinations.Add(new InternalAdminSubordination()
                        {
                            SourcePositionId = positionId,
                            TargetPositionId = positionId,
                            SubordinationTypeId = (int)EnumSubordinationTypes.Informing
                        });
                    }

                    // нахожу в subordinations существующие записи и исключаю их
                    foreach (var item in existsSubordinarions)
                    {
                        var i = subordinations.Where(x =>
                        x.SourcePositionId == item.SourcePositionId &
                        x.TargetPositionId == item.TargetPositionId &
                        x.SubordinationTypeId == (int)item.SubordinationTypeId
                        ).FirstOrDefault();

                        subordinations.Remove(i);
                    }

                    CommonDocumentUtilities.SetLastChange(_context, subordinations);

                    _adminDb.AddSubordinations(_context, subordinations);
                }
                else
                {
                    _adminDb.DeleteSubordinationsBySourcePositionId(_context, new InternalAdminSubordination { SourcePositionId = positionId });
                    _adminDb.DeleteSubordinationsByTargetPositionId(_context, new InternalAdminSubordination { TargetPositionId = positionId });
                }
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }
        }
    }
}