
using Alpaca.Markets;
using Alpaca.Markets.Extensions;
using Microsoft.Extensions.Options;
namespace FirstServ;
public class AlpacaSettings
{
    public string AlpacaKey { get; set; } = string.Empty;
    public string AlpacaSecret { get; set; } = string.Empty;
    public string AlpacaEndpoint { get; set; } = string.Empty;
}
public interface IAlpacaService
{
    Task<Decimal?> GetBuyingPower();
    Task<Decimal?> GetCash();
    Task<IEnumerable<IPosition>> GetPositions();
    Task<IAccount> GetAccount();
    Task<IEnumerable<IOrder>> GetAllMyOrders();
    Task<IIntervalCalendar> GetLastOpen();
   Task<IAsyncEnumerable<IBar>>? GetHistoricalData(string symbol, DateTime start, DateTime end, CancellationToken cancellationToken);
   Task<IAsyncEnumerable<IBar>>? GetMultipleSymbols(List<string> Symbols, DateTime start, DateTime end, BarTimeFrame barTimeFrame);
}

public class AlpacaService : IAlpacaService
{
    private readonly AlpacaSettings _settings;
    public AlpacaService(IOptions<AlpacaSettings> settings)
    {
        _settings = settings.Value;
    }
    private IAlpacaTradingClient CreateClient()
    {
        return Alpaca.Markets.Environments.Paper
            .GetAlpacaTradingClient(new SecretKey(_settings.AlpacaKey, _settings.AlpacaSecret));
    }
    private IAlpacaDataClient DataClient()
    {
        return Alpaca.Markets.Environments.Paper
            .GetAlpacaDataClient(new SecretKey(_settings.AlpacaKey, _settings.AlpacaSecret));
    }
    
    public async Task<IAccount> GetAccount()
    {
        var client = CreateClient();
        var account = await client.GetAccountAsync();
        return account;
    }
    public async Task<decimal?> GetBuyingPower()
    {
        try
        {
            var client = CreateClient();
            var account = await client.GetAccountAsync();
            return account.BuyingPower;
        }
        catch (RestClientErrorException ex)
        {
            Console.WriteLine($"Alpaca API error: {ex.Message}");
            throw;
        }
    }
    public async Task<IEnumerable<IPosition>> GetPositions()
    {
        var client = CreateClient();
        var positions = await client.ListPositionsAsync();
        return positions;
    }
     public async Task<decimal?> GetCash()
    {
        try
        {
            var client = CreateClient();
            var account = await client.GetAccountAsync();
            return account.TradableCash;
        }
        catch (RestClientErrorException ex)
        {
            Console.WriteLine($"Alpaca API error: {ex.Message}");
            throw;
        }
    }
    public async Task<IEnumerable<IOrder>> GetAllMyOrders()
    {
        var client = CreateClient(); // Should return IAlpacaTradingClient
        var orders = await client.ListOrdersAsync(new ListOrdersRequest
        {
            OrderStatusFilter = OrderStatusFilter.All // Optional: All, Open, Closed
        });
        return orders;
    }
    public async Task<IAsyncEnumerable<IBar>>? GetHistoricalData(string symbol, DateTime start, DateTime end, CancellationToken cancellationToken=default)
    {
        try
        {
            var dataClient = DataClient();
            var bartimeframe = new BarTimeFrame();

            TimeSpan difference = end - start;
            string formatted = $"{(int)difference.TotalHours}h {difference.Minutes}m";

            Console.WriteLine(formatted); 

            if(difference.TotalHours > 168 && difference.TotalHours < 721)
            {
               bartimeframe = BarTimeFrame.Day;
            }
            else if (difference.TotalHours > 721)
            {
                bartimeframe = BarTimeFrame.Week;
            }
            else if (difference.TotalHours <= 168)
            {
                bartimeframe = BarTimeFrame.Hour;
            }
            var req = new HistoricalBarsRequest(symbol, start, end, bartimeframe)
            {
                Feed = MarketDataFeed.Iex
            };
            var db = dataClient.GetHistoricalBarsAsAsyncEnumerable(req);

            return db;
        }
        catch(Exception ex)
        {
            throw new Exception($"The Error is : {ex.Message}");
        }
        
    }
    public async Task<IAsyncEnumerable<IBar>>? GetMultipleSymbols(List<string> Symbols, DateTime start, DateTime end, BarTimeFrame timeFrame)
    {
        try
        {
            var client = DataClient();
            var pages = new HistoricalBarsRequest(Symbols, start, end, timeFrame)
            {
                Feed = MarketDataFeed.Iex
            };
            var req = client.GetHistoricalBarsAsAsyncEnumerable(pages);
            return req;

        }
        catch(Exception ex)
        {
            throw new Exception($"Error {ex.Message}");
        }
    }
    public async Task<IIntervalCalendar> GetLastOpen()
    {
        var client = CreateClient();
        var d_interval = new Interval<DateTime>(DateTime.Today.AddDays(-7), DateTime.Today.AddDays(-1));

        var calendar = await client.ListIntervalCalendarAsync(
                                        new CalendarRequest().WithInterval(d_interval)
        );
        
        var lastOpenTime = calendar.LastOrDefault();
        if(lastOpenTime != null)
        {
            Console.WriteLine($"last open was {lastOpenTime.GetTradingOpenTimeUtc()}");
            return lastOpenTime;

        }
        else
        {
            throw new Exception("something");
        }
    }
}
