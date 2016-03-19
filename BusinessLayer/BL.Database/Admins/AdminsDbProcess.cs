using System.Collections.Generic;
using System.Linq;
using BL.CrossCutting.Helpers;
using BL.CrossCutting.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Database.DatabaseContext;
using BL.Model.AdminCore;
using BL.Logic.DependencyInjection;
using BL.Database.Dictionaries.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.InternalModel;
using BL.Model.Enums;
using BL.Model.Users;
using BL.Model.AdminCore.FrontModel;
using BL.Model.AdminCore.FilterModel;
using BL.Model.AdminCore.InternalModel;
using BL.Database.DBModel.Admin;

namespace BL.Database.Admins
{
    public class AdminsDbProcess : CoreDb.CoreDb, IAdminsDbProcess
    {
        private readonly IConnectionStringHelper _helper;

        public AdminsDbProcess(IConnectionStringHelper helper)
        {
            _helper = helper;
        }

        public AdminAccessInfo GetAdminAccesses(IContext context)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var res = new AdminAccessInfo();

                res.UserRoles = dbContext.AdminUserRolesSet.Select(x => new InternalDictionaryAdminUserRoles
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    UserId = x.UserId
                }).ToList();

                res.Roles = dbContext.AdminRolesSet.Select(x => new InternalDictionaryAdminRoles
                {
                    Id = x.Id

                }).ToList();

                res.PositionRoles = dbContext.AdminPositionRolesSet.Select(x => new InternalDictionaryAdminPositionRoles
                {
                    AccessLevel = (EnumDocumentAccesses)x.AccessLevelId,
                    PositionId = x.PositionId,
                    Id = x.Id,
                    RoleId = x.RoleId
                }).ToList();

                res.Actions = dbContext.SystemActionsSet.Select(x => new InternalDictionarySystemActions
                {
                    Id = x.Id,
                    Code = x.Code,
                    GrantId = x.GrantId,
                    IsGrantable = x.IsGrantable,
                    IsGrantableByRecordId = x.IsGrantableByRecordId,
                    IsVisible = x.IsVisible,
                    Object = (EnumObjects)x.ObjectId
                }).ToList();

                res.ActionAccess = dbContext.AdminRoleActionsSet.Select(x => new InternalDictionaryAdminRoleActions
                {
                    Id = x.Id,
                    RecordId = x.RecordId,
                    RoleId = x.RoleId,
                    ActionId = x.ActionId
                }).ToList();

