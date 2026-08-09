using EncryptTextEditer_WPF.Models;
using EncryptTextEditerCL;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        private OptionModel option;
        private readonly string masterPassword;

        public MainWindow(OptionModel option, string masterPassword)
        {
            InitializeComponent();

            this.option = option;
            this.masterPassword = masterPassword;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = $"Encrypt Text Editor - {Assembly.GetEntryAssembly().GetName().Version}";

            FullDefaultLocation = string.Concat(
                FolderDefaultLocation,
                option.UseDailyFile ? FileDailyName : FileOneTimeUseName
            );

            OpenFileInNewTab(FullDefaultLocation);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!SaveAllTabs())
            {
                e.Cancel = true;
            }
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = FolderDefaultLocation;

            if (openFileDialog1.ShowDialog() == true)
            {
                string fileLocation = openFileDialog1.FileName;

                foreach (TabItem existingTab in EditorTabs.Items)
                {
                    if (
                        existingTab.Tag is string existingPath
                        && string.Equals(existingPath, fileLocation, StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        EditorTabs.SelectedItem = existingTab;
                        return;
                    }
                }

                OpenFileInNewTab(fileLocation);
            }
        }

        private void OpenFileInNewTab(string fileLocation)
        {
            string textfiledata = FileIO.LoadFile(fileLocation, option.CustomKey, option.CustomVI);

            TextBox textBox = CreateEditorTextBox(textfiledata);

            TabItem tab = new TabItem { Tag = fileLocation, Content = textBox };
            tab.Header = BuildTabHeader(System.IO.Path.GetFileName(fileLocation), tab);

            EditorTabs.Items.Add(tab);
            EditorTabs.SelectedItem = tab;

            StatusBar.Text = textfiledata.Length > 0 ? string.Empty : "New file being used.";
        }

        private void NewTab_Click(object sender, RoutedEventArgs e)
        {
            TextBox textBox = CreateEditorTextBox(string.Empty);

            TabItem tab = new TabItem { Tag = null, Content = textBox };
            tab.Header = BuildTabHeader("Untitled", tab);

            EditorTabs.Items.Add(tab);
            EditorTabs.SelectedItem = tab;
        }

        private TextBox CreateEditorTextBox(string text)
        {
            TextBox textBox = new TextBox
            {
                Text = text,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
            };
            textBox.GotFocus += TextDataArea_Focus;
            textBox.Loaded += (s, e) => textBox.Focus();

            return textBox;
        }

        private object BuildTabHeader(string title, TabItem tab)
        {
            StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };

            TextBlock text = new TextBlock
            {
                Text = title,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 6, 0),
            };

            Button closeButton = new Button
            {
                Content = "x",
                Width = 16,
                Height = 16,
                Padding = new Thickness(0),
                FontSize = 10,
                VerticalAlignment = VerticalAlignment.Center,
            };
            closeButton.Click += (s, e) => CloseTab(tab);

            panel.Children.Add(text);
            panel.Children.Add(closeButton);

            return panel;
        }

        /// <summary>
        /// Saves the tab's content. Returns false only when the tab needed a file location
        /// (an "Untitled" tab) and the user cancelled the Save As dialog.
        /// </summary>
        private bool SaveTab(TabItem tab)
        {
            if (tab == null || !(tab.Content is TextBox textBox))
            {
                return true;
            }

            string filePath = tab.Tag as string;

            if (string.IsNullOrEmpty(filePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    InitialDirectory = FolderDefaultLocation,
                };

                if (saveFileDialog.ShowDialog() != true)
                {
                    return false;
                }

                filePath = saveFileDialog.FileName;
                tab.Tag = filePath;
                tab.Header = BuildTabHeader(System.IO.Path.GetFileName(filePath), tab);
            }

            FileIO.SaveFile(filePath, textBox.Text, option.CustomKey, option.CustomVI);
            return true;
        }

        /// <summary>
        /// Saves every tab that has a file location, plus any "Untitled" tab that has content
        /// (prompting for a location). Returns false if any such save was cancelled by the user.
        /// </summary>
        private bool SaveAllTabs()
        {
            bool allSaved = true;

            foreach (TabItem tab in EditorTabs.Items)
            {
                if (!(tab.Content is TextBox textBox))
                {
                    continue;
                }

                bool hasFilePath = tab.Tag is string filePath && !string.IsNullOrEmpty(filePath);

                if (hasFilePath || textBox.Text.Length > 0)
                {
                    if (!SaveTab(tab))
                    {
                        allSaved = false;
                    }
                }
            }

            return allSaved;
        }

        private void CloseTab(TabItem tab)
        {
            if (tab == null)
            {
                return;
            }

            if (!SaveTab(tab))
            {
                return;
            }

            EditorTabs.Items.Remove(tab);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveTab(EditorTabs.SelectedItem as TabItem);
        }

        private void TextDataArea_Focus(object sender, RoutedEventArgs e)
        {
            StatusBar.Text = string.Empty;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseTab(EditorTabs.SelectedItem as TabItem);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow optionsWindow = new OptionsWindow(option, masterPassword);
            optionsWindow.Owner = this;

            if (optionsWindow.ShowDialog() == true && optionsWindow.UpdatedOption != null)
            {
                option = optionsWindow.UpdatedOption;
            }
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Owner = this;
            var resutls = about.ShowDialog();
        }
    }
}
