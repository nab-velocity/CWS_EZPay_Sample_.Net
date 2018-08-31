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

namespace CWS_EasyPay_Sample
{
    public class PaymentProcessor
    {
        string terminalUri;

        private static string WORKFLOW_ID = "8D9DE00001"; // This value would vary in production; as well as based on other factors.

        private static string APP_PROFILE_ID = "71228"; // This value will be different for each integrator/merchant
        private static string MERCH_PROFILE_ID = "TMS Test EPX"; // This value will be different for each integrator/merchant

        private static string SERVICE_INFO = "https://api.cert.nabcommerce.com/REST/2.0.18/SvcInfo";
        private static string IDENTITY_TOKEN = // this value is different for different merchants/integrators. Please obtain one for your solution
            // this token will also be different from that of the production token. This is a TEST token for an existing test merchant/integrator.
        "PHNhbWw6QXNzZXJ0aW9uIE1ham9yVmVyc2lvbj0iMSIgTWlub3JWZXJzaW9uPSIxIiBBc3NlcnRpb25JRD0iXzU4ZTM5MGFjLTE0OWItNDNiYS1iOGU0LTQ3ZmEzOWUyZWM3MCIgSXNzdWVyPSJJcGNBdXRoZW50aWNhdGlvbiIgSXNzdWVJbnN0YW50PSIyMDE2LTA5LTIwVDIxOjQ5OjE1LjI0MloiIHhtbG5zOnNhbWw9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjEuMDphc3NlcnRpb24iPjxzYW1sOkNvbmRpdGlvbnMgTm90QmVmb3JlPSIyMDE2LTA5LTIwVDIxOjQ5OjE1LjI0MloiIE5vdE9uT3JBZnRlcj0iMjA0Ni0wOS0yMFQyMTo0OToxNS4yNDJaIj48L3NhbWw6Q29uZGl0aW9ucz48c2FtbDpBZHZpY2U+PC9zYW1sOkFkdmljZT48c2FtbDpBdHRyaWJ1dGVTdGF0ZW1lbnQ+PHNhbWw6U3ViamVjdD48c2FtbDpOYW1lSWRlbnRpZmllcj4xMzEzODY4NTQxMzAwMDAxPC9zYW1sOk5hbWVJZGVudGlmaWVyPjwvc2FtbDpTdWJqZWN0PjxzYW1sOkF0dHJpYnV0ZSBBdHRyaWJ1dGVOYW1lPSJTQUsiIEF0dHJpYnV0ZU5hbWVzcGFjZT0iaHR0cDovL3NjaGVtYXMuaXBjb21tZXJjZS5jb20vSWRlbnRpdHkiPjxzYW1sOkF0dHJpYnV0ZVZhbHVlPjEzMTM4Njg1NDEzMDAwMDE8L3NhbWw6QXR0cmlidXRlVmFsdWU+PC9zYW1sOkF0dHJpYnV0ZT48c2FtbDpBdHRyaWJ1dGUgQXR0cmlidXRlTmFtZT0iU2VyaWFsIiBBdHRyaWJ1dGVOYW1lc3BhY2U9Imh0dHA6Ly9zY2hlbWFzLmlwY29tbWVyY2UuY29tL0lkZW50aXR5Ij48c2FtbDpBdHRyaWJ1dGVWYWx1ZT5hNWMyYmQ3ZS1iMTA3LTQ0YjYtODhhZS01YjM2ZTYxYjEyY2U8L3NhbWw6QXR0cmlidXRlVmFsdWU+PC9zYW1sOkF0dHJpYnV0ZT48c2FtbDpBdHRyaWJ1dGUgQXR0cmlidXRlTmFtZT0ibmFtZSIgQXR0cmlidXRlTmFtZXNwYWNlPSJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcyI+PHNhbWw6QXR0cmlidXRlVmFsdWU+MTMxMzg2ODU0MTMwMDAwMTwvc2FtbDpBdHRyaWJ1dGVWYWx1ZT48L3NhbWw6QXR0cmlidXRlPjwvc2FtbDpBdHRyaWJ1dGVTdGF0ZW1lbnQ+PFNpZ25hdHVyZSB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC8wOS94bWxkc2lnIyI+PFNpZ25lZEluZm8+PENhbm9uaWNhbGl6YXRpb25NZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzEwL3htbC1leGMtYzE0biMiPjwvQ2Fub25pY2FsaXphdGlvbk1ldGhvZD48U2lnbmF0dXJlTWV0aG9kIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMC8wOS94bWxkc2lnI3JzYS1zaGExIj48L1NpZ25hdHVyZU1ldGhvZD48UmVmZXJlbmNlIFVSST0iI181OGUzOTBhYy0xNDliLTQzYmEtYjhlNC00N2ZhMzllMmVjNzAiPjxUcmFuc2Zvcm1zPjxUcmFuc2Zvcm0gQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwLzA5L3htbGRzaWcjZW52ZWxvcGVkLXNpZ25hdHVyZSI+PC9UcmFuc2Zvcm0+PFRyYW5zZm9ybSBBbGdvcml0aG09Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvMTAveG1sLWV4Yy1jMTRuIyI+PC9UcmFuc2Zvcm0+PC9UcmFuc2Zvcm1zPjxEaWdlc3RNZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwLzA5L3htbGRzaWcjc2hhMSI+PC9EaWdlc3RNZXRob2Q+PERpZ2VzdFZhbHVlPnRhbllYWlVzQmNibjhrYWhmREdwYTFkelZGUT08L0RpZ2VzdFZhbHVlPjwvUmVmZXJlbmNlPjwvU2lnbmVkSW5mbz48U2lnbmF0dXJlVmFsdWU+SGdsMktLR0VPcVBQUDAydnVTRnRHTTEwMkxoZEI2OXp2ZXh3ejZzN2VqQTBBNUl4NGk0U2FsUE1ZYW85MDJkUjZUSzY0eWxCb3oxRnJ2OXpFZUpHQW9hWjZWbktZSVNENTlmSU03dDJYME5SVk9YM1daRTFaRXhwaG1zL3JJL0EzcW41SDlBT1l5OUxhMXBlVHA2TytBenVWYzFONjdBM214WmpERlIyNGhlallwVHJ2TU9PNHQ3M1c2Tm9nMDlSaTRjclRLNERTN21Mekp2VkRzRlB1bkF1dVN4eVBDK0l4VDVNTXBaSDcxNkdzdXhmU0R1anVXaEk1YkV1bFVrNEQyL2s3MEE3RDdkRjJIV3VEMEtuUXhrV2dKM2pVNldyUHp2UC9Mc3djaFZSOWdqNHd6WnpqTXRpY3N3VXJMYnorVXpJY0h1S0IySkw2djdUZTNYb3JnPT08L1NpZ25hdHVyZVZhbHVlPjxLZXlJbmZvPjxvOlNlY3VyaXR5VG9rZW5SZWZlcmVuY2UgeG1sbnM6bz0iaHR0cDovL2RvY3Mub2FzaXMtb3Blbi5vcmcvd3NzLzIwMDQvMDEvb2FzaXMtMjAwNDAxLXdzcy13c3NlY3VyaXR5LXNlY2V4dC0xLjAueHNkIj48bzpLZXlJZGVudGlmaWVyIFZhbHVlVHlwZT0iaHR0cDovL2RvY3Mub2FzaXMtb3Blbi5vcmcvd3NzL29hc2lzLXdzcy1zb2FwLW1lc3NhZ2Utc2VjdXJpdHktMS4xI1RodW1icHJpbnRTSEExIj5ZREJlRFNGM0Z4R2dmd3pSLzBwck11OTZoQ2M9PC9vOktleUlkZW50aWZpZXI+PC9vOlNlY3VyaXR5VG9rZW5SZWZlcmVuY2U+PC9LZXlJbmZvPjwvU2lnbmF0dXJlPjwvc2FtbDpBc3NlcnRpb24+";
        
