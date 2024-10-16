﻿using HenryMilesLicenceApi.Enums;
using HenryMilesLicenceApi.Models;
using HenryMilesLicenceApi.Models.Api.createaccount;
using HenryMilesLicenceApi.Models.Api.Createlicence;
using HenryMilesLicenceApi.Models.Api.Getkeylist;
using HenryMilesLicenceApi.Models.Api.manualdeactivate;
using HenryMilesLicenceApi.Models.Api.readlicence;
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
        public Configuration? config;
        public ApiCreatesService? HenryCreate { get; private set; }
        public ApiCheckService? HenryCheck { get; private set; }
        public ApiReadsService? HenryReader { get; private set; }
        public CreateaccountModel? ActualUser { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                HenryCreate = new ApiCreatesService();
                HenryCreate.fireOn = SystemEnvironmentDbEnum.Tst;
                HenryCheck = new ApiCheckService();
                HenryCheck.fireOn = HenryCreate.fireOn;
                HenryReader = new ApiReadsService();
                HenryReader.fireOn = HenryCreate.fireOn;
                ActualUser = new CreateaccountModel();

                var builder = new ConfigurationBuilder()
                .AddUserSecrets<MainWindow>();

                optionalSecrets = builder.Build();

                FillKeyAndLicence();
            }
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }

        private void FillKeyAndLicence()
        {
            try
            {
                if (string.IsNullOrEmpty(config.AppSettings.Settings[ConfigKey.NewAccountModel].Value))
                {
                    FillDisplay("You must first create an account before this step works.");
                    return;
                }

                ActualUser = JsonConvert.DeserializeObject<CreateaccountModel>(config.AppSettings.Settings[ConfigKey.NewAccountModel].Value);

                if (ActualUser != null)
                {
                    PublicKeyTextBox.Text = ActualUser.Backdata.InfosToYourNewAccount.YourNewPublicKey;
                }

                string lastLicence = config.AppSettings.Settings[ConfigKey.LicenceKey].Value;
                if (lastLicence != null)
                {
                    LicenceTextBox.Text = lastLicence;
                }
            }
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }

        private void CheckLicenseStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string publicKey = PublicKeyTextBox.Text;
                string licence = LicenceTextBox.Text;

                if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(licence))
                {
                    FillDisplay("An account and a license must first be created.");
                    return;
                }

                ReadlicenceModel result = HenryCheck.VerifiyLicence(publicKey, licence, true);
                result.State = (result.State == null) ? 0 : result.State;
                StatusResultTextBox.Text = result.Message;
                FillDisplay(result);
            }
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }

        private void InvalidateLicense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string privateKey = ActualUser.Backdata.InfosToYourNewAccount.YourNewPrivateKey;
                string secret = ActualUser.Backdata.InfosToYourNewAccount.YourSecret;
                string licence = LicenceTextBox.Text;

                if (ActualUser == null)
                {
                    FillDisplay("You must first create an account for it to work!");
                    return;
                }

                if (licence == null)
                {
                    FillDisplay("You must first create an licence for it to work!");
                    return;
                }

                ManualdeactivateModel result = HenryCreate.DisableLicense("EN", privateKey, secret, licence);

                StatusResultTextBox.Text = result.Message;
                FillDisplay(result);
                bool stop = true;
            }
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }

        private void AccountModelButton_Click(object sender, RoutedEventArgs e)
        {
            string newAccountAsString = "{  'action': 'createaccount',  'state': 1,  'message': 'Welcome! To your new Account nJYhmyZ9QQtH0lhJVvRKaeK5EMTzmHPfq',  'backdata': { 'Version': 3, 'RequestState': 200, 'mail': null, 'optin': null, 'InfosToYourNewAccount': { 'YourNewPrivateKey': 'nJYhmyZ9QQtH0lhJVvRKaeK5EMTzmHPfq', 'YourNewPublicKey': '249335896263489723510177676594774', 'YourSecret': 'ZPbK*$4jICIx$fE$HP' }  }}";
            FillDisplay(newAccountAsString);
        }

        private void FillDisplay(object value)
        {
            try
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
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }

        private void LicenceModelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetKeylistRequestModel accountModel = HenryCreate.CreateLicenceTemplate("<SET_PRIVATE_KEY>", "<SET_SECRET>");
                // Attention: Enum values cannot be displayed correctly in the text window. The text values of enums are then always
                FillDisplay(accountModel);
            }
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }

        private void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }

        private void GetLicenceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ActualUser == null)
                {
                    FillDisplay("You must first create an account before this step works.");
                    return;
                }

                GetKeylistRequestModel requestForList = new GetKeylistRequestModel();
                requestForList.secret = ActualUser.Backdata.InfosToYourNewAccount.YourSecret;
                requestForList.userKey = ActualUser.Backdata.InfosToYourNewAccount.YourNewPrivateKey;
                GetkeylistModel resultForList = HenryReader.GetKeylist(requestForList, false);
                // Attention: Enum values cannot be displayed correctly in the text window. The text values of enums are then always

                FillDisplay(resultForList);
            }
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }

        private void NewLicenceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ActualUser == null)
                {
                    FillDisplay("You must first create an account before this step works.");
                    return;
                }

                string userKey = ActualUser.Backdata.InfosToYourNewAccount.YourNewPrivateKey;
                string secret = ActualUser.Backdata.InfosToYourNewAccount.YourSecret;

                GetKeylistRequestModel templateToCreate = HenryCreate.CreateLicenceTemplate(userKey, secret);
                CreatelicenceModel result = HenryCreate.CreateNewLicence(templateToCreate);

                if (result != null)
                {
                    FillDisplay(result);
                    ConfigHelper.UpdateValue(config, ConfigKey.LicenceKey, result.Backdata.NewKey, INeedSection.appSettings);
                    FillKeyAndLicence();
                }
            }
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }

        private void KeyListModelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetKeylistRequestModel requestForList = new GetKeylistRequestModel();
                requestForList.secret = "<HERE_YOUR_SECRET>";
                requestForList.userKey = "<HERE_YOUR_PRIVATE_KEY>";

                FillDisplay(requestForList);
            }
            catch (Exception ex)
            {
                FillDisplay(ex.Message);
            }
        }
    }
}