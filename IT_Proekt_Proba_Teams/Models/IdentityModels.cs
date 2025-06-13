using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IT_Proekt_Proba_Teams.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int? TeamId { get; set; }
        public virtual Team Team { get; set; }
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
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // Овде ги креираме DbSet-овите во базата
        public DbSet<TaskModel> TaskModels { get; set; }
        public DbSet<Team> Teams { get; set; }


        // Во OnModelCreating() - се специфицираат релациите меѓу ентитетите (моделите)
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TeamLeader во Team
            modelBuilder.Entity<Team>()
                .HasRequired(t => t.TeamLeader)
                .WithMany() // Може треба да се смени
                .HasForeignKey(t => t.TeamLeaderId)
                .WillCascadeOnDelete(false);

            // Members во Team
            modelBuilder.Entity<ApplicationUser>()
                .HasOptional(u => u.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(u => u.TeamId)
                .WillCascadeOnDelete(false);

            // TaskModel.CreatedByUserId
            modelBuilder.Entity<TaskModel>()
                .HasRequired(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .WillCascadeOnDelete(false);

            // TaskModel.AssignedToUserId
            modelBuilder.Entity<TaskModel>()
                .HasOptional(t => t.AssignedToUser) // Задачата може да биде доделена или не
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .WillCascadeOnDelete(false);

            // TaskModel.TeamId
            modelBuilder.Entity<TaskModel>()
                .HasOptional(t => t.Team) // Задачата може да припаѓа на тим или не
                .WithMany()
                .HasForeignKey(t => t.TeamId)
                .WillCascadeOnDelete(false);
        }
    }
}