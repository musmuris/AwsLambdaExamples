using Amazon.Lambda.Core;
using StatsdClient;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace nigel_lambdasimple;

public class Function
{
    private IConfiguration _config;
    private Random _rand;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<Function> _logger;
    private readonly StatsdConfig _dogstatsdConfig;

    public Function()
    {
        _config = new ConfigurationBuilder()
            .AddSystemsManager("/nigel_lambdasimple")
            .AddEnvironmentVariables()
            .Build();

        _loggerFactory = LoggerFactory.Create(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(_config.GetSection("Logging"));
            loggingBuilder.AddLambdaLogger();
        });

        _logger = _loggerFactory.CreateLogger<Function>();

        // instantiate the statsd client 
        _dogstatsdConfig = new StatsdConfig
        {
            StatsdServerName = "127.0.0.1",
            StatsdPort = 8125,
        };
        if (!DogStatsd.Configure(_dogstatsdConfig))
            throw new InvalidOperationException("Cannot initialize DogstatsD. Set optionalExceptionHandler argument in the `Configure` method for more information.");

        _rand = new Random();
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(string input, ILambdaContext context)
    {
        _logger.LogInformation($"Hello - got input {input}");
        DogStatsd.Distribution("nigel-lambdasimple.test.metric.input", 1 );
        StringBuilder b = new StringBuilder();
        b.AppendLine();
        foreach (var f in _config.AsEnumerable())
        {
            b.AppendLine($"'{f.Key}' : '{f.Value}'");
        }
        if( _rand.NextDouble() > 0.2 )
            DogStatsd.Distribution("nigel-lambdasimple.test.success", 1);
        else
            DogStatsd.Distribution("nigel-lambdasimple.test.fail", 1);

        return input.ToUpper() + b.ToString();
    }

    public static void Main(string[] args)
    {

    }
}
