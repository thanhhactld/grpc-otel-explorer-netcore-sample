using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using GrpcServer;
using Microsoft.AspNetCore.Mvc;

namespace GrpcExplorer.Client.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientController(Greeter.GreeterClient client) : ControllerBase
{
    private readonly Greeter.GreeterClient _client = client;

    [HttpGet("rest-sample")]
    public async Task<IActionResult> RestSample()
    {
        return Ok(new { Message = "Hello World!" });
    }
    
    [HttpGet("grpc-hello-world")]
    public async Task<IActionResult> CallRemoteGrpcHelloWorldAction()
    {
        var response = await _client.SayHelloAsync(new HelloRequest { Name = "World" });
        return Ok(response);
    }
    
    
    [HttpGet("grpc-yeah2")]
    public async Task<IActionResult> CallRemoteGrpcYeah2Action()
    {
        var response = await _client.SayYeahAsync(new SayYeahMessage() { Yeah = "World" });
        return Ok(response);
    }
}