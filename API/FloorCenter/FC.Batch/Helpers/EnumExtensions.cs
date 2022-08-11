using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FC.Batch.Helpers
{
    public static class EnumExtensions
    {

        public static List<EnumValue> GetValues<T>()
        {
            List<EnumValue> values = new List<EnumValue>();
            foreach (var itemType in Enum.GetValues(typeof(T)))
            {
                //For each value of this enumeration, add a new EnumValue instance
                values.Add(new EnumValue()
                {
                    Name = SplitName(Enum.GetName(typeof(T), itemType)),
                    Value = (int)itemType
                });
            }
            return values;
        }

        public static string SplitName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var regex = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);


                return regex.Replace(name, " ");
            }

            return null;
        }

        public class EnumValue
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

    }
}
