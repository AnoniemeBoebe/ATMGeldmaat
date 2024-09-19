using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ATMApp
{
    public partial class GeldStortenWindow : Window
    {
        private string rekeningnummer;

        public GeldStortenWindow(string rekeningnummer)
        {
            InitializeComponent();
            this.rekeningnummer = rekeningnummer;
        }

        private void Storten_Click(object sender, RoutedEventArgs e)
        {
            decimal bedrag;
            if (decimal.TryParse(txtBedrag.Text, out bedrag) && bedrag > 0)
            {
                string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE rekeninghouders SET saldo = saldo + @bedrag WHERE rekeningnummer = @rekeningnummer";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@bedrag", bedrag);
                        cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                        cmd.ExecuteNonQuery();
                    }

                    // Transactie opslaan
                    query = "INSERT INTO transacties (rekeningnummer, type_transactie, bedrag) VALUES (@rekeningnummer, 'storting', @bedrag)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                        cmd.Parameters.AddWithValue("@bedrag", bedrag);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"€{bedrag} succesvol gestort.");
                this.Close();  // Sluit het venster na het storten
            }
            else
            {
                MessageBox.Show("Voer een geldig bedrag in.");
            }
        }
    }
}
