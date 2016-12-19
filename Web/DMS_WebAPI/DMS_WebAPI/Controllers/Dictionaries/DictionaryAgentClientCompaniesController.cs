using BL.CrossCutting.DependencyInjection;
using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    /// <summary>
    /// Описывает клиентские компании (филиалы, единицы учета)
    /// Компании - рутовый элемент в иерархии штатного расписания
    /// </summary>
    [Authorize]
    public class DictionaryAgentClientCompaniesController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Компании"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Компании"</param>
        /// <returns>FrontDictionaryCompanies</returns>
        [ResponseType(typeof(List<FrontDictionaryAgentClientCompany>))]
        public IHttpActionResult Get([FromUri] FilterDictionaryAgentClientCompany filter)
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryAgentClientCompanies(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Компании" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryCompanies</returns>
        [ResponseType(typeof(FrontDictionaryAgentClientCompany))]
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryAgentClientCompany(cxt, id);
            return new JsonResult(tmpDict, this);
        }


        /// <summary>
        /// Добавление записи в словаре "Компании"
        /// </summary>
        /// <param name="model">ModifyDictionaryCompany</param>
        /// <returns>DictionaryCompanies</returns>
        public IHttpActionResult Post([FromBody]AddAgentClientCompany model)
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddAgentClientCompany, cxt, model));
        }

        /// <summary>
        /// Изменение записи в словаре "Компании"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryCompany</param>
        /// <returns>DictionaryCompanies</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyAgentClientCompany model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryCompany

            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyAgentClientCompany, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Компании"
        /// </summary>
        /// <returns>DictionaryCompanies</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteAgentClientCompany, cxt, id);
            FrontDictionaryAgentClientCompany tmp = new FrontDictionaryAgentClientCompany();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}