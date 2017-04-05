using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.Users;
using BL.Model.Enums;
using BL.Model.SystemCore;
using System;
using BL.CrossCutting.DependencyInjection;
using Ninject;
using Ninject.Parameters;

namespace BL.CrossCutting.Context
{
    public class AdminContext : IContext
    {
        private int? _currentPositionId;
        private const string _USER_NAME = "DmsAdmin";
        private const string _USER_PASS = "UkrPr0100_th3B3ssTC0nTry";

        public AdminContext(DatabaseModel dbModel)
        {
            //TODO ClientId
            CurrentDB = new DatabaseModel
            {
                Id = dbModel.Id,
                Name = dbModel.Name,
                ServerType = dbModel.ServerType,
                IntegrateSecurity = false,
                Address = dbModel.Address,
                DefaultDatabase = dbModel.DefaultDatabase,
                UserName = _USER_NAME,
                UserPassword = _USER_PASS,
                DefaultSchema = dbModel.DefaultSchema,
                ConnectionString = dbModel.ConnectionString,
            };
            CurrentEmployee = new Employee
            {
                Name = "System user",
                AgentId = (int)EnumSystemUsers.AdminUser,
                ClientId = dbModel.ClientId,
            };
            DbContext = DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel", CurrentDB));
        }

        //TODO DbContext NOT INITIALIZED HERE
        public AdminContext(IContext ctx)
        {
            CurrentDB = new DatabaseModel
            {
                Id = ctx.CurrentDB.Id,
                Name = ctx.CurrentDB.Name,
                ServerType = ctx.CurrentDB.ServerType,
                IntegrateSecurity = false,
                Address = ctx.CurrentDB.Address,
                DefaultDatabase = ctx.CurrentDB.DefaultDatabase,
                UserName = _USER_NAME,
                UserPassword = _USER_PASS,
                DefaultSchema = ctx.CurrentDB.DefaultSchema,
                ConnectionString = ctx.CurrentDB.ConnectionString,
            };
            CurrentEmployee = new Employee
            {
                Name = "System user",
                AgentId = (int) EnumSystemUsers.AdminUser,
                ClientId = ctx.CurrentClientId
            };

            IsChangePasswordRequired = ctx.IsChangePasswordRequired;
            DbContext =
                DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel", CurrentDB));
            IsFormed = true;

        }

        /// <summary>
        ///  Флаг TRUE если контекст сформирован и готов к работе
        /// </summary>
        public bool IsFormed { get; set; }

        public Employee CurrentEmployee { get; set; }
        public List<int> CurrentPositionsIdList
        {
            get { return new List<int> { (int)EnumSystemPositions.AdminPosition }; }
            set { }
        }

        public Dictionary<int, int> CurrentPositionsAccessLevel
        {
            get { return new Dictionary<int, int> { { (int)EnumSystemPositions.AdminPosition, 0 } }; }
            set { }
        }

        public DatabaseModel CurrentDB { get; set; }

        public int CurrentPositionId => _currentPositionId ?? (int)EnumSystemPositions.AdminPosition;

        public int CurrentAgentId => (int)EnumSystemUsers.AdminUser;

        public void SetCurrentPosition(int? position)
        {
            _currentPositionId = position;
        }

        public List<string> GetAccessFilterForFullText(string addFilter)
        {
            throw new NotImplementedException();
        }

        public bool IsAdmin => true;
        public LicenceInfo ClientLicence { get; set; }

        public int CurrentClientId
        {
            get
            {
                if (CurrentEmployee.ClientId <= 0)
                {
                    return 0;
                }
                return CurrentEmployee.ClientId;
            }
            set
            {
                CurrentEmployee.ClientId = value;
            }
        }

        public DateTime CreateDate { get; } = DateTime.UtcNow;

        public bool IsChangePasswordRequired { get; set; }

        public int? LoginLogId { get; set; }

        public string LoginLogInfo { get; set; }
        public IDmsDatabaseContext DbContext { get; set; }

        public string UserName { get; set; }
    }
}