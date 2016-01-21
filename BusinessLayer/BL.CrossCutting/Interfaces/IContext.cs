using BL.Model.Database;
using BL.Model.Users;

namespace BL.CrossCutting.Interfaces
{
    public interface IContext
    {
        Employee CurrentEmployee { get; set; }
        Position CurrentPosition { get; set; }
        DatabaseModel CurrentDB { get; set; }
    }
}