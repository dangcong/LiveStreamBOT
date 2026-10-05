using BaseSouce.Data.Entities;
using BaseSource.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Identity data
            var roleAdminId = (new Guid("c1105ce5-9dbc-49a9-a7d5-c963b6daa62a")).ToString();
            var roleManagerId = (new Guid("D32B2CC9-AABE-43B2-8BC0-66C8DFD77080")).ToString();

            modelBuilder.Entity<AppRole>().HasData(
                new AppRole
                {
                    Id = roleAdminId,
                    ConcurrencyStamp = "8917027d-3fdd-41b1-abe0-bc55cddfd741",
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Description = "Administrator role"
                },
                new AppRole
                {
                    Id = roleManagerId,
                    ConcurrencyStamp = "fffb431f-85fe-4a2c-94ae-b62d194f76bb",
                    Name = "Manager",
                    NormalizedName = "Manager",
                    Description = "Manager role"
                });

            var userAdminId = (new Guid("ffded6b0-3769-4976-841b-69459049a62d")).ToString();
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = userAdminId,
                UserName = "superadmin",
                NormalizedUserName = "superadmin",
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com",
                EmailConfirmed = true,
                ConcurrencyStamp = "e22cc615-9fb7-46fe-8998-4bc14c62a969",
                PasswordHash = "AQAAAAEAACcQAAAAELRu9QUSGfsCuBqslHOFKPSr4RyNZ7rnEHEyd4MadvHqKoZcjefJ2lvJ7cbi9NY6lw==",
                SecurityStamp = string.Empty
            });

            modelBuilder.Entity<AppUserRole>().HasData(new AppUserRole
            {
                RoleId = roleAdminId,
                UserId = userAdminId
            });


            modelBuilder.Entity<ConfigSystem>().HasData(new ConfigSystem()
            {
                Id = 1,
            });


            ////add permission admin
            //var roleClaims = new List<AppRoleClaim>();
            //var idIndex = 1;
            //foreach (var permissionModule in PermissionModules.GetAllPermissionsModules())
            //{
            //    foreach (var permission in PermissionModules.GeneratePermissionsForModule(permissionModule))
            //    {
            //        roleClaims.Add(new AppRoleClaim
            //        {
            //            Id = idIndex++,
            //            RoleId = roleAdminId,
            //            ClaimType = Permissions.PermissionType,
            //            ClaimValue = permission
            //        });
            //    }
            //}
            //modelBuilder.Entity<AppRoleClaim>().HasData(roleClaims);
        }
    }
}
