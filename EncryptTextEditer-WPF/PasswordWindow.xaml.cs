using EncryptTextEditer_WPF.Models;
using EncryptTextEditerCL;
using System;
using System.IO;
using System.Windows;

namespace EncryptTextEditer_WPF
{
    /// <summary>
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        private enum Mode
        {
            Create,
            Migrate,
            Unlock,
        }

        private readonly string vaultPath;
        private readonly Mode mode;

        public OptionModel? ResultOption { get; private set; }
        public string? Password { get; private set; }

        public PasswordWindow(string vaultPath)
        {
            InitializeComponent();

            this.vaultPath = vaultPath;

            if (!File.Exists(vaultPath))
            {
                mode = Mode.Create;
            }
            else if (FileIO.IsLegacyOptionsFile(vaultPath))
            {
                mode = Mode.Migrate;
            }
            else
            {
                mode = Mode.Unlock;
            }
        }

        private void PasswordWindow_Loaded(object sender, RoutedEventArgs e)
        {
            switch (mode)
            {
                case Mode.Create:
                    Title = "Create Master Password";
                    InstructionsText.Text =
                        "Set a master password. It will be required every time you open this app, "
                        + "and it protects the encryption key used for your files.";
                    ShowConfirmField();
                    break;
                case Mode.Migrate:
                    Title = "Set Master Password";
                    InstructionsText.Text =
                        "This is the first launch with password protection. Set a master password "
                        + "to protect your existing encryption key going forward.";
                    ShowConfirmField();
                    break;
                case Mode.Unlock:
                    Title = "Unlock";
                    InstructionsText.Text = "Enter the master password to unlock your files.";
                    break;
            }

            PasswordInput.Focus();
        }

        private void ShowConfirmField()
        {
            ConfirmLabel.Visibility = Visibility.Visible;
            ConfirmPasswordInput.Visibility = Visibility.Visible;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordInput.Password;

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Password is required.");
                return;
            }

            if (mode == Mode.Create || mode == Mode.Migrate)
            {
                if (password != ConfirmPasswordInput.Password)
                {
                    ShowError("Passwords do not match.");
                    return;
                }

                OptionModel newOption;

                if (mode == Mode.Migrate)
                {
                    OptionModel legacyOption = FileIO.ReadFromBinaryFile<OptionModel>(vaultPath);
                    newOption = legacyOption;

                    // Back up the legacy file before it's overwritten below, so the key isn't
                    // lost if the vault write is interrupted (crash, power loss, disk full).
                    File.Copy(
                        vaultPath,
                        vaultPath.Replace(".", $"{DateTime.Now:yyyyMMdd}."),
                        overwrite: true
                    );
                }
                else
                {
                    newOption = new OptionModel
                    {
                        UseDailyFile = false,
                        CustomKey = FileIO.GetKey(),
                        CustomVI = FileIO.GetVI(),
                    };
                }

                FileIO.SaveVault(vaultPath, password, newOption);

                ResultOption = newOption;
                Password = password;
                DialogResult = true;
                Close();
            }
            else
            {
                try
                {
                    ResultOption = FileIO.OpenVault<OptionModel>(vaultPath, password);
                    Password = password;
                    DialogResult = true;
                    Close();
                }
                catch (WrongPasswordException)
                {
                    ShowError("Incorrect password.");
                    PasswordInput.Password = string.Empty;
                    PasswordInput.Focus();
                }
                catch (VaultCorruptException)
                {
                    ShowError("This options file is corrupted and cannot be read.");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
