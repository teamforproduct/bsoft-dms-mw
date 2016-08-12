using System;
using System.Collections.Generic;
using System.Linq;
using BL.Database.Dictionaries.Interfaces;
using BL.Logic.Common;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;


namespace BL.Logic.DictionaryCore
{
    public class ModifyDictionaryTagCommand : BaseDictionaryCommand
    {
        private readonly IDictionariesDbProcess _dictDb;

        public ModifyDictionaryTagCommand(IDictionariesDbProcess dictDb)
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
            var spr = _dictDb.GetTags(_context, new FilterDictionaryTag
            {
                NameExact = Model.Name,
                NotContainsIDs = new List<int> {Model.Id}
            });
            if (spr.Any())
            {
                throw new DictionaryRecordNotUnique();
            }
            return true;
        }

        public override object Execute()
        {
            try
            {
                var item = new InternalDictionaryTag(Model);
                CommonDocumentUtilities.SetLastChange(_context, item);
                _dictDb.UpdateTag(_context, item);
            }
            catch (DictionaryRecordWasNotFound)
            {
                throw;
            }
            catch (DictionaryTagNotFoundOrUserHasNoAccess)
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