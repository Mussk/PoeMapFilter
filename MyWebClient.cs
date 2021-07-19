using System;
using System.Net;

namespace PoeMapFilter
{
    public class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 3 * 1000;
            return w;
        }
    }
}