        private static string REST_TXN_ENDPOINT = "https://api.cert.nabcommerce.com/REST/2.0.18/Txn";
        
        public static void ConfigurePaymentProfile(string workflowId, string serviceInfo, string endPoint, string identityToken, string appProfileId, string merchProfileId)
        {
            WORKFLOW_ID = workflowId;
            SERVICE_INFO = serviceInfo;
            REST_TXN_ENDPOINT = endPoint;
            IDENTITY_TOKEN = identityToken;

            APP_PROFILE_ID = appProfileId;
            MERCH_PROFILE_ID = merchProfileId;
        }

        public string SetDeviceInfo(string terminalIP, string terminalPort="6200")
        {
            terminalUri = string.Format("{0}:{1}", terminalIP, terminalPort);
            return terminalUri;
        }
        public TransactionResponse ProcessTransaction(double amount, CardTransactionType transactionType = CardTransactionType.Sale, string prevTransactionID = "")
        {
            TransactionResponse response = null;
            TransactionProcessor txnProcessor = new TransactionProcessor();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            if (transactionType == CardTransactionType.Sale || (transactionType == CardTransactionType.Refund && string.IsNullOrEmpty(prevTransactionID)))
            {
                response = txnProcessor.RunTerminalTransaction(terminalUri, amount, transactionType);
            }
            else
            {
                string sessionToken = GetSessionToken();
                string txnUrl = BuildRestUri(prevTransactionID);
                
                txnProcessor.SetTxnApiData(txnUrl, sessionToken, APP_PROFILE_ID, MERCH_PROFILE_ID);
                response = txnProcessor.RunApiTransaction(amount, transactionType, prevTransactionID);
            }

            return response;
        }

