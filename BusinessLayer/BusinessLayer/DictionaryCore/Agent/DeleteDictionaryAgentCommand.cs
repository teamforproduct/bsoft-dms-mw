using BL.Logic.Common;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryAgentCommand : BaseDictionaryCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false, true);

            //                FrontDictionaryAgent tmp = _dictDb.GetAgent(_context, Model);

            // Удалить можно только контрагента без роли. 
            //            if (tmp != null)
            //            {
            //                if (tmp.IsBank || tmp.IsCompany || tmp.IsEmployee || tmp.IsIndividual)
            //                {
            //                    throw new DictionaryRecordCouldNotBeDeleted();
            //                }
            //            }   
            return true;
        }

        public override object Execute()
        {
            _dictDb.DeleteAgent(_context, Model);
            return null;
        }
    }
}
