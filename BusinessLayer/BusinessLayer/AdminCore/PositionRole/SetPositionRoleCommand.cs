using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;

namespace BL.Logic.AdminCore
{
    public class SetPositionRoleCommand : BaseAdminCommand
    {

        private SetAdminPositionRole Model { get { return GetModel<SetAdminPositionRole>(); } }

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }


        public override object Execute()
        {
            var model = new InternalAdminPositionRole(Model);

            CommonDocumentUtilities.SetLastChange(_context, model);

            var exists = _adminDb.ExistsPositionRole(_context, new FilterAdminPositionRole
            {
                PositionIDs = new List<int> { Model.PositionId },
                RoleIDs = new List<int> { Model.RoleId }
            });

            if (!exists && Model.IsChecked)
            {
                _adminDb.AddPositionRole(_context, model);
            }
            else if (exists && !Model.IsChecked)
            {
                _adminDb.DeletePositionRoles(_context, new FilterAdminPositionRole { RoleIDs = new List<int> { model.RoleId }, PositionIDs = new List<int> { model.PositionId } });

                // При удалении роли у должности, удаляю роль у сотрудников, которые унасленовали ее от этой должности
                _adminDb.DeleteUserRoles(_context, new BL.Model.AdminCore.FilterModel.FilterAdminUserRole()
                {
                    PositionIDs = new List<int> { Model.PositionId },
                    RoleIDs = new List<int> { Model.RoleId }
                });
            }

            return null;

        }

    }
}