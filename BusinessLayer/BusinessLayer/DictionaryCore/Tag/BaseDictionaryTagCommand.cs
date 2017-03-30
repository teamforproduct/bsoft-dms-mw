using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryTagCommand : BaseDictionaryCommand
    {
        private AddTag Model { get { return GetModel<AddTag>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            Model.Name?.Trim();

            var filter = new FilterDictionaryTag
            {
                NameExact = Model.Name,
            };

            if(TypeModelIs<ModifyTag>())
            { filter.NotContainsIDs = new List<int> { GetModel<ModifyTag>().Id }; }

            var spr = _dictDb.GetInternalTags(_context, filter);
            if (spr.Any())
            {
                throw new DictionaryTagNotUnique(Model.Name);
            }

            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }
    }
}