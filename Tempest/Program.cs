using System;
using System.IO;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;

namespace Tempest
{
    class Program
    {
        private static SqlConnection con1;
        private static string WeatherToken = "a307d230-391e-4b32-a807-80b15072982e";
        private static string StationID = "54207";
        public class current
        {
            public DateTime time { get; set; }
            public string conditions { get; set; }
            public string icon { get; set; }
            public int air_temperature { get; set; }
            public decimal sea_level_pressure { get; set; }
            public decimal station_pressure { get; set; }
            public string pressure_trend { get; set; }
            public int relative_humidity { get; set; }
            public int wind_avg { get; set; }
            public int wind_direction { get; set; }
            public string wind_direction_cardinal { get; set; }
            public string wind_gust { get; set; }
            public int solar_radiation { get; set; }
            public int uv { get; set; }
            public string brightness { get; set; }
            public int feels_like { get; set; }
            public int lightning_strike_count_last_1hr { get; set; }
            public int lightning_strike_count_last_3hr { get; set; }
            public string lightning_strike_last_distance { get; set; }
            public string lightning_strike_last_distance_msg { get; set; }
            public DateTime lightning_strike_last_epoch { get; set; }
            public decimal precip_accum_local_day { get; set; }
            public decimal precip_accum_local_yesterday { get; set; }
            public int precip_minutes_local_day { get; set; }
            public int precip_minutes_local_yesterday { get; set; }
            public string is_precip_local_day_rain_check { get; set; }
            public string is_precip_local_yesterday_rain_check { get; set; }
        }
        //public static DateTime? Epoch(string unixTimeStamp)
        //{
        //    //int dt1 = Convert.Toint(unixTimeStamp);
        //    //dt1 = dt1 - 18000;
        //    //unixTimeStamp = Convert.ToString(dt1);

            
        //    return new DateTime(1970, 1, 1).AddSeconds(Convert.ToDouble(unixTimeStamp));
        //}

