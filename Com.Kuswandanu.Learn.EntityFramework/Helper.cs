using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Kuswandanu.Learn.EntityFramework
{
    class Helper
    {
        public static void FlagUpdate(IBasicEntity entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
        }

        public static void FlagDelete(IBasicEntity entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            entity.IsDeleted = true;
        }

        public static void Print(Blog blog)
        {
            Func<bool, string> checkStatus = delegate (bool isDeleted) { return isDeleted ? "Deleted" : "Exist"; };

            Console.WriteLine(" - {0}. {1} [{2}]", blog.BlogId, blog.Url, checkStatus(blog.IsDeleted));
            Console.WriteLine(" - {0}", blog.UpdatedDate);
            foreach (var post in blog.Posts)
            {
                Console.WriteLine("    {0}. {1} [{2}]", post.PostId, post.Title, checkStatus(post.IsDeleted));
                Console.WriteLine("      - {0}", post.UpdatedDate);
                Console.WriteLine("      - {0}", post.Content);
            }
        }

    }
}
