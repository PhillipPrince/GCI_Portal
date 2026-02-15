using GCI_Admin.DBOperations.Repositories;

namespace GCI_Admin.Utils
{
    public static class SystemConfigHelper
    {
       
            public static async Task<string> GetImageBasePathAsync(SystemConfigRepository systemConfig)
            {
                if (systemConfig == null)
                    return string.Empty;

                var result = await systemConfig.GetConfigByKeyAsync("IMAGES_FOLDER");
                return result?.Data?.ConfigValue ?? string.Empty;
            }
        
    }
}
