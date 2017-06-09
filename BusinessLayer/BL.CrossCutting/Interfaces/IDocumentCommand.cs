using System.Collections.Generic;
using BL.Model.DocumentCore.InternalModel;
using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface IDocumentCommand:ICommand
    {
        void InitializeCommand(IContext ctx, InternalDocument doc);
        void InitializeCommand(IContext ctx, InternalDocument doc, object param, EnumActions? action = null);

        EnumActions CommandType { get; }
        IEnumerable<InternalActionRecord> ActionRecords { get; }
    }
}