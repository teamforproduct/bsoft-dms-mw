using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using System;
using System.Linq;
using BL.Model.DictionaryCore.FilterModel;
using BaseDictionaryCommand = BL.Logic.Common.BaseDictionaryCommand;

namespace BL.Logic.DictionaryCore
{
    public class AddDictionaryTagCommand : BaseDictionaryCommand
    {
        private readonly IDictionariesDbProcess _dictDb;

        public AddDictionaryTagCommand(IDictionariesDbProcess dictDb)
        {
            _dictDb = dictDb;
        }

        private ModifyDictionaryTag Model
        {
            get
            {
                if (!(_param is ModifyDictionaryTag))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryTag)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _adminService.VerifyAccess(_context, CommandType, false);

            DictionaryModelVerifying.VerifyTag(_context, _dictDb, Model);

            return true;
         
        }

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