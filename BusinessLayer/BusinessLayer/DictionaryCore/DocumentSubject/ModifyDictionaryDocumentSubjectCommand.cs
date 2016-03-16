using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Exception;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.SystemCore;
using System.Collections.Generic;


namespace BL.Logic.DictionaryCore.DocumentType
{
    public class ModifyDictionaryDocumentSubjectCommand : BaseDictionaryCommand
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
            // Находим запись с таким-же именем в этой-же папке
            // Устно договорились НЕ упроцедуривать new FilterDictionaryDocumentSubject { Name = Model.Name, ParentId = Model.ParentId, NotDocumentSubjectId = new List<int> { Model.Id }} в Modify и Add коммандах.
            

            //var spr = _dictDb.GetDictionaryDocumentSubjects(_context, new FilterDictionaryDocumentSubject { Name = Model.Name, ParentId = Model.ParentId, NotContainsId = new List<int> {Model.Id}}) ;
            // если запрос пуст - то лист все равно будет создан, но пустой
            // if (spr != null && spr?.Count() == 0)


            if (_dictDb.ExistsDictionaryDocumentSubject(_context, new FilterDictionaryDocumentSubject { Name = Model.Name, ParentId = Model.ParentId, NotContainsId = new List<int> { Model.Id } }))
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
                var dds = new InternalDictionaryDocumentSubject();

                CommonDictionaryUtilities.DocumentSubjectModifyToInternal(_context, Model, dds);

                _dictDb.UpdateDictionaryDocumentSubject(_context, dds);
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