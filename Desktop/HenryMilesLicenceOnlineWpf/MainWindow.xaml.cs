﻿using HenryMilesLicenceApi.Enums;
using HenryMilesLicenceApi.Models;
using HenryMilesLicenceApi.Models.Api.createaccount;
using HenryMilesLicenceApi.Models.Api.Createlicence;
using HenryMilesLicenceApi.Models.Api.Getkeylist;
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
        public ApiReadsService HenryReader;
        public CreateaccountModel actualUser;

        public MainWindow()
        {
            InitializeComponent();
            config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            HenryCreate = new ApiCreatesService();
            HenryCreate.fireOn = SystemEnvironmentDbEnum.Tst;
            HenryCheck = new ApiCheckService();
            HenryCheck.fireOn = HenryCreate.fireOn;
            HenryReader = new ApiReadsService();
            HenryReader.fireOn = HenryCreate.fireOn;
            actualUser = new CreateaccountModel();

            var builder = new ConfigurationBuilder()
            .AddUserSecrets<MainWindow>();

            optionalSecrets = builder.Build();

            FillKeyAndLicence();
        }

        private void FillKeyAndLicence()
        {
            if (string.IsNullOrEmpty(config.AppSettings.Settings[ConfigKey.NewAccountModel].Value))
            {
                FillDisplay("You must first create an account before this step works.");
                return;
            }

            actualUser = JsonConvert.DeserializeObject<CreateaccountModel>(config.AppSettings.Settings[ConfigKey.NewAccountModel].Value);

            if (actualUser != null)
            {
                PublicKeyTextBox.Text = actualUser.Backdata.InfosToYourNewAccount.YourNewPublicKey;
            }
        }

        private async void CheckLicenseStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lese den PublicKey und die Lizenz aus den TextBoxen
                string publicKey = PublicKeyTextBox.Text;
                string licence = LicenceTextBox.Text;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void AccountModelButton_Click(object sender, RoutedEventArgs e)
        {
            string newAccountAsString = "{  'action': 'createaccount',  'state': 1,  'message': 'Welcome! To your new Account nJYhmyZ9QQtH0lhJVvRKaeK5EMTzmHPfq',  'backdata': { 'Version': 3, 'RequestState': 200, 'mail': null, 'optin': null, 'InfosToYourNewAccount': { 'YourNewPrivateKey': 'nJYhmyZ9QQtH0lhJVvRKaeK5EMTzmHPfq', 'YourNewPublicKey': '249335896263489723510177676594774', 'YourSecret': 'ZPbK*$4jICIx$fE$HP' }  }}";
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
            GetKeylistRequestModel accountModel = HenryCreate.CreateLicenceTemplate("<SET_PRIVATE_KEY>", "<SET_SECRET>");
            // Attention: Enum values cannot be displayed correctly in the text window. The text values of enums are then always
            FillDisplay(accountModel);
        }

        private void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            CreateaccountModel newAccount = HenryCreate.CreateNewAccount();
            string bringUp = JsonConvert.SerializeObject(newAccount, Formatting.Indented);
            FillDisplay(newAccount);

            if (newAccount != null)
            {
                ConfigHelper.UpdateValue(config, ConfigKey.NewAccountModel, bringUp, INeedSection.appSettings);
                FillKeyAndLicence();
            }
        }

        private void GetLicenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (actualUser == null)
            {
                FillDisplay("You must first create an account before this step works.");
                return;
            }

            GetKeylistRequestModel requestForList = new GetKeylistRequestModel();
            requestForList.secret = actualUser.Backdata.InfosToYourNewAccount.YourSecret;
            requestForList.userKey = actualUser.Backdata.InfosToYourNewAccount.YourNewPrivateKey;
            GetkeylistModel resultForList = HenryReader.GetKeylist(requestForList, false);
            // Attention: Enum values cannot be displayed correctly in the text window. The text values of enums are then always

            FillDisplay(resultForList);
        }

        private void NewLicenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (actualUser == null)
            {
                FillDisplay("You must first create an account before this step works.");
                return;
            }

            string userKey = actualUser.Backdata.InfosToYourNewAccount.YourNewPrivateKey;
            string secret = actualUser.Backdata.InfosToYourNewAccount.YourSecret;

            GetKeylistRequestModel templateToCreate = HenryCreate.CreateLicenceTemplate(userKey, secret);
            CreatelicenceModel result = HenryCreate.CreateNewLicence(templateToCreate);

            if (result != null)
            {
                FillDisplay(result);
            }
        }

        // Weitere API-Endpunkt-Methoden...
    }
}