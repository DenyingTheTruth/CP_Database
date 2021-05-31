using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using forest_report_api.Entities;
using forest_report_api.Models;

namespace forest_report_api.Extensions
{
    public static class ListExtensions
    {
        public static void AutoCalculate<T>(this List<T> list, string codeTo, string operators, params string[] codeSum)
            where T : TableItem
        {
            var model = (object) list.FirstOrDefault(x => x.CodeItem == codeTo);
            var arrOperators = operators.Split(",");
            if (model != null)
            {
                decimal? value = null;
                switch (model)
                {
                    case BalanceSheetItem balanceSheetItem:
                        foreach (var propertyInfo in balanceSheetItem.GetType().GetProperties())
                        {
                            if (!propertyInfo.Name.Contains("Value"))
                                continue;

                            var obj = list.FirstOrDefault(x => x.CodeItem == codeSum[0]) as BalanceSheetItem;

                            value = (decimal?) propertyInfo.GetValue(obj);

                            var index = 1;

                            foreach (var mathOperator in arrOperators)
                            {
                                decimal? value2 = null;
                                var nextObj =
                                    (list.FirstOrDefault(x => x.CodeItem == codeSum[index]) as BalanceSheetItem);

                                if ((decimal?) propertyInfo.GetValue(nextObj) == null)
                                    continue;
                                else
                                    value2 = (decimal?) propertyInfo.GetValue(nextObj);

                                if (value == null)
                                    value = 0;
                                switch (mathOperator)
                                {
                                    case "-":
                                        value -= value2;
                                        break;
                                    case "+":
                                        value += value2;
                                        break;
                                    case "*":
                                        value *= value2;
                                        break;
                                    case "/":
                                        value /= value2;
                                        break;
                                }

                                index++;
                            }

                            propertyInfo.SetValue(balanceSheetItem, value);
                        }

                        break;
                }
            }
        }

        public static void AutoCalculateColumn<T>(this List<T> list, string target, string operations,
            string[] ignore = null,
            params string[] from) where T : TableItem
        {
            foreach (var item in list)
            {
                if (ignore != null && ignore.Contains(item.CodeItem))
                    continue;
                
                var type = typeof(T);
                decimal? value = null;
                var indexOperator = 0;
                var arrOperations = operations.Split(",");
                if (from.Any())
                {
                    var propertyInfo = type.GetProperty(from.First());
                    value = (decimal?) propertyInfo?.GetValue(item) ?? 0;

                    foreach (var prop in from.Skip(1))
                    {
                        propertyInfo = type.GetProperty(prop);
                        if ((decimal?) propertyInfo?.GetValue(item) == null)
                            continue;
                        else if (value == null)
                            value = 0;
                        switch (arrOperations[indexOperator])
                        {
                            case "-":
                                value -= (decimal?) propertyInfo?.GetValue(item) ?? 0;
                                break;
                            case "+":
                                value += (decimal?) propertyInfo?.GetValue(item) ?? 0;
                                break;
                        }

                        indexOperator++;
                    }
                }
                

                type.GetProperty(target)?.SetValue(item, value);
            }
        }
    }
}