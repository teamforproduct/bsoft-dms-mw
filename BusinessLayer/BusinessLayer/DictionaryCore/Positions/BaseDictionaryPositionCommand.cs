using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using BL.Model.Enums;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryPositionCommand : BaseDictionaryCommand
    {
        private AddPosition Model { get { return GetModel<AddPosition>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            if (Model.Order < 1) Model.Order = int.MaxValue;

            //var fdd = new FilterDictionaryPosition { Name = Model.Name, NotContainsIDs = new List<int> { Model.Id } };

            //if (Model.ParentId != null)
            //{
            //    fdd.ParentIDs = new List<int> { Model.ParentId.Value };
            //}

            //// Находим запись с таким-же именем в этой-же папке
            //if (_dictDb.ExistsPosition(_context, fdd))
            //{
            //    throw new DictionaryRecordNotUnique();
            //}

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}