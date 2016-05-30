using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryPositionsController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Должности"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Должности"</param>
        /// <returns>FrontDictionaryPositions</returns>
        // GET: api/DictionaryPositions
        public IHttpActionResult Get([FromUri] FilterDictionaryPosition filter)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryPositions(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Должности" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryPositions</returns>
        // GET: api/DictionaryPositions/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryPosition(ctx, id);
            return new JsonResult(tmpDict, this);
        }


        /// <summary>
        /// Добавление записи в словаре "Должности"
        /// </summary>
        /// <param name="model">ModifyDictionaryPosition</param>
        /// <returns>DictionaryPositions</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryPosition model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddPosition, cxt, model));
        }

        /// <summary>
        /// Изменение записи в словаре "Должности"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryPosition</param>
        /// <returns>DictionaryPositions</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryPosition model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryPosition

            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyPosition, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Должности"
        /// </summary>
        /// <returns>DictionaryPositions</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeletePosition, cxt, id);
            FrontDictionaryPosition tmp = new FrontDictionaryPosition();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}