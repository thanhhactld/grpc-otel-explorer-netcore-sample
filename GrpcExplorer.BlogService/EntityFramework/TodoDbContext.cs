using GrpcExplorer.BlogService.Domain;
using Microsoft.EntityFrameworkCore;

namespace GrpcExplorer.BlogService.EntityFramework;

public class BloggingContext(DbContextOptions<BloggingContext> options) : DbContext(options)
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
}