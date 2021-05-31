using System.Collections.Generic;

namespace forest_report_api.Models
{
    public static class ExcelData
    {
        public static IEnumerable<ExcelPoint> Values = new List<ExcelPoint>
        {
            new ExcelPoint
            {
                TabName = TabName.BalanceTab,
                Key = "Date",
                Column = 1,
                Row = 97
            },
            new ExcelPoint
            {
                TabName = TabName.BalanceTab,
                Key = "Leader",
                Column = 5,
                Row = 95
            },
            new ExcelPoint
            {
                TabName = TabName.BalanceTab,
                Key = "ChiefAccountant",
                Column = 5,
                Row = 96
            },
            new ExcelPoint
            {
                TabName = TabName.ProfitLossTab,
                Key = "Date",
                Column = 1,
                Row = 60
            },
            new ExcelPoint
            {
                TabName = TabName.ProfitLossTab,
                Key = "Leader",
                Column = 5,
                Row = 58
            },
            new ExcelPoint
            {
                TabName = TabName.ProfitLossTab,
                Key = "ChiefAccountant",
                Column = 5,
                Row = 59
            },
            new ExcelPoint
            {
                TabName = TabName.ChangeEquityTab,
                Key = "Date",
                Column = 1,
                Row = 84
            },
            new ExcelPoint
            {
                TabName = TabName.ChangeEquityTab,
                Key = "Leader",
                Column = 9,
                Row = 82
            },
            new ExcelPoint
            {
                TabName = TabName.ChangeEquityTab,
                Key = "ChiefAccountant",
                Column = 9,
                Row = 83
            },
            new ExcelPoint
            {
                TabName = TabName.MoveMoneyTab,
                Key = "Date",
                Column = 1,
                Row = 63
            },
            new ExcelPoint
            {
                TabName = TabName.MoveMoneyTab,
                Key = "Leader",
                Column = 5,
                Row = 61
            },
            new ExcelPoint
            {
                TabName = TabName.MoveMoneyTab,
                Key = "ChiefAccountant",
                Column = 5,
                Row = 62
            },
            new ExcelPoint
            {
                TabName = TabName.DecodingAccruedTaxes,
                Key = "Date",
                Column = 1,
                Row = 34
            },
            new ExcelPoint
            {
                TabName = TabName.DecodingAccruedTaxes,
                Key = "Leader",
                Column = 6,
                Row = 32
            },
            new ExcelPoint
            {
                TabName = TabName.DecodingAccruedTaxes,
                Key = "ChiefAccountant",
                Column = 6,
                Row = 33
            },
            new ExcelPoint
            {
                TabName = TabName.DecodingFixedAssets,
                Key = "Date",
                Column = 1,
                Row = 27
            },
            new ExcelPoint
            {
                TabName = TabName.DecodingFixedAssets,
                Key = "Leader",
                Column = 4,
                Row = 25
            },
            new ExcelPoint
            {
                TabName = TabName.DecodingFixedAssets,
                Key = "ChiefAccountant",
                Column = 4,
                Row = 26
            },
            new ExcelPoint
            {
                TabName = TabName.BalanceTab,
                Key = "ApprovedDate",
                Column = 6,
                Row = 11
            },
            new ExcelPoint
            {
                TabName = TabName.BalanceTab,
                Key = "SendDate",
                Column = 6,
                Row = 12
            },
            new ExcelPoint
            {
                TabName = TabName.BalanceTab,
                Key = "AcceptedDate",
                Column = 6,
                Row = 13
            },
        };
    }

    public class ExcelPoint
    {
        public string Key { get; set; }
        public TabName TabName { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}