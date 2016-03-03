using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;

namespace BL.Logic.DictionaryCore.CustomDictionary
{
    public class AddCustomDictionaryCommand : BaseDictionaryCommand
    {
        private readonly IDictionariesDbProcess _dictDb;

        public AddCustomDictionaryCommand(IDictionariesDbProcess dictDb)
        {
            _dictDb = dictDb;
        }

        private ModifyCustomDictionary Model
        {
            get
            {
                if (!(_param is ModifyCustomDictionary))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyCustomDictionary)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId, InternalSystemAction action)
        {
            return true;
        }

        public override bool CanExecute()
        {
            var cd = _dictDb.GetInternalCustomDictionary(_context, new FilterCustomDictionary { CustomDictionaryTypeId = new List<int> { Model.DictionaryTypeId }, Code = Model.Code });
            if (cd != null)
            {
                throw new DictionaryRecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newItem = new InternalCustomDictionary
                {
                    Code = Model.Code,
                    Description = Model.Description,
                    DictionaryTypeId = Model.DictionaryTypeId
                };
                CommonDocumentUtilities.SetLastChange(_context, newItem);
                return _dictDb.AddCustomDictionary(_context, newItem);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        public override EnumDictionaryAction CommandType { get { return EnumDictionaryAction.AddCustomDictionary; } }
    }
}