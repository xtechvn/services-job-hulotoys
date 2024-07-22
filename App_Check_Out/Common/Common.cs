using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities.Contants;

namespace APP.CHECKOUT_SERVICE.Common
{
    public static class Common
    {
        public static List<T> ToList<T>(this DataTable data) where T : new()
        {
            List<T> dtReturn = new List<T>();
            if (data == null)
                return dtReturn;

            Type typeParameterType = typeof(T);

            var props = typeParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetGetMethod() != null);

            foreach (DataRow item in data.AsEnumerable())
            {
                dtReturn.Add(GetValueT<T>(item, props));
            }
            return dtReturn;
        }

        public static List<T> ToListBasic<T>(this DataTable data)
        {
            List<T> dtReturn = new List<T>();
            if (data == null)
                return dtReturn;
            Type typeParameterType = typeof(T);
            foreach (DataRow item in data.AsEnumerable())
            {
                dtReturn.Add((T)Convert.ChangeType(item[0], typeParameterType));
            }
            return dtReturn;
        }

        private static T GetValueT<T>(DataRow row, IEnumerable<PropertyInfo> props) where T : new()
        {
            T objRow = new T();
            foreach (var field in props)
            {
                string fieldName = field.Name;
                var columnName = field.CustomAttributes.FirstOrDefault(x => x.AttributeType.UnderlyingSystemType.Name == "ColumnAttribute");
                if (columnName != null)
                    fieldName = columnName.ConstructorArguments[0].Value.ToString();
                if (row.Table.Columns.Contains(fieldName) && row[fieldName] != DBNull.Value)
                {
                    Type t = Nullable.GetUnderlyingType(field.PropertyType) ?? field.PropertyType;
                    if (field.PropertyType.IsValueType)
                        field.SetValue(objRow, Convert.ChangeType(row[fieldName] == DBNull.Value ? Activator.CreateInstance(t) : row[fieldName], t));
                    else
                    {
                        field.SetValue(objRow, Convert.ChangeType(row[fieldName], t));
                    }
                }
            }
            return objRow;
        }
        private static double GetWeightFromString(string weight_str)
        {
            double value = 0;
            try
            {
                List<string> regex = new List<string>() { "(?<=.\\s)(.*)(?=kgs)", "(?<=.\\s)(.*)(?=kg)" };
                foreach(var r in regex)
                {
                    Regex expression = new Regex(r);
                    var results = expression.Matches(weight_str.ToLower());
                    foreach (Match match in results)
                    {
                        try
                        {
                            value = Convert.ToDouble(match.Groups[1].Value);
                        }catch { continue; }
                    }
                }
            }catch(Exception )
            {

            }
            return value;
        }
        public static short GetSystemTypeByOrderNo(string order_no)
        {
            short systemType = (int)SystemType.B2C;
            try
            {
                switch (order_no.Substring(0, 1).Trim())
                {

                    case "B":
                        {
                            systemType = (int)SystemType.B2B;
                        }
                        break;
                    case "C":
                        {
                            systemType = (int)SystemType.B2C;
                        }
                        break;
                    case "O":
                        {
                            systemType = (int)SystemType.BACKEND;
                        }
                        break;
                    default:
                        {
                            systemType = (int)SystemType.B2C;
                        }
                        break;
                }
            }
            catch (Exception)
            {

            }
            return systemType;
        }
       
    }
}
