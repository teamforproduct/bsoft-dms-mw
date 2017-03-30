using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore
{
    public class DeleteCustomDictionaryCommand : BaseDictionaryCommand
    {
        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;

        public override bool CanExecute()
        {
            return true;
        }

        public override object Execute()
        {
            _dictDb.DeleteCustomDictionaries(_context, new FilterCustomDictionary { IDs = new List<int> { Model } });
            return null;
        }
    }
}