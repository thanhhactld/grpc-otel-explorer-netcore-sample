using GrpcExplorer.BlogService.Domain;
using GrpcExplorer.BlogService.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace GrpcExplorer.BlogService.Services;

public class BlogDto
{
    public int Id { get; set; }
    public string Url { get; set; }

    public static BlogDto FromEntity(Blog entity)
    {
        return new BlogDto() { Id = entity.BlogId, Url = entity.Url };
    }
}

public class UpdateBlogDto
{
    public string Url { get; set; }

    public void BindToEntity(Blog entity)
    {
        entity.Url = Url;
    }
}

public class BlogService
{
    private readonly BloggingContext _bloggingContext;

    public BlogService(BloggingContext bloggingContext)
    {
        _bloggingContext = bloggingContext;
    }

    public async Task<BlogDto> AddBlog(string blogUrl)
    {
        var newBlog = Blog.NewBlog(blogUrl);
        _bloggingContext.Blogs.Add(newBlog);
        await _bloggingContext.SaveChangesAsync();
        return BlogDto.FromEntity(newBlog);
    }
    
    public async Task<BlogDto> UpdateBlog(int blogId, UpdateBlogDto updateDto)
    {
        var currentBlog = await _bloggingContext.Blogs.FindAsync(blogId);
        updateDto.BindToEntity(currentBlog);
        await _bloggingContext.SaveChangesAsync();
        return BlogDto.FromEntity(currentBlog);
    }

    public async Task<List<BlogDto>> GetBlogs()
    {
        var blogs = await _bloggingContext.Blogs.ToListAsync();
        return blogs.Select(BlogDto.FromEntity).ToList();
    }
}