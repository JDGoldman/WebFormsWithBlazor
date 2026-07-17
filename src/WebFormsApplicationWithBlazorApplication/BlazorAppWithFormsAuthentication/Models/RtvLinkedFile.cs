using System;

namespace BlazorAppWithFormsAuthentication.Models
{
    public class RtvLinkedFile
    {
        public string Id { get; set; }
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; }
        public long SizeBytes { get; set; }
        public DateTime UploadedAtUtc { get; set; }
    }
}
