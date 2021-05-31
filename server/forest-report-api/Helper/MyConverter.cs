using System;
using System.Collections.Generic;
using System.Linq;
using forest_report_api.Models;
using Newtonsoft.Json.Linq;

namespace forest_report_api.Helper
{
    public static class MyConverter
    {
        public static T ConvertToClass<T>(object obj)
        {
            return ((JObject) obj).ToObject<T>();
        }

        public static object ConvertToClass(object obj,Type t)
        {
            return obj.GetType().FullName == t.FullName ? obj : ((JObject) obj).ToObject(t);
        }
        
        public static IEnumerable<T> ConvertToClass<T>(IEnumerable<object> objectList)
        {
            return from JObject dynamicRow in objectList select dynamicRow.ToObject<T>();
        }
    }
}