using BL.Model.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DMS_WebAPI.Utilities
{
    public class ReadXml
    {
        private string _file { get; set; }
        private string _ServerFilePath
        {
            get { return HttpContext.Current.Server.MapPath(_file); }
        }
        public ReadXml(string file)
        {
            _file = file;
        }
        /*
            <Server>
            <Name>Test server</Name>
            <Address>109.197.217.79\SQLEXPRESS,1433</Address>
            <MainDB>IRF_DMS</MainDB>
            <IntegrationSecurity>False</IntegrationSecurity>
            </Server>
            <Server>
            <Name>Test server</Name>
            <Address>109.197.217.79\SQLEXPRESS,1433</Address>
            <MainDB>IRF_DMS</MainDB>
            <IntegrationSecurity>False</IntegrationSecurity>
            </Server>
            */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">/servers.xml</param>
        /// <returns>Json</returns>
        public List<Dictionary<string, object>> Read()
        {
            var fileName = _ServerFilePath;
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException();
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            var root = doc.DocumentElement;
            var res = new List<Dictionary<string, object>>();

            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.HasChildNodes)
                {
                    var values = new Dictionary<string, object>();

                    foreach (XmlNode child in n.ChildNodes)
                    {
                        values.Add(child.Name, child.InnerText);
                    }
                    res.Add(values);
                }

            }

            return res;
        }
        public List<DatabaseModel> ReadDBs()
        {
            var fileName = _ServerFilePath;
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException();
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            var root = doc.DocumentElement;
            var res = new List<DatabaseModel>();

            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.HasChildNodes)
                {
                    var isAdd = true;
                    var db = new DatabaseModel();

                    foreach (XmlNode child in n.ChildNodes)
                    {
                        switch (child.Name.ToLower())
                        {
                            case "id":
                                isAdd = true;
                                db.Id = int.Parse(child.InnerText);
                                break;
                            case "address":
                                db.Address = child.InnerText;
                                break;
                            case "name":
                                db.Name = child.InnerText;
                                break;
                            case "servertype":
                                db.ServerType = (DatabaseType)Enum.Parse(typeof(DatabaseType), child.InnerText);
                                break;
                            case "defaultdatabase":
                                db.DefaultDatabase = child.InnerText;
                                break;
                            case "integratesecurity":
                                db.IntegrateSecurity = bool.Parse(child.InnerText);
                                break;
                            case "username":
                                db.UserName = child.InnerText;
                                break;
                            case "userpassword":
                                db.UserPassword = child.InnerText;
                                break;
                            case "connectionstring":
                                db.ConnectionString = child.InnerText;
                                break;
                        }
                    }
                    if (isAdd)
                        res.Add(db);
                }

            }

            return res;
        }
        public object ReadDBsByUI()
        {
            return ReadDBs().Select(x=>new { Id = x.Id, Name= x.Name });
        }
    }
}
