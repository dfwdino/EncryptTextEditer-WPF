using EncryptTextEditer_WPF.Models;
using EncryptTextEditerCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EncryptTextEditer_WPF
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private string OptionsFileLocation = DefaultSettingsModel.OptionsFileLocation;
        private bool SawWarning = false;

        private readonly OptionModel currentOption;
        private readonly string masterPassword;

        public OptionModel? UpdatedOption { get; private set; }

        public OptionsWindow(OptionModel currentOption, string masterPassword)
        {
            InitializeComponent();

            this.currentOption = currentOption;
            this.masterPassword = masterPassword;
        }

        private void OptionsCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void OptionsSave_Click(object sender, RoutedEventArgs e)
        {
            OptionModel NewOptions = SetForm();

            //Backing up old settings.  Mostly b/c of the key so you dont lose it.  Being lazy to back up the whole thing and not just the key.  :-)
            System.IO.File.Copy(
                OptionsFileLocation,
                OptionsFileLocation.Replace(".", $"{DateTime.Now.ToString("yyyyMMdd")}."),
                overwrite: true
            );

            FileIO.SaveVault<OptionModel>(OptionsFileLocation, masterPassword, NewOptions);

            UpdatedOption = NewOptions;

            MessageBox.Show(
                "Options Saved.",
                "Options Settings",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            DialogResult = true;
            this.Close();
        }

        private void Options_Loaded(object sender, RoutedEventArgs e)
        {
            UseSingleFile.IsChecked = currentOption.UseDailyFile;
            CustomKey.Text = currentOption.CustomKey;
            CustomVI.Text = Encoding.ASCII.GetString(currentOption.CustomVI);
        }

        private OptionModel SetForm()
        {
            OptionModel option = new OptionModel();

            option.UseDailyFile = (bool)UseSingleFile.IsChecked ? true : false;
            option.CustomKey = CustomKey.Text;
            option.CustomVI = Encoding.ASCII.GetBytes(CustomVI.Text);

            return option;
        }

        private void CustomKey_Focus(object sender, RoutedEventArgs e)
        {
            if (!SawWarning)
            {
                SawWarning = true;
                MessageBox.Show(
                    "Changing encryption key could make you lose access old files.",
                    "Change encryption key",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
    }
}
