using BL.Logic.Common;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryDocumentSubjectCommand : BaseDictionaryCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);
            return true;
        }

        public override object Execute()
        {
            _dictDb.DeleteDocumentSubject(_context, Model);
            return null;
        }
    }

}

