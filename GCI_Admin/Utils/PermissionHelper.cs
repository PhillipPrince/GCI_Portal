using System.Collections.Generic;

namespace Utils
{
    public static class PermissionHelper
    {
        public static List<string> GetPermissions(int roleId)
        {
            // 🔥 Map roles to permissions
            switch (roleId)
            {
                case 1: // Admin
                    return new List<string>
                    {
                        "VIEW_MEMBERS",
                        "CREATE_MEMBERS",
                        "EDIT_MEMBERS",
                        "DELETE_MEMBERS",
                        "VIEW_EVENTS",
                        "CREATE_EVENTS"
                    };

                case 2: // Pastor
                    return new List<string>
                    {
                        "VIEW_MEMBERS",
                        "VIEW_EVENTS",
                        "CREATE_EVENTS"
                    };

                case 3: // Member
                    return new List<string>
                    {
                        "VIEW_EVENTS"
                    };

                default:
                    return new List<string>();
            }
        }
    }
}