using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Urbiss.Domain.Models;

namespace Urbiss.Repository
{
    public class UrbissDbContext : IdentityDbContext<User, Role, int,
          IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
          IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public UrbissDbContext(DbContextOptions<UrbissDbContext> options, IHttpContextAccessor contextAccessor) : base(options)
        {
            this.ContextAccessor = contextAccessor.HttpContext;
        }

        public readonly HttpContext ContextAccessor;

        public DbSet<City> Cities { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<UserSurvey> UserSurveys { get; set; }
        public DbSet<SpatialReferenceSystem> SpatialReferenceSystems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<AerialPhoto> AerialPhotos { get; set; }

        private static void ChangeIdentityTableNames(ModelBuilder builder)
        {
              
            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "users");
            });

            builder.Entity<Role>(entity =>
            {
                entity.ToTable(name: "roles");
            });
            builder.Entity<UserRole>(entity =>
            {
                entity.ToTable("userroles");
            });

            builder.Entity<IdentityUserClaim<int>>(entity =>
            {
                entity.ToTable("userclaims");
            });

            builder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("userlogins");
            });

            builder.Entity<IdentityRoleClaim<int>>(entity =>
            {
                entity.ToTable("roleclaims");
            });

            builder.Entity<IdentityUserToken<int>>(entity =>
            {
                entity.ToTable("usertokens");
            });             
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresExtension("postgis");

            ChangeIdentityTableNames(builder);

            builder.Entity<User>(user =>
            {
                user.HasMany<UserRole>()
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<SpatialReferenceSystem>()
                .ToTable("spatial_ref_sys", t => t.ExcludeFromMigrations());

            builder.Entity<UserSurvey>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<City>()
                .HasIndex(c => c.IbgeCode)
                .IsUnique();

            builder.Entity<Survey>(survey =>
            {
                survey.HasIndex(s => s.Folder).IsUnique();
                survey.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Order>(order =>
            {
                order.HasIndex(o => o.OrderId).IsUnique();
                order.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                order.HasOne<Survey>()
                .WithMany()
                .HasForeignKey(o => o.SurveyId)
                .OnDelete(DeleteBehavior.Restrict);
                //Só devo configurar o código abaixo caso eu não tenha a propriedade de navegacação UserSurvey no model
                //Se eu fizer os 2, o EF cria a chave duplicada
                /*order.HasOne<UserSurvey>()
                    .WithMany()
                    .HasForeignKey(o => o.UserSurveyId)
                    .OnDelete(DeleteBehavior.Restrict);*/
                order.HasOne<Voucher>()
                .WithMany()
                .HasForeignKey(o => o.VoucherId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Voucher>(voucher =>
            {
                voucher.HasIndex(v => v.Code).IsUnique();
            });

            builder.Entity<AerialPhoto>(ap =>
            {
                ap.HasOne<Survey>()
                .WithMany()
                .HasForeignKey(ap => ap.SurveyId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
