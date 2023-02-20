using AstraBlog.Models;

namespace AstraBlog.Services.Interfaces
{
    public interface IBlogPostService
    {

        #region BlogPost CRUD methods
        public Task AddBlogPostAsync(BlogPost blogPost);

        public Task UpdateBlogPostAsync(BlogPost blogPost);

        /// <summary>
        /// Get a single BlogPost by Id (integer)
        /// </summary>
        /// <param name="blogPostId"></param>
        /// <returns></returns>
        public Task<BlogPost> GetBlogPostAsync(int blogPostId);
        


        /// <summary>
        /// Get a single BlogPost by Id (slug)
        /// </summary>
        /// <param name="blogPostId"></param>
        /// <returns></returns>
        public Task<BlogPost> GetBlogPostAsync(string blogPostSlug);

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

        #region Additional Methods

        public Task<IEnumerable<Tag>> GetTagsAsync();

        public Task AddTagsToBlogPostAsync(string stringTags, int blogPostId);

        public Task AddTagsToBlogPostAsync(IEnumerable<int> tagIds, int blogPostId);

        public Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId);

        public Task RemoveAllBlogPostTagsAsync(int blogPostId);

        public IEnumerable<BlogPost> SearchBlogPosts(string? searchString);

        public Task<bool> ValidateSlugAsync(string title, int blogId);
        #endregion






    }
}
