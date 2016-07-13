using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface IAdminCommand:ICommand
    {
        void InitializeCommand(EnumAdminActions action, IContext ctx);
        void InitializeCommand(EnumAdminActions action, IContext ctx, object param);
        EnumAdminActions CommandType { get; }
    }
}