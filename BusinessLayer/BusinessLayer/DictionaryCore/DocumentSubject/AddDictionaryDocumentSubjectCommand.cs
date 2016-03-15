using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;

using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;

namespace BL.Logic.DictionaryCore.DocumentType
{
    public class AddDictionaryDocumentSubjectCommand : BaseDictionaryCommand
    {
   
        private ModifyDictionaryDocumentSubject Model
        {
            get
            {
                if (!(_param is ModifyDictionaryDocumentSubject))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryDocumentSubject)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            var spr = _dictDb.GetDictionaryDocumentSubjects(_context, new FilterDictionaryDocumentSubject { Name = Model.Name });
            if (spr != null)
            {
                throw new DictionaryRecordNotUnique();
            }
            _admin.VerifyAccess(_context, CommandType);
            return true;
        }

        public override object Execute()
        {
            try
            {
                var dds = new InternalDictionaryDocumentSubject
                {
                    Name = Model.Name,
                    IsActive=Model.IsActive,
                    ParentId = Model.ParentId
                };
                CommonDocumentUtilities.SetLastChange(_context, dds);
                return _dictDb.AddDictionaryDocumentSubject(_context, dds);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}