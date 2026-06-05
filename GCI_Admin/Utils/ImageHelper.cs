using Utils;

namespace GCI_Admin.Utils
{
    public static class ImageHelper
    {
        /// <summary>
        /// Saves an image to a specified folder and returns the saved file name.
        /// </summary>
        /// <param name="imageBytes">The image file as byte array</param>
        /// <param name="folderPath">Folder path where image will be stored</param>
        /// <param name="originalFileName">Original file name (to preserve extension)</param>
        /// <returns>Saved file name or null if failed</returns>
        public static string? SaveImage(
    byte[]? imageBytes,
    string folderPath,
    string originalFileName,
    string extension)
        {
            try
            {
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    Loggers.DoLogs("ImageHelper->SaveImage->Image bytes are empty.");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(folderPath) ||
                    string.IsNullOrWhiteSpace(originalFileName))
                {
                    Loggers.DoLogs("ImageHelper->SaveImage->Folder path or file name is invalid.");
                    return null;
                }

                // normalize extension
                extension = extension?.Trim().ToLower() ?? ".png";

                if (!extension.StartsWith("."))
                    extension = "." + extension;

                if (extension == ".jpeg")
                    extension = ".jpg";

                // allow only image extensions
                var allowed = new[] { ".png", ".jpg" };

                if (!allowed.Contains(extension))
                {
                    Loggers.DoLogs($"ImageHelper->SaveImage->Unsupported extension {extension}");
                    return null;
                }

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // remove extension if already present
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);

                string finalFileName = $"{fileNameWithoutExt}{extension}";

                string fullPath = Path.Combine(folderPath, finalFileName);

                File.WriteAllBytes(fullPath, imageBytes);

                return fullPath;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("ImageHelper->SaveImage->" + ex.Message);
                return null;
            }
        }


        public static byte[] ReadImage(string folderPath, string fileName)
        {
            try
            {
                string[] extensions = { ".jpg", ".jpeg", ".png", ".gif" };

                foreach (var ext in extensions)
                {
                    string fullPath = Path.Combine(folderPath, fileName + ext);

                    if (File.Exists(fullPath))
                    {
                        return File.ReadAllBytes(fullPath);
                    }
                }

                Loggers.DoLogs($"Image not found: {folderPath}\\{fileName}");
                return null;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("ImageHelper->ReadImage->" + ex.Message);
                return null;
            }
        }

        public static byte[] RemoveBase64Prefix(string base64String)
        {

            if (base64String!=null && base64String.StartsWith("data:image"))
            {
                int commaIndex = base64String.IndexOf(',');
                if (commaIndex >= 0)
                {
                    return Convert.FromBase64String(base64String.Substring(commaIndex + 1));
                }
            }
            return Convert.FromBase64String(base64String);
        }
    }
}
