using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Scheduler
{
    public partial class dailySchedule : Form
    {
        SqlConnection connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=schedulerDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        string nameOfStore;
        string dayOfWeek;
        public dailySchedule()
        {
            InitializeComponent();
        }
        public void setUpInfo(String date, string store, string day)
        {
            label1.Text = date;
            nameOfStore = store;
            dayOfWeek = day;
            displayData();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        void displayData()
        {
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            string today = nameOfStore.Trim() + dayOfWeek;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from [" + today + "]";
            cmd.ExecuteNonQuery();
            DataTable dta = new DataTable();
            SqlDataAdapter datadp = new SqlDataAdapter(cmd);
            datadp.Fill(dta);
            dataGridView1.DataSource = dta;
            displayNotScheduled(connection);
            connection.Close();
        }

        void displayNotScheduled(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            string today = nameOfStore.Trim() + dayOfWeek;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from [" + nameOfStore.Trim() + "]";
            cmd.ExecuteNonQuery();
            DataTable dataTab = new DataTable();
            SqlDataAdapter datadp = new SqlDataAdapter(cmd);
            datadp = new SqlDataAdapter(cmd);
            datadp.Fill(dataTab);
            dataGridView2.DataSource = dataTab;
            getRidOfScheduled();
            dataGridView2.Columns[0].Visible = false;
            dataGridView2.Columns[2].Visible = false;
            dataGridView2.Columns[3].Visible = false;
            for (int i = 4; i < dataGridView2.ColumnCount - 2; i++)
            {
                if (dayOfWeek.Trim().Substring(0, dayOfWeek.Length - 3) ==
                    dataGridView2.Columns[i].HeaderText.Trim().Substring(0, dataGridView2.Columns[i].HeaderText.Trim().Length - 7))
                {
                    i += 3;
                }
                else
                {

                    dataGridView2.Columns[i].Visible = false;
                }
            }
            dataGridView2.Columns[dataGridView2.ColumnCount - 2].HeaderText = "Week 1 Hours";
            dataGridView2.Columns[dataGridView2.ColumnCount - 1].HeaderText = "Week 2 Hours";
        }

        void getRidOfScheduled()
        {
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                for (int j = 0; j < dataGridView2.RowCount - 1; j++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim() == 
                        dataGridView2.Rows[j].Cells[1].Value.ToString().Trim())
                    {
                        dataGridView2.Rows.RemoveAt(j) ;
                    }
                }
            }
        }



        private void dailySchedule_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            string today = nameOfStore.Trim() + dayOfWeek;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "DELETE from " + today;
            cmd.ExecuteNonQuery();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                cmd.CommandText = "insert into " + today + "(employee,beginning,ending) values ('" +
                    dataGridView1.Rows[i].Cells[0].Value + "','" + dataGridView1.Rows[i].Cells[1].Value + "','" +
                    dataGridView1.Rows[i].Cells[2].Value + "')";
                cmd.ExecuteNonQuery();
            }
            displayNotScheduled(connection);
            connection.Close();
            displayData();
  
            /*cmd.CommandText = "select COUNT(*) from [" + today + "]";
            int rowCount = (Int32) cmd.ExecuteScalar();*/
        }
    }
}
