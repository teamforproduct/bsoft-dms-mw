using BL.Model.SystemCore;
using DMS_WebAPI.Utilities;
using System.Web.Http;

namespace DMS_WebAPI.ControllersV3.Documents
{
    /// <summary>
    /// Документы. Доступы.
    /// </summary>
    [Authorize]
    [DimanicAuthorize]
    [RoutePrefix(ApiPrefix.V3 + Modules.Documents)]
    public class DocumentAccessController : ApiController
    {

    }
}
