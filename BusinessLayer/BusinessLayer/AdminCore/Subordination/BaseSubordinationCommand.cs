using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
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
                var f = new FilterAdminSubordination();

                f.SourcePositionIDs = new List<int> { positionId };

                if (IsExecution & !IsInforming)
                {
                    f.SubordinationTypeIDs = new List<EnumSubordinationTypes> { EnumSubordinationTypes.Execution };
                }
                else if (IsInforming & !IsExecution)
                {
                    f.SubordinationTypeIDs = new List<EnumSubordinationTypes> { EnumSubordinationTypes.Informing };
                }

                _adminDb.DeleteSubordinations(_context, f);

                // -------------------

                f.SourcePositionIDs = null;
                f.TargetPositionIDs = new List<int> { positionId };

                if (IsExecution & !IsInforming)
                {
                    f.SubordinationTypeIDs = new List<EnumSubordinationTypes> { EnumSubordinationTypes.Execution };
                }
                else if (IsInforming & !IsExecution)
                {
                    f.SubordinationTypeIDs = new List<EnumSubordinationTypes> { EnumSubordinationTypes.Informing };
                }

                _adminDb.DeleteSubordinations(_context, f);

                //_adminDb.DeleteSubordinationsBySourcePositionId(_context, new InternalAdminSubordination { SourcePositionId = positionId });
                //_adminDb.DeleteSubordinationsByTargetPositionId(_context, new InternalAdminSubordination { TargetPositionId = positionId });
            }
        }

        public void SetDefaultSubordinations(int positionId, bool IsExecution, bool IsInforming)
        {
            // Удаляю существующую предустановку
            _adminDb.DeleteSubordinations(_context, new FilterAdminSubordination { PositionIDs = new List<int> { positionId } });

            //var position = _dictDb.GetInternalPositions(_context, new FilterDictionaryPosition { IDs = new List<int> { positionId } }).FirstOrDefault();


            //var depernment = _dictDb.GetInternalDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { position.DepartmentId } }).FirstOrDefault();

            //// должности в своем отделе
            //var positionsInDepartment = _dictDb.GetInternalPositions(_context, new FilterDictionaryPosition
            //{
            //    DepartmentIDs = new List<int> { depernment.Id },
            //});

            // должности в вышестоящем отделе
            //var parentDepernment = new InternalDictionaryDepartment();
            //var positionsInParentDepartment = new List<InternalDictionaryPosition>();

            //if (depernment.ParentId.HasValue)
            //{
            //    parentDepernment = _dictDb.GetInternalDepartments(_context, new FilterDictionaryDepartment { IDs = new List<int> { depernment.ParentId ?? -1 } }).FirstOrDefault();

            //    positionsInParentDepartment = _dictDb.GetInternalPositions(_context, new FilterDictionaryPosition
            //    {
            //        DepartmentIDs = new List<int> { parentDepernment.Id },
            //    }).ToList();
            //}

            var subordinations = new List<InternalAdminSubordination>();

            if (IsExecution)
            {
                // разрешаю выполнять рыссылку ОТ указанной должности для исполнения


                var childPositions = _dictService.GetChildPositions(_context, positionId);

                subordinations.AddRange(childPositions.Select(x => new InternalAdminSubordination()
                {
                    SourcePositionId = positionId,
                    TargetPositionId = x,
                    SubordinationTypeId = (int)EnumSubordinationTypes.Execution
                }));

                //// ниже по списку
                //var positions = positionsInDepartment.Where(x => x.Order > position.Order).Select(x => x.Id).ToList();

                //// в нижестоящих отделах, руководителям
                //positions.AddRange(positionsInChildDepartments.Where(x => x.Order == 1).Select(x => x.Id).ToList());

                //subordinations.AddRange(positions.Select(x => new InternalAdminSubordination()
                //{
                //    SourcePositionId = positionId,
                //    TargetPositionId = x,
                //    SubordinationTypeId = (int)EnumSubordinationTypes.Execution
                //}));

                //// Руководитель вышестоящего отдела НА указанную должность
                //if (positionsInParentDepartment.Count > 0)
                //{
                //    subordinations.Add(new InternalAdminSubordination()
                //    {
                //        SourcePositionId = positionsInParentDepartment.Where(x => x.Order == 1).Select(x => x.Id).FirstOrDefault(),
                //        TargetPositionId = positionId,
                //        SubordinationTypeId = (int)EnumSubordinationTypes.Execution
                //    });
                //}
            }

            if (IsInforming)
            {
                // разрешаю выполнять рыссылку ОТ указанной должности для сведения на всех

                var positions = _dictDb.GetInternalPositions(_context, new FilterDictionaryPosition());

                subordinations.AddRange(positions.Select(x => new InternalAdminSubordination()
                {
                    SourcePositionId = positionId,
                    TargetPositionId = x.Id,
                    SubordinationTypeId = (int)EnumSubordinationTypes.Informing
                }));

            }


            CommonDocumentUtilities.SetLastChange(_context, subordinations);

            _adminDb.AddSubordinations(_context, subordinations);
        }
    }

}