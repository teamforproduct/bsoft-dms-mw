using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.Users;
using System;

namespace BL.CrossCutting.Context
{
    public class DefaultContext : IContext
    {
        private int? _currentPositionId;
        private List<int> _currentPositionsIdList;
        private Dictionary<int, int> _currentPositionsAccessLevel;

        private DatabaseModel _currentDb;
        public Employee CurrentEmployee { get; set; }

        public DefaultContext()
        {
        }

        public DefaultContext(IContext ctx)
        {
            var def = ctx as DefaultContext;
            if (def != null)
            {
                try
                {
                    CurrentDB = new DatabaseModel
                    {
                        Id = ctx.CurrentDB.Id,
                        Name = ctx.CurrentDB.Name,
                        ServerType = ctx.CurrentDB.ServerType,
                        IntegrateSecurity = false,
                        Address = ctx.CurrentDB.Address,
                        DefaultDatabase = ctx.CurrentDB.DefaultDatabase,
                        UserName = ctx.CurrentDB.UserName,
                        UserPassword = ctx.CurrentDB.UserPassword,
                        DefaultSchema = ctx.CurrentDB.DefaultSchema,
                        ConnectionString = ctx.CurrentDB.ConnectionString,
                    };
                }
                catch (DatabaseIsNotSet)
                {
                    CurrentDB = null;
                }

                CurrentEmployee = new Employee
                {
                    AgentId = ctx.CurrentEmployee.AgentId,
                    LanguageId = ctx.CurrentEmployee.LanguageId,
                    Name = ctx.CurrentEmployee.Name,
                    Token = ctx.CurrentEmployee.Token,
                    UserId = ctx.CurrentEmployee.UserId,
                    ClientId = ctx.CurrentEmployee.ClientId,
                    ClientCode = ctx.CurrentEmployee.ClientCode,
                };

                ClientLicence = ctx.ClientLicence;

                try
                {
                    _currentPositionId = ctx.CurrentPositionId;
                }
                catch (UserPositionIsNotDefined)
                {
                    _currentPositionId = null;
                }

                try
                {
                    CurrentPositionsIdList = ctx.CurrentPositionsIdList?.ToList();
                }
                catch (UserPositionIsNotDefined)
                {
                    CurrentPositionsIdList = null;
                }

                try
                {
                    CurrentPositionsAccessLevel = ctx.CurrentPositionsAccessLevel?.ToDictionary(x=>x.Key,x=>x.Value);
                }
                catch (UserPositionIsNotDefined)
                {
                    CurrentPositionsAccessLevel = null;
                }
            }
        }


        public List<int> CurrentPositionsIdList
        {
            get
            {
                if ((_currentPositionsIdList == null) || !_currentPositionsIdList.Any())
                {
                    throw new UserPositionIsNotDefined();
                }
                return _currentPositionsIdList;
            }
            set
            {
                _currentPositionsIdList = value;
            }
        }

        public Dictionary<int, int> CurrentPositionsAccessLevel
        {
            get
            {
                if ((_currentPositionsAccessLevel == null) || !_currentPositionsAccessLevel.Any())
                {
                    throw new UserPositionIsNotDefined();
                }
                return _currentPositionsAccessLevel;
            }
            set
            {
                _currentPositionsAccessLevel = value;
            }
        }

        public int CurrentPositionId
        {
            get
            {
                if (!_currentPositionId.HasValue)
                {
                    throw new UserPositionIsNotDefined();
                }
                return _currentPositionId.Value;
            }
        }
        
        public int CurrentAgentId
        {
            get
            {
                if (CurrentEmployee?.AgentId == null)
                {
                    throw new UserNameIsNotDefined();
                }
                return CurrentEmployee.AgentId.GetValueOrDefault();
            }
        }

        public void SetCurrentPosition(int? position)
        {
            _currentPositionId = position;
        }

        public bool IsAdmin => false;
        public LicenceInfo ClientLicence { get; set; }

        public DatabaseModel CurrentDB
        {
            get
            {
                if (_currentDb == null)
                {
                    throw new DatabaseIsNotSet();
                }
                return _currentDb;
            }
            set
            {
                _currentDb = value;
            }
        }

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
    }
}