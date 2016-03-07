using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface IPropertyCommand : ICommand
    {
        void InitializeCommand(EnumPropertyAction action, IContext ctx);
        void InitializeCommand(EnumPropertyAction action, IContext ctx, object param);
        EnumPropertyAction CommandType { get; }
    }
}