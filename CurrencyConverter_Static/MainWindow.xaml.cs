using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurrencyConverter_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Root val = new Root();

        public class Root
        {

            public Rate rates { get; set;}
            public long timestamp;
            public string license;

        }

        public class Rate
        {
            public double INR { get; set; }
            public double JPY { get; set; }
            public double USD { get; set; }
            public double NZD { get; set; }
            public double EUR { get; set; }
            public double CAD { get; set; }
            public double ISK { get; set; }
            public double PHP { get; set; }
            public double DKK { get; set; }
            public double CZK { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            GetValue();


        }


        private async void GetValue()
        {

            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=1b8beff433ef4d9b8cfde98103df0165"); // API
            BindCurrency();

        }

        public static async Task<Root> GetData<T>(string url)
        {

            var myRoot = new Root();
            try
            {

                using (var client = new HttpClient()) // HttpClient class provides a base class for sending/receiving the HTTP requests
                {

                    client.Timeout = TimeSpan.FromMinutes(1); // The timespace to wait before the request times out.
                    HttpResponseMessage response = await client.GetAsync(url); // HttpResponseMessage ia a way of returning a message

                    if(response.StatusCode == System.Net.HttpStatusCode.OK) // Check API response status code ok
                    {

                        var ResponceString = await response.Content.ReadAsStringAsync(); // Serialise the HTTP content to string
                        var ResponceObject = JsonConvert.DeserializeObject<Root>(ResponceString); // JsonConvert.DeserializeObject

                        // MessageBox.Show("TimeStamp: " + ResponceObject.timestamp, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        return ResponceObject; //Return API responce

                    }

                    return myRoot;
                }

            }
            catch (Exception e)
            {
                return myRoot;
            }

        }

        private void BindCurrency()
        {
            DataTable dt = new DataTable(); // A DatTable to store the rates
            dt.Columns.Add("Text"); // Add the text column in the DataTable
            dt.Columns.Add("Value"); // Add the Value colum in the DataTable

            //Add rown in the Datatable with text and value
            dt.Rows.Add("--SELECT--", 0);
            dt.Rows.Add("INR", val.rates.INR);
            dt.Rows.Add("USD", val.rates.USD);
            dt.Rows.Add("NZD", val.rates.NZD);
            dt.Rows.Add("JPY", val.rates.JPY);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("CAD", val.rates.CAD);
            dt.Rows.Add("ISK", val.rates.ISK);
            dt.Rows.Add("PHP", val.rates.PHP);
            dt.Rows.Add("DKK", val.rates.DKK);
            dt.Rows.Add("CZK", val.rates.CZK);

            cmbFromCurrency.ItemsSource = dt.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.ItemsSource = dt.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }



        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            // Create the variable as Convertable with double
            double ConvertableValue;

            // Check if the amount textbox is Null or Blank
            if(txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amounf textbox is Null or Blank it will show this message box
                MessageBox.Show("Please Enter Currency", "information", MessageBoxButton.OK, MessageBoxImage.Information);
                //After clicking on message box OK set focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if currency From is not selected or select default text --SELECT--
            else if(cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                // Show the message
                MessageBox.Show("please Select currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set focus on the From Combobox
                cmbFromCurrency.Focus();
            }
            // Else if currency To is not selected or select default text --SELECT--
            else if(cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                // Show the message
                MessageBox.Show("please Select currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set focus on the From Combobox
                cmbToCurrency.Focus();
            }

            // Check if From and To Combobox selected values are same
            if(cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                // Amount textbox value set in ConvertableValue
                //double.parse is used for convertinf the datatype String To double.
                //textbox text have string and ConvertedValue is double Datatype
                ConvertableValue = double.Parse(txtCurrency.Text);
                //Show the label converted currency and converted currency name and ToString("N3") is used to place 000 after the decimal point
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertableValue.ToString("N3");
            }
            else
            {
                // Calculate for currency converter is From Currency value multply(*)
                // with the amount textbox value and then that total devided(/) with To Currency value.
                ConvertableValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) *
                    double.Parse(txtCurrency.Text)) / 
                    double.Parse(cmbFromCurrency.SelectedValue.ToString());

                // show the label converted currency and converted currency name
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertableValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;

            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
