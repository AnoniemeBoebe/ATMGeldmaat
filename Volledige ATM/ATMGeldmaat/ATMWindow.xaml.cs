using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ATMApp
{
    public partial class ATMWindow : Window
    {
        private string rekeningnummer;
        private decimal saldo;
        private int maxOpnamesPerDag = 3;
        private int huidigeOpnames = 0; // Aantal opnames vandaag

        public ATMWindow(string rekeningnummer)
        {
            InitializeComponent();
            this.rekeningnummer = rekeningnummer;
            // Haal saldo en transacties op bij het openen van het venster
            GetSaldo();
            GetLaatsteTransacties();
            GetAantalOpnamesVandaag(); // Haal het aantal opnames vandaag op
        }

        // Methode voor het ophalen van het saldo en het bijwerken van de UI
        private void GetSaldo()
        {
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT saldo FROM rekeninghouders WHERE rekeningnummer = @rekeningnummer";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        saldo = Convert.ToDecimal(result);
                        txtSaldo.Text = "Saldo: €" + saldo.ToString("F2");
                    }
                    else
                    {
                        MessageBox.Show("Saldo niet gevonden.");
                    }
                }
            }
        }

        // Methode om de laatste drie transacties op te halen en bij te werken in de UI
        private void GetLaatsteTransacties()
        {
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT type_transactie, bedrag, datum_transactie FROM transacties WHERE rekeningnummer = @rekeningnummer ORDER BY datum_transactie DESC LIMIT 3";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        lstTransacties.Items.Clear();  // Clear the list before adding new items
                        while (reader.Read())
                        {
                            string type = reader.GetString(0);
                            decimal bedrag = reader.GetDecimal(1);
                            DateTime datum = reader.GetDateTime(2);
                            string transactieInfo = $"{datum.ToShortDateString()} - {type}: €{bedrag.ToString("F2")}";
                            lstTransacties.Items.Add(transactieInfo);
                        }
                    }
                }
            }
        }

        // Methode om het aantal opnames vandaag op te halen en bij te werken
        private void GetAantalOpnamesVandaag()
        {
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM transacties WHERE rekeningnummer = @rekeningnummer AND type_transactie = 'opname' AND DATE(datum_transactie) = CURDATE();";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@rekeningnummer", rekeningnummer);
                    huidigeOpnames = Convert.ToInt32(cmd.ExecuteScalar());
                    txtAantalOpnames.Text = $"Opnames vandaag: {huidigeOpnames}/{maxOpnamesPerDag}";
                }
            }
        }

        // Methode om saldo en transacties opnieuw op te halen na een transactie
        private void UpdateData()
        {
            GetSaldo();  // Haal het bijgewerkte saldo op en toon het in de UI
            GetLaatsteTransacties();  // Haal de laatste transacties op en toon ze in de UI
            GetAantalOpnamesVandaag(); // Update het aantal opnames vandaag
        }

        // Open het venster om geld op te nemen
        private void GeldOpnemen_Click(object sender, RoutedEventArgs e)
        {
            if (huidigeOpnames >= maxOpnamesPerDag)
            {
                MessageBox.Show("Je hebt vandaag al 3 keer geld opgenomen. Probeer morgen opnieuw.");
                return;
            }

            GeldOpnemenWindow geldOpnemenWindow = new GeldOpnemenWindow(rekeningnummer, saldo);
            geldOpnemenWindow.Closed += (s, args) => UpdateData();  // Update saldo en transacties nadat het opnamevenster wordt gesloten
            geldOpnemenWindow.Show();
        }

        // Open het venster om geld te storten
        private void GeldStorten_Click(object sender, RoutedEventArgs e)
        {
            GeldStortenWindow geldStortenWindow = new GeldStortenWindow(rekeningnummer);
            geldStortenWindow.Closed += (s, args) => UpdateData();  // Update saldo en transacties nadat het stortingsvenster wordt gesloten
            geldStortenWindow.Show();
        }

        // Toon de laatste transacties
        private void LaatsteTransacties_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Laatste transacties worden hier weergegeven.");
        }
    }
}
