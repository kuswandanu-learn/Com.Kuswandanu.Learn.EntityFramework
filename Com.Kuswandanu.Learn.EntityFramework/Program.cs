using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Kuswandanu.Learn.EntityFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            string option = "-";
            while (option.ToLower() != "x" && option.ToLower() != string.Empty)
            {
                Console.Clear();

                DbContextOptionsBuilder<BloggingContext> optionsBuilder = new DbContextOptionsBuilder<BloggingContext>();

                optionsBuilder
                    .UseInMemoryDatabase("LearnEntityFramework")
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

                //optionsBuilder
                //    .EnableSensitiveDataLogging()
                //    .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Com.Kuswandanu.Learn.EntityFramework;Trusted_Connection=True;");

                Console.WriteLine("Hello World!");
                Console.WriteLine("1.  Create");
                Console.WriteLine("2.  Read");
                Console.WriteLine("2d. Read with Deleted");
                Console.WriteLine("3.  Read One");
                Console.WriteLine("4.  Update");
                Console.WriteLine("5.  Delete");
                Console.WriteLine("x.  Exit");
                Console.WriteLine("---");
                using (var db = new BloggingContext(optionsBuilder.Options))
                {
                    Data data = new Data(db);

                    option = Console.ReadLine();

                    int BlogId;

                    switch (option)
                    {
                        case "1":
                            data.Create();
                            break;
                        case "2":
                            data.GetExist();
                            break;
                        case "2d":
                            data.GetAll();
                            break;
                        case "3":
                            Console.Write("BlogId : ");
                            int.TryParse(Console.ReadLine(), out BlogId);
                            data.Get(BlogId);
                            break;
                        case "4":
                            Console.Write("BlogId : ");
                            int.TryParse(Console.ReadLine(), out BlogId);

                            Blog newBlog = db.Blogs.AsNoTracking().Include(b => b.Posts).SingleOrDefaultAsync(b => b.BlogId == BlogId).Result;
                            newBlog.Posts = new List<Post>();

                            data.Update(BlogId, newBlog);
                            break;
                        case "5":
                            Console.Write("BlogId : ");
                            int.TryParse(Console.ReadLine(), out BlogId);
                            data.Delete(BlogId);
                            break;
                        default:
                            break;
                    }
                }

                Console.WriteLine("End of World!");
                Console.ReadLine();
            }
        }
    }

    class Data
    {
        BloggingContext db;

        public Data(BloggingContext db)
        {
            this.db = db;
        }

        public void Old()
        {
            db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            var count = db.SaveChanges();
            Console.WriteLine("{0} records saved to database", count);

            Console.WriteLine();
            Console.WriteLine("All blogs in database:");
            foreach (var blog in db.Blogs)
            {
                Console.WriteLine(" - {0}", blog.Url);
            }
        }

        internal void Create()
        {
            Blog newBlog = new Blog
            {
                Url = "Blog Satu",
                Posts = new List<Post>
                {
                    new Post
                    {
                        Title = "Pos Pertama",
                        Content = "Lorem ipsum dolor sit amet",
                    },
                    new Post
                    {
                        Title = "Pos Kedua",
                        Content = "Lorem ipsum dolor sit amet",
                        UpdatedDate = DateTime.Now
                    },
                }
            };
            db.Blogs.Add(newBlog);

            var count = db.SaveChangesAsync().Result;
            Console.WriteLine("{0} records saved to database", count);

            Console.WriteLine();
            Console.WriteLine("All blogs in database:");
            foreach (var blog in db.Blogs)
            {
                Console.WriteLine(" - {0}", blog.Url);
            }
        }

        internal void GetAll()
        {
            var blogs = db.Blogs.Include(b => b.Posts);
            foreach (var blog in blogs)
            {
                Helper.Print(blog);
            }
        }

        internal void GetExist()
        {
            var blogs = db.Blogs.Include(b => b.Posts);
            foreach (var blog in blogs)
            {
                if (!blog.IsDeleted)
                {
                    Helper.Print(blog);
                }
            }
        }

        internal void Get(int id)
        {
            Blog blog = db.Blogs.AsNoTracking().Include(b => b.Posts).FirstOrDefaultAsync(b => b.BlogId == id).Result;
            if (blog != null)
            {
                Helper.Print(blog);
            }
        }

        internal void Update(int id, Blog blog)
        {
            if (id != 0 && blog != null && blog.IsDeleted == false)
            {
                Blog oldBlog = db.Blogs.AsNoTracking().Include(b => b.Posts).FirstOrDefaultAsync(b => b.BlogId == id).Result;

                Helper.FlagUpdate(blog);
                foreach (var post in blog.Posts)
                {
                    Helper.FlagUpdate(post);
                    post.Title = string.Concat(post.Title, " ", "Berubah");
                    post.Content = string.Concat(post.Content, " ", "Berubah");
                }

                db.Blogs.Update(blog);

                foreach (var oldPost in oldBlog.Posts)
                {
                    Post newPost = blog.Posts.FirstOrDefault(p => p.PostId == oldPost.PostId);
                    if (newPost == null)
                    {
                        Helper.FlagDelete(oldPost);
                        db.Posts.Update(oldPost);
                    }
                }

                var count = db.SaveChangesAsync().Result;
                Console.WriteLine("{0} records updated on database", count);

                if (count > 0)
                {
                    Blog newBlog = db.Blogs.AsNoTracking().Include(b => b.Posts).FirstOrDefaultAsync(b => b.BlogId == id).Result;
                    Helper.Print(newBlog);
                }
            }
        }

        internal void Delete(int blogId)
        {
            Blog blog = db.Blogs.Include(b => b.Posts).FirstOrDefaultAsync(b => b.BlogId == blogId).Result;
            if (blog != null)
            {
                Helper.FlagDelete(blog);
                foreach (var post in blog.Posts)
                {
                    Helper.FlagDelete(post);
                }
            }

            var count = db.SaveChangesAsync().Result;
            Console.WriteLine("{0} records updated on database", count);

            if (count > 0)
            {
                Helper.Print(blog);
            }
        }
    }
}
