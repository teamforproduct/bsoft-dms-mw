﻿using System;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;

namespace BL.Logic.DictionaryCore.DocumentType
{
    public class ModifyDictionaryDocumentTypeCommand : BaseDictionaryCommand
    {
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {

            _admin.VerifyAccess(_context, CommandType, false);

            var spr = _dictDb.GetInternalDictionaryDocumentType(_context, new FilterDictionaryDocumentType { Name = Model.Name,IsActive=Model.IsActive });
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
                var newDocType = new InternalDictionaryDocumentType(Model);
                CommonDocumentUtilities.SetLastChange(_context, newDocType);
                _dictDb.UpdateDocumentType(_context, newDocType);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseError(ex);
            }
            return null;
        }
    }
}