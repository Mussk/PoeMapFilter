using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Resx = PoeMapFIlter.resources.lang.Resources;


namespace PoeMapFilter
{
    class PoeWikiJSONIteraction
    {

        public static string API_STATUS_URL = "https://pathofexile.fandom.com/api.php";

        //this url gets json of all map mods
        public static string URL = Resx.PoeWikiMapModsURL;

        public bool CheckConnectionWithAPI()
        {
            bool HasConnection = false;
            try
            {
                using (MyWebClient wc = new MyWebClient())
                {
                    using (var stream = wc.OpenRead(API_STATUS_URL))
                    {
                        if (stream is not null)
                        {
                          HasConnection = true;
                        }
                            
                    }
                }
            }
            catch (WebException)
            {
                HasConnection = false;
            }
            return HasConnection;
        }
        public JObject GetJsonFromUrl(string url)
        {
            if (CheckConnectionWithAPI())
            {
                string json_str = "";

                using (MyWebClient wc = new MyWebClient())
                {
                    json_str = wc.DownloadString(url);
                    return JObject.Parse(json_str);
                }
            }
            else return default;   
        }
        public List<string> ParseJsonMods(JObject jObject)
        {
            if (jObject is null)
                return default;

            List<string> fullMods = jObject["cargoquery"]
                .Select(e =>
                {
                    if (e["title"]["stat text"].ToString().Contains(Resx.JsonParseHiddenString))
                    {

                        var arr = e["title"]["stat text"]
                                    .ToString()
                                    .Split(';');

                        e = arr[arr.Length - 5].Split("&")[0];
                    }
                    else 
                    {
                        e = e["title"]["stat text"]
                              .ToString();
                    }
                    
                    return e.ToString();
                }).ToList()
                .Select(e =>
                {

                    e = Replace_words(e);

                    if (e.Contains("[")) e = e.Replace("[", "");
                    if (e.Contains("]")) e = e.Replace("]", "");

                    e = Replace_digits(e);

                    return e;

                }).ToList();

            List<string> result = new();

            foreach (var el in fullMods) 
                result.AddRange(el.Split(";br&gt;"));

            fullMods.Clear();

            result = result.Distinct().ToList();

            result = result.Select(e => e.Split("&")[0]).ToList();

            return result;
        }

        public static string Replace_digits(string arg)
        {

            var regex_digits = new Regex(@"(\([0-9]{1,3}-[0-9]{1,3}\))|([0-9]{1,3})");

            if (regex_digits.IsMatch(arg)) arg = regex_digits.Replace(arg, "#");

            return arg;
        }

        public static string Replace_words(string arg)
        {

            var regex_words = new Regex(@"\[\[.*\|");

            if (regex_words.IsMatch(arg)) arg = regex_words.Replace(arg, "");

            return arg;
        }
    }
}
