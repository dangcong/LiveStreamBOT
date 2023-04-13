namespace BaseSource.API.Cofigurations
{
    public static class AuthorizationPolyConfigurations
    {
        public static IServiceCollection AuthorizationPolyConfiguration(this IServiceCollection services)
        {
            //AddAuthorization poly
            //services.AddAuthorization(options =>
            //{
            //    var allPermissions = new List<RoleClaimsResponse>();
            //    //get all permission
            //    allPermissions.GetAllPermissions();
            //    foreach (var item in allPermissions)
            //    {
            //        options.AddPolicy(item.Value, policy => policy.RequireClaim(Permissions.PermissionType, item.Value));
            //    }
            //});
            return services;
        }
    }
}
