using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace forest_report_api.Models
{
    public class ListTabModel : List<TabModel>
    {
        // public ListTabModel(IEnumerable<TabModel> models)
        // {
        //     AddRange(models);
        // }
        //
        // public IEnumerable<TableItem> Get(int tab, string code)
        // {
        //     var tabModel = this.FirstOrDefault(x => x.TabId == tab);
        //     return tabModel?.Table.Rows.Where(x => x.CodeItem == code);
        // }
    }
}