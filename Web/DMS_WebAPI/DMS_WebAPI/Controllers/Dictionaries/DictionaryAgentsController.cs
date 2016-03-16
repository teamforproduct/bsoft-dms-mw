using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.DictionaryCore.FrontModel;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.Model.Enums;
using BL.Model.DictionaryCore.FilterModel;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Работа с общим представлением контрагента (Наименование, признак активности, список текущих типов)
    /// От списка текущих типов в интерфейсе отходы на другие контроллеры, возвращающие детальную информацию,
    /// соответствующую выбранному типу
    /// </summary>
    [Authorize]
    public class DictionaryAgentsController : ApiController
    {
        /// <summary>
        /// Получение словаря контрагентов
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <returns>Список контрагентов
        /// </returns>
        public IHttpActionResult Get([FromUri] FilterDictionaryAgent filter)
        {
            //TODO Краткий формат если фильтр не указан или содержит несколько типов
            //     Формат конкретного типа, если тип явно указан в фильтре

            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgents(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Получение словаря агентов по ИД
        /// </summary>
        /// <param name="id">ИД агента</param>
        /// <returns>Агент</returns>
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgent(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        public IHttpActionResult Post([FromBody]ModifyDictionaryAgent model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgent, cxt, model));
        }

        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryAgent model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgent, cxt, model);
            return Get(model.Id);
        }


        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgent, cxt, id);
            FrontDictionaryAgent tmp = new FrontDictionaryAgent();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }


    }
}