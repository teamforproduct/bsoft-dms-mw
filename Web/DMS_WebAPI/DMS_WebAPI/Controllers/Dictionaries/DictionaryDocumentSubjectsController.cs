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
    /// <summary>
    /// Тематика - необязательный реквизит документа, классификатор.
    /// По функционалу пересекается с тегами.
    /// </summary>
    [Authorize]
    public class DictionaryDocumentSubjectsController : ApiController
    {
        /// <summary>
        /// Возвращает записи из словаря "Тематики документов"
        /// </summary>
        /// <param name="filter">Параметры для фильтрации записей в словаре "Тематики документов"</param>
        /// <returns>FrontDictionaryDocumentSubject</returns>
        // GET: api/DictionaryDocumentSubjects
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentSubject filter)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDocumentSubjects(ctx, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает запись из словаря "Тематики документов" по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryDocumentSubject</returns>
        // GET: api/DictionaryDocumentSubjects/5
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDocumentSubject(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление записи в словаре "Тематики документов"
        /// </summary>
        /// <param name="model">ModifyDictionaryDocumentSubject</param>
        /// <returns>Возвращает добавленную запись</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryDocumentSubject model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddDocumentSubject, ctx, model));
        }


        /// <summary>
        /// Изменение записи в словаре "Тематики документов"
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryDocumentSubject</param>
        /// <returns>Возвращает измененную запись</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryDocumentSubject model)
        {
            // Спецификация REST требует отдельного указания ID, несмотря на то, что параметр ID есть в ModifyDictionaryDocumentSubject
            
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyDocumentSubject, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаление записи в словаре "Тематики документов"
        /// </summary>
        /// <returns>FrontDictionaryDocumentSubject</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteDocumentSubject, ctx, id);
            FrontDictionaryDocumentSubject tmp = new FrontDictionaryDocumentSubject();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}