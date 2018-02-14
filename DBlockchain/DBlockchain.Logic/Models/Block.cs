using System;
using System.Collections.Generic;

namespace DBlockchain.Logic.Models
{
    public class Block
    {
        public long Index { get; set; }

        public List<Transaction> Transactions { get; set; }

        public long Difficulty { get; set; }

        public string PreviousBlockHash { get; set; }

        public string MinedBy { get; set; }

        public string BlockHash { get; set; }

        /*-----------------------------------------*/

        public long Nonce { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