        static SqlConnection SQL1()
        /*
         * Set up a connection to Database 1
         */
        {
            var localcon = new SqlConnection();
            //localcon.ConnectionString = "Data Source=sqldb;Initial Catalog=DFWeather;user id=markm;Password=Demopass@word1";
            localcon.ConnectionString = "Data Source=SQLDB;Initial Catalog=DFWeather;Integrated Security = True";
            localcon.Open();

            return localcon;
        }
        static void Main(string[] args)
        {
            con1 = SQL1();

            while(true)
            {

                try
                {

                    var url = "https://swd.weatherflow.com/swd/rest/better_forecast?station_id=" + StationID + "&units_temp=f&units_wind=mph&units_pressure=mmhg&units_precip=in&units_distance=mi&token=" + WeatherToken;

                    var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                    httpRequest.Accept = "application/json";


                    var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        JToken token1 = JToken.Parse(result);
                        JObject obs = (JObject)token1.SelectToken("current_conditions");

                        //DateTime etime = Convert.ToDateTime(Epoch(Convert.ToString(obs["time"])));

                        current record = new current
                        {
                            time = DateTime.Now,
                            conditions = Convert.ToString(obs["conditions"]),
                            icon = Convert.ToString(obs["icon"]),
                            air_temperature = Convert.ToInt16(obs["air_temperature"]),
                            sea_level_pressure = Convert.ToDecimal(obs["sea_level_pressure"]),
                            station_pressure = Convert.ToDecimal(obs["station_pressure"]),
                            pressure_trend = Convert.ToString(obs["pressure_trend"]),
                            relative_humidity = Convert.ToInt16(obs["relative_humidity"]),
                            wind_avg = Convert.ToInt16(obs["wind_avg"]),
                            wind_direction = Convert.ToInt16(obs["wind_direction"]),
                            wind_direction_cardinal = Convert.ToString(obs["wind_direction_cardinal"]),
                            wind_gust = Convert.ToString(obs["wind_gust"]),
                            solar_radiation = Convert.ToInt16(obs["solar_radiation"]),
                            uv = Convert.ToInt16(obs["uv"]),
                            brightness = Convert.ToString(obs["brightness"]),
                            feels_like = Convert.ToInt16(obs["feels_like"]),
                            lightning_strike_count_last_1hr = Convert.ToInt16(obs["lightning_strike_count_last_1hr"]),
                            lightning_strike_count_last_3hr = Convert.ToInt16(obs["lightning_strike_count_last_3hr"]),
                            lightning_strike_last_distance = Convert.ToString(obs["lightning_strike_last_distance"]),
                            lightning_strike_last_distance_msg = Convert.ToString(obs["lightning_strike_last_distance_msg"]),
                            lightning_strike_last_epoch = DateTime.Now,
                            //lightning_strike_last_epoch = Convert.ToDateTime(Epoch(Convert.ToString(obs["lightning_strike_last_epoch"]))),
                            precip_accum_local_day = Convert.ToDecimal(obs["precip_accum_local_day"]),
                            precip_accum_local_yesterday = Convert.ToDecimal(obs["precip_accum_local_yesterday"]),
                            precip_minutes_local_day = Convert.ToInt16(obs["precip_minutes_local_day"]),
                            precip_minutes_local_yesterday = Convert.ToInt16(obs["precip_minutes_local_yesterday"]),
                            is_precip_local_day_rain_check = Convert.ToString(obs["is_precip_local_day_rain_check"]),
                            is_precip_local_yesterday_rain_check = Convert.ToString(obs["is_precip_local_yesterday_rain_check"])

                        };
                        SqlCommand cmd1 = new SqlCommand();
                        cmd1.Connection = con1;
                        try
                        {
                            con1.Open();
                        }
                        catch { }
                        cmd1.CommandText = "INSERT INTO Historical(time, conditions, icon, air_temperature, sea_level_pressure, station_pressure, pressure_trend, relative_humidity, wind_avg, wind_direction, wind_direction_cardinal, wind_gust, solar_radiation, uv, brightness, feels_like, lightning_strike_count_last_1hr, lightning_strike_count_last_3hr, lightning_strike_last_distance, lightning_strike_last_distance_msg, lightning_strike_last_epoch, precip_accum_local_day, precip_accum_local_yesterday, precip_minutes_local_day, precip_minutes_local_yesterday, is_precip_local_day_rain_check, is_precip_local_yesterday_rain_check) VALUES(@param1,@param2,@param3,@param4,@param5,@param6,@param7,@param8,@param9,@param10,@param11,@param12,@param13,@param14,@param15,@param16,@param17,@param18,@param19,@param20,@param21,@param22,@param23,@param24,@param25,@param26,@param27)";

                        cmd1.Parameters.AddWithValue("@param1", DateTime.Now);
                        cmd1.Parameters.AddWithValue("@param2", record.conditions);
                        cmd1.Parameters.AddWithValue("@param3", record.icon);
                        cmd1.Parameters.AddWithValue("@param4", record.air_temperature);
                        cmd1.Parameters.AddWithValue("@param5", record.sea_level_pressure);
                        cmd1.Parameters.AddWithValue("@param6", record.station_pressure);
                        cmd1.Parameters.AddWithValue("@param7", record.pressure_trend);
                        cmd1.Parameters.AddWithValue("@param8", record.relative_humidity);
                        cmd1.Parameters.AddWithValue("@param9", record.wind_avg);
                        cmd1.Parameters.AddWithValue("@param10", record.wind_direction);
                        cmd1.Parameters.AddWithValue("@param11", record.wind_direction_cardinal);
                        cmd1.Parameters.AddWithValue("@param12", record.wind_gust);
                        cmd1.Parameters.AddWithValue("@param13", record.solar_radiation);
                        cmd1.Parameters.AddWithValue("@param14", record.uv);
                        cmd1.Parameters.AddWithValue("@param15", record.brightness);
                        cmd1.Parameters.AddWithValue("@param16", record.feels_like);
                        cmd1.Parameters.AddWithValue("@param17", record.lightning_strike_count_last_1hr);
                        cmd1.Parameters.AddWithValue("@param18", record.lightning_strike_count_last_3hr);
                        cmd1.Parameters.AddWithValue("@param19", record.lightning_strike_last_distance);
                        cmd1.Parameters.AddWithValue("@param20", record.lightning_strike_last_distance_msg);
                        cmd1.Parameters.AddWithValue("@param21", record.lightning_strike_last_epoch);
                        cmd1.Parameters.AddWithValue("@param22", record.precip_accum_local_day);
                        cmd1.Parameters.AddWithValue("@param23", record.precip_accum_local_yesterday);
                        cmd1.Parameters.AddWithValue("@param24", record.precip_minutes_local_day);
                        cmd1.Parameters.AddWithValue("@param25", record.precip_minutes_local_yesterday);
                        cmd1.Parameters.AddWithValue("@param26", record.is_precip_local_day_rain_check);
                        cmd1.Parameters.AddWithValue("@param27", record.is_precip_local_yesterday_rain_check);

                        //Console.WriteLine(DateTime.Now);
                        //Console.WriteLine("Weather Condition: "+record.conditions);
                        //Console.WriteLine("Icon: "+record.icon);
                        //Console.WriteLine("Temp: "+record.air_temperature);
                        ////Console.WriteLine(record.sea_level_pressure);
                        //Console.WriteLine("Pressure: "+record.station_pressure);
                        ////Console.WriteLine(record.pressure_trend);
                        //Console.WriteLine("Humidity: "+record.relative_humidity);
                        //Console.WriteLine("Avg Wind Speed: "+record.wind_avg);
                        ////Console.WriteLine(record.wind_direction);
                        //Console.WriteLine("Wind Direction: "+record.wind_direction_cardinal);
                        //Console.WriteLine("Wind Gust: "+record.wind_gust);
                        ////Console.WriteLine(record.solar_radiation);
                        //Console.WriteLine("UV: "+record.uv);
                        ////Console.WriteLine(record.brightness);
                        ////Console.WriteLine(record.feels_like);
                        ////Console.WriteLine(record.lightning_strike_count_last_1hr);
                        ////Console.WriteLine(record.lightning_strike_count_last_3hr);
                        ////Console.WriteLine(record.lightning_strike_last_distance);
                        ////Console.WriteLine(record.lightning_strike_last_distance_msg);
                        ////Console.WriteLine(record.lighting_strike_last_epoch);
                        //Console.WriteLine("Rain Today: "+record.precip_accum_local_day);
                        //Console.WriteLine("Rain Yesterday: "+record.precip_accum_local_yesterday);
                        //Console.WriteLine("Rain Minutes Today: "+record.precip_minutes_local_day);
                        ////Console.WriteLine(record.precip_minutes_local_yesterday);
                        ////Console.WriteLine(record.is_precip_local_day_rain_check);
                        ////Console.WriteLine(record.is_precip_local_yesterday_rain_check);

                        Console.WriteLine(DateTime.Now + "| Temp: " + record.air_temperature + "  |  Humidity: " + record.relative_humidity + "  |  Pressure: " + record.station_pressure + "  | Rain Today: " + record.precip_accum_local_day);

                        try
                        {
                            cmd1.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            //DoHeaders(0);
                            Console.WriteLine(DateTime.Now + ": " + "Connection timeout retrying..." + ex);
                            con1.Close();
                            con1.Open();
                            cmd1.ExecuteNonQuery();
                        }

                        //Console.WriteLine(Epoch(Convert.ToString(record.time)));
                        //Console.WriteLine(record.air_temperature);
                        //Console.WriteLine(record.relative_humidity);

                        Thread.Sleep(300000);

                    }

                }
                catch { }
            }
            

            //Console.WriteLine(httpResponse.StatusCode);
        }
    }
}
