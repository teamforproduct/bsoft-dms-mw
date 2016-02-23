using System;
using BL.CrossCutting.Common;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.DictionaryCore;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;

namespace BL.Logic.DictionaryCore.Tag
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

        public override bool CanBeDisplayed()
        {
            return true;
        }

        public override bool CanExecute()
        {
            //TODO добавить проверки
            return true;
        }

        public override object Execute()
        {
            try
            {
                var item = new InternalDictionaryTag
                {
                    Id = Model.Id,
                    Name = Model.Name,
                    Color = Model.Color,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = _context.CurrentAgentId,
                };
                _dictDb.UpdateDictionaryTag(_context, item);
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

        public override EnumDictionaryAction CommandType
        {
            get { return EnumDictionaryAction.ModifyTag; }
        }
    }
}