using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;
using System.Linq;

namespace BL.Logic.DictionaryCore
{
    public class BaseDictionaryAgentBankCommand : BaseDictionaryCommand
    {
        private AddAgentBank Model { get { return GetModel<AddAgentBank>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            Model.Name?.Trim();
            Model.MFOCode?.Trim();

            //if (_dictDb.ExistsAgents(_context, new FilterDictionaryAgent()
            //{
            //    NameExact = Model.Name,
            //    NotContainsIDs = new List<int> { Model.Id }
            //}))
            //{
            //    throw new DictionaryAgentNameNotUnique(Model.Name);
            //}

            if (!string.IsNullOrEmpty(Model.MFOCode))
            {
                var filter = new FilterDictionaryAgentBank
                {
                    MFOCodeExact = Model.MFOCode,
                };

                if (TypeModelIs<ModifyAgentBank>())
                { filter.NotContainsIDs = new List<int> { GetModel<ModifyAgentBank>().Id }; }

                if (_dictDb.ExistsAgentBanks(_context, filter))
                {
                    throw new DictionaryAgentBankMFOCodeNotUnique(Model.Name, Model.MFOCode);
                }
            }
            return true;
        }

        public override object Execute()
        { throw new NotImplementedException(); }

    }
}
