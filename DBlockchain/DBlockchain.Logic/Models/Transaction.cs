using System;

namespace DBlockchain.Logic.Models
{
    public class Transaction
    {
        public string From { get; set; }

        public string To { get; set; }

        public decimal Value { get; set; }

        public string SenderPublicKey { get; set; }

        public byte[] SenderSignature { get; set; }

        /*--------------------------------------------------------*/

        public string TransactionHash { get; set; }

        public DateTime DateReceived { get; set; }

        public long MinedInBlockIndex { get; set; }
    }
}
