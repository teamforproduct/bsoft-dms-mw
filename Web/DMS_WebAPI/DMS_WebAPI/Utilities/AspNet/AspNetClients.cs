using BL.Model.AspNet.FrontModel;
using BL.Model.AspNet.IncomingModel;
using BL.Model.Exception;
using DMS_WebAPI.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DMS_WebAPI.Utilities.AspNet
{
    /// <summary>
    /// Represend functionality to configure clients
    /// </summary>
    public class AspNetClients
    {
        /// <summary>
        /// Get all client parameters by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FrontAspNetClient GetClient(int id)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var items = dbContext.AspNetClientsSet
                    .Where(x => x.Id == id)
                    .Select(x => new FrontAspNetClient
                    {
                        Id = x.Id,
                        Name = x.Name,
                    }).FirstOrDefault();

                return items;
            }
        }

        /// <summary>
        /// List of all clients
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FrontAspNetClient> GetClients()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var items = dbContext.AspNetClientsSet.Select(x => new FrontAspNetClient
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();

                return items;
            }
        }

        /// <summary>
        /// Add new client to list of aceptable servers
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        /// <exception cref="DictionaryRecordCouldNotBeAdded"></exception>
        public int AddClient(ModifyAspNetClient modal)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new DBModel.AspNetClients
                    {
                        Name = modal.Name,
                    };
                    dbContext.AspNetClientsSet.Add(item);
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
        /// Modify client parameters
        /// </summary>
        /// <param name="modal"></param>
        /// <exception cref="DictionaryRecordCouldNotBeAdded"></exception>
        public void UpdateClient(ModifyAspNetClient modal)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new DBModel.AspNetClients
                    {
                        Id = modal.Id,
                        Name = modal.Name,
                    };
                    dbContext.AspNetClientsSet.Attach(item);

                    var entry = dbContext.Entry(item);
                    entry.Property(p => p.Name).IsModified = true;

                    dbContext.SaveChanges();
                }
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        /// <summary>
        /// Delete client from the list
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DictionaryRecordCouldNotBeDeleted"></exception>
        public void DeleteClient(int id)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    var item = new DBModel.AspNetClients
                    {
                        Id = id
                    };
                    dbContext.AspNetClientsSet.Attach(item);

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