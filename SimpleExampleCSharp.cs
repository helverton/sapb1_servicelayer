using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HelvertonSantos.Main
{
    public class B1SLSession
    {
        public string odatacontext { get; set; }
        public string SessionId { get; set; }
        public string Version { get; set; }
        public int SessionTimeout { get; set; }
    }

    public class B1SLLogin
    {
        public string CompanyDB { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class B1SLOrder
    {
        public int DocEntry { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public List<DocumentLines> DocumentLines { get; set; }
    }

    public class DocumentLines
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
    }

    public class B1SL_Get
    {
        private B1SLSession Session(string companyDb, string UsrName, string Psswrd)
        {
            B1SLLogin login = new B1SLLogin
            {
                CompanyDB = companyDb,
                UserName = UsrName,
                Password = Psswrd
            };

            string jsonLogin = JsonConvert.SerializeObject(login);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://host*:50000/b1s/v2/Login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.Headers.Add("B1S-WCFCompatible", "true");
            httpWebRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
            httpWebRequest.Accept = "*/*";
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            { streamWriter.Write(jsonLogin); }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            B1SLSession obj = null;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                obj = JsonConvert.DeserializeObject<B1SLSession>(result);
            }

            return obj;
        }

        public dynamic GetOrder(int docEntry)
        {
            B1SLSession obj = Session("SBO_DEMO", "manager", "123456");

            var httpWebGetRequest = (HttpWebRequest)WebRequest.Create($"https://host*:50000/b1s/v2/Orders({docEntry})");
            httpWebGetRequest.ContentType = "application/json";
            httpWebGetRequest.Method = "GET";
            httpWebGetRequest.KeepAlive = true;
            httpWebGetRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebGetRequest.Headers.Add("B1S-WCFCompatible", "true");
            httpWebGetRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
            httpWebGetRequest.Accept = "*/*";
            httpWebGetRequest.ServicePoint.Expect100Continue = false;
            httpWebGetRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            httpWebGetRequest.AutomaticDecompression = DecompressionMethods.GZip;
            CookieContainer cookies = new CookieContainer();
            cookies.Add(new Cookie("B1SESSION", obj.SessionId.ToString()) { Domain = "host*" });
            cookies.Add(new Cookie("ROUTEID", ".node1") { Domain = "host*" });
            httpWebGetRequest.CookieContainer = cookies;
            var httpGetResponse = (HttpWebResponse)httpWebGetRequest.GetResponse();
            dynamic objDocument = null;
            using (var streamReader = new StreamReader(httpGetResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                objDocument = JsonConvert.DeserializeObject<B1SLOrder>(result);
            }


            return objDocument;
        }
    }
}
