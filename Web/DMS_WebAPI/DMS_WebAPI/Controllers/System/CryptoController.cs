using DMS_WebAPI.Results;
using System.Web.Http;
using BL.CrossCutting.DependencyInjection;
using BL.CrossCutting.CryptographicWorker;

namespace DMS_WebAPI.Controllers
{
    public class CryptoController : ApiController
    {
        public IHttpActionResult Get([FromUri]string ContainerName, [FromUri]bool IncludePrivateParameters)
        {
            var cryptoService = DmsResolver.Current.Get<ICryptoService>();

            var res = cryptoService.RSAPersistKeyInCSPExport(ContainerName, IncludePrivateParameters);

            return new JsonResult(res, this);
        }
        public IHttpActionResult Post([FromUri]string ContainerName, [FromUri]string KeyXmlString)
        {
            var cryptoService = DmsResolver.Current.Get<ICryptoService>();

            cryptoService.RSAPersistKeyInCSP(ContainerName, KeyXmlString);

            return new JsonResult(null, this);
        }
    }
}
