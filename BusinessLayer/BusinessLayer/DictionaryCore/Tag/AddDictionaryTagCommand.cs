using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryTagCommand : BaseDictionaryTagCommand
    {
        private AddTag Model { get { return GetModel<AddTag>(); } }

        public override object Execute()
        {
            try
            {
                var item = new InternalDictionaryTag(Model);
                CommonDocumentUtilities.SetLastChange(_context, item);
                return _dictDb.AddTag(_context, item);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}