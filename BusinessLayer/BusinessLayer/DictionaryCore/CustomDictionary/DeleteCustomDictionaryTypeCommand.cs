using BL.Logic.Common;

namespace BL.Logic.DictionaryCore
{
    public class DeleteCustomDictionaryTypeCommand : BaseDictionaryCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            return true;
        }

        public override object Execute()
        {
            _dictDb.DeleteCustomDictionaryType(_context, Model);
            return null;
        }
    }
}