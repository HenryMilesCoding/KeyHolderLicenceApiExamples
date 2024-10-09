using HenryMilesLicenceApi.Enums;
using HenryMilesLicenceApi.Models.Api.createaccount;
using HenryMilesLicenceApi.Services;
using HenryMilesLicenceOnlineWpf.Constants;
using HenryMilesLicenceOnlineWpf.Helper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HenryMilesLicenceOnlineWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IConfiguration optionalSecrets;
        public Configuration config;
        public ApiCreatesService HenryCreate;
        public ApiCheckService HenryCheck;

        public MainWindow()
        {
            InitializeComponent();
            config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            HenryCreate = new ApiCreatesService();
            HenryCreate.fireOn = SystemEnvironmentDbEnum.Tst;
            HenryCheck = new ApiCheckService();
            HenryCheck.fireOn = HenryCreate.fireOn;

            var builder = new ConfigurationBuilder()
            .AddUserSecrets<MainWindow>();

            optionalSecrets = builder.Build();

            FillKeyAndLicence();
            //string keyBefore = config.AppSettings.Settings["NewAccountModel"].Value;
            //ConfigHelper.UpdateValue(config, "NewAccountModel", "is_empty", "appSettings");
        }

        private void FillKeyAndLicence()
        {
            if (string.IsNullOrEmpty(config.AppSettings.Settings["NewAccountModel"].Value))
            {
                return;
            }

            PublicKeyTextBox.Text = config.AppSettings.Settings["NewAccountModel"].Value;
        }

        private async void CheckLicenseStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lese den PublicKey und die Lizenz aus den TextBoxen
                string publicKey = PublicKeyTextBox.Text;
                string licence = LicenceTextBox.Text;

                // Rufe den Endpunkt auf, um den Status der Lizenz zu prüfen
                //var result = await _apiClient.CheckLicenseStatusAsync(publicKey, licence);

                // Zeige das Ergebnis im Textfeld an
                //ApiResponseTextBox.Text = result.ToString();
                //StatusResultTextBox.Text = result.IsValid ? "Valid" : "Invalid";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void InvalidateLicense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lese den PublicKey und die Lizenz aus den TextBoxen
                string publicKey = PublicKeyTextBox.Text;
                string licence = LicenceTextBox.Text;

                // Rufe den Endpunkt auf, um die Lizenz ungültig zu machen
                //var result = await _apiClient.InvalidateLicenseAsync(publicKey, licence);

                // Zeige das Ergebnis im Textfeld an
                //ApiResponseTextBox.Text = result.ToString();
                StatusResultTextBox.Text = "License invalidated";
                bool stop = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void RenewLicense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lese den PublicKey und die Lizenz aus den TextBoxen
                string publicKey = PublicKeyTextBox.Text;
                string licence = LicenceTextBox.Text;

                // Rufe den Endpunkt auf, um die Lizenz zu verlängern
                //var result = await _apiClient.RenewLicenseAsync(publicKey, licence);

                // Zeige das Ergebnis im Textfeld an
                //ApiResponseTextBox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void GetLicenseInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lese den PublicKey und die Lizenz aus den TextBoxen
                string publicKey = PublicKeyTextBox.Text;
                string licence = LicenceTextBox.Text;

                // Rufe den Endpunkt auf, um Lizenzinformationen zu erhalten
                //var result = await _apiClient.GetLicenseInfoAsync(publicKey, licence);

                // Zeige das Ergebnis im Textfeld an
                //ApiResponseTextBox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void AccountModelButton_Click(object sender, RoutedEventArgs e)
        {
            string newAccountAsString = "{\r\n  \"action\": \"createaccount\",\r\n  \"state\": 1,\r\n  \"message\": \"Welcome! To your new Account nJYhmyZ9QQtH0lhJVvRKaeK5EMTzmHPfq\",\r\n  \"backdata\": {\r\n    \"Version\": 3,\r\n    \"RequestState\": 200,\r\n    \"mail\": null,\r\n    \"optin\": null,\r\n    \"InfosToYourNewAccount\": {\r\n      \"YourNewPrivateKey\": \"nJYhmyZ9QQtH0lhJVvRKaeK5EMTzmHPfq\",\r\n      \"YourNewPublicKey\": \"249335896263489723510177676594774\",\r\n      \"YourSecret\": \"ZPbK*$4jICIx$fE$HP\"\r\n    }\r\n  }\r\n}";
            FillDisplay(newAccountAsString);
        }

        private void FillDisplay(object value)
        {
            if (value == null || ApiResponseTextBox == null)
            {
                return;
            }

            string bringUp = string.Empty;

            if (value is object)
            {
                bringUp = JsonConvert.SerializeObject(value, Formatting.Indented);
            }
            else
            {
                bringUp = value.ToString();
            }

            ApiResponseTextBox.Text = bringUp;
        }

        private void LicenceModelButton_Click(object sender, RoutedEventArgs e)
        {
            var accountModel = HenryCreate.CreateLicenceTemplate("<SET_PRIVATE_KEY>", "<SET_SECRET>");
            FillDisplay(accountModel);
        }

        private void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            CreateaccountModel newAccount = HenryCreate.CreateNewAccount();
            string bringUp = JsonConvert.SerializeObject(newAccount, Formatting.Indented);
            FillDisplay(newAccount);

            if (newAccount != null)
            {
                ConfigHelper.UpdateValue(config, INeedConfig.NewAccountModel, bringUp, INeedSection.appSettings);
            }
        }

        // Weitere API-Endpunkt-Methoden...
    }
}