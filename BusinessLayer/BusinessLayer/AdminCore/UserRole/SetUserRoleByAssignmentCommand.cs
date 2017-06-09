using BL.Logic.Common;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Enums;
using BL.Model.Exception;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.AdminCore
{
    public class SetUserRoleByAssignmentCommand : BaseAdminCommand
    {

        private ItemCheck Model { get { return GetModel<ItemCheck>(); } }

        public override bool CanBeDisplayed(int Id) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }


        public override object Execute()
        {
            var assignment = _dictDb.GetInternalPositionExecutors(_context, new FilterDictionaryPositionExecutor { IDs = new List<int> { Model.Id } }).FirstOrDefault();

            if (assignment == null) throw new DictionaryRecordWasNotFound();

            var roles = _adminDb.GetInternalPositionRoles(_context, new FilterAdminPositionRole { PositionIDs = new List<int> { assignment.PositionId } });

            if (roles.Count() > 0)
            {
                foreach (var role in roles)
                {
                    SetUserRole(new SetUserRole
                    {
                        IsChecked = Model.IsChecked,
                        PositionExecutorId = Model.Id,
                        Id = role.Id
                    });
                }
            }

            return null;

        }

        private void SetUserRole(SetUserRole model)
        {
            _adminService.ExecuteAction(EnumActions.SetUserRole, _context, model);
        }

    }
}