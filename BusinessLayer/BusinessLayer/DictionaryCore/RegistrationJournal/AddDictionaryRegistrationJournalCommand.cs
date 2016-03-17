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
    public class AddDictionaryRegistrationJournalCommand : BaseDictionaryCommand
    {
   
        private ModifyDictionaryRegistrationJournal Model
        {
            get
            {
                if (!(_param is ModifyDictionaryRegistrationJournal))
                {
                    throw new WrongParameterTypeError();
                }
                return (ModifyDictionaryRegistrationJournal)_param;
            }
        }

        public override bool CanBeDisplayed(int positionId)
        {
            return true;
        }

        public override bool CanExecute()
        {
            _admin.VerifyAccess(_context, CommandType, false);
            // Находим запись с таким-же именем и индексом журнала в этом-же подразделении
            if (_dictDb.ExistsDictionaryRegistrationJournal(_context, new FilterDictionaryRegistrationJournal { Name = Model.Name, Index = Model.Index, DepartmentIDs = new List<int> { Model.DepartmentId} }))
            {
                throw new DictionaryRecordNotUnique();
            }
         
            return true;
        }

        public override object Execute()
        {
            try
            {
                var drj = new InternalDictionaryRegistrationJournal();

                // pss Перегонка значений DictionaryRegistrationJournals
                CommonDictionaryUtilities.RegistrationJournalModifyToInternal(_context, Model, drj);

                return _dictDb.AddDictionaryRegistrationJournal(_context, drj);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }
    }
}