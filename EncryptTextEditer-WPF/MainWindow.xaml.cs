using EncryptTextEditer_WPF.Models;
using EncryptTextEditerCL;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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

namespace EncryptTextEditer_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string FolderDefaultLocation = System.IO.Directory.GetCurrentDirectory();
        private readonly string FileDailyName = $"\\{DateTime.Now.ToString("yyyyMMdd")}.txt";
        private readonly string FileOneTimeUseName = "\\textfile.txt";

        private string FullDefaultLocation = string.Empty;

        private string OptionsFileLocation = System.IO.Directory.GetCurrentDirectory() + "\\options.txt";

        private OptionModel option = new OptionModel();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = $"Encrypt Text Editor - {Assembly.GetEntryAssembly().GetName().Version}";
           

            if (System.IO.File.Exists(OptionsFileLocation).Equals(false))
            {
                OptionModel NewOptions = new OptionModel();

                NewOptions.UseDailyFile = false;
                NewOptions.CustomKey = FileIO.GetKey();

                FileIO.WriteToBinaryFile<OptionModel>(OptionsFileLocation, NewOptions);
            }
            else
            {
                option = FileIO.ReadFromBinaryFile<OptionModel>(OptionsFileLocation);
            }

            if (option.UseDailyFile)
            {
                FullDefaultLocation = string.Concat(FolderDefaultLocation, FileDailyName);

                LoadFile(FullDefaultLocation, option.CustomKey);
            }
            else
            {
                FullDefaultLocation = string.Concat(FolderDefaultLocation, FileOneTimeUseName);
                LoadFile(FullDefaultLocation, option.CustomKey); 
            }


            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            FileIO.SaveFile(FullDefaultLocation, TextDataArea.Text);
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = FolderDefaultLocation;

            if (openFileDialog1.ShowDialog() == true)
            {
                string fileLocation = openFileDialog1.FileName;

                LoadFile(fileLocation,option.CustomKey);
            }
        }

        private void LoadFile(string filelocation,string key)
        {
            string textfiledata = FileIO.LoadFile(FullDefaultLocation,key);

            if (textfiledata.Length > 0)
            {
                TextDataArea.Text = textfiledata;
            }
            else
            {
                StatusBar.Text = "New file being used.";
                
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            FileIO.SaveFile(FullDefaultLocation, TextDataArea.Text);
        }

        private void TextDataArea_Focus(object sender, RoutedEventArgs e)
        {
            StatusBar.Text = string.Empty;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            FileIO.SaveFile(FullDefaultLocation, TextDataArea.Text);
            Close();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow optionsWindow = new OptionsWindow();
            optionsWindow.Owner = this;
            var resutls = optionsWindow.ShowDialog();

           
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Owner = this;
            var resutls = about.ShowDialog();
        }
    }
}
