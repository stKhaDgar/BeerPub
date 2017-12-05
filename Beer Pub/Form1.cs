using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Beer_Pub
{
    public partial class Form1 : Form
    {
        SqlConnection sqlConnection;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            
            // Connecting the database to the project
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\VS C++\Проэкты\Beer Pub\Beer Pub\Database.mdf;Integrated Security=True";
            sqlConnection = new SqlConnection(connectionString);
            await sqlConnection.OpenAsync();

            // Getting data from the database
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Products]", sqlConnection);

            try
            {
                sqlReader = await command.ExecuteReaderAsync();

                // Required columns for the table
                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("Продукт");
                dt.Columns.Add("Объём");
                dt.Columns.Add("Цена");
                int countRows = 1;

                // Displaying Database Cells on the Screen
                while (await sqlReader.ReadAsync())
                {
                    DataRow r = dt.NewRow();
                    r["ID"] = Convert.ToString(sqlReader["ID"]);
                    r["Продукт"] = Convert.ToString(sqlReader["Продукт"]);
                    r["Объём"] = Convert.ToString(sqlReader["Объём"]);
                    r["Цена"] = Convert.ToString(sqlReader["Цена"]);
                    dt.Rows.Add(r);
                    dataGridView2.Rows.Add();
                    dataGridView2.Rows[countRows - 1].Cells[0].Value = 0;
                    countRows++;
                }
                // Определение для второй таблицы
                
                dataGridView2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

                dataGridView1.DataSource = dt;
                countRows *= 20;
                tabPage1.AutoScrollMinSize = new System.Drawing.Size(0, countRows+7);

                // Prohibition on sorting the columns of a table
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Allow edit column
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.Columns[3].ReadOnly = true;
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[0].Width = 20;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close the flow
                if (sqlReader != null)
                    sqlReader.Close();
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                sqlConnection.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlConnection != null && sqlConnection.State != ConnectionState.Closed)
                sqlConnection.Close();
        }

        // Срабатывает при нажатии на "Расчитать"
        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            int countRow = 0;
            double priceAll = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (Convert.ToDouble(row.Cells[0].Value.ToString()) != 0)
                {
                    double PriceNew = Convert.ToDouble(dataGridView1.Rows[countRow].Cells[3].Value.ToString());
                    row.Cells[1].Value = PriceNew * Convert.ToDouble(row.Cells[0].Value.ToString());

                    // Удаление символов из строки "Объём"
                    char[] mass = dataGridView1.Rows[countRow].Cells[2].Value.ToString().ToCharArray();
                    string numberSize = "";
                    foreach(char h in mass)
                    {
                        if(char.IsDigit(h) == true)
                        {
                            numberSize += h;
                        }
                    }
                    // Добавление нового айтема в лист
                    listBox1.Items.Add(dataGridView1.Rows[countRow].Cells[1].Value.ToString() + "    -    " + Convert.ToDouble(row.Cells[0].Value.ToString()) * Convert.ToDouble(numberSize) + "    -    " + row.Cells[1].Value.ToString());
                    
                    priceAll += Convert.ToDouble(row.Cells[1].Value.ToString());
                    label2.Text = " =       " + priceAll.ToString();
                }
                else if (Convert.ToDouble(row.Cells[0].Value.ToString()) == 0)
                {
                    row.Cells[1].Value = null;
                }
                countRow++;
            }
            if (listBox1.Items.Count == 0)
            {
                listBox1.Items.Add("Вы ничего не выбрали.");
            }
        }

        // Необходимо для ввода только цифровых значений в 1 колонку второй таблицы
        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex == 0)
            {
                TextBox tb = (TextBox)e.Control;
                tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
            }
            else
            {
                TextBox tb = (TextBox)e.Control;
                tb.KeyPress -= tb_KeyPress;
            }
        }
        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!Char.IsNumber(e.KeyChar) && (e.KeyChar != '-') && (e.KeyChar != ',')))
            {
                if ((e.KeyChar != (char)Keys.Back) || (e.KeyChar != (char)Keys.Delete))
                { e.Handled = true; }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("Вы очистили список.");
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                row.Cells[0].Value = 0;
                row.Cells[1].Value = null;
            }
            label2.Text = "";
        }
    }
}
