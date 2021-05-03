using JobFinder.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobFinder.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Job> Jobs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Job>()
                .HasMany(job => job.UsersSavedBy)
                .WithMany(user => user.SavedJobs)
                .UsingEntity(etb => etb.ToTable("UserJobsSaved"));

            builder.Entity<Job>()
                .HasMany(job => job.UsersAppliedBy)
                .WithMany(user => user.JobsAppliedTo)
                .UsingEntity(etb => etb.ToTable("UserJobsApplied"));

            builder.Entity<Job>()
                .HasMany(job => job.UsersInterviewing)
                .WithMany(user => user.JobsInterviewingWith)
                .UsingEntity(etb => etb.ToTable("UserJobsInterviewing"));

            builder.Entity<Job>()
                .HasMany(job => job.UsersOfferedTo)
                .WithMany(user => user.JobsOffered)
                .UsingEntity(etb => etb.ToTable("UserJobsOffered"));
        }


        public async Task<ApplicationUser> GetUserWithJobsAsync(ApplicationUser currentUser, bool tracking)
        {
            IQueryable<ApplicationUser> query = Users
                .Include(u => u.SavedJobs)
                .Include(u => u.JobsAppliedTo)
                .Include(u => u.JobsInterviewingWith)
                .Include(u => u.JobsOffered);

            if (!tracking)
                query = query.AsNoTracking();

            return await query.SingleAsync(u => u == currentUser);
        }
    }
}
