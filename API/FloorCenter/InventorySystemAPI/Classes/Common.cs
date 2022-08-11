using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySystemAPI.Classes
{
    public class Common
    {

        public string[] dateToBeFiltered { get; set; }


        /// <summary>
        /// Get the properties of the model that has a value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Dictionary<string, object> getModelPropertyWithValueAsDictionary(object obj)
        {
            return obj.GetType().GetProperties().ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null)).Where(x => x.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value);
        }



        /// <summary>
        /// Filters the records on the context during where in LINQ - [FOR LIKE SEARCHING]
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public bool filterModelLike(object obj, Dictionary<string, object> criteria)
        {
            // Convert the model to Dictionary<string, object>
            var data = obj.GetType().GetProperties().ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));

            // Iterate the criteria dictionary
            foreach (var c in criteria)
            {
                if (data.ContainsKey(c.Key))
                {
                    // Current filtering is for string and number only.
                    if (data[c.Key].GetType() != typeof(DateTime))
                    {
                        // Check the Current record in the list if the value contains the criteria.
                        //
                        // data[c.Key].ToString()        - get the data of the model base on the key of the criteria.
                        // .Contains(c.Value.ToString()) - checks the value of the model if it contains the value of the criteria.
                        if (!data[c.Key].ToString().Contains(c.Value.ToString()))
                        {
                            // Returns false once the value of the model do not contains the criteria value.
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < dateToBeFiltered.Count(); i++)
                    {
                        if (c.Key == "DateFrom")
                        {
                            if ( Convert.ToDateTime(c.Value) <= Convert.ToDateTime(data[dateToBeFiltered[i]]) )
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            } 
                        }
                        else if (c.Key == "DateTo")
                        {
                            if (Convert.ToDateTime(c.Value) >= Convert.ToDateTime(data[dateToBeFiltered[i]]))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            // Returns the model if criteria value meets the values of the model.
            return true;

        }

        /// <summary>
        /// Filters the records on the context during where in LINQ - [FOR EQUAL SEARCHING]
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public bool filterModelEqual(object obj, Dictionary<string, object> criteria)
        {

            // Convert the model to Dictionary<string, object>
            var data = obj.GetType().GetProperties().ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));

            // Iterate the criteria dictionary
            foreach (var c in criteria)
            {

                // Current filtering is for string and number only.
                if (data[c.Key].GetType() != typeof(DateTime))
                {
                    // Check the Current record in the list if the value contains the criteria.
                    //
                    // data[c.Key].ToString()        - get the data of the model base on the key of the criteria.
                    if (data[c.Key].ToString() != c.Value.ToString())
                    {
                        // Returns false once the value of the model do not contains the criteria value.
                        return false;
                    }
                }
                else
                {
                    if (c.Key == "DateFrom")
                    {
                        if (Convert.ToDateTime(data["DateFrom"]) < Convert.ToDateTime(c.Value))
                        {
                            return false;
                        }
                    }
                    else if (c.Key == "DateTo")
                    {
                        if (Convert.ToDateTime(data["DateTo"]) > Convert.ToDateTime(c.Value))
                        {
                            return false;
                        }
                    }
                }


            }

            // Returns the model if criteria value meets the values of the model.
            return true;

        }




    }
}
