using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using forest_report_api.Helper;
using forest_report_api.Models;
using Newtonsoft.Json;

namespace forest_report_api.Extensions
{
    public static class ObjectExtensions
    {
        public static T GetValue<T>(this object sender, string propName)
        {
            var propertyInfo = sender?.GetType().GetProperties().FirstOrDefault(x => x.Name == propName);
            return (T) propertyInfo?.GetValue(sender);
        }

        public static TabModel DeepClone(this TabModel obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var model = JsonConvert.DeserializeObject<TabModel>(json);
            var list = new List<object>();
            foreach (var row in obj.Table.Rows)
            {
                switch (obj.TabName)
                {
                    case TabName.BalanceTab:
                        list.Add(new BalanceSheetItem
                        {
                            Title = ((BalanceSheetItem)row).Title,
                            CodeItem = ((BalanceSheetItem)row).CodeItem,
                            CodeParent = ((BalanceSheetItem)row).CodeParent,
                            TitlePosition = ((BalanceSheetItem)row).TitlePosition,
                            Value1 = ((BalanceSheetItem)row).Value1,
                            Value2 = ((BalanceSheetItem)row).Value2
                        });
                        break;
                    case TabName.ProfitLossTab:
                        list.Add(new ProfitLossItem()
                        {
                            Title = ((ProfitLossItem)row).Title,
                            CodeItem = ((ProfitLossItem)row).CodeItem,
                            CodeParent = ((ProfitLossItem)row).CodeParent,
                            TitlePosition = ((ProfitLossItem)row).TitlePosition,
                            Value1 = ((ProfitLossItem)row).Value1,
                            Value2 = ((ProfitLossItem)row).Value2
                        });
                        break;
                    case TabName.ChangeEquityTab:
                        list.Add(new ChangeEquityItem()
                        {
                            Title = ((ChangeEquityItem)row).Title,
                            CodeItem = ((ChangeEquityItem)row).CodeItem,
                            CodeParent = ((ChangeEquityItem)row).CodeParent,
                            TitlePosition = ((ChangeEquityItem)row).TitlePosition,
                            Value1 = ((ChangeEquityItem)row).Value1,
                            Value2 = ((ChangeEquityItem)row).Value2,
                            Value3 = ((ChangeEquityItem)row).Value3,
                            Value4 = ((ChangeEquityItem)row).Value4,
                            Value5 = ((ChangeEquityItem)row).Value5,
                            Value6 = ((ChangeEquityItem)row).Value6,
                            Value7 = ((ChangeEquityItem)row).Value7,
                            Value8 = ((ChangeEquityItem)row).Value8
                        });
                        break;
                    case TabName.MoveMoneyTab:
                        list.Add(new MoveMoneyItem()
                        {
                            Title = ((MoveMoneyItem)row).Title,
                            CodeItem = ((MoveMoneyItem)row).CodeItem,
                            CodeParent = ((MoveMoneyItem)row).CodeParent,
                            TitlePosition = ((MoveMoneyItem)row).TitlePosition,
                            Value1 = ((MoveMoneyItem)row).Value1,
                            Value2 = ((MoveMoneyItem)row).Value2
                        });
                        break;
                    case TabName.DecodingAccruedTaxes:
                        list.Add(new DecodingAccruedTaxesItem()
                        {
                            Title = ((DecodingAccruedTaxesItem)row).Title,
                            CodeItem = ((DecodingAccruedTaxesItem)row).CodeItem,
                            CodeParent = ((DecodingAccruedTaxesItem)row).CodeParent,
                            TitlePosition = ((DecodingAccruedTaxesItem)row).TitlePosition,
                            Value1 = ((DecodingAccruedTaxesItem)row).Value1,
                            Value2 = ((DecodingAccruedTaxesItem)row).Value2,
                            Value3 = ((DecodingAccruedTaxesItem)row).Value3
                        });
                        break;
                    case TabName.DecodingFixedAssets:
                        list.Add(new DecodingFixedAssetsItem()
                        {
                            Title = ((DecodingFixedAssetsItem)row).Title,
                            CodeItem = ((DecodingFixedAssetsItem)row).CodeItem,
                            CodeParent = ((DecodingFixedAssetsItem)row).CodeParent,
                            TitlePosition = ((DecodingFixedAssetsItem)row).TitlePosition,
                            Value1 = ((DecodingFixedAssetsItem)row).Value1,
                            Value2 = ((DecodingFixedAssetsItem)row).Value2,
                            Value3 = ((DecodingFixedAssetsItem)row).Value3,
                            Value4 = ((DecodingFixedAssetsItem)row).Value4
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                model.Table.Rows = list;
            }
            return model;
        }
    }
}