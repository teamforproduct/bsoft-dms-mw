﻿using System;
using BL.CrossCutting.Common;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore.DocumentType
{
    public class AddDictionaryDocumentTypeCommand : BaseDictionaryCommand
    {
        private readonly IDictionariesDbProcess _dictDb;

        public AddDictionaryDocumentTypeCommand(IDictionariesDbProcess dictDb)
        {
            _dictDb = dictDb;
        }

        private ModifyDictionaryDocumentType Model
        {
            get
            {
                if (!(_param is ModifyDictionaryDocumentType))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryDocumentType)_param;
            }
        }

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            var spr = _dictDb.GetInternalDictionaryDocumentType(_context, new FilterDictionaryDocumentType { Name = Model.Name });
            if (spr != null)
            {
                throw new DictionaryRecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newDocType = new InternalDictionaryDocumentType
                {
                    Name = Model.Name,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = _context.CurrentAgentId,
                };
                return _dictDb.AddDictionaryDocumentType(_context, newDocType);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        public override EnumDictionaryAction CommandType { get { return EnumDictionaryAction.AddDocumentType; } }
    }
}