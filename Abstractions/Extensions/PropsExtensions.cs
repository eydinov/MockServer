using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MockServer.Environment.Extensions
{
    public static class PropsExtensions
    {
        public static string GetString(this IDictionary<string, string> props, string key)
        {
            return props.GetValueByKey<string>(key);
        }

        public static T GetValueByKey<T>(this IDictionary<string, string> props, string key)
        {
            var item = props.FirstOrDefault(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Value;
            return item != null ? (T)Convert.ChangeType(item, typeof(T)) : default;
        }

        public static string SubstituteProps(this IDictionary<string, string> props, string property)
        {
            if (props != null)
            {
                foreach (var prop in props)
                {
                    property = property.Replace("{" + prop.Key + "}", JsonEncodedText.Encode(prop.Value).ToString());
                }
            }

            return property;
        }

    }
}
