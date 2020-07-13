using System;
using System.Collections.Generic;

namespace Microsoft.Marketplace.SaasKit.Client.DataAccess.Entities
{
    public partial class BatchUsageUploadHistory
    {
        public int Id { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string BatchId { get; set; }
        public string Filename { get; set; }
        public int? UploadBy { get; set; }
        public DateTime? UploadDate { get; set; }
    }
}
