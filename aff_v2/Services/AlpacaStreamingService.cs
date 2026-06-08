using Alpaca.Markets;
using FirstServ;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

public class StockHub : Hub
{
    
}
public class AlpacaStreamingService : BackgroundService
{
    private readonly IAlpacaDataStreamingClient _client;
    private readonly IHubContext<StockHub> _hub;
    private readonly ILogger<AlpacaStreamingService> _logger;
    private readonly AlpacaSettings _settings;
    public AlpacaStreamingService(IHubContext<StockHub> hub, ILogger<AlpacaStreamingService> logger, IOptions<AlpacaSettings> settings)
    {
        _hub = hub;
        _logger = logger;
        _settings = settings.Value;
        _client = Alpaca.Markets.Environments.Paper
                    .GetAlpacaDataStreamingClient(new SecretKey(_settings.AlpacaKey, _settings.AlpacaSecret));
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConnectandSubscribeAsync(stoppingToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Subscribtion failed retrying in 5 seconds");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
    private async Task ConnectandSubscribeAsync(CancellationToken stoppingToken)
    {
        await _client.ConnectAndAuthenticateAsync(stoppingToken);
        _logger.LogInformation("Connected to Alpaca Stream");

        var sub = _client.GetTradeSubscription("AAPL");

        sub.Received += async (trade) =>
        {
            await _hub.Clients.All.SendAsync("PriceUpdated", new
            {
                symbol = trade.Symbol,
                time = trade.TimestampUtc,
                price = trade.Price
            }, stoppingToken);
        };

        await _client.SubscribeAsync(sub);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}