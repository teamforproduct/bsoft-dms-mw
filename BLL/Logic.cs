using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Security.Cryptography;

namespace BLL
{
    public static class Logic
    {
        public static void GetDocuments()
        {
            string query = "begin comm.SET_CLIENT_INFO(:userid,:dataset,:complex, :session_id); :p1:=DOB_CURSORS.GET_DOCUMENT(:jsontext); end;";

            using (OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["EntitiesBSOFT_DOB"].ConnectionString))
            {
                OracleCommand cmd = new OracleCommand(query, con);
                cmd.CommandType = System.Data.CommandType.Text;

                var p1 = cmd.Parameters.Add(":p1", OracleDbType.RefCursor, ParameterDirection.ReturnValue);
                var jsontext = cmd.Parameters.Add(":jsontext", OracleDbType.Clob);
                jsontext.Value = "{\"CRT\" : {},\"LIM\" : {\"START\" : 0, \"LENGTH\" : 1}}";

                var userid = cmd.Parameters.Add(":userid", OracleDbType.Varchar2);
                jsontext.Value = "";
                var dataset = cmd.Parameters.Add(":dataset", OracleDbType.Varchar2);
                jsontext.Value = "";
                var complex = cmd.Parameters.Add(":complex", OracleDbType.Varchar2);
                jsontext.Value = "";
                var session_id = cmd.Parameters.Add(":session_id", OracleDbType.Varchar2);
                jsontext.Value = "";

                con.Open();

                OracleDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //Console.WriteLine("level: {0}", reader.GetDecimal(0));
                }

                //OracleDataAdapter ad = new OracleDataAdapter(cmd);
                //OracleCommandBuilder cb = new OracleCommandBuilder(ad);

                //DataTable dt = new DataTable();
                //ad.Fill(dt);

            }
        }
        public static void GetDocuments1()
        {
            string query = "AUTH.LOGIN";//"BSOFT_INI_REAL.AUTH.LOGIN";

            using (OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["EntitiesBSOFT_INI_REAL"].ConnectionString))
            {
                OracleCommand cmd = new OracleCommand(query, con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var result = cmd.Parameters.Add("Result", OracleDbType.Varchar2, ParameterDirection.ReturnValue);
                result.Size = 32767;
                var loginP = cmd.Parameters.Add("P_UID", OracleDbType.Varchar2, 32767, ParameterDirection.Input);
                loginP.Size = 32767;
                loginP.Value = "aav";
                var PassP = cmd.Parameters.Add("P_PWD", OracleDbType.Varchar2, 32767, ParameterDirection.Input);
                PassP.Size = 32767;
                PassP.Value = "698D51A19D8A121CE581499D7B701668";

                con.Open();

                cmd.ExecuteNonQuery();

            }
        }
        public static void Auth_login(string login, string md5_password)
        {
            string query = "AUTH.LOGIN";//"BSOFT_INI_REAL.AUTH.LOGIN";

            using (OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["EntitiesBSOFT_INI_REAL"].ConnectionString))
            {
                OracleCommand cmd = new OracleCommand(query, con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var result = cmd.Parameters.Add("Result", OracleDbType.Varchar2, ParameterDirection.ReturnValue);
                result.Size = 32767;
                var loginP = cmd.Parameters.Add("P_UID", OracleDbType.Varchar2, 32767, ParameterDirection.Input);
                loginP.Size = 32767;
                loginP.Value = "aav";
                var PassP = cmd.Parameters.Add("P_PWD", OracleDbType.Varchar2, 32767, ParameterDirection.Input);
                PassP.Size = 32767;
                PassP.Value = "698D51A19D8A121CE581499D7B701668";

                con.Open();

                cmd.ExecuteNonQuery();

            }
        }
        public static void Comm_Set_Client_Info(string login, string md5_password)
        {
            string query = "COMM.SET_CLIENT_INFO";

            using (OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["EntitiesBSOFT_INI_REAL"].ConnectionString))
            {
                OracleCommand cmd = new OracleCommand(query, con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var pUser = cmd.Parameters.Add("P_USER", OracleDbType.Int32, ParameterDirection.Input);
                //pUser.Value =
                var pDataset = cmd.Parameters.Add("P_DATASET", OracleDbType.Int32, ParameterDirection.Input);
                var pComplexId = cmd.Parameters.Add("P_COMPLEX_ID", OracleDbType.Int32, ParameterDirection.Input);
                var pClientSessionId = cmd.Parameters.Add("P_CLIENTSESSIONID", OracleDbType.Int32, ParameterDirection.Input);

                con.Open();

                cmd.ExecuteNonQuery();

            }
        }
        public static string MD5Signature(string str)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] bSignature = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
            var sbSignature = new StringBuilder();
            foreach (byte b in bSignature)
                sbSignature.AppendFormat("{0:x2}", b);
            return sbSignature.ToString();
        }
    }
}
