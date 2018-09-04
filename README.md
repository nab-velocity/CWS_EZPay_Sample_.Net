# CWS_EZPay_Sample_.Net
Example of calling to the commerce web-services of the NABANCARD Velocity Gateway.  Includes samples of simple transaction requests needed in most point-of-sale implementation.

The code in the project (including unit test project) is provided only for reference purposes and is only a sample implementation
and may not necessarily be suitable for production usage. All responsibility lies with the user of the sample code.

//** DISCLAIMER
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
//** DISCLAIMER

Summary:
1. The project is a demonstration of how to make calls for processing simple payment transactions,
   using an Ingenico terminal (setup for EPX processor) and Velocity gateway.
2. PaymentProcessor is the class that can be used to run payment transactions.
3. Refer to unit test project - Test_EasyPay_Sample for example calls to make.

Workflow:
1. Contact NAB Integration support (by emailing to certifications@nabcommerce.com) for a CERT environment account.
2. Gather these details:
   a. CERT account keys  i) ServiceID ii) IdentityToken iii) Merchant Profie ID iv) Application Profile Id 
   b. IP and Communication Port on terminal (can be retrieved from the terminal itself).
3. Use those details to setup PaymentProcessor object
4. Create PaymentProcessor object
5. Call SetDeviceInfo with the IP of the terminal (this is required only for transactions that need a terminal, e.g. Sale or pre-Auth)
6. Call ProcessTransaction on the PaymentProcessor object with required parameters.

Technical:
0. This project requires NewtonSoft.Json.dll for handling JSON. 
   This can be downloaded from https://www.newtonsoft.com/json - Download and add to project references.
1. For API calls, a session token is obtained using Identity Token.
   a. This session token is valid only for a limited amount of time.
   b. The session token is supplied to the api call, along with payload data.
2. For non-API calls (i.e. transactions routed through terminal), no session token is required, 
   but IP (and port) of the target terminal is required.
3. ProcessTransaction returns an object TransactionResponse. This object has relevant data regarding the state of the transaction.
   Use the TransactionResponse data in your application as appropriate.

Notes:
1. This documentation, as well as code is subject to change and is provided with no warranties. Use at your own risk.
