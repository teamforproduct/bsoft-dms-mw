using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class DuplicateSubordinationsCommand : BaseSubordinationCommand
    {
        private CopyAdminSettingsByPosition Model
        {
            get
            {
                if (!(_param is CopyAdminSettingsByPosition)) throw new WrongParameterTypeError();
                return (CopyAdminSettingsByPosition)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                if (Model.CopyMode == BL.Model.Enums.EnumCopyMode.Сoverage)
                {
                    // ощищаю настроки для Model.TargetPositionId
                    _adminDb.DeleteSubordinationsBySourcePositionId(_context, new InternalAdminSubordination() { SourcePositionId = Model.TargetPositionId });
                }

                // Для копирования беру только те должности на которые может выполнять рассылку
                // выгребаю все настройки для Model.SourcePosition
                var items = _adminDb.GetSubordinations(_context, new FilterAdminSubordination() { SourcePositionIDs = new List<int> { Model.SourcePositionId } });

                // добавляю 
                foreach (var item in items)
                {
                    // подменил SourcePosition
                    var model = new SetSubordination()
                    {
                        SourcePositionId = Model.TargetPositionId,
                        TargetPositionId = item.TargetPositionId,
                        SubordinationTypeId = item.SubordinationTypeId,
                        IsChecked = true,
                    };

                    SetSubordination(model);

                }

                return null;
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }

        }

        private void SetSubordination(SetSubordination model)
        {
            _adminService.ExecuteAction(EnumAdminActions.SetSubordination, _context, model);
        }
    }
}