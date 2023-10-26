using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptovayaBazaMDK
{
    public partial class Form2 : Form
    {
        private SQLiteConnection connection;
        public Form2()
        {
            InitializeComponent();

            // Инициализация подключения к базе данных MenuPrice.db
            string menuDbPath = "MenuPrice.db";
            bool createMenuTable = false;

            if (!System.IO.File.Exists(menuDbPath))
            {
                createMenuTable = true;
                SQLiteConnection.CreateFile(menuDbPath);
            }

            connection = new SQLiteConnection($"Data Source={menuDbPath};");

            connection.Open();

            if (createMenuTable)
            {
                using (SQLiteCommand cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Menu (Id INTEGER PRIMARY KEY, Name TEXT, Price REAL);";
                    cmd.ExecuteNonQuery();

                    // Вставляем начальные данные в таблицу Menu
                    InsertInitialMenuData(cmd);
                }
            }

            // Загрузка данных из Menu в DataGridView
            LoadMenuDataToDataGridView();
        }

        // Метод для вставки начальных данных в таблицу Menu
        private void InsertInitialMenuData(SQLiteCommand cmd)
        {
            string[] initialItems = {
                "Сыр, 1 кг", "Колбаса, 1 кг", "Вода, 1 л", "Макароны, 1 кг", "Гречневая крупа, 1 кг", "Специи", "Лук, 1 кг", "Картофель, 1 кг", "Морковь, 1 кг", "Чеснок, 1 кг", "Манная крупа, 1 кг", "Кукуруза консервированная", "Горох консервированный", "Сардины консервированные", "Килька консервированная", "Перловая крупа, 1 кг", "Бананы, 1 кг", "Яблоки, 1 кг", "Треска, 1кг"
            };

            double[] initialPrices = { 239, 314, 23, 69, 54, 190, 34, 66, 47, 71, 42, 67, 91, 37, 90, 213, 82, 75, 75 };

            for (int i = 0; i < initialItems.Length; i++)
            {
                cmd.CommandText = "INSERT INTO Menu (Name, Price) VALUES (@Name, @Price);";
                cmd.Parameters.AddWithValue("@Name", initialItems[i]);
                cmd.Parameters.AddWithValue("@Price", initialPrices[i]);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
        }

        private void LoadMenuDataToDataGridView()
        {
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Menu", connection))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Создаем столбец DataGridViewButtonColumn
                    DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                    buttonColumn.HeaderText = "Добавить";
                    buttonColumn.Text = "Добавить";
                    buttonColumn.UseColumnTextForButtonValue = true;
                    dataGridView1.Columns.Add(buttonColumn);

                    // Устанавливаем источник данных для DataGridView
                    dataGridView1.DataSource = dataTable;
                }
            }

            // Установите ширину каждого столбца в DataGridView
            dataGridView1.Columns[0].Width = 90;
            dataGridView1.Columns[1].Width = 40;
            dataGridView1.Columns[2].Width = 230;
            dataGridView1.Columns[3].Width = 40;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что событие произошло в столбце кнопок (колонке с индексом 0)
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                // Получаем значение ячейки, которая была нажата
                object cellValue = dataGridView1.Rows[e.RowIndex].Cells[3].Value; // 3 - индекс столбца с числовым значением

                // Проверяем, что значение не пустое
                if (cellValue != null)
                {
                    if (double.TryParse(cellValue.ToString(), out double cellDoubleValue))
                    {
                        // Теперь добавим соответствующую строку в ListBox1
                        string selectedRowText = $"[{dataGridView1.Rows[e.RowIndex].Cells[1].Value}] `{dataGridView1.Rows[e.RowIndex].Cells[2].Value}` - ({dataGridView1.Rows[e.RowIndex].Cells[3].Value}) руб.";
                        checkedListBox1.Items.Add(selectedRowText, true);
                    }
                }
            }
        }

        private void UpdateCheckedListBoxCount()
        {
            // Создадим словарь для отслеживания количества каждой строки и сумму всех результатов
            Dictionary<string, int> checkedItemsCount = new Dictionary<string, int>();
            double totalResultSum = 0;

            // Пройдемся по всем элементам CheckedListBox, включая невыделенные
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                string itemText = checkedListBox1.Items[i].ToString();

                if (checkedListBox1.GetItemChecked(i))
                {
                    if (checkedItemsCount.ContainsKey(itemText))
                    {
                        checkedItemsCount[itemText]++;
                    }
                    else
                    {
                        checkedItemsCount[itemText] = 1;
                    }
                }
            }

            // Очистим ListBoxResult
            listBoxResult.Items.Clear();

            // Выведем результаты подсчета и вычислим сумму результатов
            foreach (var kvp in checkedItemsCount)
            {
                string itemText = kvp.Key;
                int count = kvp.Value;

                // Ищем число в круглых скобках
                int startIndex = itemText.IndexOf('(');
                int endIndex = itemText.IndexOf(')');
                double numberInParentheses = 0;

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    string numberStr = itemText.Substring(startIndex + 1, endIndex - startIndex - 1);
                    if (double.TryParse(numberStr, out numberInParentheses))
                    {
                        // Вычисляем произведение числа в скобках на количество
                        double result = numberInParentheses * count;

                        string resultText = $"{itemText} - {count} шт.   ИТОГО: {result}";
                        listBoxResult.Items.Add(resultText);

                        // Добавляем результат в общую сумму
                        totalResultSum += result;
                    }
                }
            }

            // Выводим сумму результатов в label2
            label3.Text = $"К оплате: {totalResultSum} рублей.";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Обработка изменений в CheckedListBox1
            UpdateCheckedListBoxCount();

            Form3 form3 = new Form3();
            form3.SetLabel1Text(label3.Text);
            form3.SetListBoxResult(listBoxResult);
            form3.Show();
            this.Hide();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Если запись найдена, откройте Form2
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }
    }
}
