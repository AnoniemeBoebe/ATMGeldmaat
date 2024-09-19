using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace ATMApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Login functionaliteit
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string rekeningnummer = txtRekeningnummer.Text;
            string pincode = txtPincode.Password;

            if (ValidateLogin(rekeningnummer, pincode))
            {
                MessageBox.Show("Login succesvol!");
                ATMWindow atmWindow = new ATMWindow(rekeningnummer);
                atmWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ongeldig rekeningnummer of pincode.");
            }
        }

        // Validatie van de login
        private bool ValidateLogin(string rekeningnummer, string pincode)
        {
            bool isValid = false;
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT pincode_hashed FROM rekeninghouders WHERE rekeningnummer = @rekeningnummer AND actief = 1";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                    var result = cmd.ExecuteScalar();

                    if (result != null && BCrypt.Net.BCrypt.Verify(pincode, result.ToString()))
                    {
                        isValid = true;
                    }
                }
            }
            return isValid;
        }

        public MainWindow(TextBox txtRekeningnummer, PasswordBox txtPincode, bool contentLoaded)
        {
            this.txtRekeningnummer = txtRekeningnummer;
            this.txtPincode = txtPincode;
            _contentLoaded = contentLoaded;
        }
    }
}
