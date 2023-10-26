using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Xml.Linq;

namespace OptovayaBazaMDK
{
    public partial class Form1 : Form
    {
        private SQLiteConnection connection;
        private string LogPLH = "Ваш логин";
        private string PassPLH = "Ваш пароль";

        public Form1()
        {
            InitializeComponent();
            pReg.Hide();

            // Инициализация подключения к базе данных
            string dbPath = "UserLogin.db";
            bool createTable = false;

            if (!System.IO.File.Exists(dbPath))
            {
                createTable = true;
                SQLiteConnection.CreateFile(dbPath);
            }

            connection = new SQLiteConnection($"Data Source={dbPath};"); // Используйте поле класса

            connection.Open();

            if (createTable)
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "CREATE TABLE Users (Login TEXT, Password TEXT);";
                    cmd.ExecuteNonQuery();

                    // Вставляем значения User и 123 в первую строку
                    cmd.CommandText = "INSERT INTO Users (Login, Password) VALUES ('User', '123');";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE Admins (Login TEXT, Password TEXT);";
                    cmd.ExecuteNonQuery();

                    // Вставляем значения User и 123 в первую строку
                    cmd.CommandText = "INSERT INTO Admins (Login, Password) VALUES ('Admin', '123');";
                    cmd.ExecuteNonQuery();
                }
            }
            PlaceholderAdder();
        }

        private void PlaceholderAdder()
        {
            tbUser.AddPlaceholder(LogPLH);
            tbPass.AddPlaceholder(PassPLH);
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            if (tbUser.Text != LogPLH || tbPass.Text != PassPLH)
            {
                string username = tbUser.Text; // Получаем имя пользователя из элемента управления
                string password = tbPass.Text; // Получаем пароль из элемента управления

                // Попробуйте искать в таблице Admins
                if (CheckCredentialsInTable(username, password, "Admins"))
                {
                    // Если запись найдена, откройте Form3
                    Form5 form5 = new Form5();
                    form5.Show();
                    this.Hide();
                }
                // Если не найдено в Admins, попробуйте в таблице Users
                else if (CheckCredentialsInTable(username, password, "Users"))
                {
                    // Если запись найдена, откройте Form2
                    Form2 form2 = new Form2();
                    form2.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Неверные учетные данные. Пожалуйста, попробуйте снова.");
                }
            }
            else
            {
                MessageBox.Show("Неверные учетные данные.");
            }
        }

        private bool CheckCredentialsInTable(string username, string password, string tableName)
        {
            string query = $"SELECT * FROM {tableName} WHERE Login = @username AND Password = @password";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable.Rows.Count > 0;
                }
            }
        }


        private void btnReg_Click(object sender, EventArgs e)
        {
            if (tbUser.Text != LogPLH || tbPass.Text != PassPLH)
            {
                string username = tbUser.Text; // Получаем имя пользователя из элемента управления
                string password = tbPass.Text; // Получаем пароль из элемента управления

                // Добавление новой записи в базу данных
                string insertQuery = "INSERT INTO Users (Login, Password) VALUES (@username, @password)";
                using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@username", username);
                    insertCommand.Parameters.AddWithValue("@password", password);

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Регистрация прошла успешно.");
                    }
                    else
                    {
                        MessageBox.Show("Не удалось зарегистрироваться. Пожалуйста, попробуйте снова.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Не удалось зарегистрироваться.");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Закрытие подключения к базе данных при закрытии формы
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            pLog.Hide();
            pReg.Show();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            pReg.Hide();
            pLog.Show();
        }
    }
}
