using BlogContracts;
using Google.Protobuf.Collections;
using Grpc.Core;
using GrpcExplorer.BlogService.Services;
using GrpcServer;

namespace GrpcExplorer.BlogService.Controllers;

public class BloggingGrpcService(Services.BlogService blogService) : BlogGrpc.BlogGrpcBase
{
    public override async Task<BlogItemResponse> Create(CreateRequest request, ServerCallContext context)
    {
        var blogDto = await blogService.AddBlog(request.Url);
        return new BlogItemResponse()
        {
            Url = blogDto.Url,
            Id = blogDto.Id,
        };
    }

    public override async Task<BlogItemResponse> Update(UpdateRequest request, ServerCallContext context)
    {
        var blogDto = await blogService.UpdateBlog(request.Id, new UpdateBlogDto()
        {
            Url= request.Url,
        });
        return new BlogItemResponse()
        {
            Url = blogDto.Url,
            Id = blogDto.Id,
        };
    }

    public override async Task<BlogPagedResponse> GetBlogList(BlogListFilter request, ServerCallContext context)
    {   
        var blogDto = await blogService.GetBlogs();

        var blogPageResponse = new BlogPagedResponse()
        {
            TotalCount = blogDto.Count,
        };
        
        blogPageResponse.Blogs.AddRange(blogDto.Select(x => new BlogItemResponse()
        {
            Url = x.Url,
            Id = x.Id,
        }));

        return blogPageResponse;
    }
}