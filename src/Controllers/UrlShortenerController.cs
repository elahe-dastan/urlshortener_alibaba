using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using urlshortener.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace urlshortener.Controllers
{
    [ApiController]
    [Route("/")]
    public class UrlShortenerController : ControllerBase
    {
        private int shortUrlLength = 8;
        public IConfiguration Configuration { get; set; }
        private readonly LongUrlContext context;
        public UrlShortenerController (IConfiguration config) {
            Configuration = config;
            context = GetContext();
        }

        // Summary:
        //     Post method which gets an instance of LongUrl class and first checks whether
        //     the given url matches the expected format of a url or not if that was true it
        //     makes a request to the url and if the url is not valid returns 404 if it passes
        //     validation then tries to save the url in the database and gets the id of the 
        //     inserted row back then finds a unique short url based on the unique id.
        //
        // Parameters:
        //   LongUrl:
        //     a model for long url.
        //
        // Returns:
        //     If the long url is valid returns 201 status code and short url in the location 
        //     header and the long url itself in the body if not returns 404 status code. 
        [HttpPost("urls")]
        public ActionResult<LongUrl> PostLongUrl(LongUrl longUrl) {
            if (!CheckUrl(longUrl.Url)) {
                return NotFound();
            }
            if (!CallUrl(longUrl.Url)) {
                return NotFound();
            }
            
            int id =  SaveToDatabase(longUrl).Result;
            string shortUrl = IdToShortUrl(id);

            return CreatedAtAction(nameof(ShortUrlToLongUrl), new {shortUrl = shortUrl}, longUrl);
        }

        // Summary:
        //     Get method which gets a short url specified inside the url and converts the  
        //     short url to id of the long url related to it then gets the long url from 
        //     database
        //
        // Parameters:
        //   short url
        //
        // Returns:
        //     If the short url is valid and present in the database returns 200 status code and 
        //     redirects to the main url otherwise returns 404
        [HttpGet("redirect/{*shortUrl}")]
        [ProducesResponseType(typeof(LongUrl), 201)]
        public IActionResult RedirectToLongUrl(string shortUrl)
        {
            int id = ShortUrlToId(shortUrl);

            if (id == 0) {
                return NotFound();
            }

            LongUrl longUrl = RetriveFromDatabase(id).Result;

            if (longUrl == null) {
                return NotFound();
            }

            return Ok(Redirect(longUrl.Url));
        }

        [HttpGet("long/{*shortUrl}")]
        [ProducesResponseType(typeof(LongUrl), 201)]
        public IActionResult ShortUrlToLongUrl(string shortUrl)
        {
            int id = ShortUrlToId(shortUrl);

            if (id == 0) {
                return NotFound();
            }

            LongUrl longUrl = RetriveFromDatabase(id).Result;

            if (longUrl == null) {
                return NotFound();
            }
            return Ok(longUrl);
        }

        // Summary:
        //     Checks if the url matches the expected format of a url
        public bool CheckUrl(string url) {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) 
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        // Summary:
        //     Requests to the given url and returns false if cannot get any response back
        private bool CallUrl(string url)  
        {   
            WebRequest request = HttpWebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                return true;
            } 
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            } 
              
            // StreamReader reader = new StreamReader(response.GetResponseStream());  
            // string urlText = reader.ReadToEnd(); // it takes the response from your url. now you can use as your need  
            // Console.WriteLine(urlText.ToString());  
        } 

        private async Task<int> SaveToDatabase(LongUrl longUrl) {
            context.Add(longUrl);
            await context.SaveChangesAsync();
            return longUrl.Id;
        }


        // Summary:
        //     Initializes a new instance of the LongUrlContext class and configure given 
        //     Microsoft.EntityFrameworkCore.DbContextOptions to specify the connection string.
        private LongUrlContext GetContext() {
            var optionsBuilder = new DbContextOptionsBuilder<LongUrlContext>();
            optionsBuilder.UseNpgsql(Configuration["ConnectionStrings:urlshortener"]);
            LongUrlContext context = new LongUrlContext(optionsBuilder.Options);
            return context;
        }

        private string IdToShortUrl(int n) {
            char[] map = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXY".ToCharArray();

            string shorturl = "";
  
            // Convert given integer id to a base 62 number 
            while (n != 0) 
            { 
                // use above map to store actual character 
                // in short url 
                shorturl += map[n%51]; 
                n = n/51; 
            } 
  
            // Reverse shortURL to complete base conversion 
            char[] charArray = shorturl.ToCharArray();
            Array.Reverse(charArray);

            shorturl = new string(charArray);
            shorturl = ExpandUrlLength(shorturl);
            return shorturl;
        }

        private string ExpandUrlLength(string url)
        {
            string shortUrl = "";
            int diff = shortUrlLength - url.Length;
            for (int i = 0; i < diff; i++)
            {
                shortUrl = "Z" + shortUrl;
            }

            shortUrl += url;
            return shortUrl;
        }

        private int ShortUrlToId(string shortURL) {
            if (shortURL.Length != shortUrlLength)
            {
                return 0;
            }

            shortURL = shortURL.Replace("Z", "");

            int id = 0; // initialize result 
  
            // A simple base conversion logic 
            for (int i=0; i < shortURL.Length; i++) { 
                if ('a' <= shortURL[i] && shortURL[i] <= 'z') {
                    id = id*51 + shortURL[i] - 'a'; 
                }
                else if ('A' <= shortURL[i] && shortURL[i] <= 'Z') {
                    id = id*51 + shortURL[i] - 'A' + 26;
                }
                else {
                    return 0;
                }
            } 
            return id; 
        }


        private async Task<LongUrl> RetriveFromDatabase(int id) {
            LongUrl ans = await context.longUrls.FindAsync(id);

            if (ans == null) {
                return null;
            }

            return ans;
        }
    
    }

}        