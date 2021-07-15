using System.Text.Json;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;

namespace PoeBadMapsMod
{
    public class Serialization<T>
    {
        public static void Serialize(T objectToSerialize, string fileName) {
 
            string jsonString = JsonConvert.SerializeObject(objectToSerialize);

            File.WriteAllText(fileName, jsonString);

        }

        public static void Serialize(T objectToSerialize, string fileName, CultureInfo cultureInfo)
        {
            fileName = fileName.Split(".")[0] + "_" + cultureInfo.EnglishName + ".json";

            string jsonString = JsonConvert.SerializeObject(objectToSerialize);

            File.WriteAllText(fileName, jsonString);

        }

        public static T Deserialize<T>(string fileName) where T : new()
        {

            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);

                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            else return new T();
        }

        public static T Deserialize<T>(string fileName, CultureInfo cultureInfo) where T : new()
        {
            fileName = fileName.Split(".")[0] + "_" + cultureInfo.EnglishName + ".json";

            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);

                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            else return new T();
        }
    }
}
