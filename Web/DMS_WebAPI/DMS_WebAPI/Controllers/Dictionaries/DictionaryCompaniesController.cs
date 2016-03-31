using BL.Logic.DependencyInjection;
using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryCompaniesController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Компании"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Компании"</param>
        /// <returns>FrontDictionaryCompanies</returns>
        // GET: api/DictionaryCompanies
        public IHttpActionResult Get([FromUri] FilterDictionaryCompany filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryCompanies(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Компании" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryCompanies</returns>
        // GET: api/DictionaryCompanies/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryCompany(cxt, id);
            return new JsonResult(tmpDict, this);
        }


        /// <summary>
        /// Добавление записи в словаре "Компании"
        /// </summary>
        /// <param name="model">ModifyDictionaryCompany</param>
        /// <returns>DictionaryCompanies</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryCompany model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddCompany, cxt, model));
        }

        /// <summary>
        /// Изменение записи в словаре "Компании"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryCompany</param>
        /// <returns>DictionaryCompanies</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryCompany model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryCompany

            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyCompany, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Компании"
        /// </summary>
        /// <returns>DictionaryCompanies</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteCompany, cxt, id);
            FrontDictionaryCompany tmp = new FrontDictionaryCompany();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}