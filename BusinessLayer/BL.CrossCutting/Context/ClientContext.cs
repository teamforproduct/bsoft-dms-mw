using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.Interfaces;
using BL.Model.Context;
using BL.Model.Enums;
using BL.Model.Exception;
using BL.Model.SystemCore;
using Ninject;
using Ninject.Parameters;
using System.Collections.Generic;
using System.Linq;

namespace BL.CrossCutting.Context
{
    /// <summary>
    /// Контекст пользователя - информационный класс, который НЕ должен содержать логику. Просто набор параметров, которые доступны на всех уровнях
    /// </summary>
    public class ClientContext : IClientContext
    {
        private bool _isFormed;
        private int? _currentPositionId;
        private List<int> _currentPositionsIdList;
        private Dictionary<int, int> _currentPositionsAccessLevel;


        public ClientContext() { }
        
        /// <summary>
        /// Создает новый контекст пользователя и новое ПОДКЛЮЧЕНИЕ к базе
        /// </summary>
        /// <param name="ctx"></param>
        public ClientContext(IClientContext ctx)
        {
            var def = ctx as ClientContext;
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
                DbContext = ctx.DbContext;
                Employee = new Employee
                {
                    Id = ctx.Employee.Id,
                    Name = ctx.Employee.Name,
                    IsActive = ctx.Employee.IsActive,
                    AssigmentsCount = ctx.Employee.AssigmentsCount,
                };
                Client = new Client
                {
                    Id = ctx.Client.Id,
                    Code = ctx.Client.Code,
                };

                ClientLicence = ctx.ClientLicence;
                IsFormed = ctx.IsFormed;

                //TODO CurrentDB может быть null
                // тут поднимается коннекшн к базе
                DbContext = DmsResolver.Current.Kernel.Get<IDmsDatabaseContext>(new ConstructorArgument("dbModel", CurrentDB));

                _currentPositionId = ctx.CurrentPositionDefined
                    ? ctx.CurrentPositionId
                    :(int?)null;

                CurrentPositionsIdList = ctx.CurrentPositionsIdListDefined
                    ? ctx.CurrentPositionsIdList?.ToList()
                    :null;

                CurrentPositionsAccessLevel = ctx.CurrentPositionsAccessLevelDefined
                    ? ctx.CurrentPositionsAccessLevel?.ToDictionary(x => x.Key, x => x.Value)
                    :null;
            }
        }


        public Employee Employee { get; set; }

        public Client Client { get; set; }

        /// <summary>
        ///  Флаг TRUE если контекст сформирован и готов к работе
        /// </summary>
        public bool IsFormed
        {
            get { return _isFormed; }
            set { _isFormed = value; }
        }

        public bool CurrentPositionsIdListDefined => !((_currentPositionsIdList == null) || !_currentPositionsIdList.Any());

        public List<int> CurrentPositionsIdList
        {
            get
            {
                if ((_currentPositionsIdList == null) || !_currentPositionsIdList.Any())
                {
                    throw new UserContextIsNotDefined();
                }
                return _currentPositionsIdList;
            }
            set
            {
                _currentPositionsIdList = value;
            }
        }

        public bool CurrentPositionsAccessLevelDefined => !((_currentPositionsAccessLevel == null) || !_currentPositionsAccessLevel.Any());

        public Dictionary<int, int> CurrentPositionsAccessLevel
        {
            get
            {
                if ((_currentPositionsAccessLevel == null) || !_currentPositionsAccessLevel.Any())
                {
                    throw new UserContextIsNotDefined();
                }
                return _currentPositionsAccessLevel;
            }
            set
            {
                _currentPositionsAccessLevel = value;
            }
        }

        public bool CurrentPositionDefined => _currentPositionId.HasValue;

        public int CurrentPositionId
        {
            get
            {
                if (!_currentPositionId.HasValue)
                {
                    throw new UserContextIsNotDefined();
                }
                return _currentPositionId.Value;
            }
        }

        public int CurrentAgentId
        {
            get
            {
                if (Employee?.Id == 0)
                {
                    throw new UserContextIsNotDefined();
                }
                return Employee.Id;
            }
        }

        public void SetCurrentPosition(int? position)
        {
            _currentPositionId = position;
        }

        public List<string> GetAccessFilterForFullText(string addFilter)
        {
            return CurrentPositionsAccessLevel
                        .Select(x => $"{x.Key}{FullTextFilterTypes.AccessPosition30}{addFilter}")
                        .Concat(CurrentPositionsAccessLevel.Where(x => x.Value <= (int)EnumAccessLevels.PersonallyAndReferents)
                        .Select(x => $"{x.Key}{FullTextFilterTypes.AccessPosition20}{addFilter}"))
                        .Concat(CurrentPositionsAccessLevel.Where(x => x.Value <= (int)EnumAccessLevels.Personally)
                        .Select(x => $"{x.Key}{FullTextFilterTypes.AccessPosition10}{addFilter}"))
                        .ToList();
        }

        public bool IsAdmin => false;

        public LicenceInfo ClientLicence { get; set; }

        public DatabaseModel CurrentDB
        {
            get
            {
                if (_currentDb == null) throw new DatabaseIsNotSet();
                return _currentDb;
            }
            set
            {
                _currentDb = value;
            }
        }
        private DatabaseModel _currentDb;

        public IDmsDatabaseContext DbContext { get; set; }

    }
}