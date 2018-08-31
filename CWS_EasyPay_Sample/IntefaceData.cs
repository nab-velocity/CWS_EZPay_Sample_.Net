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

// Reference: msdn.microsoft.com -- C#, HTTP, REST documentation/examples
// Reference: https://www.newtonsoft.com/json

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace CWS_EasyPay_Sample
{
    public struct Schemas
    {
        // The Urls below will need to be changed for a production deployment. These currently point to "cert" environment.
        public static readonly string restTxnEndPointUrl = "https://api.cert.nabcommerce.com/REST/2.0.18/Txn";
        public static readonly string restTxnSchemaUrl = "http://schemas.ipcommerce.com/CWS/v2.0/Transactions/Rest";
        public static readonly string bankCardTxnSchemaUrl = "http://schemas.ipcommerce.com/CWS/v2.0/Transactions/Bankcard";
    }

    public class DifferenceData
    {
        private string type;
        private string transactionId;
        private string transactionDateTime;
        private double amount;
        private string addendum;

        public DifferenceData(string txnType, string txnId, double txnAmount, string txnDateTime = "", string txnAddendum = "")
        {
            type = txnType;
            transactionId = txnId;
            transactionDateTime = txnDateTime;
            amount = txnAmount;
            addendum = txnAddendum;
        }

        public string toJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                if (!string.IsNullOrEmpty(type))
                {
                    writer.WritePropertyName("$type");
                    writer.WriteValue(string.Format("{0},{1}", type, Schemas.bankCardTxnSchemaUrl));
                }
                writer.WritePropertyName("TransactionId");
                writer.WriteValue(transactionId);
                if (!string.IsNullOrEmpty(transactionDateTime))
                {
                    writer.WritePropertyName("TransactionDateTime");
                    writer.WriteValue(transactionDateTime);
                }
                if (!(Double.IsNaN(amount)))
                {
                    writer.WritePropertyName("Amount");
                    writer.WriteValue(amount);
                }
                writer.WritePropertyName("Addendum");
                writer.WriteValue(addendum);
                writer.WriteEndObject();
            }
            return sb.ToString();
        }
    }

    class VelocityApiTxn
    {
        protected string txnActiontype;
        protected string merchantProfileId;
        protected string applicationProfileId;

        protected DifferenceData differenceData;

        public VelocityApiTxn(string txnType, string appProfileId, string merchProfileId = "")
        {
            txnActiontype = txnType;
            merchantProfileId = merchProfileId;
            applicationProfileId = appProfileId;
        }
        protected string toJsonString()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("$type");
                writer.WriteValue(string.Format("{0},{1}", txnActiontype, Schemas.restTxnSchemaUrl));
                writer.WritePropertyName("ApplicationProfileId");
                writer.WriteValue(applicationProfileId);
                if (!string.IsNullOrEmpty(merchantProfileId))
                {
                    writer.WritePropertyName("MerchantProfileId");
                    writer.WriteValue(merchantProfileId);
                }
                writer.WritePropertyName("DifferenceData");
                writer.WriteRawValue(differenceData.toJson());
                writer.WriteEndObject();
            }
            return sb.ToString();
        }

    }

    class UndoTxn : VelocityApiTxn
    {
        private string transactionId;
        public UndoTxn(string appProfileId, string txnId)
            : base("Undo", appProfileId)
        {
            transactionId = txnId;
        }

        public string toJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            differenceData = new DifferenceData("BankcardUndo", transactionId, Double.NaN);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("$type");
                writer.WriteValue(string.Format("{0},{1}", txnActiontype, Schemas.restTxnSchemaUrl));
                writer.WritePropertyName("ApplicationProfileId");
                writer.WriteValue(applicationProfileId);
                writer.WritePropertyName("DifferenceData");
                writer.WriteRawValue(differenceData.toJson());
                writer.WriteEndObject();
            }
            return sb.ToString();
        }
    }

    class ReturnByIdTxn : VelocityApiTxn
    {
        private readonly string action = "ReturnById";
        private string transactionId;
        private double transactionAmount;
        public ReturnByIdTxn(string appProfileId, string merchProfileId, string txnId, double txnAmount)
            : base("ReturnById", appProfileId, merchProfileId)
        {
            transactionId = txnId;
            transactionAmount = txnAmount;
        }

        public string toJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            differenceData = new DifferenceData("BankcardReturn", transactionId, transactionAmount);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("$type");
                writer.WriteValue(string.Format("{0},{1}", action, Schemas.restTxnSchemaUrl));
                writer.WritePropertyName("ApplicationProfileId");
                writer.WriteValue(applicationProfileId);
                writer.WritePropertyName("MerchantProfileId");
                writer.WriteValue(merchantProfileId);
                writer.WritePropertyName("DifferenceData");
                writer.WriteRawValue(differenceData.toJson());
                writer.WriteEndObject();
            }
            return sb.ToString();
        }
    }

    class AdjustTxn : VelocityApiTxn
    {
        private readonly string action = "Adjust";
        private string transactionId;
        public AdjustTxn(string appProfileId, string txnId)
            : base("Adjust", appProfileId)
        {
            transactionId = txnId;
        }

        public string toJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            DifferenceData diffData = new DifferenceData("", transactionId, Double.NaN);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("$type");
                writer.WriteValue(string.Format("{0},{1}", action, Schemas.restTxnSchemaUrl));
                writer.WritePropertyName("ApplicationProfileId");
                writer.WriteValue(applicationProfileId);
                writer.WritePropertyName("MerchantProfileId");
                writer.WriteValue(merchantProfileId);
                writer.WriteRaw(diffData.toJson());
                writer.WriteEndObject();
            }
            return sb.ToString();
        }
    }

    public struct ServiceTxnDateTime
    {
        public String Date { get; set; }
        public String Time { get; set; }
        public String TimeZone { get; set; }

    }

    public struct AnyStruct
    {
        public String[] ID { get; set; }
    }
    public struct UnmanagedStruct
    {
        public IList<String> Any { get; set; }

    }

    public struct AddendumStruct
    {
        public UnmanagedStruct Unmanaged { get; set; }
    }
    public class VelocityTxnResponse
    {
        public String AdviceResponse { get; set; }
        public Double Amount { get; set; }
        public String Status { get; set; }

        public String CommercialCardResponse { get; set; }

        public String CardType { get; set; }

        public String StatusCode { get; set; }

        public String ReturnedACI { get; set; }
        public String FeeAmount { get; set; }
        public String StatusMessage { get; set; }
        public String ApprovalCode { get; set; }
        public String TransactionId { get; set; }
        public String AVSResult { get; set; }
        public String OriginatorTransactionId { get; set; }
        public String BatchId { get; set; }
        public String ServiceTransactionId { get; set; }
        public String CVResult { get; set; }
        public ServiceTxnDateTime ServiceTransactionDateTime { get; set; }
        public String CardLevel { get; set; }

        public AddendumStruct Addendum { get; set; }
        public String DowngradeCode { get; set; }
        public String MaskedPAN { get; set; }
        public String TransactionState { get; set; }
        public String PaymentAccountDataToken { get; set; }
        public String IsAcknowledged { get; set; }
        public String RetrievalReferenceNumber { get; set; }
        public String Reference { get; set; }
        public String Resubmit { get; set; }
        public String SettlementDate { get; set; }
        public Double FinalBalance { get; set; }
        public String OrderId { get; set; }
        public Double CashBackAmount { get; set; }
        public String PrepaidCard { get; set; }

        string toJson()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }

        VelocityTxnResponse fromJson(string json)
        {
            VelocityTxnResponse apiTxnResponse = JsonConvert.DeserializeObject<VelocityTxnResponse>(json);

            return apiTxnResponse;
        }

        public TransactionResponse GetTransactionResponse()
        {
            TransactionResponse response = new TransactionResponse();

            response.AuthCode = this.ApprovalCode;
            response.AvailableTerminals = "";
            response.CardBalance = this.FinalBalance;

            //response.CardPresenceType = this.somethingnothere;

            response.CardType = this.CardType;
            response.CashBackAmount = this.CashBackAmount;

            //response.ClerkID = this.somethingnothere;
            //response.CustomerAccountNO = this.notsurewhichoneifavailable;
            //response.EntryMode = this.somethingnothere;

            //response.GiftCardReferenceNumber = this.somethingnothere;
            //response.HostResponseISOCode = this.somethingnothere;
            //response.HostValidationCode = this.somethingnothere;

            response.InvoiceNumber = "";
            response.IsApproved = this.StatusCode == "00" ? true : false;
            response.IsConnectionError = false;
            //response.IsCVVMatched = string.Equals(this.CVResult, "Match", StringComparison.OrdinalIgnoreCase) ? true : false;
            response.IsError = !response.IsApproved; // this.StatusCode == "Successful" ? false : true;
            response.ErrorMessage = response.IsError ? this.StatusMessage : "";
            //response.IsZipMatched = this.AVSResult;
            response.Json = this.toJson();

            response.Jxon = "";
            
            response.OrderNumber = this.OrderId;
            //response.OriginalTransactionType = this.somethingnothere;
            //response.ReceiptEncoded = "";
            //response.ReceiptText = this.somethingnothereyet;
            response.ReferenceNumber = this.ServiceTransactionId;
            response.RequestAmount = this.Amount;
            response.ResponseAmount = this.Amount;
            response.ResponseCode = this.StatusCode;
            response.ResponseMessage = this.StatusMessage;

            //response.SignatureRequired = this.somethingnothere;
            
            //response.SurchargeAmount = this.somethingnothere;
            //response.TaxAmount = this.somethingnothere;
            //response.TerminalID = this.somethingnothere;
            //response.TerminalName = this.somethingnothere;
            //response.TerminalType = this.somethingnothere
            //response.TicketNumber = this.somethingnothere;            
            //response.TipAmount = this.somethingnothere;

            response.TotalAmount = this.Amount;
            response.TransactionID = this.TransactionId;
            response.TransactionSequenceNO = this.OriginatorTransactionId;
            response.TransactionStatus = this.TransactionState;

            //response.TransactionType = this.somethingnothere;

            response.VerboseMessage = this.StatusMessage;

            response.VoucherNumber = this.RetrievalReferenceNumber; //need to see if voucher number means anything specific.


            return response;
        }
    }

    /// <summary>
    /// Transaction response for secure 
    /// payment authorization transaction processing
    /// </summary>
    [Serializable]
    public class TerminalTxnResponse
    {
        [XmlElement(ElementName = "ROUTING")]
        public string Routing { get; set; }

        [XmlElement(ElementName = "AUTH_CARD_TYPE")]
        public string AuthCardType { get; set; }

        [XmlElement(ElementName = "CARD_ENT_METH")]
        public string CardEntMeth { get; set; }

        [XmlElement(ElementName = "SI_SIGNATURE_REQUIRED")]
        public string IsSignatureRequired { get; set; }

        [XmlElement(ElementName = "SI_QUICK_SERVICE")]
        public string IsQuickService { get; set; }

        [XmlElement(ElementName = "CURRENCY_CODE")]
        public string CurrencyCode { get; set; }

        [XmlElement(ElementName = "CUST_NBR")]
        public string CustNbr { get; set; }

        [XmlElement(ElementName = "MERCH_NBR")]
        public string MerchNbr { get; set; }

        [XmlElement(ElementName = "DBA_NBR")]
        public string DbaNbr { get; set; }

        [XmlElement(ElementName = "TERMINAL_NBR")]
        public string TerminalNbr { get; set; }

        [XmlElement(ElementName = "AUTH_AMOUNT")]
        public double AuthAmount { get; set; }

        [XmlElement(ElementName = "AUTH_AMOUNT_REQUESTED")]
        public double AuthAmountRequested { get; set; }

        [XmlElement(ElementName = "AUTH_CODE")]
        public string AuthCode { get; set; }

        [XmlElement(ElementName = "AUTH_CVV2")]
        public string AuthCVV2 { get; set; }

        [XmlElement(ElementName = "AUTH_GUID")]
        public string AuthGUID { get; set; }

        [XmlElement(ElementName = "AUTH_MASKED_ACCOUNT_NBR")]
        public string AuthMaskedAccountNbr { get; set; }

        [XmlElement(ElementName = "AUTH_RESP")]
        public string AuthResp { get; set; }

        [XmlElement(ElementName = "AUTH_RESP_TEXT")]
        public string AuthRespText { get; set; }

        [XmlElement(ElementName = "AUTH_TRAN_DATE_GMT")]
        public string AuthTranDateGMT { get; set; }

        [XmlElement(ElementName = "BATCH_ID")]
        public string BatchId { get; set; }

        [XmlElement(ElementName = "LOCAL_DATE")]
        public string LocalDate { get; set; }

        [XmlElement(ElementName = "LOCAL_TIME")]
        public string LocalTime { get; set; }

        [XmlElement(ElementName = "TRAN_NBR")]
        public string TranNbr { get; set; }

        [XmlElement(ElementName = "TRAN_TYPE")]
        public string TranType { get; set; }

        // Get TransactionResponse object from XMLTransactionResponse
        // Both types do not exactly match and some fields are unused or unassigned
        TransactionResponse GetTransactionResponse()
        {
            TransactionResponse response = new TransactionResponse();

            //unassigned = this.Routing;
            response.CardType = this.AuthCardType;
            response.EntryMode = this.CardEntMeth;
            response.SignatureRequired = this.IsSignatureRequired;
            //unassigned = this.IsQuickService;
            //unassigned = this.CurrencyCode;
            response.CustomerAccountNO = string.Format("{0}-{1}-{2}-{3}", this.CustNbr, this.MerchNbr, this.DbaNbr, this.TerminalNbr);
            //unassigned = this.MerchNbr;
            //unassigned = this.DbaNbr;
            response.TerminalID = this.TerminalNbr;
            response.ResponseAmount = this.AuthAmount;
            response.RequestAmount = this.AuthAmountRequested;
            response.AuthCode = this.AuthCode;
            //unassigned = this.AuthCVV2;
            response.ReferenceNumber = this.AuthGUID;
            //unassigned = this.AuthMaskedAccountNbr;
            response.ResponseCode = this.AuthResp;
            response.ResponseMessage = this.AuthRespText;
            response.IsApproved = this.AuthRespText == "APPROVAL" ? true : false;

            response.IsError = this.AuthRespText.ToUpper().Contains("ERROR") ? true : false;
            response.ErrorMessage = response.IsError ? this.AuthRespText : "";
            //unassigned = this.AuthTranDateGMT;
            //unassigned = this.BatchId;
            //unassigned = this.LocalDate;
            //unassigned = this.LocalTime;
            response.TransactionID = this.TranNbr;
            response.TransactionType = this.TranType;
            return response;
        }

        public static TransactionResponse GetTransactionResponseFromXml(string xml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute();
            xmlRoot.ElementName = "RESPONSE";
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TerminalTxnResponse), xmlRoot);

            TransactionResponse txnResponse = new TransactionResponse();
            TerminalTxnResponse xmlTxnResponse = new TerminalTxnResponse();
            using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(xml ?? "")))
            {
                xmlTxnResponse = (TerminalTxnResponse)xmlSerializer.Deserialize(ms);

                txnResponse = xmlTxnResponse.GetTransactionResponse();
            }
            return txnResponse;
        }
    }

}
