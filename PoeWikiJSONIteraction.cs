using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Resx = PoeMapFIlter.resources.lang.Resources;


namespace PoeMapFilter
{
    class PoeWikiJSONIteraction
    {
        public static readonly string API_STATUS_URL = "https://pathofexile.fandom.com/api.php";

        //this url gets json of all map mods
        public static string URL = Resx.PoeWikiMapModsURL;

        public bool CheckConnectionWithAPI()
        {
            bool HasConnection = false;
            try
            {
                using (MyWebClient wc = new())
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
                using (MyWebClient wc = new())
                {
                    string jsonStr = wc.DownloadString(url);
                    return JObject.Parse(jsonStr);
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

                        var Mods = e["title"]["stat text"]
                                    .ToString()
                                    .Split(';');

                        e = Mods[Mods.Length - 5].Split("&")[0];
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

                    e = ReplaceWords(e);

                    if (e.Contains("[")) e = e.Replace("[", "");
                    if (e.Contains("]")) e = e.Replace("]", "");

                    e = ReplaceDigits(e);

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

        public static string ReplaceDigits(string arg)
        {

            var regexDigits = new Regex(@"(\([0-9]{1,3}-[0-9]{1,3}\))|([0-9]{1,3})");

            if (regexDigits.IsMatch(arg)) arg = regexDigits.Replace(arg, "#");

            return arg;
        }

        public static string ReplaceWords(string arg)
        {

            var regexWords = new Regex(@"\[\[.*\|");

            if (regexWords.IsMatch(arg)) arg = regexWords.Replace(arg, "");

            return arg;
        }
    }
}
