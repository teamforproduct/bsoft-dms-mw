using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface ISystemCommand:ICommand
    {
        void InitializeCommand(EnumSystemActions action, IContext ctx);
        void InitializeCommand(EnumSystemActions action, IContext ctx, object param);
        EnumSystemActions CommandType { get; }
    }
}