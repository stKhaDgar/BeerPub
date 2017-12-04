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

                // Displaying Database Cells on the Screen
                while (await sqlReader.ReadAsync())
                {
                    DataRow r = dt.NewRow();
                    r["ID"] = Convert.ToString(sqlReader["ID"]);
                    r["Продукт"] = Convert.ToString(sqlReader["Продукт"]);
                    r["Объём"] = Convert.ToString(sqlReader["Объём"]);
                    r["Цена"] = Convert.ToString(sqlReader["Цена"]);
                    dt.Rows.Add(r);
                }
                dataGridView1.DataSource = dt;

                // Prohibition on sorting the columns of a table
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                // Allow edit column "Количество"
                dataGridView1.Columns[4].ReadOnly = true;
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
    }
}
