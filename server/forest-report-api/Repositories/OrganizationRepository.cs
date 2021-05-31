using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forest_report_api.Context;
using forest_report_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        public async Task<List<Organization>> GetAll()
        {
            var context = new ApplicationDbContext();
            return await context.Organizations
                .Include(x => x.Users)
                .Include(x => x.TypeActivity)
                .ToListAsync();
        }

        public async Task<List<Organization>> GetFree()
        {
            var context = new ApplicationDbContext();
            return await context.Organizations
                .Include(x => x.Users)
                .Include(x => x.TypeActivity)
                .Where(x => !x.Users.Any())
                .ToListAsync();
        }

        public async Task<Organization> GetById(string id)
        {
            var context = new ApplicationDbContext();
            return await context.Organizations
                .Include(x => x.Users)
                .Include(x => x.TypeActivity)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Organization> GetByUserId(string userId)
        {
            var context = new ApplicationDbContext();
            return (await context.Users
                .Include(x => x.Organization)
                .FirstOrDefaultAsync(x => x.Id == userId))?.Organization;
        }

        public async Task<int> Save(Organization model)
        {
            var context = new ApplicationDbContext();

            if (model.IsNew)
            {
                model.Id = Guid.NewGuid().ToString();
                await context.Organizations.AddAsync(model);
            }
            else
                context.Organizations.Update(model);

            return await context.SaveChangesAsync();
        }

        public async Task<int> Remove(string id)
        {
            var context = new ApplicationDbContext();
            var item = await GetById(id);
            context.Organizations.Remove(item);
            return await context.SaveChangesAsync();
        }

        public async Task<int> SaveGeneralInfo(Organization model)
        {
            var context = new ApplicationDbContext();

            var oldModel = await context.Organizations.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (oldModel != null)
            {
                oldModel.UNP = model.UNP;
                oldModel.TypeEconomicActivity = model.TypeEconomicActivity;
                oldModel.OrganizationalLegalForm = model.OrganizationalLegalForm;
                oldModel.GovermentForReport = model.GovermentForReport;
                oldModel.UnitForReport = model.UnitForReport;
                oldModel.Address = model.Address;
                oldModel.Position1 = model.Position1;
                oldModel.FullName1 = model.FullName1;
                oldModel.Position2 = model.Position2;
                oldModel.FullName2 = model.FullName2;
                 
                context.Organizations.Update(oldModel);
            }

            return await context.SaveChangesAsync();
        }
    }
}