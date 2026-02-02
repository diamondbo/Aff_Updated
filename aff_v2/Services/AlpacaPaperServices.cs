using Alpaca.Markets;
using Microsoft.AspNetCore.Http.HttpResults;
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
}
