using BL.CrossCutting.Helpers;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.AdminCore
{
    public class SetJournalAccessDefault_JournalCommand : BaseJournalAccessCommand
    {
        private SetJournalAccessDefault_Journal Model { get { return GetModel<SetJournalAccessDefault_Journal>(); } }

        public override object Execute()
        {
            var journal = _dictDb.GetInternalRegistrationJournals(_context, new BL.Model.DictionaryCore.FilterModel.FilterDictionaryRegistrationJournal { IDs = new List<int> { Model.JournalId } }).FirstOrDefault();

            if (journal == null) return null;

            // даю доступ на регистрацию всем должностям отдела, которому принадлежит журнал

            using (var transaction = Transactions.GetTransaction())
            {

                _adminDb.DeleteRegistrationJournalPositions(_context, new FilterAdminRegistrationJournalPosition() { RegistrationJournalIDs = new List<int> { Model.JournalId } });

                //SetRegistrationJournalPositionByDepartment(new ModifyAdminRegistrationJournalPositionByDepartment
                //{
                //    DepartmentId = position.DepartmentId,
                //    IsChecked = true,
                //    PositionId = Model.PositionId,
                //    RegJournalAccessTypeId = EnumRegistrationJournalAccessTypes.View,
                //    IgnoreChildDepartments = true,
                //});

                SetRegistrationJournalByDepartment(new SetJournalAccessByDepartment_Journal
                {
                    DepartmentId = journal.DepartmentId,
                    IsChecked = true,
                    JournalId = Model.JournalId,
                    RegJournalAccessTypeId = EnumRegistrationJournalAccessTypes.Registration,
                    IgnoreChildDepartments = true,
                });

                transaction.Complete();

            }

            return null;

        }

        protected void SetRegistrationJournalByDepartment(SetJournalAccessByDepartment_Journal model)
        {
            _adminService.ExecuteAction(EnumActions.SetJournalAccessByDepartment_Journal, _context, model);
        }
    }
}