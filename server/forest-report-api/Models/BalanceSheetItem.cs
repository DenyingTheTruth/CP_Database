using forest_report_api.Attribute;

namespace forest_report_api.Models
{
    [TabItemEnumReference(TabName.BalanceTab)]
    public class BalanceSheetItem : TableItem
    {
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
    }

    [TabItemEnumReference(TabName.ProfitLossTab)]
    public class ProfitLossItem : TableItem
    {
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
    }
    [TabItemEnumReference(TabName.ChangeEquityTab)]
    public class ChangeEquityItem : TableItem
    {
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
        public decimal? Value3 { get; set; }
        public decimal? Value4 { get; set; }
        public decimal? Value5 { get; set; }
        public decimal? Value6 { get; set; }
        public decimal? Value7 { get; set; }
        public decimal? Value8 { get; set; }
    }
    [TabItemEnumReference(TabName.MoveMoneyTab)]
    public class MoveMoneyItem : TableItem
    {
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
    }
    [TabItemEnumReference(TabName.DecodingAccruedTaxes)]
    public class DecodingAccruedTaxesItem : TableItem
    {
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
        public decimal? Value3 { get; set; }
    }
    [TabItemEnumReference(TabName.DecodingFixedAssets)]
    public class DecodingFixedAssetsItem : TableItem
    {
        public decimal? Value1 { get; set; }
        public decimal? Value2 { get; set; }
        public decimal? Value3 { get; set; }
        public decimal? Value4 { get; set; }
    }
}