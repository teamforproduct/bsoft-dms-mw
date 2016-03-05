using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.InternalModel;
using BL.Logic.AdminCore.Interfaces;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;

namespace BL.Logic.DictionaryCore.DocumentType
{
    public class DeleteDictionaryDocumentTypeCommand : BaseDictionaryCommand
   
    {
        private readonly IDictionariesDbProcess _dictDb;
        private readonly IAdminService _admin;

        public DeleteDictionaryDocumentTypeCommand(IDictionariesDbProcess dictDb, IAdminService admin)
        {
            _dictDb = dictDb;
            _admin = admin;
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

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }


        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType );
            return true;
        }

        public override object Execute()
        {
            try
            {
                var newDocType = new InternalDictionaryDocumentType
                {
                    Id = Model
                  
                };
                _dictDb.DeleteDictionaryDocumentType(_context, newDocType);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }
    }

}

