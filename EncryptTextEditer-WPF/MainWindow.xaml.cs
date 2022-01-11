﻿using EncryptTextEditerCL;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EncryptTextEditer_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FileDefaultLocation = $"{DateTime.Now.ToString("yyyyMMdd")}.txt";
        private string FolderDefaultLocation = @"c:\\temp\\";
        private string FullDefaultLocation = string.Empty;

        public MainWindow()
        {
            FullDefaultLocation = string.Concat(FileDefaultLocation, FolderDefaultLocation);
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFile(FullDefaultLocation);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FileIO.SaveFile(FullDefaultLocation, TextDataArea.Text);
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            string fileLocation = FileIO.OpenFileDialog();

            LoadFile(fileLocation);
        }

        private void LoadFile(string filelocation)
        {
            string textfiledata = FileIO.LoadFile(FullDefaultLocation);

            if (textfiledata.Length > 0)
            {
                TextDataArea.Text = textfiledata;
            }
            else
            {
                MessageBox.Show("No data in file.", "Warning Message", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


    }
}
