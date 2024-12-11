using System.Globalization;

Console.WriteLine("Hello, World!");

DateTime referenceDate = DateTime.Parse(Console.ReadLine());
int n = int.Parse(Console.ReadLine());
List<ITrade> trades = new List<ITrade>();

for (int i = 0; i < n; i++)
{
    var input = Console.ReadLine().Split(' ');
    trades.Add(new Trade
    {
        Value = double.Parse(input[0]),
        ClientSector = input[1],
        NextPaymentDate = DateTime.ParseExact(input[2], "MM/dd/yyyy", CultureInfo.InvariantCulture),
        IsPoliticallyExposed = bool.Parse(input[3])
    });
}

TradeCategorizer categorizer = new();

foreach (var trade in trades)
{
    Console.WriteLine(categorizer.CategorizeTrade(trade, referenceDate));
}


interface ITrade
{
    double Value { get; }
    string ClientSector { get; }
    DateTime NextPaymentDate { get; }
    public bool IsPoliticallyExposed { get; set; } 
}

class Trade : ITrade
{
    public double Value { get; set; }
    public string ClientSector { get; set; }
    public DateTime NextPaymentDate { get; set; }
    public bool IsPoliticallyExposed { get; set; }
}

interface ITradeCategory
{
    string CategoryName { get; }
    bool IsMatch(ITrade trade, DateTime referenceDate);
}

class ExpiredCategory : ITradeCategory
{
    public string CategoryName => "EXPIRED";

    public bool IsMatch(ITrade trade, DateTime referenceDate)
    {
        return (referenceDate - trade.NextPaymentDate).TotalDays > 30;
    }
}

class HighRiskCategory : ITradeCategory
{
    public string CategoryName => "HIGHRISK";

    public bool IsMatch(ITrade trade, DateTime referenceDate)
    {
        return trade.Value > 1_000_000 && trade.ClientSector == "Private";
    }
}

class MediumRiskCategory : ITradeCategory
{
    public string CategoryName => "MEDIUMRISK";

    public bool IsMatch(ITrade trade, DateTime referenceDate)
    {
        return trade.Value > 1_000_000 && trade.ClientSector == "Public";
    }
}
class PepCategory : ITradeCategory
{
    public string CategoryName => "PEP";

    public bool IsMatch(ITrade trade, DateTime referenceDate)
    {
        return (trade as dynamic)?.IsPoliticallyExposed == true; // new prop
    }
}


class TradeCategorizer
{
    private readonly List<ITradeCategory> _categories;

    public TradeCategorizer()
    {
        _categories = new List<ITradeCategory>
        {
            new PepCategory(),//question 2 ,add new category
            new ExpiredCategory(),
            new HighRiskCategory(),
            new MediumRiskCategory(),            
        };
    }

    public string CategorizeTrade(ITrade trade, DateTime referenceDate)
    {
        foreach (var category in _categories)
        {
            if (category.IsMatch(trade, referenceDate))
            {
                return category.CategoryName;
            }
        }

        return "UNCATEGORIZED";
    }
}


      