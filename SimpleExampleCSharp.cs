
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
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
        public List<Documentline> DocumentLines { get; set; }
    }

    public class Documentline
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public int Usage { get; set; }
        public double LineTotal { get; set; }
    }

    public class B1SL_Req
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

        public dynamic OrderGetAsync(int docEntry)
        {
            B1SLSession obj = Session("SBO_DEMO", "manager", "123456");

            HttpWebRequest httpWebGetRequest = (HttpWebRequest)WebRequest.Create($"https://host*:50000/b1s/v2/Orders({docEntry})");
            httpWebGetRequest.Method = "GET";
            httpWebGetRequest.KeepAlive = true;
            httpWebGetRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebGetRequest.Accept = "*/*";
            httpWebGetRequest.ServicePoint.Expect100Continue = false;
            httpWebGetRequest.AutomaticDecompression = DecompressionMethods.GZip;

            httpWebGetRequest.Headers.Add("B1S-WCFCompatible", "true");
            httpWebGetRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
            httpWebGetRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");

            CookieContainer cookies = new CookieContainer();
            cookies.Add(new Cookie("B1SESSION", obj.SessionId.ToString()) { Domain = "host*" });
            cookies.Add(new Cookie("ROUTEID", ".node1") { Domain = "host*" });
            httpWebGetRequest.CookieContainer = cookies;

            httpWebGetRequest.ContentType = "application/json";
            HttpWebResponse httpGetResponse = (HttpWebResponse)httpWebGetRequest.GetResponse();

            B1SLOrder objDocument = null;
            using (StreamReader streamReader = new StreamReader(httpGetResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                objDocument = JsonConvert.DeserializeObject<B1SLOrder>(result);
            }

            return objDocument;
        }

        public void OrderPatchAsync(int docEntry, object order)
        {
            B1SLSession obj = Session("SBO_DEMO", "manager", "123456");

            HttpWebRequest httpWebGetRequest = (HttpWebRequest)WebRequest.Create($"https://host*:50000/b1s/v2/Orders({docEntry})");
            httpWebGetRequest.Method = "PATCH";
            httpWebGetRequest.KeepAlive = true;
            httpWebGetRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebGetRequest.Accept = "*/*";
            httpWebGetRequest.ServicePoint.Expect100Continue = false;
            httpWebGetRequest.AutomaticDecompression = DecompressionMethods.GZip;

            httpWebGetRequest.Headers.Add("B1S-WCFCompatible", "true");
            httpWebGetRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
            httpWebGetRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");

            CookieContainer cookies = new CookieContainer();
            cookies.Add(new Cookie("B1SESSION", obj.SessionId.ToString()) { Domain = "host*" });
            cookies.Add(new Cookie("ROUTEID", ".node1") { Domain = "host*" });
            httpWebGetRequest.CookieContainer = cookies;

            string strbody = JsonConvert.SerializeObject(order);
            Encoding encoding = Encoding.Default;
            byte[] buffer = encoding.GetBytes(strbody);
            Stream dataStream = httpWebGetRequest.GetRequestStream();
            dataStream.Write(buffer, 0, buffer.Length);
            dataStream.Close();

            httpWebGetRequest.ContentType = "application/json";
            HttpWebResponse httpGetResponse = (HttpWebResponse)httpWebGetRequest.GetResponse();

            using (StreamReader streamReader = new StreamReader(httpGetResponse.GetResponseStream(), Encoding.Default))
            {
                var result = streamReader.ReadToEnd();
            }
        }

        public void UpdateOrder()
        {
            try
            {

            }
            catch (System.Exception)
            {

                throw;
            }

            try
            {
                //Get
                //Object as OData Order SL 
                B1SLOrder ordrG = OrderGetAsync(123456);
                //Get


                //Read
                //Using List C#
                List<List<Documentline>> splitUsages = ordrG.DocumentLines.OrderBy(y => y.Usage)
                                                                          .GroupBy(l => l.Usage)
                                                                          .Select(group => group.ToList())
                                                                          .ToList();

                //Read data main
                string strCardCode = ordrG.CardCode;
                string strCardName = ordrG.CardName;


                //Update data in line
                foreach (var line in ordrG.DocumentLines)
                {
                    string strItemCode = line.ItemCode;
                }


                foreach (var lst in splitUsages)
                {
                    if (lst.First().Usage == 10)
                    {
                        var DocTotal = lst.Select(x => x.LineTotal).Sum();
                    }
                }
                //Read


                //Patch
                dynamic tst = new System.Dynamic.ExpandoObject();
                //Update data main
                tst.Comments = "Test SL";
                tst.DiscountPercent = 0;

                //Update data in line
                tst.DocumentLines = ordrG.DocumentLines.Select(x => { dynamic aux = new System.Dynamic.ExpandoObject(); aux.LineNum = x.LineNum; aux.WarehouseCode = "03"; return aux; }).ToList();

                OrderPatchAsync(123456, tst);
                //Patch
            }
            catch (Exception e)
            {
                var error = e.Message;
            }
        }
    }
}
