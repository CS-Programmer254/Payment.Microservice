using Newtonsoft.Json;
using System.Collections.Generic;

namespace PayPal.Application.DTOs
{
    public class PayoutRequestDto
    {
        [JsonProperty("sender_batch_header")]
        public SenderBatchHeader SenderBatchHeader { get; set; }

        [JsonProperty("items")]
        public List<PayoutItem> Items { get; set; }
    }

    public class SenderBatchHeader
    {
        [JsonProperty("sender_batch_id")]
        public string SenderBatchId { get; set; }

        [JsonProperty("email_subject")]
        public string EmailSubject { get; set; } = "You have a payout!";

        [JsonProperty("email_message")]
        public string EmailMessage { get; set; } = "You have received a payout! Thanks for using our service!";
    }

    public class PayoutItem
    {
        [JsonProperty("recipient_type")]
        public string RecipientType { get; set; } = "EMAIL"; 

        [JsonProperty("amount")]
        public Money Amount { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("sender_item_id")]
        public string SenderItemId { get; set; }

        [JsonProperty("receiver")]
        public string Receiver { get; set; }
    }

    public class Money
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = "USD";
    }
}
