using BL.CrossCutting.Helpers;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using System.Collections.Generic;

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
            using (var transaction = Transactions.GetTransaction())
            {
                _dictDb.DeleteCustomDictionaries(_context, new FilterCustomDictionary { TypeId = Model });
                _dictDb.DeleteCustomDictionaryType(_context, new FilterCustomDictionaryType { IDs = new List<int> { Model } });

                transaction.Complete();
            }
            return null;
        }
    }
}