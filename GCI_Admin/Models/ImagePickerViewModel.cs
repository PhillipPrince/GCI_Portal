namespace GCI_Admin.Models
{
   
        public class ImagePickerViewModel
        {
            public string ControlId { get; set; } = "imagePicker";
            public string LabelText { get; set; } = "Profile Image";
            public string IconClass { get; set; } = "fa fa-image";
            public string HelpText { get; set; } = "Supported formats: JPEG, PNG, GIF, WEBP. Max size: 5MB";
            public string UploadButtonText { get; set; } = "Upload Image";
            public string CurrentImageUrl { get; set; }
            public string FileInputName { get; set; } = "ImageFile";
            public string ImageUrlFieldName { get; set; } = "ImageUrl";
            public string Base64FieldName { get; set; } = "ImageBase64";
            public int PreviewWidth { get; set; } = 200;
            public int PreviewHeight { get; set; } = 200;
            public int MaxFileSizeMB { get; set; } = 5;
            public bool IsRequired { get; set; } = false;
            public bool Rounded { get; set; } = true;
            public bool HidePreview { get; set; } = false;
            public bool HideUploadButton { get; set; } = false;
            public bool HideClearButton { get; set; } = false;
        }
    
}
