using AstraBlog.Models;

namespace AstraBlog.Services.Interfaces
{
    public interface IBlogPostService
    {

        #region BlogPost CRUD methods
        public Task AddBlogPostAsync(BlogPost blogPost);

        public Task UpdateBlogPostAsync(BlogPost blogPost);

        public Task<BlogPost> GetBlogPostAsync(int blogPostId);

        public Task DeleteBlogPostAsync(BlogPost blogPost);
        #endregion

        #region Get BlogPosts Methods
        public Task<IEnumerable<BlogPost>> GetBlogPostsAsync();

        public Task<IEnumerable<BlogPost>> GetPopularPostsAsync();

        public Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count);

        public Task<IEnumerable<BlogPost>> GetRecentPostsAsync();

        public Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count);

        #endregion

        #region Category CRUD

        public Task AddCategoryAsync(Category category);

        public Task UpdateCategoryAsync(Category category);

        public Task<IEnumerable<Category>> GetCategoriesAsync();

        public Task<Category> GetCategoryAsync(int categoryId);

        public Task DeleteCategoryAsync(Category category);

        #endregion
    }
}
