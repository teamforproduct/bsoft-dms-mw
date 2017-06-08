using BL.Database.SystemDb;
using BL.Logic.Common;
using BL.Model.Enums;
using BL.Model.SystemCore.Filters;
using System.Collections.Generic;

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
            _systDb.DeletePropertyLinks(_context, new FilterPropertyLink { PropertyLinkId = new List<int> { Model } });
            return null;
        }

        public override EnumActions CommandType => EnumActions.DeletePropertyLink;
    }

}

