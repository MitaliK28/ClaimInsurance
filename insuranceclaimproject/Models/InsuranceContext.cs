using insuranceclaimproject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace insuranceclaimproject.Models
{
    public class InsuranceContext : IdentityDbContext<AppUser>
    {
        public InsuranceContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Policy> Policies { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<PolicyCategory> PolicyCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedRoles(modelBuilder);

            // Enum conversions to string for database
            modelBuilder.Entity<AppUser>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<SupportTicket>()
                .Property(t => t.TicketStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Claim>()
                .Property(c => c.ClaimStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Document>()
                .Property(d => d.DocumentType)
                .HasConversion<string>();

            modelBuilder.Entity<Policy>()
                .Property(p => p.PolicyStatus)
                .HasConversion<string>();

            // Configure relationships
            modelBuilder.Entity<Policy>()
                .HasOne(p => p.Policyholder)
                .WithMany()
                .HasForeignKey(p => p.PolicyholderId)
                .IsRequired();

            // Corrected Policy to Claim relationship
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Policy)
                .WithMany(p => p.Claims)
                .HasForeignKey(c => c.PolicyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Adjuster)
                .WithMany()
                .HasForeignKey(c => c.AdjusterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Claim)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SupportTicket>()
                .HasOne(st => st.User)
                .WithMany()
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship for the new PolicyCategory model
            modelBuilder.Entity<PolicyCategory>()
                .HasOne(pc => pc.User)
                .WithMany()
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData
            (
                new IdentityRole() { Name = "ADMIN", ConcurrencyStamp = "1", NormalizedName = "ADMIN" },
                new IdentityRole() { Name = "AGENT", ConcurrencyStamp = "2", NormalizedName = "AGENT" },
                new IdentityRole() { Name = "CLAIM_ADJUSTER", ConcurrencyStamp = "3", NormalizedName = "CLAIM_ADJUSTER" },
                new IdentityRole() { Name = "POLICYHOLDER", ConcurrencyStamp = "4", NormalizedName = "POLICYHOLDER" }
            );
        }
    }
}
