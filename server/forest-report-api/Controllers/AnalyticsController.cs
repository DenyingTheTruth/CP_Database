using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Entities.Enums;
using forest_report_api.Facade;
using forest_report_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace forest_report_api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("analytics")]
    [ApiController]
    public class AnalyticsController : Controller
    {
        private readonly ReportServiceFacade _reportServiceFacade;

        public AnalyticsController(ReportServiceFacade reportServiceFacade)
        {
            _reportServiceFacade = reportServiceFacade;
        }

        [HttpPost("export")]
        public async Task<object> Export(ReportStatisticsFilterModel filterModel, AnalyticsTypeEnum analyticsTypeEnum)
        {
            return await _reportServiceFacade.ExportFileAnalytics(filterModel, analyticsTypeEnum);
        }
        
        [HttpPost("balance-asset")]
        public async Task<object> GetBalanceAsset(ReportStatisticsFilterModel filterModel)
        {
            return await _reportServiceFacade.GetBalanceAsset(filterModel);
        }

        [HttpPost("structure-obligations")]
        public async Task<object> GetStructureObligations(ReportStatisticsFilterModel filterModel)
        {
            return await _reportServiceFacade.GetStructureObligations(filterModel);
        }

        [HttpPost("balance-liabilities")]
        public async Task<object> GetBalanceLiabilities(ReportStatisticsFilterModel filterModel)
        {
            return await _reportServiceFacade.GetBalanceSheetLiabilitiesStructure(filterModel);
        }
        
        [HttpPost("financial-indicators")]
        public async Task<object> GetFinancialIndicators(ReportStatisticsFilterModel filterModel)
        {
            return await _reportServiceFacade.GetFinancialIndicators(filterModel);
        }

        [HttpPost("solvency-ratios")]
        public async Task<object> GetSolvencyRatios(ReportStatisticsFilterModel filterModel)
        {
            return await _reportServiceFacade.GetSolvencyRatios(filterModel);
        }

        [HttpPost("working-capital")]
        public async Task<object> GetStatusOfOwnWorkingCapital(ReportStatisticsFilterModel filterModel)
        {
            return await _reportServiceFacade.GetStatusOfOwnWorkingCapital(filterModel);
        }
    }
}