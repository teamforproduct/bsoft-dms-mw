using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.Tree;
using System.Web.Http.Description;
using System.Collections.Generic;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    [RoutePrefix("api/v2/DictionaryRegistrationJournals")]
    public class DictionaryRegistrationJournalsController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Журналы регистрации"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Журналы регистрации"</param>
        /// <returns>FrontDictionaryRegistrationJournals</returns>
        // GET: api/DictionaryRegistrationJournals
        public IHttpActionResult Get([FromUri] FilterDictionaryRegistrationJournal filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetRegistrationJournals(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Журналы регистрации" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryRegistrationJournals</returns>
        // GET: api/DictionaryRegistrationJournals/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetRegistrationJournal(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Возвращает дерево Компании-Отделы-Журналы регистрации
        /// </summary>
        /// <param name="filter">TreeItem</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTree")]
        [ResponseType(typeof(List<TreeItem>))]
        [ResponseType(typeof(List<FrontDictionaryDepartmentTreeItem>))]
        public IHttpActionResult GetTree([FromUri] FilterTree filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetRegistrationJournalsTree(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Добавление записи в словаре "Журналы регистрации"
        /// </summary>
        /// <param name="model">ModifyDictionaryRegistrationJournal</param>
        /// <returns>FrontDictionaryRegistrationJournals</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryRegistrationJournal model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddRegistrationJournal, ctx, model));
        }

        /// <summary>
        /// Изменение записи в словаре "Журналы регистрации"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryRegistrationJournal</param>
        /// <returns>FrontDictionaryRegistrationJournals</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryRegistrationJournal model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryRegistrationJournal

            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyRegistrationJournal, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Журналы регистрации"
        /// </summary>
        /// <returns>FrontDictionaryRegistrationJournal</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteRegistrationJournal, ctx, id);
            FrontDictionaryRegistrationJournal tmp = new FrontDictionaryRegistrationJournal();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
       
    }
}