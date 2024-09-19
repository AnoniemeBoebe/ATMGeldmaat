using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ATMApp
{
    public partial class AdminLoginWindow : Window
    {
        public AdminLoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (ValidateAdminLogin(username, password))
            {
                // Geen meldingen, direct naar het AdminWindow
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
                this.Close();
            }
            else
            {
                // Alleen een melding bij een foutieve login
                MessageBox.Show("Ongeldige gebruikersnaam of wachtwoord.");
            }
        }

        private bool ValidateAdminLogin(string username, string password)
        {
            bool isValid = false;
            string connectionString = "server=localhost;database=geldmaat;uid=root;pwd=;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT password_hashed FROM admins WHERE username = @username";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    var result = cmd.ExecuteScalar();

                    if (result != null && BCrypt.Net.BCrypt.Verify(password, result.ToString()))
                    {
                        isValid = true;
                    }
                }
            }
            return isValid;
        }
    }
}
