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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CWS_EasyPay_Sample;

namespace Test_EasyPay_Sample
{
    [TestClass]
    public class TestEasyPaySample
    {
        [TestMethod]
        public void TestSale()
        {
            // Sale or Auth and Capture
            PaymentProcessor pmtProcessor = new PaymentProcessor();
            pmtProcessor.SetDeviceInfo("192.168.1.XXX"); // Provide target terminal's IP
            TransactionResponse txnResponse = pmtProcessor.ProcessTransaction(0.01, CardTransactionType.Sale);
            Console.WriteLine("AuthCode is %s, and ErrorMessage is %s", txnResponse.AuthCode, txnResponse.ErrorMessage);
        }

        [TestMethod]
        public void TestReturnUnlinked()
        {
            // Refund without a prior sale transaction id.
            PaymentProcessor pmtProcessor = new PaymentProcessor();
            pmtProcessor.SetDeviceInfo("192.168.1.XXX"); // Provide target terminal's IP
            // This is a "Return Unlinked" transaction - a refund, for e.g. a refund transaction without a prior sale transaction reference
            TransactionResponse txnResponse = pmtProcessor.ProcessTransaction(0.01, CardTransactionType.Refund);
            Console.WriteLine("AuthCode is %s, and ErrorMessage is %s", txnResponse.AuthCode, txnResponse.ErrorMessage);
        }

        [TestMethod]
        public void TestUndo()
        {
            // Void a prior transaction
            PaymentProcessor pmtProcessor = new PaymentProcessor();            
            string prevTransactionId = "65A4DC9E3B3A4087A9C5068F060E4C36"; // Supply prior transaction id that needs to be voided
            TransactionResponse txnResponse = pmtProcessor.ProcessTransaction(0.0, CardTransactionType.Void, prevTransactionId);
            Console.WriteLine(txnResponse.ErrorMessage);
        }

        [TestMethod]
        public void TestRefund()
        {
            // Refund a prior sale - with a prior sale reference number;
            PaymentProcessor pmtProcessor = new PaymentProcessor();
            string prevTransactionId = "B6EFEE1A10F64B49B918E9BE7017A6BA"; // Supply prior transaction id to refund against
            TransactionResponse txnResponse = pmtProcessor.ProcessTransaction(0.0, CardTransactionType.Void, prevTransactionId);
            Console.WriteLine(txnResponse.ErrorMessage);
        }

        [TestMethod]
        public void TestGetSessionToken()
        {
            PaymentProcessor pmtProcessor = new PaymentProcessor();
            string sessionToken = pmtProcessor.GetSessionToken();
            Console.WriteLine(sessionToken);
        }
    }
}
