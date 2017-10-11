//-----------------------------------------------------------------------
// <copyright file="Serializer.cs" company="Anderton Engineered Solutions">
//    Copyright Anderton Engineered Solutions. All rights reserved.
// </copyright>
//--------------------------------------------------------------

namespace Yahvol.Services
{
    using Newtonsoft.Json;

    /// <summary>
    /// Serializer for serializing the messages being persisted to the Database
    /// </summary>
    public class Serializer
    {
        public static string Serialize<T>(T input)
        {
            return JsonConvert.SerializeObject(input, Formatting.Indented);
        }

        public static T Deserializer<T>(ServiceCommand input)
        {
            return (T)JsonConvert.DeserializeObject<T>(input.SerializedCommand);
        }
    }
}