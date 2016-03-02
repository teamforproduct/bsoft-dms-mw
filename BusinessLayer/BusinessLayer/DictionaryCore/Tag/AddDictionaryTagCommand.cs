using System;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BaseDictionaryCommand = BL.Logic.Common.BaseDictionaryCommand;

namespace BL.Logic.DictionaryCore.Tag
{
    public class AddDictionaryTagCommand : BaseDictionaryCommand
    {
        private readonly IDictionariesDbProcess _dictDb;

        public AddDictionaryTagCommand(IDictionariesDbProcess dictDb)
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

        public override bool CanBeDisplayed(int positionId, InternalSystemAction action)
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
                    PositionId = _context.CurrentPositionId,
                    Color = Model.Color,
                    Name = Model.Name,
                    LastChangeDate = DateTime.Now,
                    LastChangeUserId = _context.CurrentAgentId,
                };
                return _dictDb.AddDictionaryTag(_context, item);
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeAdded(ex);
            }
        }

        public override EnumDictionaryAction CommandType { get { return EnumDictionaryAction.AddTag; } }
    }
}