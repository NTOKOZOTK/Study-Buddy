using Study_Buddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Study_Buddy.ViewModels;
using PagedList;
using PagedList.Mvc;
using System.Web.Script.Serialization;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;


namespace Study_Buddy.Controllers
{
    public class HomeController : ApplicationBaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        // one day per city forecast
        [HttpPost]
        public String WeatherDetail(string City)
        {

            //Assign API KEY which received from OPENWEATHERMAP.ORG
            string appId = "8113fcc5a7494b0518bd91ef3acc074f";

            //API path with CITY parameter and other parameters.
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&cnt=1&APPID={1}", City, appId);
            //string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q=Durban,ZA&units=metric&cnt=1&APPID={1}", City, appId);

            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);


                //Converting to OBJECT from JSON string.
                RootObject weatherInfo = new JavaScriptSerializer().Deserialize<RootObject>(json);

                //Special VIEWMODEL design to send only required fields not all fields which received from 
                //www.openweathermap.org api
                ResultViewModel rslt = new ResultViewModel();

                rslt.Country = weatherInfo.sys.country;
                rslt.City = weatherInfo.name;
                rslt.SearchDate = DateTime.Now;
                rslt.Lat = Convert.ToString(weatherInfo.coord.lat);
                rslt.Lon = Convert.ToString(weatherInfo.coord.lon);
                rslt.Description = weatherInfo.weather[0].description;
                rslt.Humidity = Convert.ToString(weatherInfo.main.humidity);
                rslt.Temp = Convert.ToString(weatherInfo.main.temp);
                rslt.TempFeelsLike = Convert.ToString(weatherInfo.main.feels_like);
                rslt.TempMax = Convert.ToString(weatherInfo.main.temp_max);
                rslt.TempMin = Convert.ToString(weatherInfo.main.temp_min);
                rslt.WeatherIcon = weatherInfo.weather[0].icon;

                //Converting OBJECT to JSON String 
                var jsonstring = new JavaScriptSerializer().Serialize(rslt);

                //Return JSON string.
                return jsonstring;
            }
        }

        // 5 days weather forecast

        //protected string _apikey;
        // GET: Forecast
        public async Task<ActionResult> Weather(string txtCity)
        {

            string _apikey = "8113fcc5a7494b0518bd91ef3acc074f";


            HttpResponseMessage httpResponse = null;
            using (var Client = new HttpClient())
            {
                try
                {
                    Client.BaseAddress = new Uri("http://api.openweathermap.org");
                    if (txtCity != null && txtCity != "")
                    {
                        //httpResponse = await Client.GetAsync($"/data/2.5/forecast?q={txtCity}&units=metric&cnt=1&APPID={appId}");
                        httpResponse = await Client.GetAsync($"/data/2.5/forecast?q={txtCity}&appid={_apikey}&units=metric");
                        //http://api.openweathermap.org/data/2.5/forecast/daily?q=Melbourne,AU&cnt=5&&appid=2e0bfcd75d67a0452c6890c445c51030&units=metric
                    }
                    else
                    {
                        httpResponse = await Client.GetAsync($"/data/2.5/forecast?q=Durban,ZA&appid={_apikey}&units=metric");
                        //httpResponse = await Client.GetAsync($"/data/2.5/forecast?q={txtCity}&units=metric&cnt=1&APPID={appId}");
                    }

                    httpResponse.EnsureSuccessStatusCode();
                    var stringResult = await httpResponse.Content.ReadAsStringAsync();
                    ForecastModel rawWeather = new ForecastModel();
                    rawWeather = JsonConvert.DeserializeObject<ForecastModel>(stringResult);
                    return View(rawWeather);
                }
                catch (HttpRequestException httpRequestException)
                {
                    return View($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }

            }
        }
    }
}