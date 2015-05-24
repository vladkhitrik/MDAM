using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MDAM.Models.Messages;
using MDAM.Models.Dialogs;
using MDAM.Models.DialogTypes;
using MDAM.Models.Attachments;
using MDAM.Models.Boards;
using MDAM.Models.Offices;
using MDAM.Models.News;

namespace MDAM.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Фотография")]
        public string Image { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<Dialog> Dialogs { get; set; }

        //public string IsApproved { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Dialog> Dialogs { get; set; }
        public DbSet<DialogType> DialogTypes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<New> News { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public ApplicationDbContext(string connectionString)
            : base(connectionString)
        {
            Configuration.AutoDetectChangesEnabled = false;
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Vlad Khitrik
            modelBuilder.Entity<Attachment>().HasKey(t => t.AttachmentId);
            modelBuilder.Entity<Message>().HasRequired(t => t.Attachment).WithRequiredPrincipal(t => t.Message);
            modelBuilder.Entity<Dialog>()
                .HasMany(t => t.ApplicationUsers)
                .WithMany(t => t.Dialogs);
        }
    }
}