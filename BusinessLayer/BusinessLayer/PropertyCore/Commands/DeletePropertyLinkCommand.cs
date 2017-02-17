using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.Enums;
using BL.Model.SystemCore.InternalModel;

namespace BL.Logic.PropertyCore.Commands
{
    public class DeletePropertyLinkCommand : BasePropertyCommand

    {
        private readonly ISystemDbProcess _systDb;

        public DeletePropertyLinkCommand(ISystemDbProcess systDb)
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
            var item = new InternalPropertyLink
            {
                Id = Model
            };
            _systDb.DeletePropertyLink(_context, item);
            return null;
        }

        public override EnumPropertyActions CommandType => EnumPropertyActions.DeletePropertyLink;
    }

}

