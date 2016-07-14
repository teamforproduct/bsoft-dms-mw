using BL.Model.Enums;

namespace BL.CrossCutting.Interfaces
{
    public interface IEncryptionCommand : ICommand
    {
        void InitializeCommand(EnumEncryptionActions action, IContext ctx);
        void InitializeCommand(EnumEncryptionActions action, IContext ctx, object param);
        EnumEncryptionActions CommandType { get; }
    }
}