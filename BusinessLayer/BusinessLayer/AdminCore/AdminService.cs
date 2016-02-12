using System.Collections.Generic;
using BL.CrossCutting.Interfaces;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.AdminCore.Interfaces;
using BL.Database.Admins.Interfaces;
using BL.Model.AdminCore;
using System;
using BL.Model.Exception;

namespace BL.Logic.AdminCore
{
    public class AdminService : IAdminService
    {
        #region AdminAccessLevels
        public BaseAdminAccessLevel GetAdminAccessLevel(IContext context, int id)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            return admDb.GetAdminAccessLevel(context, id);
        }

        public IEnumerable<BaseAdminAccessLevel> GetAdminAccessLevels(IContext context, FilterAdminAccessLevel filter)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            return admDb.GetAdminAccessLevels(context, filter);
        }

        #endregion AdminAccessLevels

        public IEnumerable<BaseAdminUserRole> GetPositionsByCurrentUser(IContext context)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            return admDb.GetPositionsByUser(context, new FilterAdminUserRole() { UserId  = new List<int>() { context.CurrentAgentId } });
        }

        /// <summary>
        /// Проверка доступа к должностям для текущего пользователя
        /// </summary>
        /// <param name="cxt"></param>
        /// <param name="positionsIdList"></param>
        public void VerifyAccessForCurrentUser(IContext context, List<int> positionsIdList)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            if (!(admDb.VerifyAccess(context, new VerifyAccess() { UserId = context.CurrentAgentId, PositionsIdList = positionsIdList })))
            {
                throw new AccessIsDenied(); //!!!Как красиво передать string obj, string act, int? id = null в сообщение?
            }

            // throw new NotImplementedException();
        }

        /// <summary>
        /// Проверка прав доступа на действия для текущего пользователя
        /// </summary>
        /// <param name="context">контекст</param>
        /// <param name="obj">Объект системы</param>
        /// <param name="act">Действие</param>
        /// <param name="positionId">Ид должности, если нужно проверять доступ учитывая конкретную должность</param>
        /// <param name="id">ИД записи при выдаче доступа по каждой записи</param>
        public void VerifyAccessForCurrentUser(IContext context, string obj, string act, int? positionId = null, int? id = null)
        {
            var admDb = DmsResolver.Current.Get<IAdminsDbProcess>();
            if (positionId.HasValue && !context.CurrentPositionsIdList.Contains((int)positionId))
            {
                throw new UserPositionIsNotDefined();
            };
            if (!(admDb.VerifyAccess(context, new VerifyAccess()
                                            {   UserId = context.CurrentAgentId,
                                                PositionsIdList = context.CurrentPositionsIdList,
                                                ObjectCode = obj, ActionCode = act,
                                                RecordId = id, PositionId = positionId })))
            {
                throw new AccessIsDenied(); //!!!Как красиво передать string obj, string act, int? id = null в сообщение?
            }
        }

    }
}
