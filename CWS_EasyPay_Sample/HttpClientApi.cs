#region DISCLAIMER
/* Copyright (c) 2018 NAB, LLC. - All Rights Reserved.
 *
 * This software and documentation is subject to and made
 * available only pursuant to the terms of an executed license
 * agreement, and may be used only in accordance with the terms
 * of said agreement. This software may not, in whole or in part,
 * be copied, photocopied, reproduced, translated, or reduced to
 * any electronic medium or machine-readable form without
 * prior consent, in writing, from NAB, LLC.
 *
 * Use, duplication or disclosure by the U.S. Government is subject
 * to restrictions set forth in an executed license agreement
 * and in subparagraph (c)(1) of the Commercial Computer
 * Software-Restricted Rights Clause at FAR 52.227-19; subparagraph
 * (c)(1)(ii) of the Rights in Technical Data and Computer Software
 * clause at DFARS 252.227-7013, subparagraph (d) of the Commercial
 * Computer Software--Licensing clause at NASA FAR supplement
 * 16-52.227-86; or their equivalent.
 *
 * Information in this software is subject to change without notice
 * and does not represent a commitment on the part of NAB.
 * 
 * Sample Code is for reference Only and is intended to be used for educational purposes. It's the responsibility of 
 * the software company to properly integrate into thier solution code that best meets thier production needs. 
*/
#endregion DISCLAIMER

// Reference: https://msdn.microsoft.com -- C#, HTTP, REST documentation/examples
// Reference: https://www.newtonsoft.com/json

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;


namespace CWS_EasyPay_Sample
{
    public class HttpClientApi
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private string httpResponseData = "";
        private string httpExceptionType = "NONE";
        private string httpReadException = "NONE";
        private string httpWriteException = "NONE";
        private string httpExceptionData = "";
        private string webReqStatusDesc = "NONE";
        private string webReqStatusCode = "NONE";
        private string httpExceptionStatus = "NONE";
        private HttpStatusCode httpStatusCode;

        public string responseData
        {
            get { return httpResponseData; }
        }
        public string exceptionType
        {
            get { return httpExceptionType; }
        }
        public string readException
        {
            get { return httpReadException; }
        }
        public string writeException
        {
            get { return httpWriteException; }
        }
        public string exceptionData
        {
            get { return httpExceptionData; }
        }
        public string statusCode
        {
            get { return webReqStatusCode; }
        }

        public string lastStatus
        {
            get { return webReqStatusDesc; }
        }

        public HttpStatusCode httpStatus
        {
            get { return httpStatusCode; }
        }

        public string exceptionStatus
        {
            get { return httpExceptionStatus; }
        }
        public async Task<string> ProcessTransactionAsync(string uri, string postDataXml)
        {
            string transactionResponse;
            try
            {
                transactionResponse = await performHttpRequestAsync(uri, postDataXml, "POST", "application/xml");
            }
            catch (Exception ex)
            {
                transactionResponse = ex.Message;
            }
            return transactionResponse;
        }

        public string ProcessTransaction(string uri, string postDataXml)
        {
            string transactionResponse;
            try
            {
                transactionResponse = performHttpRequest(uri, postDataXml, "POST", "application/xml");
            }
            catch (Exception ex)
            {
                transactionResponse = ex.Message;
            }
            return transactionResponse;
        }

        public string ProcessRestTransaction(string uri, string postDataJson, string sessionToken, string sessionPwd = "", string httpMethod = "GET")
        {
            string transactionResponse;
            try
            {
                transactionResponse = performRestRequest(uri, postDataJson, sessionToken, sessionPwd, httpMethod);
            }
            catch (Exception ex)
            {
                transactionResponse = ex.Message;
            }
            return transactionResponse;
        }

        public string GetSessionToken(string uri, string protocol = "https://", string method = "GET", string contentType = "application/json", string identityToken = "", string pwd = "")
        {
            string sessionToken = "";
            if (!uri.Contains("http")) { uri = protocol + uri; }
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
            webReq.Method = method;
            webReq.ContentType = contentType;

            string authHeader = GetAuthHeader(identityToken, pwd);
            webReq.Headers.Add(HttpRequestHeader.Authorization, authHeader);

            HttpWebResponse webResp = null;
            try
            {
                webResp = (HttpWebResponse)webReq.GetResponse();
                httpStatusCode = webResp.StatusCode;
                webReqStatusDesc = webResp.StatusCode.ToString();
                webReqStatusCode = webResp.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                httpExceptionType = "GETRESPONSE";
                httpStatusCode = webResp.StatusCode;
                webReqStatusDesc = "FAILGETRESPONSE";
                webReqStatusCode = webResp.StatusCode.ToString();
                httpExceptionData = ex.StackTrace;
                httpExceptionStatus = "EXCEPTION";
            }
            if (webResp.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    sessionToken = readData(webResp);
                    sessionToken = sessionToken.Substring(1, sessionToken.Length - 2);
                }
                catch (Exception ex)
                {
                    httpExceptionType = "READRESPONSEDATA";
                    webReqStatusDesc = "FAILREADRESPONSEDATA";
                    httpExceptionData = ex.StackTrace;
                    httpExceptionStatus = "EXCEPTION";
                }
            }

