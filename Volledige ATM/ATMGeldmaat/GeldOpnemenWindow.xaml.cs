using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ATMApp
{
    public partial class GeldOpnemenWindow : Window
    {
        private string rekeningnummer;
        private decimal saldo;

        public GeldOpnemenWindow(string rekeningnummer, decimal saldo)
        {
            InitializeComponent();
            this.rekeningnummer = rekeningnummer;
            this.saldo = saldo;
        }

        private void Opnemen_Click(object sender, RoutedEventArgs e)
        {
            decimal bedrag;
            if (decimal.TryParse(txtBedrag.Text, out bedrag) && bedrag > 0)
            {
                if (bedrag > saldo)
                {
                    MessageBox.Show("Onvoldoende saldo.");
                    return;
                }

                if (bedrag > 500)
                {
                    MessageBox.Show("Maximaal €500 per opname toegestaan.");
                    return;
                }

                string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE rekeninghouders SET saldo = saldo - @bedrag WHERE rekeningnummer = @rekeningnummer";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@bedrag", bedrag);
                        cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                        cmd.ExecuteNonQuery();
                    }

                    // Transactie opslaan
                    query = "INSERT INTO transacties (rekeningnummer, type_transactie, bedrag) VALUES (@rekeningnummer, 'opname', @bedrag)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                        cmd.Parameters.AddWithValue("@bedrag", bedrag);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"€{bedrag} succesvol opgenomen.");
                this.Close();  // Sluit het venster na de opname
            }
            else
            {
                MessageBox.Show("Voer een geldig bedrag in.");
            }
        }
    }
}
