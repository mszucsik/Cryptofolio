/*
 *  Cryptofolio
 *  Version 1.0 (November 30, 2018)
 *  by Michael Szucsik
 *  
 *  I, Michael Szucsik, 000286230, certify that this is my original work.
 *  No other persons work was used without due acknowledgement.
 *  
 */
using System;

namespace Cryptofolio.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}