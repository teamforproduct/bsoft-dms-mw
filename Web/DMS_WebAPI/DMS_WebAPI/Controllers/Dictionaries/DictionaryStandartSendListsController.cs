using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Типовой список рассылки
    /// </summary>
    [Authorize]
    public class DictionaryStandartSendListsController : ApiController
    {
        /// <summary>
        /// Получение всех типовых списков рассылки
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        // GET: api/DictionaryStandartSendLists
        public IHttpActionResult Get([FromUri] FilterDictionaryStandartSendList filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryStandartSendLists(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Получение списка рассылки по ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/DictionaryStandartSendLists/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryStandartSendList(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление типового списка рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryStandartSendList model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddStandartSendList, cxt, model));
        }

        /// <summary>
        /// Изменение типового списка рассылки
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryStandartSendList model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyStandartSendList, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет из справочника запись
        /// </summary>
        /// <returns>Возвращает id удаленной записи</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteStandartSendListContent, cxt, id);
            var tmp = new FrontDictionaryStandartSendList {Id = id};

            return new JsonResult(tmp, this);

        }
    }
}