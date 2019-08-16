using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Globalization;

namespace Scheduler
{
    public partial class viewEmployees : Form
    {
        Employee newEmployee;
        String nameOfStore;
        SqlConnection connection;

        public viewEmployees()
        {
            InitializeComponent();
            newEmployee.avail.fromOne = new DateTime[7];
            newEmployee.avail.toOne = new DateTime[7];
            newEmployee.avail.fromTwo = new DateTime[7];
            newEmployee.avail.toTwo = new DateTime[7];
            setAvail();
        }

        public void sendOverInfo(String selectedStore, SqlConnection con)
        {
            nameOfStore = selectedStore;
            connection = con;
            display_Data();
        }

        void setAvail()
        {
            for (int i = 0; i < 7; i++)
            {
                newEmployee.avail.fromOne[i] = DateTime.Parse("00:00:00");
                newEmployee.avail.toOne[i] = DateTime.Parse("00:00:00");
                newEmployee.avail.fromTwo[i] = DateTime.Parse("00:00:00");
                newEmployee.avail.toTwo[i] = DateTime.Parse("00:00:00");
            }
        }

        void getEmployeeListInfo()
        {
            int rowCount = dataGridView1.RowCount;
            int counter = 0;
            /*while (listCount < rowCount)
            {
                list[listCount].employeeId = (int)dataGridView1.Rows[listCount].Cells[counter].Value;
                counter++;

                list[listCount].name = dataGridView1.Rows[listCount].Cells[counter].Value.ToString();
                counter++;

                //list[listCount].pay = float.Parse(dataGridView1.Rows[listCount].Cells[counter].Value.ToString, CultureInfo.InvariantCulture.NumberFormat);
                counter++;

                list[listCount].age = (int)dataGridView1.Rows[listCount].Cells[counter].Value;
                counter++;

                for (int i = 0; i < 7; i++)
                {
                    list[listCount].avail.fromOne[i] = DateTime.Parse(dataGridView1.Rows[listCount].Cells[counter].Value.ToString());
                    counter += 4;
                }

                counter = 5;

                for (int i = 0; i < 7; i++)
                {
                    list[listCount].avail.toOne[i] = DateTime.Parse(dataGridView1.Rows[listCount].Cells[counter].Value.ToString());
                    counter += 4;
                }

                counter = 6;

                for (int i = 0; i < 7; i++)
                {
                    list[listCount].avail.fromTwo[i] = DateTime.Parse(dataGridView1.Rows[listCount].Cells[counter].Value.ToString());
                    counter += 4;
                }

                counter = 7;

                for (int i = 0; i < 7; i++)
                {
                    list[listCount].avail.toTwo[i] = DateTime.Parse(dataGridView1.Rows[listCount].Cells[counter].Value.ToString());
                    counter += 4;
                }
                listCount++;
            }*/
        }

        public void setEmployee()
        {
            int cellNumber = 4;
            int selectedRow = dataGridView1.SelectedCells[0].RowIndex;
            newEmployee.name = dataGridView1.Rows[selectedRow].Cells[1].Value.ToString();
            newEmployee.pay = float.Parse(dataGridView1.Rows[selectedRow].Cells[2].Value.ToString(), CultureInfo.InvariantCulture.NumberFormat);
            newEmployee.age = (int)dataGridView1.Rows[selectedRow].Cells[3].Value;
            for (int i = 0; i < 7; i++)
            {
                newEmployee.avail.fromOne[i] = DateTime.Parse(dataGridView1.Rows[selectedRow].Cells[cellNumber * (i+1)].Value.ToString());
                newEmployee.avail.toOne[i] = DateTime.Parse(dataGridView1.Rows[selectedRow].Cells[(cellNumber * (i + 1)) + 1].Value.ToString());
                newEmployee.avail.fromTwo[i] = DateTime.Parse(dataGridView1.Rows[selectedRow].Cells[(cellNumber * (i + 1)) + 2].Value.ToString());
                newEmployee.avail.toTwo[i] = DateTime.Parse(dataGridView1.Rows[selectedRow].Cells[(cellNumber * (i + 1)) + 3].Value.ToString());
            }
        }

        public void display_Data()
        {
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from [" + nameOfStore + "]";
            cmd.ExecuteNonQuery();
            DataTable dta = new DataTable();
            SqlDataAdapter datadp = new SqlDataAdapter(cmd);
            datadp.Fill(dta);
            dataGridView1.DataSource = dta;
            for (int i = 4; i < 32; i++)
            {
                dataGridView1.Columns[i].Visible = false;
            }
            connection.Close();
        }

        public void swapEmployees(ref Employee x, ref Employee y)
        {
            Employee temp = y;
            y = x;
            x = temp;
        }
        public void sortEmployeeList(ref Employee[] names, int x)
        {
            for (int i = x-1; i >= 0; i--)
            {
                for (int j = i-1; j >= 0; j--)
                {
                    if (names[i].name[0] < names[j].name[0])
                    {
                        swapEmployees(ref names[i], ref names[j]);
                    }

                }

            }
        }

        

        public void sendBackInfo(ref Employee[] names, ref int x)
        {
            //names = list;
            //x = listCount;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            //dataGridView2.Rows.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "delete from [" + nameOfStore + "] where employeeId = '" + dataGridView1.SelectedCells[0].Value + "'";
            cmd.ExecuteNonQuery();
            connection.Close();
            textBox1.Text = "";
            textBox2.Text = "";
            display_Data();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("You did not enter in the required information");
            }
            else
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "insert into [" + nameOfStore + "] (name,Pay,MaxPerWeek," +
                    "[Sunday from 1],[Sunday to 1],[Sunday from 2],[Sunday to 2]," +
                    "[Monday from 1],[Monday to 1],[Monday from 2],[Monday to 2]," +
                    "[Tuesday from 1],[Tuesday to 1],[Tuesday from 2],[Tuesday to 2]," +
                    "[Wednesday from 1],[Wednesday to 1],[Wednesday from 2],[Wednesday to 2]," +
                    "[Thursday from 1],[Thursday to 1],[Thursday from 2],[Thursday to 2]," +
                    "[Friday from 1],[Friday to 1],[Friday from 2],[Friday to 2]," +
                    "[Saturday from 1],[Saturday to 1],[Saturday from 2],[Saturday to 2]) " +
                    "values ('" + textBox1.Text.Trim() + "','" + textBox2.Text + "','" + textBox3.Text;

                for (int i = 0; i < 7; i++)
                {
                    cmd.CommandText = cmd.CommandText + "','" + newEmployee.avail.fromOne[i].ToShortTimeString();
                    cmd.CommandText = cmd.CommandText + "','" + newEmployee.avail.toOne[i].ToShortTimeString();
                    cmd.CommandText = cmd.CommandText + "','" + newEmployee.avail.fromTwo[i].ToShortTimeString();
                    cmd.CommandText = cmd.CommandText + "','" + newEmployee.avail.toTwo[i].ToShortTimeString();
                }
                cmd.CommandText = cmd.CommandText + "')";
                cmd.ExecuteNonQuery();
                connection.Close();
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                display_Data();
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            dataGridView1.Hide();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            availabilityForm avail = new availabilityForm();
            avail.setText("Availability","enter times when you are NOT available","open availability");
            this.Hide();
            avail.ShowDialog();
            avail.sendBackHours(ref newEmployee.avail);
            this.Show();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
