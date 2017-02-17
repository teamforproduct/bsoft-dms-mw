using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryAgentImageCommand : BaseDictionaryCommand
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
            var item = new InternalDictionaryAgentImage { Id = Model, Image = new byte[] { } };
            CommonDocumentUtilities.SetLastChange(_context, item);
            _dictDb.SetAgentImage(_context, item);
            return null;
        }
    }
}
