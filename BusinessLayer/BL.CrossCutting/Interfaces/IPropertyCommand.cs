using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface IPropertyCommand : ICommand
    {
        void InitializeCommand(EnumPropertyActions action, IContext ctx);
        void InitializeCommand(EnumPropertyActions action, IContext ctx, object param);
        EnumPropertyActions CommandType { get; }
    }
}