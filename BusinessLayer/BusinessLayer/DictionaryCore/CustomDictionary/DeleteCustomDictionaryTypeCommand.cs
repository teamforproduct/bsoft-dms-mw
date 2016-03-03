using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;

namespace BL.Logic.DictionaryCore.CustomDictionary
{
    public class DeleteCustomDictionaryTypeCommand : BaseDictionaryCommand
    {
        private readonly IDictionariesDbProcess _dictDb;

        public DeleteCustomDictionaryTypeCommand(IDictionariesDbProcess dictDb)
        {
            _dictDb = dictDb;
        }

        private int Model
        {
            get
            {
                if (!(_param is int))
                {
                    throw new WrongParameterTypeError();
                }
                return (int)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId, InternalSystemAction action)
        {
            return true;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override object Execute()
        {
            try
            {
                _dictDb.DeleteCustomDictionaryType(_context, Model);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }

        public override EnumDictionaryAction CommandType { get { return EnumDictionaryAction.DeleteCustomDictionaryType; } }
    }
}