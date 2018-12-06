using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Kuswandanu.Learn.EntityFramework
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Com.Kuswandanu.Learn.EntityFramework;Trusted_Connection=True;");
        }
    }

    public class Blog : BasicEntity
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public class Post : BasicEntity
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class BasicEntity : IBasicEntity
    {
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public interface IBasicEntity
    {
        DateTime UpdatedDate { get; set; }
        bool IsDeleted { get; set; }
    }
}
