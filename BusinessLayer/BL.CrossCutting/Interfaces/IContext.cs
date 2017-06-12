namespace BL.CrossCutting.Interfaces
{
    // Текущий контекст, который передается в методы для выполнения действий в опр клиентской базе опр сотрудником
    public interface IContext : IAuthContext , IClientContext
    { 

    }
}