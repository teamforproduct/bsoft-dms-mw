using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.Exception;
using System.Collections.Generic;

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
            _dictDb.DeleteTags(_context, new FilterDictionaryTag { IDs = new List<int> { Model } });
            return null;
        }
    }

}

