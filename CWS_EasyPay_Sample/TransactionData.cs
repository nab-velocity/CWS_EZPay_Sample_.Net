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
    public enum CardTransactionType
    {
        Sale = 0,
        Void = 1,
        Refund = 2
        // For "ReturnUnlinked" transaction (where amount is to be refunded without a prior sale),
        // use "Refund" as CardTransactionType, but supply empty string "" for prior txn reference (prevTransactionId)

        /////////////////////////////////////////////////////////////////////
        /************* Transaction types below are NOT IMPLEMENTED ************
         * ******************************************************************
        PreAuth = 3,
        PostAuth = 4,
        ForceAuth = 5,
        CardBalanceInquiry = 6,
        AuthOnly = 7,
        None = 8,
        Settlement = 9,
        AutoSettelement = 10,
        ReprintReceipt = 11,
        DetailReport = 12,
        SummaryReport = 13,
        EmvLastTransactionReport = 14,
        ClerkSummaryReport = 15,
        ParametersReport = 16,
        OpenAuth = 17,
        RecentError = 18,
        ActivityReport = 19,
        ClerkIdList = 20,
        EmvParameters = 21,
        EmvStatistic = 22,
        EmvPublicKey = 23,
        TerminalInfo = 24,
        TerminaleExtInfo = 25,
        IssuanceReload = 26,
        Activation = 27,
        BlockActivation = 28,
        Redemption = 29,
        AddTip = 30,
        ForceIssuance = 31,
        ForceActivation = 32,
        ForceRedemtion = 33,
        Deactivation = 34,
        BlockDeactivation = 35,
        Reactivation = 36,
        ZeroGiftCardBalance = 37,
        BlockReactivation = 38,
        CashdrawerStatus = 39,
        CashdrawerOpen = 40,
        CashdrawerCapability = 41,
        RecallLastTx = 42,
        PrintingStatus = 43
         * *********************************************************
        ************************************************************/
        ////////////////////////////////////////////////////////////
    }

    public class TransactionResponse
    {
        public String TransactionID { get; set; }
        public Double ResponseAmount { get; set; }
        public Double RequestAmount { get; set; }
        public String OrderNumber { get; set; }
        public String AuthCode { get; set; }
        public Boolean IsApproved { get; set; }
        public Boolean IsError { get; set; }
        public string CardType { get; set; }
        public String ErrorMessage { get; set; }
        public String VerboseMessage { get; set; }
        public Boolean IsCVVMatched { get; internal set; }
        public Boolean IsZipMatched { get; internal set; }
        public String ResponseCode { get; set; }
        public String ResponseMessage { get; set; }
        public string Jxon { get; set; }
        public string Json { get; set; }
        public string SignatureRequired { get; set; }
        public double CardBalance { get; set; }
        public String CardPresenceType { get; set; }
        public double CashBackAmount { get; set; }
        public String ClerkID { get; set; }
        public String EntryMode { get; set; }
        public String CustomerAccountNO { get; set; }
        public String GiftCardReferenceNumber { get; set; }
        public int HostResponseISOCode { get; set; }
        public String HostValidationCode { get; set; }
        public String InvoiceNumber { get; set; }
        public String TransactionType { get; set; }
        public String OriginalTransactionType { get; set; }
        public String ReferenceNumber { get; set; }
        public double SurchargeAmount { get; set; }
        public double TaxAmount { get; set; }
        public String TerminalID { get; set; }
        public int TicketNumber { get; set; }
        public double TotalAmount { get; set; }
        public double TipAmount { get; set; }
        public String TransactionSequenceNO { get; set; }
        public String VoucherNumber { get; set; }
        public String TransactionStatus { get; set; }
        public String ReceiptEncoded { get; set; }
        public String ReceiptText { get; set; }
        public String TerminalName { get; set; }
        public String TerminalType { get; set; }
        public String AvailableTerminals { get; set; }
        public Boolean IsConnectionError { get; set; }
    }

    public static class TerminalTransactionType
    {
        public static string CCR0 = "CCR0"; //—Retail Account Verification transaction
        public static string CCR1 = "CCR1"; //—Retail Purchase Authorization & Capture transaction
        public static string CCR2 = "CCR2"; //—Retail Purchase Authorization Only transaction
        public static string CCR4 = "CCR4"; //—Retail Purchase Capture Only transaction
        public static string CCR7 = "CCR7"; //—Retail Purchase Authorization Reversal
        public static string CCR8 = "CCR8"; //—Retail BRIC Storage or Updates transaction
        public static string CCR9 = "CCR9"; //—Retail Return Capture transaction
        public static string CCRH = "CCRH"; //—Retail Payment Adjustment Capture and Hold transaction
        public static string CCRI = "CCRI"; //—Retail Payment Adjustment Capture and Hold Release transaction
        public static string CCRN = "CCRN"; //—Retail Edit Authorization and Capture transaction
        public static string CCRQ = "CCRQ"; //—Retail Quasi Cash Authorization & Capture transaction
        public static string CCRR = "CCRR"; //—Retail Quasi Cash Authorization Only transaction
        public static string CCRS = "CCRS"; //—Retail Quasi Cash Capture Only transaction
        public static string CCRT = "CCRT"; //—Capture Payment and Hold transaction. Citi Private Label only
        public static string CCRU = "CCRU"; //—Capture Payment and Hold Release transaction. Citi Private Label only
        public static string CCRX = "CCRX"; //—Retail Void transaction
        public static string CCRY = "CCRY"; //—Open to Buy/Balance Inquiry transaction. Citi Private Label only
        public static string CCRZ = "CCRZ"; //—Retail Close Batch transaction
    }
    
    public static class VelocityTransactionType
    {
        public static readonly string Sale = "Sale";
        public static readonly string Void = "Undo";
        public static readonly string Refund = "ReturnById"; // Return/Refund based ona  prior sale (CNP)
        public static readonly string ReturnUnlinked = "ReturnUnlinked"; // Return/Refund without a prior sale (CP)

        public static string GetTranType(CardTransactionType txnType, string prevTxnReference = "")
        {
            string velocityTxnType = "";

            switch (txnType)
            {
                case CardTransactionType.Sale:
                    velocityTxnType = TerminalTransactionType.CCR1;
                    break;
                case CardTransactionType.Refund:
                    velocityTxnType = string.IsNullOrEmpty(prevTxnReference) ? TerminalTransactionType.CCR9 : Refund;
                    break;
                case CardTransactionType.Void:
                    velocityTxnType = Void;
                    break;
                default:
                    velocityTxnType = "";
                    break;
            }

            return velocityTxnType;
        }
    }
}
