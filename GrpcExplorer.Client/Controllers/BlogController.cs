using BlogContracts;
using Microsoft.AspNetCore.Mvc;

namespace GrpcExplorer.Client.Controllers;

[Route("api/[controller]")]
public class BlogController(BlogGrpc.BlogGrpcClient blogGrpcClient) : ControllerBase
{
    [HttpPost()]
    public async Task<IActionResult> CreateBlogAction([FromBody] CreateRequest request)
    {
        await Task.CompletedTask;
        var response =  await blogGrpcClient.CreateAsync(request);
        return Ok(response);
    }
    
    [HttpPut("{blogId}")]
    public async Task<IActionResult> UpdateBlogAction(int blogId, [FromBody] CreateRequest request)
    {
        await Task.CompletedTask;
        var response =  await blogGrpcClient.UpdateAsync(new UpdateRequest()
        {
            Id = blogId,
            Url = request.Url,
        });
        return Ok(response);
    }
    
    
    [HttpGet()]
    public async Task<IActionResult> ListBlogAction()
    {
        await Task.CompletedTask;
        var response =  await blogGrpcClient.GetBlogListAsync(new BlogListFilter()
        {
        });
        return Ok(response);
    }
}