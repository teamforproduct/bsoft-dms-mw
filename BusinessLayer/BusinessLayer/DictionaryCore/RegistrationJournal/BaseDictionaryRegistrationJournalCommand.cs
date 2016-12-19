using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryRegistrationJournalCommand : BaseDictionaryCommand
    {
        private AddRegistrationJournal Model { get { return GetModel<AddRegistrationJournal>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();

            var filter = new FilterDictionaryRegistrationJournal
            {
                NameExact = Model.Name,
                IndexExact = Model.Index,
                DepartmentIDs = new List<int> { Model.DepartmentId },
            };

            if (TypeModelIs<ModifyRegistrationJournal>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyRegistrationJournal>().Id }; }

            // Находим запись с таким-же именем и индексом журнала в этом-же подразделении
            if (_dictDb.ExistsDictionaryRegistrationJournal(_context, filter))
            {
                throw new DictionaryRegistrationJournalNotUnique(Model.Name);
            }

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}