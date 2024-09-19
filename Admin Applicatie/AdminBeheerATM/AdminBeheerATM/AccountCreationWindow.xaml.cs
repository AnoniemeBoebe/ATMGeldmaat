using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ATMApp
{
    public partial class AccountCreationWindow : Window
    {
        public AccountCreationWindow()
        {
            InitializeComponent();
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            string voornaam = txtVoornaam.Text;
            string achternaam = txtAchternaam.Text;
            string pincode = txtPincode.Password;

            if (string.IsNullOrEmpty(voornaam) || string.IsNullOrEmpty(achternaam) || string.IsNullOrEmpty(pincode))
            {
                MessageBox.Show("Vul alle velden in.");
                return;
            }

            string hashedPincode = BCrypt.Net.BCrypt.HashPassword(pincode);

            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO rekeninghouders (voornaam, achternaam, pincode_hashed, actief) VALUES (@voornaam, @achternaam, @pincode_hashed, 1)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@voornaam", voornaam);
                        cmd.Parameters.AddWithValue("@achternaam", achternaam);
                        cmd.Parameters.AddWithValue("@pincode_hashed", hashedPincode);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Account succesvol aangemaakt!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Er is een fout opgetreden bij het aanmaken van het account: {ex.Message}");
                }
            }
        }
    }
}
