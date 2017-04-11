using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Exception;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryAddressTypeCommand : BaseDictionaryCommand

    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            string specCode = _dictDb.GetAddressTypeSpecCode(_context, Model);

            if (!string.IsNullOrEmpty(specCode))
            {
                throw new DictionarySystemRecordCouldNotBeDeleted();
            }

            return true;
        }

        public override object Execute()
        {
            _dictDb.DeleteAddressTypes(_context, new FilterDictionaryAddressType { IDs = new List<int> { Model } });
            return null;
        }
    }

}

