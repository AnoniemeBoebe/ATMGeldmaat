using System.Windows;
using MySql.Data.MySqlClient;

namespace ATMApp
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadRekeningen();
        }

        private void LoadRekeningen()
        {
            lstRekeningen.Items.Clear();
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT rekeningnummer, voornaam, achternaam, actief FROM rekeninghouders";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string rekeningnummer = reader["rekeningnummer"].ToString();
                            string voornaam = reader["voornaam"].ToString();
                            string achternaam = reader["achternaam"].ToString();
                            bool actief = (bool)reader["actief"];
                            string status = actief ? "Actief" : "Geblokkeerd";
                            lstRekeningen.Items.Add($"{rekeningnummer} - {voornaam} {achternaam} - {status}");
                        }
                    }
                }
            }
        }

        private void Zoeken_Click(object sender, RoutedEventArgs e)
        {
            string zoekterm = txtZoekTerm.Text.Trim();
            lstRekeningen.Items.Clear();
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT rekeningnummer, voornaam, achternaam, actief FROM rekeninghouders WHERE rekeningnummer LIKE @zoekterm OR achternaam LIKE @zoekterm";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@zoekterm", "%" + zoekterm + "%");
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string rekeningnummer = reader["rekeningnummer"].ToString();
                            string voornaam = reader["voornaam"].ToString();
                            string achternaam = reader["achternaam"].ToString();
                            bool actief = (bool)reader["actief"];
                            string status = actief ? "Actief" : "Geblokkeerd";
                            lstRekeningen.Items.Add($"{rekeningnummer} - {voornaam} {achternaam} - {status}");
                        }
                    }
                }
            }
        }

        private void Toevoegen_Click(object sender, RoutedEventArgs e)
        {
            AccountCreationWindow accountCreationWindow = new AccountCreationWindow();
            accountCreationWindow.ShowDialog();
            LoadRekeningen();
        }

        private void Blokkeren_Click(object sender, RoutedEventArgs e)
        {
            if (lstRekeningen.SelectedItem != null)
            {
                string geselecteerdeRekening = lstRekeningen.SelectedItem.ToString().Split(' ')[0];
                UpdateRekeningStatus(geselecteerdeRekening, false);
                LoadRekeningen();
            }
        }

        private void Deblokkeren_Click(object sender, RoutedEventArgs e)
        {
            if (lstRekeningen.SelectedItem != null)
            {
                string geselecteerdeRekening = lstRekeningen.SelectedItem.ToString().Split(' ')[0];
                UpdateRekeningStatus(geselecteerdeRekening, true);
                LoadRekeningen();
            }
        }

        private void Bewerken_Click(object sender, RoutedEventArgs e)
        {
            if (lstRekeningen.SelectedItem != null)
            {
                string[] onderdelen = lstRekeningen.SelectedItem.ToString().Split(' ');
                string rekeningnummer = onderdelen[0];
                string voornaam = onderdelen[2];
                string achternaam = onderdelen[3];

                RekeninghouderBewerkenWindow bewerkenWindow = new RekeninghouderBewerkenWindow(rekeningnummer, voornaam, achternaam);
                bewerkenWindow.ShowDialog();
                LoadRekeningen();
            }
        }

        private void UpdateRekeningStatus(string rekeningnummer, bool actief)
        {
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE rekeninghouders SET actief = @actief WHERE rekeningnummer = @rekeningnummer";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@actief", actief);
                    cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
