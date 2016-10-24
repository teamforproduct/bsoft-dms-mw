using BL.CrossCutting.Interfaces;
using BL.Model.AdminCore.Clients;
using BL.Model.Common;
using BL.Model.Enums;
using BL.Model.Users;
using System.Collections.Generic;

namespace BL.Logic.ClientCore.Interfaces
{
    public interface IClientService
    {
        #region [+] General ...

        //object ExecuteAction(EnumClientActions act, IContext context, object param);



        #endregion

        #region [+] AddNewClient ...
        void AddNewClient(IContext context, AddClientContent model);

        void AddClientRoles(IContext context);
        #endregion

    }
}
