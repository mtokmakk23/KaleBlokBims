using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KaleBlokBims.Models.Classlar
{
    public class SmsGonderme
    {
        string userName = "8503023094";
        string password = "s9-3R914";
        public void smsGonder(string Metin,string gsm)
        {
             
            if (gsm[gsm.Length - 1].ToString() != ",")
            {
                gsm = gsm + ",";
            }
            var bccString = gsm.Split(',');
            for (int i = 0; i < bccString.Length - 1; i++)
            {
                var client = new RestClient("http://soap.netgsm.com.tr:8080/Sms_webservis/SMS?wsdl");
                var request = new RestRequest();
                request.Method = RestSharp.Method.Post;
                request.AddHeader("Content-Type", "text/xml");
                var body = @"<?xml version=""1.0""?>
" + "\n" +
                @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/""
" + "\n" +
                @"             xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
" + "\n" +
                @"  xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
" + "\n" +
                @"    <SOAP-ENV:Body>
" + "\n" +
                @"        <ns3:smsGonder1NV2 xmlns:ns3=""http://sms/"">
" + "\n" +
                @"            <username>" + userName + "</username>" +
        "\n" +
                @"            <password>" + password + "</password>" +
        "\n" +
                @"            <header>KALEBLKBIMS</header>
" + "\n" +
                @"            <msg>" + Metin + "</msg>" +
        "\n" +
                @"            <gsm>" + bccString[i] + "</gsm>" +
        "\n" +
                @"            <encoding>TR</encoding>
" + "\n" +
                @"        </ns3:smsGonder1NV2>
" + "\n" +
                @"    </SOAP-ENV:Body>
" + "\n" +
                @"</SOAP-ENV:Envelope>";
                request.AddParameter("text/xml", body, ParameterType.RequestBody);
                var response = client.Execute(request);
            }


           
        }
    }
}