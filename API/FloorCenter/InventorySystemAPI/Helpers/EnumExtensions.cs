using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Helpers
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
                    Name = Enum.GetName(typeof(T), itemType),
                    Value = (int)itemType
                });
            }
            return values;
        }

        public class EnumValue
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

    }
}
