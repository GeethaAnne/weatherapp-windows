

using System.ComponentModel;

namespace WeatherForecast
{
    /// <summary>
    /// This has the information about a city
    /// </summary>
    public class City : INotifyPropertyChanged
    {
        private string cityName;
        private string latitude;
        private string longitude;


        public string CityName
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

        public string Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                if (value != latitude)
                {
                    latitude = value;
                    NotifyPropertyChanged("Latitude");
                }
            }
        }

        public string Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                if (value != longitude)
                {
                    longitude = value;
                    NotifyPropertyChanged("Longitude");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Constructor with full city information
        /// </summary>
        public City(string cityName, string latitude, string longitude)
        {
            CityName = cityName;
            Latitude = latitude;
            Longitude = longitude;
        }

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

    }
}
