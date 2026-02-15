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
        public static string? SaveImage(byte[]? imageBytes, string folderPath, string originalFileName)
        {
            try
            {
                // Only proceed if imageBytes is not null or empty
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(originalFileName))
                {
                    Loggers.DoLogs("ImageHelper->SaveImage->Folder path or file name is invalid.");
                    return null;
                }

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fullPath = Path.Combine(folderPath, originalFileName);

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
    }
}
