using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;using BL.CrossCutting.DependencyInjection;
namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Содержание типового списка рассылки
    /// </summary>
    [Authorize]
    public class DictionaryStandartSendListContentsController : ApiController
    {
        /// <summary>
        /// Список содержаний типовых списков рассылки
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        // GET: api/DictionaryStandartSendListContents
        public IHttpActionResult Get([FromUri] FilterDictionaryStandartSendListContent filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryStandartSendListContents(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Получение содержания типового списка рассылки по ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/DictionaryStandartSendListContents/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryStandartSendListContent(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление содержания типового списка рассылки
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryStandartSendListContent model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddStandartSendListContent, cxt, model));
        }

        /// <summary>
        /// Изменение содержания типового списка рассылки
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryStandartSendListContent model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyStandartSendListContent, cxt, model);
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
            var tmp = new FrontDictionaryStandartSendListContent {Id = id};

            return new JsonResult(tmp, this);

        }

    }
}