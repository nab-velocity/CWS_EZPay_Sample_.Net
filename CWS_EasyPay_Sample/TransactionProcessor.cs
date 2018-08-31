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
using Newtonsoft.Json;

namespace CWS_EasyPay_Sample
{
    class TransactionProcessor
    {
        private string txnEndPoint;
        private string txnSessionToken;

        private string applnProfileId;
        private string merchantProfileId;

        public void SetTxnApiData(string txnUrl, string sessionToken, string appProfileId, string merchProfileId)
        {
            txnEndPoint = txnUrl;
            txnSessionToken = sessionToken;
            applnProfileId = appProfileId;
            merchantProfileId = merchProfileId;
        }
        public TransactionResponse RunTerminalTransaction(string uri, double amount, CardTransactionType transactionType)
        {
            TransactionResponse response = new TransactionResponse();
            string txnType = VelocityTransactionType.GetTranType(transactionType);

            string postDataXml = string.Format("<DETAIL><TRAN_TYPE>{0}</TRAN_TYPE><AMOUNT>{1}</AMOUNT></DETAIL>", txnType, amount.ToString());
            HttpClientApi httpClientApi = new HttpClientApi();
            string responseMessage = httpClientApi.ProcessTransaction(uri, postDataXml);
            if (httpClientApi.httpStatus == System.Net.HttpStatusCode.OK && httpClientApi.exceptionStatus == "NONE")
            {
                response = TerminalTxnResponse.GetTransactionResponseFromXml(responseMessage);
            }
            else
            {
                response.ErrorMessage = string.Format("Error processing {0} on device {1} : Result: {2} : Detail: {3}", txnType, uri, httpClientApi.statusCode, httpClientApi.exceptionData);
                response.IsError = true;
            }

            return response;
        }

        public TransactionResponse RunApiTransaction(double amount, CardTransactionType transactionType, string prevTxnId)
        {
            TransactionResponse response = new TransactionResponse();

            string txnType = VelocityTransactionType.GetTranType(transactionType, prevTxnId);
            
            if (!string.IsNullOrEmpty(txnSessionToken))
            {
                if (string.Equals(txnType, VelocityTransactionType.Void))
                {
                    UndoTxn undoTran = new UndoTxn(applnProfileId, prevTxnId);
                    string postDataJson = undoTran.toJson();
                    response = ExecRestTransaction(txnEndPoint, postDataJson, txnSessionToken, "PUT", txnType);
                }
                else if (string.Equals(txnType, VelocityTransactionType.Refund))
                {
                    ReturnByIdTxn refundTxn = new ReturnByIdTxn(applnProfileId, merchantProfileId, prevTxnId, amount);
                    string postDataJson = refundTxn.toJson();
                    response = ExecRestTransaction(txnEndPoint, postDataJson, txnSessionToken, "POST", txnType);
                }
            }
            else
            {
                response.ErrorMessage = string.Format("Error processing {0} on endPoint {1} : Result: {2}", txnType, txnEndPoint, "Empty or Invalid session token");
                response.IsError = true;
            }
            return response;
        }

        private TransactionResponse ExecRestTransaction(string uri, string jsonData, string sessionToken, string method, string txnType)
        {
            TransactionResponse response = new TransactionResponse();
            HttpClientApi httpClientApi = new HttpClientApi();

            string restResponse = httpClientApi.ProcessRestTransaction(uri, jsonData, sessionToken, httpMethod: method);
            if (httpClientApi.httpStatus == System.Net.HttpStatusCode.OK && httpClientApi.exceptionStatus == "NONE")
            {
                VelocityTxnResponse apiResp = JsonConvert.DeserializeObject<VelocityTxnResponse>(restResponse);
                response = apiResp.GetTransactionResponse();
            }
            else
            {
                response.ErrorMessage = string.Format("Error processing {0} on device {1} : Result: {2} : Detail: {3}", txnType, uri, httpClientApi.statusCode, httpClientApi.exceptionData);
                response.IsError = true;
            }

            return response;
        }
    }
}
