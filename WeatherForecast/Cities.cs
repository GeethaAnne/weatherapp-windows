
using System.Collections.ObjectModel;

namespace WeatherForecast
{
    /// <summary>
    /// Class to hold a collection of cities
    /// </summary>
    public class Cities : ObservableCollection<City>
    {
        public Cities() { }

        /// <summary>
        /// Create a default list of cities and their latitudes and longitudes
        /// </summary>
        public void LoadDefaultData()
        {
            App.cityList.Add(new City("SanFrancisco", "37.7833", "-122.416"));
            App.cityList.Add(new City("San Jose", "37.3333", "-121.900"));
            App.cityList.Add(new City("Oakland", "37.8044", "-122.270"));
            App.cityList.Add(new City("Fremont", "37.5483", "-121.988"));
            App.cityList.Add(new City("Santa Clara", "37.3542", "-121.954"));
          //  App.cityList.Add(new City("Hayward", "37.6689", "-122.080"));
            App.cityList.Add(new City("Sunnyvale", "37.3711", "-122.037"));
            App.cityList.Add(new City("Concord", "37.9781", "-122.031"));
            App.cityList.Add(new City("SantaRosa", "38.4486", "-122.704"));
            App.cityList.Add(new City("Fairfield", "38.2578", "-122.054"));

        }

    }
}
