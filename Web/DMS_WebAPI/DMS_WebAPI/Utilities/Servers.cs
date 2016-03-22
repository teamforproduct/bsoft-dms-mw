using BL.Model.Database;
using BL.Model.Database.FrontModel;
using BL.Model.Database.IncomingModel;
using BL.Model.Exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace DMS_WebAPI.Utilities
{
    /// <summary>
    /// Represend functionality to configure available servers
    /// </summary>
    public class Servers
    {
        private string _file = "/servers.xml";

        private string _ServerFilePath
        {
            get { return HttpContext.Current.Server.MapPath(_file); }
        }

        private XmlDocument _File
        {
            get
            {
                var fileName = _ServerFilePath;
                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException();
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);
                return doc;
            }
        }

        /// <summary>
        /// Get list of the available servers to display for user.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FrontServer> GetServersByUser()
        {
            return GetServers().ToList().Select(x => new FrontServer { Id = x.Id, Name = x.Name });
        }

        /// <summary>
        /// Get all server parameters by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DatabaseModel GetServer(int id)
        {
            return GetServers().FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// List of all aceptable servers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DatabaseModel> GetServers()
        {
            var root = _File.DocumentElement;
            var res = new List<DatabaseModel>();

            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.HasChildNodes)
                {
                    var item = new DatabaseModel();

                    foreach (XmlNode child in n.ChildNodes)
                    {
                        switch (child.Name.ToLower())
                        {
                            case "id":
                                item.Id = int.Parse(child.InnerText);
                                break;
                            case "address":
                                item.Address = child.InnerText;
                                break;
                            case "name":
                                item.Name = child.InnerText;
                                break;
                            case "servertype":
                                item.ServerType = (DatabaseType)Enum.Parse(typeof(DatabaseType), child.InnerText);
                                break;
                            case "defaultdatabase":
                                item.DefaultDatabase = child.InnerText;
                                break;
                            case "integratesecurity":
                                item.IntegrateSecurity = bool.Parse(child.InnerText);
                                break;
                            case "username":
                                item.UserName = child.InnerText;
                                break;
                            case "userpassword":
                                item.UserPassword = child.InnerText;
                                break;
                            case "connectionstring":
                                item.ConnectionString = child.InnerText;
                                break;
                            case "defaultschema":
                                item.DefaultSchema = child.InnerText;
                                break;
                        }
                    }
                    res.Add(item);
                }

            }

            return res;
        }

        /// <summary>
        /// Add new server to list of aceptable servers
        /// </summary>
        /// <param name="modal"></param>
        /// <returns></returns>
        /// <exception cref="DictionaryRecordCouldNotBeAdded"></exception>
        public int AddServer(ModifyServer modal)
        {
            try
            {
                modal.Id = GetServers().Max(x => x.Id + 1);

                var doc = _File;
                var root = doc.DocumentElement;

                XmlElement item = doc.CreateElement("Server");
                var tmpItem = doc.CreateElement("Id");
                tmpItem.InnerText = modal.Id.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("Name");
                tmpItem.InnerText = modal.Name;
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("Address");
                tmpItem.InnerText = modal.Address;
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("ServerType");
                tmpItem.InnerText = modal.ServerType.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("DefaultDatabase");
                tmpItem.InnerText = modal.DefaultDatabase;
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("IntegrateSecurity");
                tmpItem.InnerText = modal.IntegrateSecurity.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("UserName");
                tmpItem.InnerText = modal.UserName;
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("UserPassword");
                tmpItem.InnerText = modal.UserPassword;
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("ConnectionString");
                tmpItem.InnerText = modal.ConnectionString;
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("DefaultSchema");
                tmpItem.InnerText = modal.DefaultSchema.ToString();
                item.AppendChild(tmpItem);

                root.AppendChild(item);

                doc.Save(_ServerFilePath);

                return modal.Id;
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        /// <summary>
        /// Modify server parameters
        /// </summary>
        /// <param name="modal"></param>
        /// <exception cref="DictionaryRecordCouldNotBeAdded"></exception>
        public void UpdateServer(ModifyServer modal)
        {
            try
            {
                var doc = _File;
                var root = doc.DocumentElement;

                foreach (XmlNode n in root.ChildNodes)
                {
                    if (n.HasChildNodes)
                    {
                        if (n["Id"].InnerText == modal.Id.ToString())
                        {
                            n["Name"].InnerText = modal.Name;
                            n["Address"].InnerText = modal.Address;
                            n["ServerType"].InnerText = modal.ServerType.ToString();
                            n["DefaultDatabase"].InnerText = modal.DefaultDatabase;
                            n["IntegrateSecurity"].InnerText = modal.IntegrateSecurity.ToString();
                            n["UserName"].InnerText = modal.UserName;
                            n["UserPassword"].InnerText = modal.UserPassword;
                            n["ConnectionString"].InnerText = modal.ConnectionString;
                            n["DefaultSchema"].InnerText = modal.DefaultSchema.ToString();
                            break;
                        }
                    }

                }
                doc.Save(_ServerFilePath);
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeAdded();
            }
        }

        /// <summary>
        /// Delete server from the list
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="DictionaryRecordCouldNotBeDeleted"></exception>
        public void DeleteServer(int id)
        {
            try
            {
                var doc = _File;
                var root = doc.DocumentElement;

                foreach (XmlNode n in root.ChildNodes)
                {
                    if (n.HasChildNodes)
                    {
                        if (n["Id"].InnerText == id.ToString())
                        {
                            n.ParentNode.RemoveChild(n);
                            break;
                        }
                    }
                }
                doc.Save(_ServerFilePath);
            }
            catch
            {
                throw new DictionaryRecordCouldNotBeDeleted();
            }
        }
    }
}