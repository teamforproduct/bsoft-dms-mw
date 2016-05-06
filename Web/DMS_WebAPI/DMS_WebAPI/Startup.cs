using System.Linq;
using BL.CrossCutting.DependencyInjection;
using BL.Logic.DependencyInjection;
using BL.Logic.SystemServices.AutoPlan;
using BL.Logic.SystemServices.FullTextSearch;
using BL.Logic.SystemServices.MailWorker;
using BL.Model.Database;
using DMS_WebAPI.Utilities;
using Microsoft.Owin;
using Owin;
using BL.Logic.SystemServices.ClearTrashDocuments;
using BL.CrossCutting.Helpers.Crypto;

[assembly: OwinStartup(typeof(DMS_WebAPI.Startup))]

namespace DMS_WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            var srv = new Servers();
            var dbs = srv.GetServers().Where(x=>x.ServerType == DatabaseType.SQLServer);

            //foreach (var srv in DmsResolver.Current.GetAll<ISystemWorkerService>())
            //{
            //    srv.Initialize(dbs);
            //}

            //var mailService = DmsResolver.Current.Get<IMailSenderWorkerService>();
            //mailService.Initialize(dbs);

            //var indexService = DmsResolver.Current.Get<IFullTextSearchService>();
            //indexService.Initialize(dbs);

            //var si = new SystemInfo();
            //var dbw = new SystemDbWorker();
            //var cd = si.GetRegCode(dbw.GetLicenceInfo(1));

            var autoPlanService = DmsResolver.Current.Get<IAutoPlanService>();
            autoPlanService.Initialize(dbs);

            var clearTrashDocumentsService = DmsResolver.Current.Get<IClearTrashDocumentsService>();
            clearTrashDocumentsService.Initialize(dbs);

            var userContextService = DmsResolver.Current.Get<UserContextWorkerService>();
            userContextService.Initialize();

            
            var cryptoService = DmsResolver.Current.Get<ICryptoService>();

            //TODO Нужно откудато брать этотт ключ
            var publicKeyXml = "<RSAKeyValue><Modulus>4OkKxEXOcPFZSbeOmaOjlPNtgmzfL+1syF3ZVorRpinQe4UStUpSIeD4bHw3eoXLGh2tj8C/yY64fLzqKngZFpYqjCw57ImYW9YL6L1aAQq5R85IoIRqc1H1HuUu7asRJ6KxdrjWHPprlxrmKluUUW00V6rgtsyegQbw+TYGxys=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            cryptoService.RSAPersistPublicKeyInCSP(BL.Model.Constants.SettingConstants.CRYPTO_OBTAINED_PUBLIC_KEY_NAME_FOR_VERIFY_SIGNED_HASH, publicKeyXml);

            //TODO remove in release version
            var keyXml = "<RSAKeyValue><Modulus>4OkKxEXOcPFZSbeOmaOjlPNtgmzfL+1syF3ZVorRpinQe4UStUpSIeD4bHw3eoXLGh2tj8C/yY64fLzqKngZFpYqjCw57ImYW9YL6L1aAQq5R85IoIRqc1H1HuUu7asRJ6KxdrjWHPprlxrmKluUUW00V6rgtsyegQbw+TYGxys=</Modulus><Exponent>AQAB</Exponent><P>9fcg9fXBBVyjFBf/MmcGGO9lQQIoZKD6uhCOl1Yag5d4tHFSpjBSl8xdVCScwfCH5JqnQ2/LYYzLhlVwfIc52Q==</P><Q>6hYDiiLLY4uSRqn65P+RbLJtiGTb3pXMSnkrJUsEfWpdk2LioVnBcUjdG3L9TYH+vvYxSZgGeqTdL028SjtCow==</Q><DP>XwBp0bZvLEQAlQVDw1L5ju4APOC5e0yWKwG0IY3XRdZef8t5cckeTZVYwuQ1S888nbaCPlDSzEXWYndjsCWG2Q==</DP><DQ>0h9olfqdr4F9kEjMmfp+w592itReOPMmKkOR4yvvn9R2ovJElKlI5zOMpjMWBRkHXssHexQn3LdYhDm3JrsDGQ==</DQ><InverseQ>YmWn6//4/EnCK8wEc0JS6IfSjJrrK4YoCxkpkBEzskUMWMJyJWhvPS/EG4/MpDBv3PWmoRxb9gdNGZI9RANlGw==</InverseQ><D>P6j2yTNRCZrmwPzZuhcdWC3G02HpknFLlzMpL1u/l57CShQShYnx0XESH/LQ6Tcxk8TOuZ+/KVfiVDxMs5Sm1Lvvesa387OM6HeBJ7GFQlrPlum4yjeSXrTTLyDZcqOkDkz/z2Q4N1fIMBa0+Bb0gaetRW+3UqXSyS/KK5Fa7SE=</D></RSAKeyValue>";
            cryptoService.RSAPersistKeyInCSP(BL.Model.Constants.SettingConstants.CRYPTO_LOCAL_SIGNDATA_KEY_NAME, keyXml);
        }
    }
}
