using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.Logic.Context;
using BL.Model.Database;
using BL.Model.Users;

namespace BL.CrossCutting.Context
{
    public class AdminContext : IContext
    {
        private int? _currentPositionId;
        private const string _USER_NAME = "DmsAdmin";
        private const string _USER_PASS = "UkrPr0100_th3B3ssTC0nTry";

        public AdminContext(DatabaseModel dbModel)
        {
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
                DefaultSchema = dbModel.DefaultSchema
            };
            CurrentEmployee = new Employee
            {
                Name = "System user",
                AgentId = -1
            };
        }

        public AdminContext(IContext ctx)
        {
            var def = ctx as DefaultContext;
            if (def != null)
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
                    DefaultSchema = ctx.CurrentDB.DefaultSchema
                };
                CurrentEmployee = new Employee
                {
                    Name = "System user",
                    AgentId = -1
                };
            }
        }

        public Employee CurrentEmployee { get; set; }
        public List<int> CurrentPositionsIdList {
            get
            {
            return new List<int> {0};    
            }
            set { }
        }
        public DatabaseModel CurrentDB { get; set; }

        public int CurrentPositionId {
            get { return _currentPositionId??0; }
        }
        public int CurrentAgentId {
            get { return -1; }
        }
        public void SetCurrentPosition(int? position)
        {
            _currentPositionId = position;
        }

        public bool IsAdmin => true;
    }
}