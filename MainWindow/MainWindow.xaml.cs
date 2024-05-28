using MainWindow.Pages;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Data.SQLite;

namespace MainWindow
{
    public partial class GameEngine : Window
    {

        public GameEngine()
        {
            InitializeComponent();
            MainFrame.Navigate(new Hub(MainFrame));


            string connectionString = "Data Source=UnoAccount.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Створення таблиці
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Users (
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            NickName TEXT NOT NULL,
                                            Password TEXT NOT NULL,
                                            Volume INTEGER NOT NULL)";
                using (SQLiteCommand createTableCmd = new SQLiteCommand(createTableQuery, connection))
                {
                    createTableCmd.ExecuteNonQuery();
                }
                connection.Close();

            }
        }

        
    }
}
