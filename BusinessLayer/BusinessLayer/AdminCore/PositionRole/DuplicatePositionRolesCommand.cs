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
    public class DuplicatePositionRolesCommand : BasePositionRoleCommand
    {
        private CopyAdminSubordinations Model
        {
            get
            {
                if (!(_param is CopyAdminSubordinations)) throw new WrongParameterTypeError();
                return (CopyAdminSubordinations)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                if (Model.CopyMode == BL.Model.Enums.EnumCopyMode.Сoverage)
                {
                    // ощищаю настроки для Model.TargetPositionId
                    _adminDb.DeletePositionRoles(_context, new FilterAdminPositionRole() { PositionIDs = new List<int> { Model.TargetPositionId } });
                }

                // выгребаю все настройки для Model.SourcePosition
                var items = _adminDb.GetInternalPositionRoles(_context, new FilterAdminPositionRole() { PositionIDs = new List<int> { Model.SourcePositionId } });

                // добавляю 
                foreach (var item in items)
                {
                    // подменил SourcePosition
                    var model = new ModifyAdminPositionRole()
                    {
                        Id = item.Id,
                        PositionId = Model.TargetPositionId,
                        RoleId = item.RoleId,
                    };

                    if (!_adminDb.ExistsPositionRole(_context, new FilterAdminPositionRole()
                    {
                        PositionIDs = new List<int> { model.PositionId },
                        RoleIDs = new List<int> { model.RoleId },
                    }))
                        _adminDb.AddPositionRole(_context, CommonAdminUtilities.PositionRoleModifyToInternal(_context, model));
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new AdminRecordCouldNotBeAdded(ex);
            }

        }
    }
}