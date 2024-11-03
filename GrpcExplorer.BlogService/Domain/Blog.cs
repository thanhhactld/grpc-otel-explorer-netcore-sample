namespace GrpcExplorer.BlogService.Domain;

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    public List<Post> Posts { get; set; }

    public static Blog NewBlog(string url)
    {
        return new Blog() { Url = url };
    }
}