        private string BuildRestUri(string txnId = "")
        {
            string uri = REST_TXN_ENDPOINT;

            uri += @"/" + WORKFLOW_ID;
            if (!string.IsNullOrEmpty(txnId))
            {
                uri += @"/" + txnId.Trim();
            }
            return uri;
        }

        public string GetSessionToken(string userName = "", string password = "123456")
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            string uri = SERVICE_INFO;
            string idToken = IDENTITY_TOKEN;
                // "PHNhbWw6QXNzZXJ0aW9uIE1ham9yVmVyc2lvbj0iMSIgTWlub3JWZXJzaW9uPSIxIiBBc3NlcnRpb25JRD0iXzU4ZTM5MGFjLTE0OWItNDNiYS1iOGU0LTQ3ZmEzOWUyZWM3MCIgSXNzdWVyPSJJcGNBdXRoZW50aWNhdGlvbiIgSXNzdWVJbnN0YW50PSIyMDE2LTA5LTIwVDIxOjQ5OjE1LjI0MloiIHhtbG5zOnNhbWw9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjEuMDphc3NlcnRpb24iPjxzYW1sOkNvbmRpdGlvbnMgTm90QmVmb3JlPSIyMDE2LTA5LTIwVDIxOjQ5OjE1LjI0MloiIE5vdE9uT3JBZnRlcj0iMjA0Ni0wOS0yMFQyMTo0OToxNS4yNDJaIj48L3NhbWw6Q29uZGl0aW9ucz48c2FtbDpBZHZpY2U+PC9zYW1sOkFkdmljZT48c2FtbDpBdHRyaWJ1dGVTdGF0ZW1lbnQ+PHNhbWw6U3ViamVjdD48c2FtbDpOYW1lSWRlbnRpZmllcj4xMzEzODY4NTQxMzAwMDAxPC9zYW1sOk5hbWVJZGVudGlmaWVyPjwvc2FtbDpTdWJqZWN0PjxzYW1sOkF0dHJpYnV0ZSBBdHRyaWJ1dGVOYW1lPSJTQUsiIEF0dHJpYnV0ZU5hbWVzcGFjZT0iaHR0cDovL3NjaGVtYXMuaXBjb21tZXJjZS5jb20vSWRlbnRpdHkiPjxzYW1sOkF0dHJpYnV0ZVZhbHVlPjEzMTM4Njg1NDEzMDAwMDE8L3NhbWw6QXR0cmlidXRlVmFsdWU+PC9zYW1sOkF0dHJpYnV0ZT48c2FtbDpBdHRyaWJ1dGUgQXR0cmlidXRlTmFtZT0iU2VyaWFsIiBBdHRyaWJ1dGVOYW1lc3BhY2U9Imh0dHA6Ly9zY2hlbWFzLmlwY29tbWVyY2UuY29tL0lkZW50aXR5Ij48c2FtbDpBdHRyaWJ1dGVWYWx1ZT5hNWMyYmQ3ZS1iMTA3LTQ0YjYtODhhZS01YjM2ZTYxYjEyY2U8L3NhbWw6QXR0cmlidXRlVmFsdWU+PC9zYW1sOkF0dHJpYnV0ZT48c2FtbDpBdHRyaWJ1dGUgQXR0cmlidXRlTmFtZT0ibmFtZSIgQXR0cmlidXRlTmFtZXNwYWNlPSJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcyI+PHNhbWw6QXR0cmlidXRlVmFsdWU+MTMxMzg2ODU0MTMwMDAwMTwvc2FtbDpBdHRyaWJ1dGVWYWx1ZT48L3NhbWw6QXR0cmlidXRlPjwvc2FtbDpBdHRyaWJ1dGVTdGF0ZW1lbnQ+PFNpZ25hdHVyZSB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC8wOS94bWxkc2lnIyI+PFNpZ25lZEluZm8+PENhbm9uaWNhbGl6YXRpb25NZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzEwL3htbC1leGMtYzE0biMiPjwvQ2Fub25pY2FsaXphdGlvbk1ldGhvZD48U2lnbmF0dXJlTWV0aG9kIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMC8wOS94bWxkc2lnI3JzYS1zaGExIj48L1NpZ25hdHVyZU1ldGhvZD48UmVmZXJlbmNlIFVSST0iI181OGUzOTBhYy0xNDliLTQzYmEtYjhlNC00N2ZhMzllMmVjNzAiPjxUcmFuc2Zvcm1zPjxUcmFuc2Zvcm0gQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwLzA5L3htbGRzaWcjZW52ZWxvcGVkLXNpZ25hdHVyZSI+PC9UcmFuc2Zvcm0+PFRyYW5zZm9ybSBBbGdvcml0aG09Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvMTAveG1sLWV4Yy1jMTRuIyI+PC9UcmFuc2Zvcm0+PC9UcmFuc2Zvcm1zPjxEaWdlc3RNZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwLzA5L3htbGRzaWcjc2hhMSI+PC9EaWdlc3RNZXRob2Q+PERpZ2VzdFZhbHVlPnRhbllYWlVzQmNibjhrYWhmREdwYTFkelZGUT08L0RpZ2VzdFZhbHVlPjwvUmVmZXJlbmNlPjwvU2lnbmVkSW5mbz48U2lnbmF0dXJlVmFsdWU+SGdsMktLR0VPcVBQUDAydnVTRnRHTTEwMkxoZEI2OXp2ZXh3ejZzN2VqQTBBNUl4NGk0U2FsUE1ZYW85MDJkUjZUSzY0eWxCb3oxRnJ2OXpFZUpHQW9hWjZWbktZSVNENTlmSU03dDJYME5SVk9YM1daRTFaRXhwaG1zL3JJL0EzcW41SDlBT1l5OUxhMXBlVHA2TytBenVWYzFONjdBM214WmpERlIyNGhlallwVHJ2TU9PNHQ3M1c2Tm9nMDlSaTRjclRLNERTN21Mekp2VkRzRlB1bkF1dVN4eVBDK0l4VDVNTXBaSDcxNkdzdXhmU0R1anVXaEk1YkV1bFVrNEQyL2s3MEE3RDdkRjJIV3VEMEtuUXhrV2dKM2pVNldyUHp2UC9Mc3djaFZSOWdqNHd6WnpqTXRpY3N3VXJMYnorVXpJY0h1S0IySkw2djdUZTNYb3JnPT08L1NpZ25hdHVyZVZhbHVlPjxLZXlJbmZvPjxvOlNlY3VyaXR5VG9rZW5SZWZlcmVuY2UgeG1sbnM6bz0iaHR0cDovL2RvY3Mub2FzaXMtb3Blbi5vcmcvd3NzLzIwMDQvMDEvb2FzaXMtMjAwNDAxLXdzcy13c3NlY3VyaXR5LXNlY2V4dC0xLjAueHNkIj48bzpLZXlJZGVudGlmaWVyIFZhbHVlVHlwZT0iaHR0cDovL2RvY3Mub2FzaXMtb3Blbi5vcmcvd3NzL29hc2lzLXdzcy1zb2FwLW1lc3NhZ2Utc2VjdXJpdHktMS4xI1RodW1icHJpbnRTSEExIj5ZREJlRFNGM0Z4R2dmd3pSLzBwck11OTZoQ2M9PC9vOktleUlkZW50aWZpZXI+PC9vOlNlY3VyaXR5VG9rZW5SZWZlcmVuY2U+PC9LZXlJbmZvPjwvU2lnbmF0dXJlPjwvc2FtbDpBc3NlcnRpb24+";
                uri += @"/token";
            HttpClientApi httpClientApi = new HttpClientApi();
            string sessionToken = httpClientApi.GetSessionToken(uri, identityToken: idToken, pwd: password);
            return sessionToken;
        }

    }
}
