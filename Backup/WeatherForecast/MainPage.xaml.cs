

using System;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace WeatherForecast
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            CityList.ItemsSource = App.cityList;
        }

        /// <summary>
        /// Event handler called when user selects a city to get a forecast for
        /// </summary>
        private void CityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if an item is selected
            if (CityList.SelectedIndex != -1)
            {
                // get the currently selected city and pass the information to the 
                // forecast details page
                City curCity = (City)CityList.SelectedItem;
                this.NavigationService.Navigate(new Uri("/ForecastPage.xaml?City=" +
                    curCity.CityName + "&Latitude=" + curCity.Latitude + "&Longitude=" +
                    curCity.Longitude, UriKind.Relative));
            }

        }

        /// <summary>
        /// Event handler called when user navigates away from this page
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            // make sure no item is highlighted in the list of cities
            CityList.SelectedIndex = -1;
            CityList.SelectedItem = null;
        }

    }
}