                return res;
            }
        }

        public IEnumerable<BaseAdminUserRole> GetPositionsByUser(IContext ctx, FilterAdminUserRole filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                var qry = dbContext.AdminUserRolesSet.AsQueryable();

                if (filter.UserRoleId?.Count > 0)
                {
                    qry = qry.Where(x => filter.UserRoleId.Contains(x.Id));
                }
                if (filter.UserId?.Count > 0)
                {
                    qry = qry.Where(x => filter.UserId.Contains(x.UserId));
                }
                if (filter.RoleId?.Count > 0)
                {
                    qry = qry.Where(x => filter.UserId.Contains(x.RoleId));
                }
                return qry.Distinct().SelectMany(x => x.Role.PositionRoles).Select(x => new BaseAdminUserRole
                {
                    RolePositionId = x.PositionId,
                    RolePositionName = x.Position.Name,
                    RolePositionExecutorAgentName = x.Position.ExecutorAgent.Name
                }).Distinct().ToList();
            }
        }

        /// <summary>
        /// Проверка соблюдения субординации
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool VerifySubordination(IContext context, VerifySubordination model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var dictDb = DmsResolver.Current.Get<IDictionariesDbProcess>();
                var pos = dictDb.GetDictionaryPositions(context, new FilterDictionaryPosition() { IDs = new List<int> { model.TargetPosition }, SubordinatedPositions = model.SourcePositions }).FirstOrDefault();
                if (pos == null || pos.MaxSubordinationTypeId < (int)model.SubordinationType)
                {
                    return false;
                }
                return true;
            }
        }

        public Employee GetEmployee(IContext ctx, int id)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(ctx)))
            {
                return dbContext.DictionaryAgentsSet.Where(x => x.Id == id).Select(x => new Employee
                {
                    AgentId = x.Id,
                    Name = x.Name,
                    LanguageId = x.LanguageId ?? 0
                }).FirstOrDefault();
            }
        }

        public IEnumerable<CurrentPosition> GetPositionsByUser(Employee employee)
        {
            return null;
        }


        #region AdminLanguages

        public IEnumerable<FrontAdminLanguage> GetAdminLanguages(IContext context, FilterAdminLanguage filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.AdminLanguagesSet.AsQueryable();

                if (filter.LanguageId?.Count > 0)
                {
                    qry = qry.Where(x => filter.LanguageId.Contains(x.Id));
                }

                return qry.Select(x => new FrontAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).ToList();
            }
        }

        public InternalAdminLanguage GetInternalAdminLanguage(IContext context, FilterAdminLanguage filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.AdminLanguagesSet.AsQueryable();

                if (filter.LanguageId?.Count > 0)
                {
                    qry = qry.Where(x => filter.LanguageId.Contains(x.Id));
                }

                if (!string.IsNullOrEmpty(filter.Code))
                {
                    qry = qry.Where(x => filter.Code.Equals(x.Code));
                }

                return qry.Select(x => new InternalAdminLanguage
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    IsDefault = x.IsDefault
                }).FirstOrDefault();
            }
        }

        public int AddAdminLanguage(IContext context, InternalAdminLanguage model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new AdminLanguages
                {
                    Code = model.Code,
                    Name = model.Name,
                    IsDefault = model.IsDefault
                };
                dbContext.AdminLanguagesSet.Add(item);
                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void UpdateAdminLanguage(IContext context, InternalAdminLanguage model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new AdminLanguages
                {
                    Id = model.Id,
                    Code = model.Code,
                    Name = model.Name,
                    IsDefault = model.IsDefault
                };
                dbContext.AdminLanguagesSet.Attach(item);
                var entity = dbContext.Entry(item);

                entity.Property(x => x.Code).IsModified = true;
                entity.Property(x => x.Name).IsModified = true;
                entity.Property(x => x.IsDefault).IsModified = true;
                dbContext.SaveChanges();
            }
        }


        public void DeleteAdminLanguage(IContext context, InternalAdminLanguage model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var item = dbContext.AdminLanguagesSet.FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.AdminLanguagesSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion AdminLanguages

        #region AdminLanguageValues

        public IEnumerable<FrontAdminLanguageValue> GetAdminLanguageValues(IContext context, FilterAdminLanguageValue filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.AdminLanguageValuesSet.AsQueryable();

                if (filter.LanguageValueId?.Count > 0)
                {
                    qry = qry.Where(x => filter.LanguageValueId.Contains(x.Id));
                }

                if (filter.LanguageId.HasValue)
                {
                    if (filter.LanguageId > 0)
                    {
                        qry = qry.Where(x => x.Id == filter.LanguageId);
                    }
                    else
                    {
                        qry = qry.Where(x => x.Language.IsDefault);
                    }
                }

                if (!string.IsNullOrEmpty(filter.Label))
                {
                    qry = qry.Where(x => filter.Label.Equals(x.Label));
                }

                return qry.Select(x => new FrontAdminLanguageValue
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId,
                    Label = x.Label,
                    Value = x.Value
                }).ToList();
            }
        }

        public InternalAdminLanguageValue GetInternalAdminLanguageValue(IContext context, FilterAdminLanguageValue filter)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var qry = dbContext.AdminLanguageValuesSet.AsQueryable();

                if (filter.LanguageValueId?.Count > 0)
                {
                    qry = qry.Where(x => filter.LanguageValueId.Contains(x.Id));
                }

                if (filter.LanguageId.HasValue)
                {
                    qry = qry.Where(x => x.Id == filter.LanguageId);
                }

                if (!string.IsNullOrEmpty(filter.Label))
                {
                    qry = qry.Where(x => filter.Label.Equals(x.Label));
                }

                return qry.Select(x => new InternalAdminLanguageValue
                {
                    Id = x.Id,
                    LanguageId = x.LanguageId,
                    Label = x.Label,
                    Value = x.Value
                }).FirstOrDefault();
            }
        }

        public int AddAdminLanguageValue(IContext context, InternalAdminLanguageValue model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new AdminLanguageValues
                {
                    LanguageId = model.LanguageId,
                    Label = model.Label,
                    Value = model.Value
                };
                dbContext.AdminLanguageValuesSet.Add(item);
                dbContext.SaveChanges();
                model.Id = item.Id;
                return item.Id;
            }
        }

        public void UpdateAdminLanguageValue(IContext context, InternalAdminLanguageValue model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {
                var item = new AdminLanguageValues
                {
                    Id = model.Id,
                    LanguageId = model.LanguageId,
                    Label = model.Label,
                    Value = model.Value
                };
                dbContext.AdminLanguageValuesSet.Attach(item);
                var entity = dbContext.Entry(item);

                entity.Property(x => x.LanguageId).IsModified = true;
                entity.Property(x => x.Label).IsModified = true;
                entity.Property(x => x.Value).IsModified = true;
                dbContext.SaveChanges();
            }
        }


        public void DeleteAdminLanguageValue(IContext context, InternalAdminLanguageValue model)
        {
            using (var dbContext = new DmsContext(_helper.GetConnectionString(context)))
            {

                var item = dbContext.AdminLanguageValuesSet.FirstOrDefault(x => x.Id == model.Id);
                if (item != null)
                {
                    dbContext.AdminLanguageValuesSet.Remove(item);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion AdminLanguageValues
    }
}
