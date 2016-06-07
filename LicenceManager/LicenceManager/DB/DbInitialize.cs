using System.Collections.Generic;
using System.Data.Entity;

namespace LicenceManager.DB
{
    public class DbInitialize : CreateDatabaseIfNotExists<LicenceManagerDb>
    {
        protected override void Seed(LicenceManagerDb context)
        {
            context.LicenceTypes.AddRange(GetLicenceTypes());

            base.Seed(context);
        }

        private IEnumerable<LicenceType> GetLicenceTypes()
        {
            var res = new List<LicenceType>();
            res.Add(new LicenceType {Name = "Base licence", Activ = true, ConcurenteNumberOfConnections = 10, DurationDay = 365, NamedNumberOfConnections = 0});
            res.Add(new LicenceType { Name = "Small business licence", Activ = true, ConcurenteNumberOfConnections = 50, DurationDay = 365, NamedNumberOfConnections = 0 });
            res.Add(new LicenceType { Name = "Fixed Name business", Activ = true, ConcurenteNumberOfConnections = 0, DurationDay = 365, NamedNumberOfConnections = 50 });
            res.Add(new LicenceType { Name = "Unlimited", Activ = true, ConcurenteNumberOfConnections = null, DurationDay = null, NamedNumberOfConnections = null });

            return res;
        }
    }
}