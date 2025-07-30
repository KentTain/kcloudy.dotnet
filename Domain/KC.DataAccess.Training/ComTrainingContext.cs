using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using KC.Model.Training;
using KC.Model.Training.Constants;
using KC.Framework.Base;
using KC.Database.EFRepository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace KC.DataAccess.Training
{
    public class ComTrainingContext : MultiTenantDataContext
    {
        /// <summary>
        /// Only used by migrations With default tenantId "Dba"
        /// </summary>
        //public ComConfigContext(DbContextOptions<ComConfigContext> options)
        //    : base(options, TenantConstant.DbaTenantApiAccessInfo)
        //{
        //    ////For Debug
        //    //if (System.Diagnostics.Debugger.IsAttached == false)
        //    //    System.Diagnostics.Debugger.Launch();
        //}
        public ComTrainingContext(
            DbContextOptions<ComTrainingContext> options,
            Tenant tenant,
            IMemoryCache cache = null,
            //ICoreConventionSetBuilder builder = null,
            ILogger<ComTrainingContext> logger = null)
            : base(options, tenant, cache, logger)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Curriculum> Curriculums { get; set; }
        public DbSet<CourseRecord> CourseRecords { get; set; }
        public DbSet<BooksInCourses> BooksInCourses { get; set; }
        public DbSet<StudentsInCourses> StudentsInCourses { get; set; }
        public DbSet<TeachersInCourses> TeachersInCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }
            
            base.OnModelCreating(modelBuilder);

            //移除一对多的级联删除约定，想要级联删除可以在 EntityTypeConfiguration<TEntity>的实现类中进行控制
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //多对多启用级联删除约定，不想级联删除可以在删除前判断关联的数据进行拦截
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(18, 4));

            modelBuilder.Entity<Book>()
                .ToTable(Tables.Book, TenantName)
                .HasMany(m => m.BookSelects)
                .WithOne()
                .HasForeignKey(m => m.BookId);
            modelBuilder.Entity<Course>()
                .ToTable(Tables.Course, TenantName)
                .HasMany(m => m.BookSelects)
                .WithOne()
                .HasForeignKey(m => m.CourseId);

            modelBuilder.Entity<BooksInCourses>()
                .ToTable(Tables.BooksInCourses, TenantName)
                .HasKey(t => new { t.BookId, t.CourseId });
            modelBuilder.Entity<BooksInCourses>()
                .HasOne(m => m.Book)
                .WithMany(m => m.BookSelects)
                .HasForeignKey(m => m.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BooksInCourses>()
                .HasOne(m => m.Course)
                .WithMany(m => m.BookSelects)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClassRoom>()
                .ToTable(Tables.ClassRoom, TenantName)
                .HasMany(m => m.Curriculums)
                .WithOne()
                .HasForeignKey(m => m.ClassRoomId);

            modelBuilder.Entity<Student>()
                .ToTable(Tables.Student, TenantName)
                .HasMany(m => m.CourseSelects)
                .WithOne()
                .HasForeignKey(m => m.StudentId);
            modelBuilder.Entity<Course>()
                .ToTable(Tables.Course, TenantName)
                .HasMany(m => m.StudentSelects)
                .WithOne()
                .HasForeignKey(m => m.CourseId);

            modelBuilder.Entity<StudentsInCourses>()
                .ToTable(Tables.StudentsInCourses, TenantName)
                .HasKey(t => new { t.StudentId, t.CourseId });
            modelBuilder.Entity<StudentsInCourses>()
                .HasOne(m => m.Student)
                .WithMany(m => m.CourseSelects)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StudentsInCourses>()
                .HasOne(m => m.Course)
                .WithMany(m => m.StudentSelects)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Teacher>()
                .ToTable(Tables.Teacher, TenantName)
                .HasMany(m => m.CourseSelects)
                .WithOne()
                .HasForeignKey(m => m.TeacherId);
            modelBuilder.Entity<Course>()
                .ToTable(Tables.Course, TenantName)
                .HasMany(m => m.TeacherSelects)
                .WithOne()
                .HasForeignKey(m => m.CourseId);

            modelBuilder.Entity<TeachersInCourses>()
                .ToTable(Tables.TeachersInCourses, TenantName)
                .HasKey(t => new { t.TeacherId, t.CourseId });
            modelBuilder.Entity<TeachersInCourses>()
                .HasOne(m => m.Teacher)
                .WithMany(m => m.CourseSelects)
                .HasForeignKey(m => m.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TeachersInCourses>()
                .HasOne(m => m.Course)
                .WithMany(m => m.TeacherSelects)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .ToTable(Tables.Course, TenantName)
                .HasMany(m => m.Curriculums)
                .WithOne()
                .HasForeignKey(m => m.CourseId);

            modelBuilder.Entity<Curriculum>().ToTable(Tables.Curriculum, TenantName);
            modelBuilder.Entity<CourseRecord>().ToTable(Tables.CourseRecord, TenantName);

        }
    }
}
