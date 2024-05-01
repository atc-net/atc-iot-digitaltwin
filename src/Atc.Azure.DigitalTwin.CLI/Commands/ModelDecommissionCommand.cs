namespace Atc.Azure.DigitalTwin.CLI.Commands;

public sealed class ModelDecommissionCommand : AsyncCommand<ModelCommandSettings>
{
    private readonly DigitalTwinsClient client;
    private readonly JsonSerializerOptions jsonSerializerOptions;
    private readonly ILogger<ModelDecommissionCommand> logger;

    public ModelDecommissionCommand(
        ILoggerFactory loggerFactory,
        DigitalTwinsClient client)
    {
        logger = loggerFactory.CreateLogger<ModelDecommissionCommand>();
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ModelCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return ExecuteInternalAsync(settings);
    }

    private async Task<int> ExecuteInternalAsync(
        ModelCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var modelId = settings.ModelId;
        logger.LogInformation($"Decommissioning Model with id '{modelId}'");

        try
        {
            var result = await client.DecommissionModelAsync(modelId);
            if (result is null)
            {
                return ConsoleExitStatusCodes.Failure;
            }

            logger.LogInformation(JsonSerializer.Serialize(result, jsonSerializerOptions));

            logger.LogInformation("Successfully decommissioned model.");
            return ConsoleExitStatusCodes.Success;
        }
        catch (RequestFailedException e)
        {
            logger.LogError($"Error {e.Status}: {e.Message}");
            return ConsoleExitStatusCodes.Failure;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return ConsoleExitStatusCodes.Failure;
        }
    }
}