using System;

interface ITrade
{
    double Value();
    string ClientSector();
    DateTime NextPaymentDate();
}

class CTrade : ITrade 
{
    double PrivateValue = 0;
    string PrivateClientSector = "";
    DateTime PrivateNextPaymentDate;

    public double Value()
    {
        return PrivateValue;
    }

    public string ClientSector()
    {
        return PrivateClientSector;
    }

    public DateTime NextPaymentDate()
    {
        return PrivateNextPaymentDate;
    }

    public bool IsExpired( DateTime RefDate )
    {
        return (RefDate.ToOADate()-PrivateNextPaymentDate.ToOADate() > 30);
    }

    public bool IsHighRisk()
    {
        return (PrivateValue > 1000000 && PrivateClientSector == "Private");
    }

    public bool IsMediumRisk()
    {
        return (PrivateValue > 1000000 && PrivateClientSector == "Public");
    }

    public bool GenericAttribute( string Attribute, DateTime RefDate )
    {
        if(Attribute == "EXPIRED")
            return IsExpired( RefDate );
        if(Attribute == "HIGHRISK")
            return IsHighRisk();
        if(Attribute == "MEDIUMRISK")
            return IsMediumRisk();
        return false;
    }

    public string[] CategoriesOrder()
    {
        string[] Categories = {"EXPIRED", "HIGHRISK", "MEDIUMRISK"};
        return Categories;
    }

    public string Category( DateTime RefDate )
    {
        string[] AllCategories = CategoriesOrder();
        foreach (string cat in AllCategories)
        {
            if(GenericAttribute(cat, RefDate))
                return cat;
        }
        return "";
    }

    public void SetInputs(double newValue, string newClientSector, DateTime newNextPaymentDate)
    {
        PrivateValue = newValue;
        PrivateClientSector = newClientSector;
        PrivateNextPaymentDate = newNextPaymentDate;
    }
}

class InputParser
{
    public DateTime ConvertDate(string StrDate)
    {
        string[] ListDate = StrDate.Split("/");
        int month = int.Parse(ListDate[0]);
        int day   = int.Parse(ListDate[1]);
        int year  = int.Parse(ListDate[2]);
        return new DateTime(year, month, day);
    }

    public object[] ParseLine(string Line)
    {
        string[] LineSplit = Line.Split(" ");
        double Value = int.Parse(LineSplit[0]);
        string Sector = LineSplit[1];
        DateTime Date = ConvertDate(LineSplit[2]);
        return new object[] {Value, Sector, Date};
    }
}

class Program 
{
    static void Main(string[] args) 
    {
        //string Input = Console.ReadLine();
        //Or...
        string Input = "12/11/2020\n" +
                       "4\n" +
                       "2000000 Private 12/29/2025\n" +
                       "400000 Public 07/01/2020\n" +
                       "5000000 Public 01/02/2024\n" +
                       "3000000 Public 10/26/2023";
        string[] InputSplit = Input.Split("\n");
        InputParser input_parser = new InputParser();
        DateTime Today = input_parser.ConvertDate(InputSplit[0]);
        int NuberOfTrades = int.Parse(InputSplit[1]);
        for(int i = 2; i < (NuberOfTrades+2); i++)
        {
            object[] ParsedLine = input_parser.ParseLine(InputSplit[i]);
            CTrade ThisTrade = new CTrade();
            ThisTrade.SetInputs(
                Convert.ToDouble(ParsedLine[0]), 
                Convert.ToString(ParsedLine[1]), 
                Convert.ToDateTime(ParsedLine[2])
            );
            Console.WriteLine(ThisTrade.Category(Today));
        }
    }
}
