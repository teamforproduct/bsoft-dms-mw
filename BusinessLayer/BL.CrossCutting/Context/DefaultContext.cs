using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Interfaces;
using BL.Model.Database;
using BL.Model.Exception;
using BL.Model.SystemCore;
using BL.Model.Users;

namespace BL.CrossCutting.Context
{
    public class DefaultContext : IContext
    {
        private int? _currentPositionId;
        private List<int> _currentPositionsIdList;
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
                        DefaultSchema = ctx.CurrentDB.DefaultSchema
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
            }
        }


        public List<int> CurrentPositionsIdList
        {
            get
            {
                //var position = CurrentPositions?.FirstOrDefault();
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
    }
}