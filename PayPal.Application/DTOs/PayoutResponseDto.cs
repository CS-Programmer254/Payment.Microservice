using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayPal.Application.DTOs
{
    public class PayoutResponseDto
    {
        [JsonProperty("batch_header")]
        public BatchHeader BatchHeader { get; set; }

        [JsonProperty("links")]
        public List<Link> Links { get; set; }
    }

    public class BatchHeader
    {
        [JsonProperty("payout_batch_id")]
        public string PayoutBatchId { get; set; }

        [JsonProperty("batch_status")]
        public string BatchStatus { get; set; }

        [JsonProperty("sender_batch_header")]
        public ResponseSenderBatchHeader SenderBatchHeader { get; set; }
    }

    public class ResponseSenderBatchHeader
    {
        [JsonProperty("sender_batch_id")]
        public string SenderBatchId { get; set; }

        [JsonProperty("email_subject")]
        public string EmailSubject { get; set; }

        [JsonProperty("email_message")]
        public string EmailMessage { get; set; }
    }

    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("encType")]
        public string EncType { get; set; }
    }
}
