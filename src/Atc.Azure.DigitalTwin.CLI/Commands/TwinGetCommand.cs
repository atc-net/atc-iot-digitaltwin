namespace Atc.Azure.DigitalTwin.CLI.Commands;

public sealed class TwinGetCommand : AsyncCommand<TwinCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<TwinGetCommand> logger;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    public TwinGetCommand(
        ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<TwinGetCommand>();
        jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        TwinCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        TwinCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var twinService = TwinServiceFactory.Create(
            loggerFactory,
            settings.TenantId!,
            settings.AdtInstanceUrl!);

        var digitalTwinId = settings.TwinId;
        logger.LogInformation($"Getting Twin with id '{digitalTwinId}'");

        var twin = await twinService.GetTwinById(digitalTwinId);
        if (twin is null)
        {
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation(JsonSerializer.Serialize(twin, jsonSerializerOptions));

        return ConsoleExitStatusCodes.Success;
    }
}