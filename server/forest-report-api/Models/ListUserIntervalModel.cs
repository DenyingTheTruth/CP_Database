using System;
using System.Collections.Generic;
using System.Linq;
using forest_report_api.Entities;
using forest_report_api.Extensions;

namespace forest_report_api.Models
{
    public class ListUserIntervalModel : List<IntervalModel>
    {
        public ListUserIntervalModel(List<UserIntervalModel> models, List<Organization> organizations, int year,
            IEnumerable<Period> periods)
        {
            var enumerable = periods.ToList();
            foreach (var organization in organizations.OrderBy(x => x.Name))
            {
                var newModel = new IntervalModel
                {
                    Organization = models
                                       .FirstOrDefault(x => x.OrganizationId == organization.Id)?.Organization
                                       .GetFrontObject()
                                   ?? organization.GetFrontObject(),
                    OrganizationId = organization.Id,
                    Periods = new List<PeriodModel>(),
                    Year = year
                };

                foreach (var period in enumerable)
                {
                    var periodModel = models.FirstOrDefault(x =>
                        x.OrganizationId == organization.Id && x.PeriodId == period.Id);
                    if (periodModel != null)
                    {
                        newModel.Periods.Add(new PeriodModel
                        {
                            Id = periodModel.Period.Id,
                            Name = periodModel.Period.Name,
                            Date = periodModel.EndDate,
                            UserCheckinIntervalId = periodModel.Id
                        });
                    }
                    else
                    {
                        newModel.Periods.Add(new PeriodModel
                        {
                            Id = period.Id,
                            Name = period.Name
                        });
                    }
                }

                Add(newModel);
            }
        }

        public ListUserIntervalModel(IEnumerable<FullUserIntervalModel> models, List<Organization> organizations, int year,
            IEnumerable<Period> periods)
        {
            var enumerable = periods.ToList();
            foreach (var organization in organizations.OrderBy(x => x.Name))
            {
                var newModel = new IntervalModel
                {
                    Organization = models
                                       .FirstOrDefault(x => x.OrganizationId == organization.Id)?.Organization
                                       .GetFrontObject()
                                   ?? organization.GetFrontObject(),
                    OrganizationId = organization.Id,
                    Periods = new List<PeriodModel>(),
                    Year = year
                };

                foreach (var period in enumerable)
                {
                    var periodModel = models.FirstOrDefault(x =>
                        x.OrganizationId == organization.Id && x.PeriodId == period.Id);
                    if (periodModel != null)
                    {
                        newModel.Periods.Add(new PeriodModel
                        {
                            Id = periodModel.Period.Id,
                            Name = periodModel.Period.Name,
                            Date = periodModel.EndDate,
                            UserCheckinIntervalId = periodModel.Id,
                            Report = periodModel.Report != null ? periodModel.Report.GetMiniObject() : null
                        });
                    }
                    else
                    {
                        newModel.Periods.Add(new PeriodModel
                        {
                            Id = period.Id,
                            Name = period.Name
                        });
                    }
                }

                Add(newModel);
            }
        }

        public ListUserIntervalModel(List<UserIntervalModel> models, string organizationId, int year)
        {
            var newModel = new IntervalModel
            {
                Organization = models
                    .FirstOrDefault(x => x.OrganizationId == organizationId)?.Organization
                    .GetFrontObject(),
                OrganizationId = organizationId,
                Periods = models.Where(x => x.OrganizationId == organizationId)
                    .OrderBy(x => x.Period.Name)
                    .Select(x => new PeriodModel
                    {
                        Id = x.Period.Id,
                        Name = x.Period.Name,
                        Date = x.EndDate,
                        UserCheckinIntervalId = x.Id,
                        IsFree = x.CanBeCreated
                    })
                    .ToList(),
                Year = year
            };
            Add(newModel);
        }
    }

    public class IntervalModel
    {
        public int Year { get; set; }
        public string OrganizationId { get; set; }
        public object Organization { get; set; }
        public List<PeriodModel> Periods { get; set; }
        public bool IsFull => Periods.All(x => x.Date.HasValue);
    }

    public class PeriodModel
    {
        public string Id { get; set; }
        public string UserCheckinIntervalId { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public object Report { get; set; }
        public bool IsFree { get; set; }
    }
}