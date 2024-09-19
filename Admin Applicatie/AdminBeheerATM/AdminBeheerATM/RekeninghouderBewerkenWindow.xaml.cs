using System.Windows;
using MySql.Data.MySqlClient;

namespace ATMApp
{
    public partial class RekeninghouderBewerkenWindow : Window
    {
        private string rekeningnummer;

        public RekeninghouderBewerkenWindow(string rekeningnummer, string voornaam, string achternaam)
        {
            InitializeComponent();
            this.rekeningnummer = rekeningnummer;
            txtVoornaam.Text = voornaam;
            txtAchternaam.Text = achternaam;
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            string nieuweVoornaam = txtVoornaam.Text;
            string nieuweAchternaam = txtAchternaam.Text;
            UpdateRekeninghouder(rekeningnummer, nieuweVoornaam, nieuweAchternaam);
            MessageBox.Show("Rekeninghouder succesvol bijgewerkt.");
            this.Close();
        }

        private void UpdateRekeninghouder(string rekeningnummer, string voornaam, string achternaam)
        {
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE rekeninghouders SET voornaam = @voornaam, achternaam = @achternaam WHERE rekeningnummer = @rekeningnummer";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@voornaam", voornaam);
                    cmd.Parameters.AddWithValue("@achternaam", achternaam);
                    cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
