using System;
using BL.Logic.Common;
using BL.Model.Exception;
using BL.Database.SystemDb;
using BL.Model.SystemCore.InternalModel;
using BL.Model.Enums;

namespace BL.Logic.PropertyCore.Commands
{
    public class DeletePropertyCommand : BasePropertyCommand
   
    {
        private readonly ISystemDbProcess _systDb;

        public DeletePropertyCommand(ISystemDbProcess systDb)
        {
            _systDb = systDb;
        }

        private int Model { get { return GetModel<int>(); } }

        public override bool CanBeDisplayed(int positionId) => true;


        public override bool CanExecute()
        {
            //TODO: Проверка возможности удаления записи
            return true;
        }

        public override object Execute()
        {
            try
            {
                var item = new InternalProperty
                {
                    Id = Model
                };
                _systDb.DeleteProperty(_context, item);
                return null;
            }
            catch (Exception ex)
            {
                throw new DictionaryRecordCouldNotBeDeleted(ex);
            }
        }

        public override EnumPropertyActions CommandType => EnumPropertyActions.DeleteProperty;
    }

}

