﻿using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XamarinMemeGenerator
{
    public static class WantSomeMemesNowClass
    {

        public async static Task<ObservableCollection<string>> ShowMeThoseMemes()
        {

            var client = new HttpClient();

            //headers required to call the service (API key and Accept type)
            client.DefaultRequestHeaders.Add("X-Mashape-Key", "nods3vHgv8mshbh740g4cJQgBzTTp1Xukb0jsnW6aLwBRpjIpq");
            client.DefaultRequestHeaders.Add("Accept", "text/plain");

            //Actually calls the service and returns a json string
            string response = await client.GetStringAsync("https://ronreiter-meme-generator.p.mashape.com/images");

            //converts json string to na ObservableCollection of strings
            return JsonConvert.DeserializeObject<ObservableCollection<string>>(response);

        }

        public async static Task<byte[]> GenerateMyMeme(string meme, string topText, string bottomText)
        {

            //This Meme Generator Api has a problem with non-ascii chars, so we strip them just to avoid it crashing.
            bottomText = Regex.Replace(bottomText, @"[^\u0000-\u007F]", string.Empty);
            topText = Regex.Replace(topText, @"[^\u0000-\u007F]", string.Empty);

            var client = new HttpClient();

            //headers required to call the service (API key and Accept type)
            client.DefaultRequestHeaders.Add("X-Mashape-Key", "nods3vHgv8mshbh740g4cJQgBzTTp1Xukb0jsnW6aLwBRpjIpq");

            //Actually calls the service and returns a byte array for the image
            return await client.GetByteArrayAsync("https://ronreiter-meme-generator.p.mashape.com/meme?bottom=" + bottomText + "&meme=" + meme + "&top=" + topText);

        }
    }
}
