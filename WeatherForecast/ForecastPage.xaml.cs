﻿

using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace WeatherForecast
{
    public partial class ForecastPage : PhoneApplicationPage
    {
        Forecast forecast;

        public ForecastPage()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Event handler to handle when this page is navigated to
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // get the city, latitude, and longitude out of the query string 
            string cityName = this.NavigationContext.QueryString["City"];
            string latitude = this.NavigationContext.QueryString["Latitude"];
            string longitude = this.NavigationContext.QueryString["Longitude"];

            forecast = new Forecast();

            // get the forecast for the given latitude and longitude
            forecast.GetForecast(latitude, longitude);

            // set data context for page to this forecast
            DataContext = forecast;

            // set source for ForecastList box in page to our list of forecast time periods
            ForecastList.ItemsSource = forecast.ForecastList;
        }


        /// <summary>
        /// Make sure no item can be selected in the forecast list box
        /// </summary>
        private void ForecastList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ForecastList.SelectedIndex = -1;
            ForecastList.SelectedItem = null;

        }

    }
}