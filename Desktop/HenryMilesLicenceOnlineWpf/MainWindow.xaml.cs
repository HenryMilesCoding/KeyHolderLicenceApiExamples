﻿using HenryMilesLicenceApi.Enums;
using HenryMilesLicenceApi.Services;
using HenryMilesLicenceOnlineWpf.Helper;
using Microsoft.Extensions.Configuration;
using System.Configuration;
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
        public ApiCreatesService ApiCreatesService;

        public MainWindow()
        {
            InitializeComponent();
            config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ApiCreatesService = new ApiCreatesService();
            ApiCreatesService.fireOn = SystemEnvironmentDbEnum.Tst;

            var builder = new ConfigurationBuilder()
            .AddUserSecrets<MainWindow>();

            optionalSecrets = builder.Build();

            //string keyBefore = config.AppSettings.Settings["NewAccountModel"].Value;
            //ConfigHelper.UpdateValue(config, "NewAccountModel", "is_empty", "appSettings");
        }
    }
}