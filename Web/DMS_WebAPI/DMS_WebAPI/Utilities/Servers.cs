using BL.CrossCutting.Interfaces;
using BL.Logic.Context;
using BL.Model.Database;
using BL.Model.Exception;
using BL.Model.SystemCore.FrontModel;
using BL.Model.SystemCore.IncomingModel;
using BL.Model.SystemCore.InternalModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace DMS_WebAPI.Utilities
{
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

        public IEnumerable<FrontServer> GetServersByUser()
        {
            return GetServers().ToList().Select(x => new FrontServer { Id = x.Id, Name = x.Name });
        }
        public InternalServer GetServer(int id)
        {
            return GetServers().FirstOrDefault(x => x.Id == id);
        }
        public IEnumerable<InternalServer> GetServers()
        {
            var root = _File.DocumentElement;
            var res = new List<InternalServer>();

            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.HasChildNodes)
                {
                    var isAdd = true;
                    var item = new InternalServer();

                    foreach (XmlNode child in n.ChildNodes)
                    {
                        switch (child.Name.ToLower())
                        {
                            case "id":
                                isAdd = true;
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
                        }
                    }
                    if (isAdd)
                        res.Add(item);
                }

            }

            return res;
        }
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
                tmpItem.InnerText = modal.Name.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("Address");
                tmpItem.InnerText = modal.Address.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("ServerType");
                tmpItem.InnerText = modal.ServerType.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("DefaultDatabase");
                tmpItem.InnerText = modal.DefaultDatabase.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("IntegrateSecurity");
                tmpItem.InnerText = modal.IntegrateSecurity.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("UserName");
                tmpItem.InnerText = modal.UserName.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("UserPassword");
                tmpItem.InnerText = modal.UserPassword.ToString();
                item.AppendChild(tmpItem);

                tmpItem = doc.CreateElement("ConnectionString");
                tmpItem.InnerText = modal.ConnectionString.ToString();
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
                            n["Name"].InnerText = modal.Name.ToString();
                            n["Address"].InnerText = modal.Address.ToString();
                            n["ServerType"].InnerText = modal.ServerType.ToString();
                            n["DefaultDatabase"].InnerText = modal.DefaultDatabase.ToString();
                            n["IntegrateSecurity"].InnerText = modal.IntegrateSecurity.ToString();
                            n["UserName"].InnerText = modal.UserName.ToString();
                            n["UserPassword"].InnerText = modal.UserPassword.ToString();
                            n["ConnectionString"].InnerText = modal.ConnectionString.ToString();
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