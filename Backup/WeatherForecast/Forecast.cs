

using System;
using System.Net;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace WeatherForecast
{
    /// <summary>
    /// Class for holding the forecast information
    /// </summary>
    public class Forecast : INotifyPropertyChanged
    {

        #region member variables

        // name of city forecast is for
        private string cityName;
        // elevation of city
        private int height;
        // source of information
        private string credit;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        #region Accessors

        // collection of forecasts for each time period
        public ObservableCollection<ForecastPeriod> ForecastList
        {
            get;
            set;
        }

        public String CityName
        {
            get
            {
                return cityName;
            }
            set
            {
                if (value != cityName)
                {
                    cityName = value;
                    NotifyPropertyChanged("CityName");
                }
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value != height)
                {
                    height = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }

        public String Credit
        {
            get
            {
                return credit;
            }
            set
            {
                if (value != credit)
                {
                    credit = value;
                    NotifyPropertyChanged("Credit");
                }
            }
        }

        #endregion


        #region constructors

        public Forecast()
        {
            ForecastList = new ObservableCollection<ForecastPeriod>();
        }

        #endregion


        #region private Helpers

        /// <summary>
        /// Raise the PropertyChanged event and pass along the property that changed
        /// </summary>
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        /// <summary>
        /// Get a forecast for the given latitude and longitude
        /// </summary>
        public void GetForecast(string latitude, string longitude)
        {
            // form the URI
            UriBuilder fullUri = new UriBuilder("http://forecast.weather.gov/MapClick.php");
            //UriBuilder fullUri = new UriBuilder("http://www.weather.com/");
            fullUri.Query = "lat=" + latitude + "&lon=" + longitude + "&FcstType=dwml";

            // initialize a new WebRequest
            HttpWebRequest forecastRequest = (HttpWebRequest)WebRequest.Create(fullUri.Uri);

            // set up the state object for the async request
            ForecastUpdateState forecastState = new ForecastUpdateState();
            forecastState.AsyncRequest = forecastRequest;

            // start the asynchronous request
            forecastRequest.BeginGetResponse(new AsyncCallback(HandleForecastResponse),
                forecastState);
        }

        /// <summary>
        /// Handle the information returned from the async request
        /// </summary>
        /// <param name="asyncResult"></param>
        private void HandleForecastResponse(IAsyncResult asyncResult)
        {
            // get the state information
            ForecastUpdateState forecastState = (ForecastUpdateState)asyncResult.AsyncState;
            HttpWebRequest forecastRequest = (HttpWebRequest)forecastState.AsyncRequest;

            // end the async request
            forecastState.AsyncResponse = (HttpWebResponse)forecastRequest.EndGetResponse(asyncResult);

            Stream streamResult;

            string newCredit = "";
            string newCityName = "";
            int newHeight = 0;

            // create a temp collection for the new forecast information for each 
            // time period
            ObservableCollection<ForecastPeriod> newForecastList =
                new ObservableCollection<ForecastPeriod>();

            try
            {
                // get the stream containing the response from the async call
                streamResult = forecastState.AsyncResponse.GetResponseStream();

                // load the XML
                XElement xmlWeather = XElement.Load(streamResult);

                // start parsing the XML.  You can see what the XML looks like by 
              

                // find the source element and retrieve the credit information
                XElement xmlCurrent = xmlWeather.Descendants("source").First();
                newCredit = (string)(xmlCurrent.Element("credit"));

                // find the source element and retrieve the city and elevation information
                xmlCurrent = xmlWeather.Descendants("location").First();
                newCityName = (string)(xmlCurrent.Element("city"));
                newHeight = (int)(xmlCurrent.Element("height"));

                // find the first time layout element
                xmlCurrent = xmlWeather.Descendants("time-layout").First();

                int timeIndex = 1;

                // search through the time layout elements until you find a node 
                // contains at least 12 time periods of information. Other nodes can be ignored
                while (xmlCurrent.Elements("start-valid-time").Count() < 12)
                {
                    xmlCurrent = xmlWeather.Descendants("time-layout").ElementAt(timeIndex);
                    timeIndex++;
                }

                ForecastPeriod newPeriod;

             
                foreach (XElement curElement in xmlCurrent.Elements("start-valid-time"))
                {
                    try
                    {
                        newPeriod = new ForecastPeriod();
                        newPeriod.TimeName = (string)(curElement.Attribute("period-name"));
                        newForecastList.Add(newPeriod);
                    }
                    catch (FormatException)
                    {

                    }
                }

                //  reading in the weather data for each time period
                GetMinMaxTemperatures(xmlWeather, newForecastList);
                GetChancePrecipitation(xmlWeather, newForecastList);
                GetCurrentConditions(xmlWeather, newForecastList);
                GetWeatherIcon(xmlWeather, newForecastList);
                GetTextForecast(xmlWeather, newForecastList);


                // copying
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // copy forecast object over
                    Credit = newCredit;
                    Height = newHeight;
                    CityName = newCityName;

                    ForecastList.Clear();

                    // copy the list of forecast time periods over
                    foreach (ForecastPeriod forecastPeriod in newForecastList)
                    {
                        ForecastList.Add(forecastPeriod);
                    }
                });


            }
            catch (FormatException)
            {
              
                return;
            }

        }


        /// <summary>
        /// Get the minimum and maximum temperatures for all the time periods
        /// </summary>
        private void GetMinMaxTemperatures(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
        {
            XElement xmlCurrent;

            // Find the temperature parameters.   if first time period is "Tonight",
            // then the Daily Minimum Temperatures are listed first.
            // Otherwise the Daily Maximum Temperatures are listed first
            xmlCurrent = xmlWeather.Descendants("parameters").First();

            int minTemperatureIndex = 1;
            int maxTemperatureIndex = 0;

         
            if (newForecastList.ElementAt(0).TimeName == "Tonight's Minimum Temp")
            {
                minTemperatureIndex = 0;
                maxTemperatureIndex = 1;

            
                foreach (XElement curElement in xmlCurrent.Elements("temperature").
                    ElementAt(0).Elements("value"))
                {
                    newForecastList.ElementAt(minTemperatureIndex).Temperature =
                        int.Parse(curElement.Value);

                    minTemperatureIndex += 2;
                }

          
                foreach (XElement curElement in xmlCurrent.Elements("temperature").
                    ElementAt(1).Elements("value"))
                {
                    newForecastList.ElementAt(maxTemperatureIndex).Temperature =
                        int.Parse(curElement.Value);

                    maxTemperatureIndex += 2;
                }

            }
           
            else
            {
             
                foreach (XElement curElement in xmlCurrent.Elements("temperature").
                    ElementAt(0).Elements("value"))
                {
                    newForecastList.ElementAt(maxTemperatureIndex).Temperature =
                        int.Parse(curElement.Value);

                    maxTemperatureIndex += 2;
                }

               
                foreach (XElement curElement in xmlCurrent.Elements("temperature").
                    ElementAt(1).Elements("value"))
                {
                    newForecastList.ElementAt(minTemperatureIndex).Temperature =
                        int.Parse(curElement.Value);

                    minTemperatureIndex += 2;
                }
            }


        }


    
        private void GetChancePrecipitation(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
        {
            XElement xmlCurrent;

          
            xmlCurrent = xmlWeather.Descendants("probability-of-precipitation").First();

            int elementIndex = 0;

            foreach (XElement curElement in xmlCurrent.Elements("value"))
            {
                try
                {
                    newForecastList.ElementAt(elementIndex).ChancePrecipitation =
                        int.Parse(curElement.Value);
                }
             
                catch (FormatException)
                {
                    newForecastList.ElementAt(elementIndex).ChancePrecipitation = 0;
                }

                elementIndex++;
            }

        }


      
        private void GetCurrentConditions(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
        {
            XElement xmlCurrent;
            int elementIndex = 0;

         
            xmlCurrent = xmlWeather.Descendants("weather").First();

            foreach (XElement curElement in xmlCurrent.Elements("weather-conditions"))
            {
                try
                {
                    newForecastList.ElementAt(elementIndex).WeatherType =
                        (string)(curElement.Attribute("weather-summary"));
                }
                catch (FormatException)
                {
                    newForecastList.ElementAt(elementIndex).WeatherType = "";
                }

                elementIndex++;
            }


        }


      
      
        /// <summary>
        /// Get the long text forecast for all the time periods
        /// </summary>
        private void GetTextForecast(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
        {
            XElement xmlCurrent;
            int elementIndex = 0;

            // get a text forecast for each time period
            xmlCurrent = xmlWeather.Descendants("wordedForecast").First();

            foreach (XElement curElement in xmlCurrent.Elements("text"))
            {
                try
                {
                    newForecastList.ElementAt(elementIndex).TextForecast =
                        (string)(curElement.Value);
                }
                catch (FormatException)
                {
                    newForecastList.ElementAt(elementIndex).TextForecast = "";
                }

                elementIndex++;
            }

        }

        /// <param name="xmlWeather"></param>
        /// <param name="newForecastList"></param>
        private void GetWeatherIcon(XElement xmlWeather, ObservableCollection<ForecastPeriod> newForecastList)
        {
            XElement xmlCurrent;
            int elementIndex = 0;

            // get a link to the weather icon for each time period
            xmlCurrent = xmlWeather.Descendants("conditions-icon").First();

            foreach (XElement curElement in xmlCurrent.Elements("icon-link"))
            {
                try
                {
                    newForecastList.ElementAt(elementIndex).ConditionIcon =
                        (string)(curElement.Value);
                }
                catch (FormatException)
                {
                    newForecastList.ElementAt(elementIndex).ConditionIcon = "";
                }

                elementIndex++;
            }

        }



    }

    /// <summary>
    /// State information for our BeginGetResponse async call
    /// </summary>
    public class ForecastUpdateState
    {
        public HttpWebRequest AsyncRequest { get; set; }
        public HttpWebResponse AsyncResponse { get; set; }
    }

}
