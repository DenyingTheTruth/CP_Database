using System;
using System.Collections.Generic;
using System.Globalization;
using forest_report_api.Models;

namespace forest_report_api.Extensions
{
    public static class AnalyticsModelExtensions
    {
        public static Dictionary<string, string> ToDictionary(this AnalyticsModel model, int maxItemCount = 0, int decimals = 2, bool isExcelExport = true)
        {
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("1", model.TitleRow);
            if (model.Items != null && model.Items.Count > 0)
            {
                foreach (var item in model.Items)
                {
                    dictionary.Add(item.Index, item.Value.HasValue
                        ? isExcelExport ? Math.Round(item.Value ?? 0, decimals).ToString(CultureInfo.InvariantCulture) :
                        Math.Round(item.Value ?? 0, decimals).GetForExcel()
                        : "-");
                }
            }
            else
                for (var i = 0; i < maxItemCount; i++)
                    dictionary.Add($"{i + 2}", "-");

            if (model.IsBold)
                dictionary.Add("isBold",true.ToString());

            return dictionary;
        }
    }
}