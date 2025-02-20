﻿namespace Tzkt.Api
{
    public class TokenBalanceShortFilter
    {
        /// <summary>
        /// Filter by account address.  
        /// Click on the parameter to expand more details.
        /// </summary>
        public AccountParameter account { get; set; }

        /// <summary>
        /// Filter by token.  
        /// Click on the parameter to expand more details.
        /// </summary>
        public TokenInfoFilter token { get; set; }

        /// <summary>
        /// Filter by balance.  
        /// Click on the parameter to expand more details.
        /// </summary>
        public NatParameter balance { get; set; }
    }
}
