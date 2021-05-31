using System;
using System.Collections.Generic;
using System.Dynamic;
using forest_report_api.Context;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Repositories
{
    public class EntityRepository : IEntityRepository
    {
        public object Execute(string query)
        {
            var context = new ApplicationDbContext();
            using var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.Connection?.Open();
            var reader = command.ExecuteReader();
            var result = new List<object>();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var row = new ExpandoObject() as IDictionary<string, object>;
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader.GetValue(i));
                        }

                        result.Add(row);
                    }
                }
                else
                {   
                    return new List<object>();
                }
            }
            catch (Exception e)
            {
                // ignored
            }
            finally
            {
                reader.Close();
            }
            
            return result;
        }
    }

    public interface IEntityRepository
    {
        object Execute(string query);
    }
}