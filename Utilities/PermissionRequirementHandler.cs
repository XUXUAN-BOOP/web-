using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace NetFavorite.Utilities
{
    /// <summary>
    /// 2、定义策略处理类
    /// </summary>
    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly NetFavoriteDbContext _db;
        public PermissionRequirementHandler(NetFavoriteDbContext db)
        {
            _db = db;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var role = context.User.FindFirst(c => c.Type == "UserRole");
            //var role = context.User.FindFirst(c => c.Type == ClaimTypes.Role);
            if (role != null)
            {
                if (_db.RolePermission.Any(
                    it => it.RolePermission_Permission == requirement.PermissionName &&
                          it.RolePermission_Role == role.Value
                ))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
