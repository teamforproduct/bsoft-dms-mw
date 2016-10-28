using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.AdminCore.InternalModel;
using BL.Model.Exception;
using BL.Model.AdminCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;
using System.Transactions;
using BL.Model.SystemCore.Filters;
using System.Linq;
using BL.Database.DBModel.Admin;

namespace BL.Logic.AdminCore
{
    public class SetRoleActionByObjectCommand : BaseRoleActionCommand
    {
        private new ModifyAdminRoleActionByObject Model
        {
            get
            {
                if (!(_param is ModifyAdminRoleActionByObject)) throw new WrongParameterTypeError();
                return (ModifyAdminRoleActionByObject)_param;
            }
        }

        public override object Execute()
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {

                    if (Model.IsChecked)
                    {
                        var actions = _systemDb.GetSystemActions(_context, new FilterSystemAction()
                        {
                            ObjectIDs = new List<int> { Model.ObjectId },
                            IsGrantable = true,
                            IsVisible = true,
                            IsGrantableByRecordId = false,
                        });

                        if (actions.Count() > 0)
                        {
                            var checkedActions = _adminDb.GetRoleActions(_context, new FilterAdminRoleAction() { RoleIDs = new List<int> { Model.RoleId } });

                            var roleActions = new List<InternalAdminRoleAction>();

                            roleActions = actions.Where(x => !checkedActions.Select(y => y.ActionId).ToList().Contains(x.Id)).Select(c => new InternalAdminRoleAction()
                            {
                                ActionId = c.Id,
                                RoleId = Model.RoleId,
                            }).ToList();

                            if (roleActions.Count > 0)
                            {
                                CommonDocumentUtilities.SetLastChange(_context, roleActions);
                                _adminDb.AddRoleActions(_context, roleActions);
                            }
                        }

                    }
                    else
                    {
                        _adminDb.DeleteRoleActions(_context, new FilterAdminRoleAction() { RoleIDs = new List<int> { Model.RoleId } });
                    }


                    transaction.Complete();
                }

            }
            catch (AdminRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }

        private void AddRoleAction(ModifyAdminRoleAction model)
        {
            _adminService.ExecuteAction(BL.Model.Enums.EnumAdminActions.AddRoleAction, _context, model);
        }

        private void DeleteRoleAction(ModifyAdminRoleAction model)
        {
            _adminService.ExecuteAction(BL.Model.Enums.EnumAdminActions.DeleteRoleAction, _context, model);
        }
    }
}