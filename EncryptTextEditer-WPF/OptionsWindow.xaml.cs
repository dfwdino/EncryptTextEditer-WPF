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

        private string OptionsFileLocation = System.IO.Directory.GetCurrentDirectory() + "options.txt";

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
            Option NewOptions = SetForm();

            FileIO.WriteToBinaryFile<Option>(OptionsFileLocation,NewOptions);
        }

        private void Options_Loaded(object sender, RoutedEventArgs e)
        {
            GetOptionValues();
        }

        private Option SetForm()
        {
            Option option = new Option();

            option.UseOneFile = (bool)UseSingleFile.IsChecked ? true : false;
            option.CustomKey = CustomKey.Text;

            return option;
        }

        private Option GetOptionValues()
        {
            Option CurrentOptions = FileIO.ReadFromBinaryFile<Option>(OptionsFileLocation);

            UseSingleFile.IsChecked = CurrentOptions.UseOneFile;
            CustomKey.Text = String.IsNullOrEmpty(CustomKey.Text) ? String.Empty : CurrentOptions.CustomKey;

            return CurrentOptions;
        }

    }
}
