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
    public class DictionaryDocumentSubjectsController : ApiController
    {
        /// <summary>
        /// Возвращает список тематик документов
        /// </summary>
        /// <param name="filter">Параметры для фильтрации тематик документов</param>
        /// <returns>FrontDictionaryDocumentSubject</returns>
        // GET: api/DictionaryDocumentSubjects
        public IHttpActionResult Get([FromUri] FilterDictionaryDocumentSubject filter)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetDictionaryDocumentSubjects(cxt, filter);
            return new JsonResult(tmpDicts, this);
        }

        /// <summary>
        /// Возвращает тематику документов по ID 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>FrontDictionaryDocumentSubject</returns>
        // GET: api/DictionaryDocumentSubjects/5
        public IHttpActionResult Get(int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetDictionaryDocumentSubject(cxt, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление тематики документов
        /// </summary>
        /// <param name="model">ModifyDictionaryDocumentSubject</param>
        /// <returns>Измененный запись словаря типа документа</returns>
        public IHttpActionResult Post([FromBody]ModifyDictionaryDocumentSubject model)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDict.ExecuteAction(EnumDictionaryActions.AddDocumentSubject, cxt, model));
        }

        /// <summary>
        /// Изменение словаря тип документа 
        /// </summary>
        /// <param name="id">int</param>
        /// <param name="model">ModifyDictionaryDocumentSubject</param>
        /// <returns>Измененный запись словаря типа документа</returns>
        public IHttpActionResult Put(int id, [FromBody]ModifyDictionaryDocumentSubject model)
        {
            model.Id = id;
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();
            tmpDict.ExecuteAction(EnumDictionaryActions.ModifyDocumentSubject, cxt, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет из справочника запись
        /// </summary>
        /// <returns>FrontDictionaryDocumentSubject</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var cxt = DmsResolver.Current.Get<UserContext>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteDocumentSubject, cxt, id);
            FrontDictionaryDocumentSubject tmp = new FrontDictionaryDocumentSubject();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}