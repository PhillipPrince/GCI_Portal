using System.Collections.Generic;

namespace Utils
{
    public static class PermissionHelper
    {
        // Role ID Constants for better maintainability
        public static class Roles
        {
            public const int ChurchAdmin = 1;
            public const int BranchAdmin = 2;
            public const int Member = 3;
            public const int GrowthCenterLeader = 4;
            public const int Pastor = 5;
            public const int Finance = 6;
        }

        public static List<string> GetPermissions(int roleId)
        {
            // Map roles to permissions
            switch (roleId)
            {
                case Roles.ChurchAdmin: // Church Admin - Full system access across all branches
                    return new List<string>
                    {
                        "VIEW_MEMBERS",
                        "CREATE_MEMBERS",
                        "EDIT_MEMBERS",
                        "DELETE_MEMBERS",
                        "VIEW_ALL_BRANCHES",
                        "MANAGE_ALL_BRANCHES",
                        "VIEW_EVENTS",
                        "CREATE_EVENTS",
                        "EDIT_EVENTS",
                        "DELETE_EVENTS",
                        "VIEW_REPORTS",
                        "CREATE_REPORTS",
                        "EXPORT_REPORTS",
                        "MANAGE_USERS",
                        "MANAGE_ROLES",
                        "VIEW_SETTINGS",
                        "EDIT_SETTINGS",
                        "MANAGE_MINISTRIES",
                        "VIEW_GROWTH_CENTERS",
                        "MANAGE_GROWTH_CENTERS"
                    };

                case Roles.BranchAdmin: // Branch Admin - Manages a specific church branch
                    return new List<string>
                    {
                        "VIEW_MEMBERS",
                        "CREATE_MEMBERS",
                        "EDIT_MEMBERS",
                        "DELETE_MEMBERS",
                        "VIEW_BRANCH",
                        "MANAGE_BRANCH",
                        "VIEW_EVENTS",
                        "CREATE_EVENTS",
                        "EDIT_EVENTS",
                        "DELETE_EVENTS",
                        "VIEW_REPORTS",
                        "EXPORT_REPORTS",
                        "MANAGE_MINISTRIES",
                        "VIEW_GROWTH_CENTERS"
                    };

                case Roles.GrowthCenterLeader: // Growth Center Leader
                    return new List<string>
                    {
                        "VIEW_MEMBERS",
                        "CREATE_MEMBERS",
                        "EDIT_MEMBERS",
                        "VIEW_GROWTH_CENTER",
                        "MANAGE_GROWTH_CENTER",
                        "VIEW_GROWTH_CENTER_REPORTS",
                        "CREATE_GROWTH_CENTER_REPORTS",
                        "EDIT_GROWTH_CENTER_REPORTS",
                        "VIEW_EVENTS",
                        "VIEW_MINISTRIES"
                    };

                case Roles.Pastor: // Pastor - Responsible for spiritual leadership
                    return new List<string>
                    {
                        "VIEW_MEMBERS",
                        "VIEW_EVENTS",
                        "CREATE_EVENTS",
                        "EDIT_EVENTS",
                        "VIEW_REPORTS",
                        "VIEW_MINISTRIES",
                        "VIEW_GROWTH_CENTERS"
                    };

                case Roles.Finance: // Finance - Handles financial management
                    return new List<string>
                    {
                        "VIEW_FINANCE",
                        "CREATE_FINANCE",
                        "EDIT_FINANCE",
                        "VIEW_REPORTS",
                        "EXPORT_REPORTS",
                        "VIEW_MEMBERS",
                        "VIEW_FINANCIAL_REPORTS"
                    };

                case Roles.Member: // Regular church member
                    return new List<string>
                    {
                        "VIEW_PUBLIC_CONTENT",
                        "VIEW_EVENTS",
                        "REGISTER_EVENTS",
                        "VIEW_PROFILE",
                        "EDIT_PROFILE"
                    };

                default:
                    return new List<string>
                    {
                        "VIEW_PUBLIC"
                    };
            }
        }

        // Helper method to get role name by ID
        

        // Helper method to get redirect URL based on role ID
        public static string GetRedirectUrlByRoleId(int roleId)
        {
            return roleId switch
            {
                Roles.ChurchAdmin => "/Home/",
                Roles.BranchAdmin => "/Home/",
                Roles.Member => "/Member/Dashboard",
                Roles.GrowthCenterLeader => "/GrowthCenter/Dashboard",
                Roles.Pastor => "/Pastoral/Dashboard",
                Roles.Finance => "/Finance",
                _ => "/Home/Index"
            };
        }

        // Helper method to check if a role has a specific permission
        public static bool HasPermission(int roleId, string permission)
        {
            var permissions = GetPermissions(roleId);
            return permissions.Contains(permission);
        }
    }
}