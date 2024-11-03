using Grpc.Core;
using GrpcExplorer.OpenTelemetry;
using GrpcServer;

namespace GrpcExplorer.Server;

public class GreeterService : Greeter.GreeterBase
{

    private readonly ILogger<GreeterService> _logger;
    private readonly Instrumentation _instrumentation;

    public GreeterService(ILogger<GreeterService> logger, Instrumentation instrumentation)
    {
        _logger = logger;
        _instrumentation = instrumentation;
    }

    public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        
        // var shareInstrumentation = context.Re
        foreach (var i in Enumerable.Range(0, 10))
        {
            using var activity = _instrumentation.ActivitySource.StartActivity($"SayHello step {i}");
            await Task.Delay(200);
        }
        
        return await Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
    
    public override Task<SayYeahResponse> SayYeah(SayYeahMessage request, ServerCallContext context)
    {
        return Task.FromResult(new SayYeahResponse
        {
            Yeah = "1212122121212"
        });
    }
}