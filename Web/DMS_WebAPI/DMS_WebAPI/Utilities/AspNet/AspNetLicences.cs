using BL.Model.AspNet.FrontModel;
using BL.Model.AspNet.IncomingModel;
using BL.Model.Exception;
using DMS_WebAPI.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DMS_WebAPI.Utilities.AspNet
{
    public class AspNetLicences
    {
        /// <summary>
        /// Get licence parameters by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FrontAspNetLicence GetLicence(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var items = dbContext.AspNetLicencesSet
                    .Where(x => x.Id == id)
                    .Select(x => new FrontAspNetLicence
                    {
                        Id = x.Id,
                        IsTrial = x.IsTrial,

                        IsDateLimit = x.DurationDay.HasValue,
                        DurationDay = x.DurationDay,

                        IsConcurenteLicence = x.ConcurenteNumberOfConnections.HasValue,
                        ConcurenteNumberOfConnections = x.ConcurenteNumberOfConnections,

                        IsNamedLicence = x.NamedNumberOfConnections.HasValue,
                        NamedNumberOfConnections = x.NamedNumberOfConnections,

                        IsFunctionals = x.Functionals != null,
                        Functionals = x.Functionals,
                    }).FirstOrDefault();

                return items;
            }
        }

        /// <summary>
        /// List of all licences
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FrontAspNetLicence> GetLicences()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var items = dbContext.AspNetClientsSet.Select(x => new FrontAspNetLicence
                {
                    Id = x.Id,
                    IsTrial = x.IsTrial,

                    IsDateLimit = x.DurationDay.HasValue,
                    DurationDay = x.DurationDay,

                    IsConcurenteLicence = x.ConcurenteNumberOfConnections.HasValue,
                    ConcurenteNumberOfConnections = x.ConcurenteNumberOfConnections,

                    IsNamedLicence = x.NamedNumberOfConnections.HasValue,
                    NamedNumberOfConnections = x.NamedNumberOfConnections,

                    IsFunctionals = x.Functionals != null,
                    Functionals = x.Functionals,
                }).ToList();

                return items;
            }
        }

        /// <summary>
        /// Add new licence
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        /// <exception cref="DictionaryRecordCouldNotBeAdded"></exception>
        public int AddLicence(ModifyAspNetLicence modal)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new DBModel.AspNetLicences
                    {
                        IsTrial = modal.IsTrial,
                        NamedNumberOfConnections = modal.NamedNumberOfConnections,
                        ConcurenteNumberOfConnections = modal.ConcurenteNumberOfConnections,
                        DurationDay = modal.DurationDay,
                        Functionals = modal.Functionals
                    };
                    dbContext.AspNetLicencesSet.Add(item);
                    dbContext.SaveChanges();

                    modal.Id = item.Id;

                    return modal.Id;
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        /// <summary>
        /// Modify licence parameters
        /// </summary>
        /// <param name="modal"></param>
        /// <exception cref="DictionaryRecordCouldNotBeAdded"></exception>
        public void UpdateLicence(ModifyAspNetLicence modal)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new DBModel.AspNetLicences
                    {
                        Id = modal.Id,
                        IsTrial = modal.IsTrial,
                        NamedNumberOfConnections = modal.NamedNumberOfConnections,
                        ConcurenteNumberOfConnections = modal.ConcurenteNumberOfConnections,
                        DurationDay = modal.DurationDay,
                        Functionals = modal.Functionals
                    };
                    dbContext.AspNetLicencesSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(p => p.IsTrial).IsModified = true;
                    entry.Property(p => p.NamedNumberOfConnections).IsModified = true;
                    entry.Property(p => p.ConcurenteNumberOfConnections).IsModified = true;
                    entry.Property(p => p.DurationDay).IsModified = true;
                    entry.Property(p => p.Functionals).IsModified = true;

                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        /// <summary>
        /// Delete licence from the list
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DictionaryRecordCouldNotBeDeleted"></exception>
        public void DeleteLicence(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new DBModel.AspNetLicences
                    {
                        Id = id
                    };
                    dbContext.AspNetLicencesSet.Attach(item);

                    dbContext.Entry(item).State = EntityState.Deleted;

                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }
    }
}