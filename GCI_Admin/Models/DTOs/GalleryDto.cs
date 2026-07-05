using System;

namespace GCI_Admin.Models.DTOs
{
    public class GalleryImageDto
    {
        public string FileName { get; set; } = "";
        public string ImageBytes { get; set; } = ""; 
        public long SizeBytes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GalleryUploadRequestDto
    {
        public string ImageBase64 { get; set; } = "";
    }
}
