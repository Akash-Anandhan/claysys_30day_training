namespace ShoppingCartAPI.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Admin"))
                roleManager.Create(new IdentityRole("Admin"));
            if (!roleManager.RoleExists("User"))
                roleManager.Create(new IdentityRole("User"));

            if (userManager.FindByEmail("admin@shop.com") == null)
            {
                var admin = new ApplicationUser { UserName = "admin@shop.com", Email = "admin@shop.com", FullName = "Admin" };
                userManager.Create(admin, "Admin@1234");
                userManager.AddToRole(admin.Id, "Admin");
            }
        }
    }
}
