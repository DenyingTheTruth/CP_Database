using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using forest_report_api.Helper;

namespace forest_report_api.Models
{
    public class TabModel
    {
        public string Id { get; set; }
        public int TabId { get; set; }
        public TabName TabName { get; set; }
        public string ReportTypeName { get; set; }
        public string Title { get; set; }
        public object Attachment { get; set; }
        public string TitleDate { get; set; }
        public Header Header { get; set; }
        public Table Table { get; set; }
        public AdditionTable AdditionTable { get; set; }
        public Footer Footer { get; set; }
        public IEnumerable<Binding> Bindings { get; set; }
        public IEnumerable<ValidationItem> Validations { get; set; }
        public IEnumerable<string> SubtractedRows { get; set; }
        public IEnumerable<string> SubtractedColumns { get; set; }
        public List<ValidationItem> ReadOnlyCells { get; set; }

        public T GetRow<T>(string code, string property)
        {
            object model = null;
            switch (TabName)
            {
                case TabName.BalanceTab:
                    var l1 = MyConverter.ConvertToClass<BalanceSheetItem>(Table.Rows);
                    model = l1.FirstOrDefault(x => x.CodeItem == code);
                    break;
                case TabName.ProfitLossTab:
                    var l2 = MyConverter.ConvertToClass<ProfitLossItem>(Table.Rows);
                    model = l2.FirstOrDefault(x => x.CodeItem == code);
                    break;
                case TabName.ChangeEquityTab:
                    var l3 = MyConverter.ConvertToClass<ChangeEquityItem>(Table.Rows);
                    model = l3.FirstOrDefault(x => x.CodeItem == code);
                    break;
                case TabName.MoveMoneyTab:
                    var l4 = MyConverter.ConvertToClass<MoveMoneyItem>(Table.Rows);
                    model = l4.FirstOrDefault(x => x.CodeItem == code);
                    break;
                case TabName.DecodingAccruedTaxes:
                    var l5 = MyConverter.ConvertToClass<DecodingAccruedTaxesItem>(Table.Rows);
                    model = l5.FirstOrDefault(x => x.CodeItem == code);
                    break;
                case TabName.DecodingFixedAssets:
                    var l6 = MyConverter.ConvertToClass<DecodingFixedAssetsItem>(Table.Rows);
                    model = l6.FirstOrDefault(x => x.CodeItem == code);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var prop = model?.GetType().GetProperty(property);
            return (T)prop?.GetValue(model);
        }
    }

    public class Header
    {
        public string Organization { get; set; }
        public string Number { get; set; }
        public string TypeEconomicActivity { get; set; }
        public string OrganizationalLegalForm { get; set; }
        public string Government { get; set; }
        public string Unit { get; set; }
        public string Address { get; set; }
    }

    public class Table
    {
        public List<ColumnItem> Columns { get; set; }
        public IEnumerable<object> Rows { get; set; }
    }

    public class ColumnItem
    {
        public string Title { get; set; }
        public string DataIndex { get; set; }
    }

    public class AdditionTable
    {
        public DateTime? ApprovedDate { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
    }

    public class Binding
    {
        public string Target { get; set; }
        public string Operations { get; set; }
        public string From { get; set; }
        public string Type { get; set; }
        public string Ignore { get; set; }
    }

    public class Footer
    {
        public string Leader { get; set; }
        public string ChiefAccountant { get; set; }

        public DateTime? Date { get; set; }

        //TODO: add for excel import
        public string LeaderName { get; set; } = "Руководитель";
        public string AccountantGeneral { get; set; } = "Главный бухгалтер";
    }

    public class Linkage
    {
        public int TabId { get; set; }
        public string Message { get; set; }
    }

    public enum TabName
    {
        BalanceTab,
        ProfitLossTab,
        ChangeEquityTab,
        MoveMoneyTab,
        DecodingAccruedTaxes,
        DecodingFixedAssets
    }
}