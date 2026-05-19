using Microsoft.AspNetCore.Authorization;

namespace NetFavorite.Utilities
{
    /// <summary>
    /// 4、定义自定义特性
    /// </summary>
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "Permission";
        public PermissionAuthorizeAttribute(string permissionName) => PermissionName = permissionName;
        public string PermissionName
        {
            get
            {
                return Policy?.Substring(POLICY_PREFIX.Length) ?? "";
            }
            set
            {
                Policy = $"{POLICY_PREFIX}{value}";
            }
        }
    }
}
