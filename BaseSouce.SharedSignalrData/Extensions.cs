using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseSouce.SharedSignalrData
{
    public static class Extensions
    {
        public static JsonSerializerOptions SignalR_JsonSerializerOptions(this JsonSerializerOptions jsonSerializerOptions)
        {
            jsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            return jsonSerializerOptions;
        }
    }
}
