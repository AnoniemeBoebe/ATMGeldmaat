using System.Diagnostics;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ATMApp
{
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();
            
        }

        // Deze methode wordt aangeroepen wanneer de admin op de 'Inloggen' knop klikt
        private void AdminLogin_Click(object sender, RoutedEventArgs e)
        {
            // Haal de ingevoerde gebruikersnaam en wachtwoord op uit de invoervelden
            string adminUsername = txtAdminUsername.Text.Trim();  // Verwijder spaties aan het begin en einde
            string adminPassword = txtAdminPassword.Password.Trim();  // Verwijder spaties aan het begin en einde

            // Roep de methode aan om te controleren of de inloggegevens correct zijn
            if (ValidateAdminLogin(adminUsername, adminPassword))
            {
                MessageBox.Show("Inloggen succesvol!");
                AdminWindow adminWindow = new AdminWindow();  // Open het adminbeheer venster
                adminWindow.Show();
                this.Close();  // Sluit het huidige inlogvenster
            }
            else
            {
                MessageBox.Show("Ongeldige gebruikersnaam of wachtwoord.");
            }
        }

        // Deze methode controleert of het ingevoerde wachtwoord overeenkomt met het opgeslagen wachtwoord
        private bool ValidateAdminLogin(string adminUsername, string adminPassword)
        {
            bool isValid = false;
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT password_hashed FROM admins WHERE username = @adminUsername";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@adminUsername", adminUsername);
                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string storedPassword = result.ToString();

                   
                       
                        //Debug.WriteLine(BCrypt.Net.BCrypt.HashPassword("123"));
                        // Test eenvoudige vergelijking zonder bcrypt
                        bool isMatch = BCrypt.Net.BCrypt.Verify(adminPassword, storedPassword);
                        

                        if (isMatch)
                        {
                            isValid = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Geen wachtwoord gevonden voor deze gebruikersnaam.");
                    }
                }
            }
            return isValid;
        }

    }



    }