            return sessionToken;
        }

        private string GetAuthHeader(string userName, string password)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(userName + ":" + password);
            string authHeader = "Basic " + Convert.ToBase64String(plainTextBytes);
            return authHeader;
        }

        private string performRestRequest(string uri, string data, string userName, string pwd = "", string method = "GET", string contentType = "application/json")
        {
            if (!uri.Contains("http")) { uri = "https://" + uri; }
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
            webReq.Method = method;
            webReq.ContentType = contentType;

            string authHeader = GetAuthHeader(userName, pwd);
            webReq.Headers.Add(HttpRequestHeader.Authorization, authHeader);
            try
            {
                try
                {
                    writeRequestData(webReq, data);
                }
                catch (Exception ex)
                {
                    httpExceptionType = "WRITEREQUESTDATA";
                    webReqStatusDesc = "FAILWRITEREQUEST";
                    httpExceptionData = ex.StackTrace;
                    httpExceptionStatus = "EXCEPTION";
                    throw ex;
                }

                try
                {
                    webReqStatusDesc = "SENDGETRESPONSE";
                    HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                    httpStatusCode = webResp.StatusCode;
                    webReqStatusCode = webResp.StatusCode.ToString();
                    webReqStatusDesc = "RESPONSERECEIVED";
                    httpResponseData = readData(webResp);
                    webReqStatusDesc = "RESPONSEDATARETRIEVED";
                }
                catch (Exception ex)
                {
                    httpExceptionType = "FAILRESPONSE";
                    httpExceptionData = ex.StackTrace;
                    httpExceptionStatus = "EXCEPTION";

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                httpResponseData = "Exception performRestRequest " + httpStatusCode + " " + webReqStatusDesc + " " + ex.Message + " " + httpExceptionData;
            }
            return httpResponseData;
        }

        private async Task<string> performRestRequestAsync(string uri, string data, string userName, string pwd = "", string method = "GET", string contentType = "application/json")
        {
            HttpResponseMessage httpResp = new HttpResponseMessage();
            if (!uri.Contains("http")) { uri = "https://" + uri; }

            string authHeader = GetAuthHeader(userName, pwd);
            _httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);

            try
            {
                httpResp = await _httpClient.PostAsync(uri, new StringContent(data));
                httpResp.EnsureSuccessStatusCode();
                httpStatusCode = httpResp.StatusCode;
                webReqStatusCode = httpResp.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                httpExceptionType = "ASYNCPOST";
                httpExceptionData = ex.StackTrace;
                webReqStatusDesc = "POSTASYNCFAIL";
                httpExceptionStatus = "EXCEPTION";
            }
            httpResponseData = await httpResp.Content.ReadAsStringAsync();
            httpStatusCode = httpResp.StatusCode;
            webReqStatusCode = httpResp.StatusCode.ToString();
            return httpResponseData;
        }
        private string performHttpRequest(string uri, string data, string method = "GET", string contentType = "application/json")
        {
            if (!uri.Contains("http")) { uri = "http://" + uri; }
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
            webReq.Method = method;
            webReq.ContentType = contentType;
            try
            {
                try
                {
                    writeRequestData(webReq, data);
                }
                catch (Exception ex)
                {
                    httpExceptionType = "WRITEREQUESTDATA";
                    webReqStatusDesc = "FAILWRITEREQUESTDATA";
                    httpExceptionData = ex.StackTrace;
                    httpExceptionStatus = "EXCEPTION";
                    throw ex;
                }

                try
                {
                    webReqStatusDesc = "SENDGETRESPONSE";
                    HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                    httpStatusCode = webResp.StatusCode;
                    webReqStatusCode = webResp.StatusCode.ToString();
                    webReqStatusDesc = "RESPONSERECEIVED";
                    httpResponseData = readData(webResp);
                    webReqStatusDesc = "RESPONSEDATARETRIEVED";
                }
                catch (Exception ex)
                {
                    httpExceptionType = "FAILRESPONSE";
                    httpExceptionData = ex.StackTrace;
                    httpExceptionStatus = "EXCEPTION";
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                httpResponseData = "Exception performHttpRequest " + webReqStatusDesc + " " + ex.Message + " " + httpExceptionData;
            }

            return httpResponseData;
        }

        private async Task<string> performHttpRequestAsync(string uri, string data, string method = "GET", string contentType = "application/json")
        {
            HttpResponseMessage responseMsg = new HttpResponseMessage();
            if (!uri.Contains("http")) { uri = "http://" + uri; }
            try
            {
                responseMsg = await _httpClient.PostAsync(uri, new StringContent(data));
                responseMsg.EnsureSuccessStatusCode();

                httpResponseData = await responseMsg.Content.ReadAsStringAsync();
                httpStatusCode = responseMsg.StatusCode;
                webReqStatusCode = responseMsg.StatusCode.ToString();
            }
            catch (Exception ex)
            {
                httpExceptionType = "ASYNCPOST";
                httpExceptionData = ex.StackTrace;
                webReqStatusDesc = "POSTASYNCFAIL";
                httpExceptionStatus = "EXCEPTION";
                httpResponseData = "Exception performHttpRequest " + webReqStatusDesc + " " + ex.Message + " " + httpExceptionData;
            }

            return httpResponseData;
        }

        private void writeRequestData(HttpWebRequest webReq, string postDataXml)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(webReq.GetRequestStream()))
            {
                // Write the xml as text into the stream
                sw.WriteLine(postDataXml);  
            }
        }

        private string readData(HttpWebResponse webResp)
        {
            string responseData;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(webResp.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                responseData = sr.ReadToEnd();
            }
            return responseData;
        }
    }
}
