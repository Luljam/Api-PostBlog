using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreServices.Models;
using CoreServices.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace CoreServices.Repository
{
    public class PostRepository : IPostRepository
    {
        BlogDBContext db;
        public PostRepository(BlogDBContext _db)
        {
            db = _db;
        }

        public async Task<int> AddPost(Post post)
        {
            if (db != null)
            {
                await db.Post.AddAsync(post);
                await db.SaveChangesAsync();

                return post.PostId;
            }

            return 0;
        }

        public async Task<int> DeletePost(int? postId)
        {
            int result = 0;

            if (db != null)
            {
                //Encontra o  post especifico ppr id
                var post = await db.Post.FirstOrDefaultAsync(x => x.PostId == postId);

                if (post != null)
                {
                    //Deleta o post
                    db.Post.Remove(post);

                    //Commit a transação
                    result = await db.SaveChangesAsync();
                }
                return result;
            }

            return result;
        }

        public async Task<List<Category>> GetCategories()
        {
            if (db != null)
            {
                return await db.Category.ToListAsync();
            }
            return null;
        }

        public async Task<PostViewModel> GetPost(int? postId)
        {
            if (db != null)
            {
                return await (from p in db.Post
                              from c in db.Category
                              where p.PostId == postId
                              select new PostViewModel
                              {
                                  PostId = p.PostId,
                                  Title = p.Title,
                                  Description = p.Description,
                                  CategoryId = p.CategoryId,
                                  CategoryName = c.Name,
                                  CreatedDate = p.CreatedDate
                              }).FirstOrDefaultAsync();
            }

            return null;
        }

        public async Task<List<PostViewModel>> GetPosts()
        {
            if (db != null)
            {
                return await(from p in db.Post
                             from c in db.Category
                             where p.CategoryId == c.Id
                             select new PostViewModel
                             {
                                 PostId = p.PostId,
                                 Title = p.Title,
                                 Description = p.Description,
                                 CategoryId = p.CategoryId,
                                 CategoryName = c.Name,
                                 CreatedDate = p.CreatedDate
                             }).ToListAsync();
            }
            return null;
        }

        public async Task UpdatePost(Post post)
        {
            if (db != null)
            {
                //Deleta o post
                db.Post.Update(post);

                //Commita the transação
                await db.SaveChangesAsync();
            }
        }
    }
}
