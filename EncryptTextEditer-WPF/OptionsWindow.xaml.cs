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

        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void OptionsCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OptionsSave_Click(object sender, RoutedEventArgs e)
        {
            OptionModel NewOptions = SetForm();

            //Backing up old settings.  Mostly b/c of the key so you dont lose it.  Being lazy to back up the whole thing and not just the key.  :-)
            System.IO.File.Move(OptionsFileLocation, OptionsFileLocation.Replace(".", $"{DateTime.Now.ToString("yyyyMMdd")}."));

            FileIO.WriteToBinaryFile<OptionModel>(OptionsFileLocation,NewOptions);

            MessageBox.Show("Options Saved.","Options Settings",MessageBoxButton.OK,MessageBoxImage.Information);

            this.Close();
        }

        private void Options_Loaded(object sender, RoutedEventArgs e)
        {
           GetOptionValues();
        }

        private OptionModel SetForm()
        {
            OptionModel option = new OptionModel();

            option.UseDailyFile = (bool)UseSingleFile.IsChecked ? true : false;
            option.CustomKey = CustomKey.Text;
            option.CustomVI =  Encoding.ASCII.GetBytes(CustomVI.Text);

            return option;
        }

        private OptionModel GetOptionValues()
        {
            OptionModel CurrentOptions = FileIO.ReadFromBinaryFile<OptionModel>(OptionsFileLocation);

            UseSingleFile.IsChecked = CurrentOptions.UseDailyFile;
            CustomKey.Text = CurrentOptions.CustomKey;
            CustomVI.Text = Encoding.ASCII.GetString(CurrentOptions.CustomVI);

            return CurrentOptions;
        }

        private void CustomKey_Focus(object sender, RoutedEventArgs e)
        {
            if (!SawWarning)
            {
                SawWarning = true;
                MessageBox.Show("Changing encryption key could make you lose access old files.", "Change encryption key", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}

