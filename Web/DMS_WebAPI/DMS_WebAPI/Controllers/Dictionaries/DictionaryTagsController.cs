using BL.Logic.DictionaryCore.Interfaces;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using DMS_WebAPI.Results;
using DMS_WebAPI.Utilities;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.Model.DictionaryCore.FilterModel;
using BL.Model.DictionaryCore.FrontModel;
using BL.Model.SystemCore;
using BL.Model.FullTextSearch;

namespace DMS_WebAPI.Controllers.Dictionaries
{
    [Authorize]
    public class DictionaryTagsController : ApiController
    {
        // GET: api/DictionaryTags
        /// <summary>
        /// Получить список доступных тегов для выставленых должностей
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Список доступных тегов для выставленых должностей</returns>
        public IHttpActionResult Get([FromUri]FullTextSearch ftSearch, [FromUri]FilterDictionaryTag filter, UIPaging paging)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDicts = tmpDictProc.GetMainTags(ctx, ftSearch,  filter, paging);
            return new JsonResult(tmpDicts, this);
        }

        // GET: api/DictionaryTags/5
        /// <summary>
        /// Получить тег для выставленых должностей
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Тег. Если тег не найден или недоступен для выставленных должностей вернеться ошибка</returns>
        public IHttpActionResult Get(int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            var tmpDict = tmpDictProc.GetTag(ctx, id);
            return new JsonResult(tmpDict, this);
        }

        /// <summary>
        /// Добавление тега для конкретной позиции
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Тег</returns>
        public IHttpActionResult Post([FromBody]AddTag model)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            return Get((int)tmpDictProc.ExecuteAction(EnumDictionaryActions.AddTag, ctx, model));
        }

        /// <summary>
        /// Модификация тега для конкретной позиции.
        /// В списке выбранных должностей должна быть должность такая же как у изменяемого тега
        /// CurrentPosition - не нужно
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Тег</returns>
        /// 
        public IHttpActionResult Put(int id, [FromBody]ModifyTag model)
        {
            model.Id = id;
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDictProc = DmsResolver.Current.Get<IDictionaryService>();
            tmpDictProc.ExecuteAction(EnumDictionaryActions.ModifyTag, ctx, model);
            return Get(model.Id);
        }

        /// <summary>
        /// Удаляет из справочника запись
        /// </summary>
        /// <returns>Возвращает id удаленной записи</returns> 
        public IHttpActionResult Delete([FromUri] int id)
        {
            var ctx = DmsResolver.Current.Get<UserContexts>().Get();
            var tmpDict = DmsResolver.Current.Get<IDictionaryService>();

            tmpDict.ExecuteAction(EnumDictionaryActions.DeleteTag, ctx, id);
            FrontTag tmp = new FrontTag();
            tmp.Id = id;

            return new JsonResult(tmp, this);

        }
    }
}