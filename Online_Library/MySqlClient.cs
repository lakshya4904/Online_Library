using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Online_Library
{
    internal class MySqlClient
    {
        public static string ClassToSqlCommand(Type classType)
        {
            // Use reflection to inspect properties and create SQL INSERT statement
            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(classType.Name);
            sql.Append(" (");

            // Iterate through properties, extracting names and values
            PropertyInfo[] properties = classType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                sql.Append(property.Name);
                sql.Append(",");
            }

            // Remove trailing comma and add values section
            sql.Remove(sql.Length - 1, 1);
            sql.Append(") VALUES (");

            // Generate placeholders for values
            foreach (PropertyInfo property in properties)
            {
                sql.Append("@");
                sql.Append(property.Name);
                sql.Append(",");
            }

            // Remove trailing comma and close statement
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");

            //INSERT INTO `online_library`.`student` (`Id`, `Name`, `Age`, `Standard`) VALUES ('1', 'a', '12', 'a');
            var a = sql.ToString();
            return sql.ToString();
        }

    }
}
