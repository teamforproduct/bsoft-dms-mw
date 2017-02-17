using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryContactCommand : BaseDictionaryCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            return true;
        }

        public override object Execute()
        {
            _dictDb.DeleteContacts(_context, new FilterDictionaryContact { IDs = new List<int> { Model } });
            return null;
        }
    }
}
