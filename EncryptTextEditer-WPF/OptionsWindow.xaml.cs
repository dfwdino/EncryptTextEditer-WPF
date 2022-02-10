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

            FileIO.WriteToBinaryFile<OptionModel>(OptionsFileLocation,NewOptions);
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

            return option;
        }

        private OptionModel GetOptionValues()
        {
            OptionModel CurrentOptions = FileIO.ReadFromBinaryFile<OptionModel>(OptionsFileLocation);

            UseSingleFile.IsChecked = CurrentOptions.UseDailyFile;
            CustomKey.Text = String.IsNullOrEmpty(CustomKey.Text) ? String.Empty : CurrentOptions.CustomKey;

            return CurrentOptions;
        }

        private void CustomKey_Focus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Changing encryption key could make you lose access old files.", "Change encryption key",MessageBoxButton.OK,MessageBoxImage.Warning);
        }
    }
}
