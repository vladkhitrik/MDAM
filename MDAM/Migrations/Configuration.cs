namespace MDAM.Migrations
{
    using MDAM.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<MDAM.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MDAM.Models.ApplicationDbContext context)
        {
            // ���� � �� ���� ������������� � �����
            if (!context.Users.Any() || !context.Roles.Any())
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                // �������� ���� �����: Admin � User
                var roleAdmin = new IdentityRole { Name = "Admin" };
                var roleUser = new IdentityRole { Name = "User" };

                // ���������� ����� � ��
                roleManager.Create(roleAdmin);
                roleManager.Create(roleUser);

                // �������� �������������
                var admin = new ApplicationUser { UserName = "admin", Email="vlad_khitrik@inbox.ru" };
                string password = "mdamadm";
                var result = userManager.Create(admin, password);

                // ���� �������� ������������ ������ �������
                if (result.Succeeded)
                {
                    // ���������� ��� ������������ �����
                    userManager.AddToRole(admin.Id, roleAdmin.Name);
                    userManager.AddToRole(admin.Id, roleUser.Name);
                }
                else
                {
                    var errMsg = new StringBuilder();
                    result.Errors.ToList().ForEach(delegate(string msg)
                    {
                        errMsg.Append(msg);
                    });
                    var e = new Exception("������ ���������� ��������! " + errMsg);

                    var count = 0;
                    foreach (var error in result.Errors)
                    {
                        e.Data.Add(count++, error);
                    }

                    throw e;
                }

                base.Seed(context);
            }
        }
    }
}
