using BL.Logic.Common;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore
{
    public class DeleteDictionaryTagCommand : BaseDictionaryCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            if (_dictDb.DocsWithTagCount(_context, Model) > 0)
            {
                throw new DictionaryRecordCouldNotBeDeleted();
            }
            return true;
        }

        public override object Execute()
        {
            _dictDb.DeleteTag(_context, Model);
            return null;
        }
    }

}

