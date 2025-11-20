using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using src.Model;

namespace src.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet cho tất cả entity
        public DbSet<Department> Departments { get; set; }
        public DbSet<ResourceCategory> ResourceCategories { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingComment> BookingComments { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            
            base.OnModelCreating(builder);
            

            // ---------------------------
            // Department - ApplicationUser
            // ---------------------------
            builder.Entity<Department>()
                .HasMany(d => d.Users)
                .WithOne(u => u.Department)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------------------
            // Department - Resource
            // ---------------------------
            builder.Entity<Department>()
                .HasMany(d => d.Resources)
                .WithOne(r => r.Department)
                .HasForeignKey(r => r.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------------------
            // ResourceCategory - Resource
            // ---------------------------
            builder.Entity<ResourceCategory>()
                .HasMany(c => c.Resources)
                .WithOne(r => r.Category)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------------------
            // Resource - Booking
            // ---------------------------
            builder.Entity<Resource>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Resource)
                .HasForeignKey(b => b.ResourceId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------------------
            // ApplicationUser - Booking
            // ---------------------------
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // ---------------------------
            // Booking - BookingComment
            // ---------------------------
            builder.Entity<Booking>()
                .HasMany(b => b.Comments)
                .WithOne(c => c.Booking)
                .HasForeignKey(c => c.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------------------
            // ApplicationUser - BookingComment
            // ---------------------------
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------------------
            // Soft Delete Filter (tuỳ chọn)
            // ---------------------------
            builder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
            builder.Entity<ResourceCategory>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<Resource>().HasQueryFilter(r => !r.IsDeleted);
            builder.Entity<Booking>().HasQueryFilter(b => !b.IsDeleted);
            builder.Entity<BookingComment>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<AuditTrail>().HasQueryFilter(a => !a.IsDeleted);
        }
    }