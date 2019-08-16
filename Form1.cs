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
    public struct WeeklyHours
    {
        public String[] daysOfWeek;//"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday";
        public DateTime[] fromOne;
        public DateTime[] toOne;
        public DateTime[] fromTwo;
        public DateTime[] toTwo;
        public bool[] open;

    }

    public struct Employee
    {
        public int employeeId;
        public String name;
        public int age;
        public float pay;
        public WeeklyHours avail;
    }

    public struct Store
    {
        public String nameOfStore;
        public float maxDailyHours;
        public float[] maxWeeklyHours;
        public bool lunchBreak;
        public int lunchDuration;
        public int breakLength;
        public int maxBeforeLunch;
        public WeeklyHours storeHours;
    }


    public partial class Form1 : Form
    {
        SqlConnection connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=schedulerDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        Employee[] employees = new Employee[100];
        String selectedStore;
        int numOfEmployees = 0;
        Store currentStore;
        String[] daysOfWeek;
        public Form1()
        {

            InitializeComponent();
            //connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            //cmd.CommandText = "ALTER DATABASE testLocalDb SET ONLINE";
            //cmd.ExecuteNonQuery();
            //connection.Close();

            display_Data();
            comboBox1.Items.AddRange(GetAllStores());
            //dataGridView1.Hide();
            dataGridView2.Hide();
            displayDates();
            daysOfWeek = getDaysOfWeek();
        }

        public void display_Data()
        {
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from storeTable";
            cmd.ExecuteNonQuery();
            DataTable dta = new DataTable();
            SqlDataAdapter datadp = new SqlDataAdapter(cmd);
            datadp.Fill(dta);
            dataGridView1.DataSource = dta;
            connection.Close();

        }

        void displayEmployeeData()
        {
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from ["+ selectedStore.Trim() +"]";
            cmd.ExecuteNonQuery();
            DataTable dta = new DataTable();
            SqlDataAdapter datadp = new SqlDataAdapter(cmd);
            datadp.Fill(dta);
            dataGridView2.DataSource = dta;
            connection.Close();
        }

        void displayDates()
        {
            DateTime x = DateTime.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek);
            int i = 23;
            int j = 0;
            Button[] dates = new Button[34];
            foreach (Control ctrl in Controls)
            {
                if (ctrl is Button)
                {
                    i--;
                    dates[i] = (Button)ctrl;

                }
            }




            for (i = 0; i < 20; i++)
            {
                if (dates[i].Text == "")
                {
                    dates[i].Text = x.AddDays(j).DayOfWeek.ToString() + Environment.NewLine + x.AddDays(j).Date.ToString("MM/dd/yy");
                    j++;
                }
                else
                {

                }
            }
        }

        string[] GetAllStores()
        {
            string[] stores = new string[dataGridView1.RowCount - 1];
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                stores[i] = dataGridView1.Rows[i].Cells[1].Value.ToString();
            }
            return stores;
        }

        string[] getDaysOfWeek()
        {
            string[] days = new string[14];
            days[0] = "SundayOne";
            days[2] = "MondayOne";
            days[4] = "TuesdayOne";
            days[6] = "WednesdayOne";
            days[8] = "ThursdayOne";
            days[10] = "FridayOne";
            days[12] = "SaturdayOne";
            days[1] = "SundayTwo";
            days[3] = "MondayTwo";
            days[5] = "TuesdayTwo";
            days[7] = "WednesdayTwo";
            days[9] = "ThursdayTwo";
            days[11] = "FridayTwo";
            days[13] = "SaturdayTwo";
            return days;
        }

        int findStore(string storeName)
        {
            int rowNumber = 99;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value.ToString() == storeName)
                {
                    rowNumber = i;
                    i = dataGridView1.RowCount;
                }
            }
            return rowNumber;
        }

        void setArray(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 999;
            }
        }

        int getDayIndex(string day)
        {
            for (int i = 0; i < 14; i++)
            {
                if (day.Trim() == daysOfWeek[i].Trim())
                {
                    return (i/2);
                }
            }

                return 99;
        }

        void setUsed(int[] used, int[] maxed,ref int usedCount)
        {
            while (maxed[usedCount] != 999)
            {
                used[usedCount] = maxed[usedCount];
                usedCount++;
            }
        }

        void setDaySchedule(string day, float todaysHours, float maxDayHours)
        {
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            int storeRow = findStore(selectedStore);//employee sundayFromOne starts at index 4, store SundayFromOne starts at index 7
            int dayIndex = getDayIndex(day);

            int[] used = new int[dataGridView2.RowCount - 1];
            setArray(used);
            int usedIndex = 0;

            int[] tempUsed = new int[dataGridView2.RowCount - 1];
            setArray(tempUsed);
            int tempUsedIndex = 0;

            int employee;
            float employeeWeekHours;
            int compare;
            int week;
            bool up = true;

            DateTime start = DateTime.Parse(dataGridView1.Rows[storeRow].Cells[7+(4*dayIndex)].Value.ToString());
            DateTime end = DateTime.Parse(dataGridView1.Rows[storeRow].Cells[8+(4*dayIndex)].Value.ToString());
            DateTime shiftBegin = start;
            DateTime shiftEnd = shiftBegin.AddHours(maxDayHours);

            DateTime employeeFromOne;
            DateTime employeeToOne;
            DateTime employeeFromTwo;
            DateTime employeeToTwo;

            if (day.Contains("One"))
            {
                week = 32;
            }
            else
            {
                week = 33;
            }

            while ((todaysHours >= maxDayHours) && (usedIndex + tempUsedIndex < used.Length) && (start != end))
            {
                employee = randomNumber(dataGridView2.RowCount - 1);
                
                employeeWeekHours = float.Parse(dataGridView2.Rows[employee].Cells[week].Value.ToString());
                
                if (todaysHours % maxDayHours >= 3)
                {
                    shiftEnd = shiftBegin.AddHours(todaysHours % maxDayHours);
                }
                else
                {
                    
                }

                employeeFromOne = DateTime.Parse(dataGridView2.Rows[employee].Cells[4+dayIndex].Value.ToString());
                employeeToOne = DateTime.Parse(dataGridView2.Rows[employee].Cells[dayIndex + 5].Value.ToString());
                employeeFromTwo = DateTime.Parse(dataGridView2.Rows[employee].Cells[dayIndex + 6].Value.ToString());
                employeeToTwo = DateTime.Parse(dataGridView2.Rows[employee].Cells[dayIndex + 7].Value.ToString());
                if (isUsed(used, employee) == true)
                {

                }
                else if (isUsed(tempUsed, employee) == true)
                {

                }
                else if (employeeWeekHours + maxDayHours > float.Parse(dataGridView2.Rows[employee].Cells[3].Value.ToString()))
                {
                    used[usedIndex] = employee;
                    usedIndex++;
                }
                else if (DateTime.Compare(shiftBegin, employeeFromOne) > 0 && DateTime.Compare(shiftEnd, employeeFromOne) < 0)
                {
                    tempUsed[tempUsedIndex] = employee;
                    tempUsedIndex++;
                }
                else if ((DateTime.Compare(shiftBegin, employeeToOne) >= 0 && DateTime.Compare(shiftEnd, employeeToOne) <= 0))
                {
                    tempUsed[tempUsedIndex] = employee;
                    tempUsedIndex++;
                }
                else if ((DateTime.Compare(shiftBegin, employeeFromTwo) >= 0 && DateTime.Compare(shiftEnd, employeeFromTwo) <= 0))
                {
                    tempUsed[tempUsedIndex] = employee;
                    tempUsedIndex++;
                }
                else if ((DateTime.Compare(shiftBegin, employeeToTwo) >= 0 && DateTime.Compare(shiftEnd, employeeToTwo) <= 0))
                {
                    tempUsed[tempUsedIndex] = employee;
                    tempUsedIndex++;
                }
                else if ((DateTime.Compare(shiftBegin, employeeFromOne) >= 0 && DateTime.Compare(shiftBegin, employeeToOne) <= 0))
                {
                    tempUsed[tempUsedIndex] = employee;
                    tempUsedIndex++;
                }
                else if ((DateTime.Compare(shiftEnd, employeeFromOne) >= 0 && DateTime.Compare(shiftEnd, employeeToOne) <= 0))
                {
                    tempUsed[tempUsedIndex] = employee;
                    tempUsedIndex++;
                }
                else if ((DateTime.Compare(shiftBegin, employeeFromTwo) >= 0 && DateTime.Compare(shiftBegin, employeeToTwo) <= 0))
                {
                    tempUsed[tempUsedIndex] = employee;
                    tempUsedIndex++;
                }
                else if ((DateTime.Compare(shiftEnd, employeeFromTwo) >= 0 && DateTime.Compare(shiftEnd, employeeToTwo) <= 0))
                {
                    tempUsed[tempUsedIndex] = employee;
                    tempUsedIndex++;
                }
                else
                {
                    used[usedIndex] = employee;
                    usedIndex++;
                    cmd.CommandText = "UPDATE [" + selectedStore.Trim() + "] SET ";
                    if (week == 32)
                    {
                        cmd.CommandText += "weekOneHours = "; 
                    }
                    else
                    {
                        cmd.CommandText += "weekTwoHours = ";
                    }
                    

                    if (todaysHours % maxDayHours >= 3)
                    {
                        cmd.CommandText += (employeeWeekHours += (todaysHours % maxDayHours)).ToString() + 
                            " WHERE employeeId = " + (employee+1).ToString();
                        todaysHours -= (todaysHours % maxDayHours);
                    }
                    else
                    {
                        cmd.CommandText += (employeeWeekHours += maxDayHours).ToString() +
                            " WHERE employeeId = " + (employee+1).ToString();
                        todaysHours -= maxDayHours;
                    }
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "insert into [" + selectedStore.Trim() + day + "] (employee,beginning,ending) " +
                        "values ('" + dataGridView2.Rows[employee].Cells[1].Value.ToString().Trim() + "','" + shiftBegin.ToShortTimeString() +
                        "','" + shiftEnd.ToShortTimeString() + "')";
                    cmd.ExecuteNonQuery();// prob here

                    if (up)
                    {
                        shiftBegin = shiftEnd;
                        shiftEnd = shiftEnd.AddHours(maxDayHours);
                    }
                    else
                    {
                        shiftEnd = shiftBegin;
                        shiftBegin = shiftBegin.AddHours(-maxDayHours);
                    }

                    if (DateTime.Compare(shiftEnd, end) > 0)
                    {
                        shiftEnd = end;
                        shiftBegin = shiftEnd.AddHours(-maxDayHours);
                        up = false;
                    }
                    else if (DateTime.Compare(shiftBegin, start) < 0)
                    {
                        shiftBegin = start;
                        shiftEnd = shiftBegin.AddHours(maxDayHours);
                        up = true;
                    }
                    setArray(tempUsed);
                    tempUsedIndex = 0;
                }

            }
            connection.Close();
            displayEmployeeData();

        }

        bool isUsed(int[] usedArray,int usedIndex)
        {
            for (int i = 0; i < usedArray.Length - 1; i++)
            {
                if (usedArray[i] == usedIndex)
                {
                    return true;
                }
            }
            return false;
        }

        int randomNumber(int opperand)
        {
            DateTime x = DateTime.Now;
            return (x.Millisecond % opperand);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            
            viewEmployees view = new viewEmployees();
            view.sendOverInfo(selectedStore, connection);
            this.Hide();
            view.ShowDialog();
            view.sendBackInfo(ref employees, ref numOfEmployees);
            this.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            for (int i = 0; i < 14; i+=2)
            {
                cmd.CommandText = "DELETE from [" + selectedStore.Trim() + daysOfWeek[i] + "]";
                cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                cmd.CommandText = "UPDATE [" + selectedStore.Trim() + "] SET weekOneHours = 0 WHERE employeeId = " + i.ToString();
                cmd.ExecuteNonQuery();
            }
            connection.Close();
            displayEmployeeData();
            /*
             connection.Open();
             SqlCommand cmd = connection.CreateCommand();
             cmd.CommandType = CommandType.Text;
             if(x)
             {
                for(int i = 0; i < 7; i++)
                {
                    cmd.CommandText = "ALTER TABLE " + selectedStore.Trim() + days[i*2] + " RENAME TO temp" + i.ToString();
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "ALTER TABLE " + selectedStore.Trim() + days[(i*2) + 1] + " RENAME TO " +
                        selectedStore.Trim() + days[i*2];
                    cmd.ExecuteNonQuery();

                    cmd.CommqndText = "ALTER TABLE temp" + i.ToString() + " RENAME TO " + selectedStore.Trim() + days[i*2];
                    cmd.ExecuteNonQuery();
                }
             }
             else
             {
             
             }
             
             connection.Close()
             
             */

        }

        private void button5_Click(object sender, EventArgs e)
        {
            int storeIndex = findStore(selectedStore);
            bool changeStuff = false;
            scheduleSettingsForm scheduleSet = new scheduleSettingsForm();
            scheduleSet.setUpScheduleSettings(dataGridView1, storeIndex);
            this.Hide();
            scheduleSet.ShowDialog();
            scheduleSet.sendBackStoreInfo(ref currentStore, ref changeStuff);
            if (changeStuff)
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE storeTable " +
                    "SET maxHoursWeekOne = " + currentStore.maxWeeklyHours[0] + ", maxHoursWeekTwo = " + currentStore.maxWeeklyHours[1] +
                    ", [sunday from 1] = '" + currentStore.storeHours.fromOne[0].ToShortTimeString() + "', [sunday to 1] = '" + currentStore.storeHours.toOne[0].ToShortTimeString() +
                    "', [monday from 1] = '" + currentStore.storeHours.fromOne[1].ToShortTimeString() + "', [monday to 1] = '" + currentStore.storeHours.toOne[1].ToShortTimeString() +
                    "', [tuesday from 1] = '" + currentStore.storeHours.fromOne[2].ToShortTimeString() + "', [tuesday to 1] = '" + currentStore.storeHours.toOne[2].ToShortTimeString() +
                    "', [wednesday from 1] = '" + currentStore.storeHours.fromOne[3].ToShortTimeString() + "', [wednesday to 1] = '" + currentStore.storeHours.toOne[3].ToShortTimeString() +
                    "', [thursday from 1] = '" + currentStore.storeHours.fromOne[4].ToShortTimeString() + "', [thursday to 1] = '" + currentStore.storeHours.toOne[4].ToShortTimeString() +
                    "', [friday from 1] = '" + currentStore.storeHours.fromOne[5].ToShortTimeString() + "', [friday to 1] = '" + currentStore.storeHours.toOne[5].ToShortTimeString() +
                    "', [saturday from 1] = '" + currentStore.storeHours.fromOne[6].ToShortTimeString() + "', [saturday to 1] = '" + currentStore.storeHours.toOne[6].ToShortTimeString() +
                    "' WHERE Id = " + dataGridView1.Rows[storeIndex].Cells[0].Value;
                cmd.ExecuteNonQuery();
                connection.Close();
                display_Data();
            }
            else
            {

            }
            this.Show();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button31_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button32_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {

            }
            else
            {
                selectedStore = comboBox1.SelectedItem.ToString();
                display_Data();
                if (button1.Enabled == true)
                {

                }
                else
                {
                    Button buttonToEnable;
                    foreach (Control ctrl in Controls)
                    {
                        if (ctrl.Name.StartsWith("button"))
                        {
                            buttonToEnable = (Button)ctrl;
                            buttonToEnable.Enabled = true;

                        }
                    }
                }
                displayEmployeeData();
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            scheduleSettingsForm scheduleSet = new scheduleSettingsForm();
            bool create = false;
            this.Hide();
            scheduleSet.ShowDialog();
            scheduleSet.sendBackStoreInfo(ref currentStore, ref create);
            this.Show();
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            
            cmd.CommandText = "CREATE TABLE [" + currentStore.nameOfStore + "]([employeeId] INT NOT NULL IDENTITY,[name] NCHAR(25) NOT NULL," +
                                "[Pay] FLOAT(53) NOT NULL,[MaxPerWeek] FLOAT NOT NULL," +
                                "[Sunday from 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Sunday to 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Sunday from 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Sunday to 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Monday from 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Monday to 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Monday from 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Monday to 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Tuesday from 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Tuesday to 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Tuesday from 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Tuesday to 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Wednesday from 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Wednesday to 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Wednesday from 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Wednesday to 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Thursday from 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Thursday to 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Thursday from 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Thursday to 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Friday from 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Friday to 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Friday from 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Friday to 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Saturday from 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Saturday to 1] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[Saturday from 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM'),[Saturday to 2] NCHAR(10) NOT NULL DEFAULT('12:00 AM')," +
                                "[weekOneHours] FLOAT NOT NULL DEFAULT 0,[weekTwoHours] FLOAT NOT NULL DEFAULT 0," +
                                "PRIMARY KEY CLUSTERED([employeeId] ASC))";
            
            try
            {
                cmd.ExecuteNonQuery();

                cmd.CommandText = "insert into [storeTable] (storeName,maxEmployeeDailyHours," +
                        "maxHoursWeekOne,maxHoursWeekTwo,maxHoursWeekThree,maxHoursWeekFour," +
                        "[Sunday from 1],[Sunday to 1],[Sunday from 2],[Sunday to 2]," +
                        "[Monday from 1],[Monday to 1],[Monday from 2],[Monday to 2]," +
                        "[Tuesday from 1],[Tuesday to 1],[Tuesday from 2],[Tuesday to 2]," +
                        "[Wednesday from 1],[Wednesday to 1],[Wednesday from 2],[Wednesday to 2]," +
                        "[Thursday from 1],[Thursday to 1],[Thursday from 2],[Thursday to 2]," +
                        "[Friday from 1],[Friday to 1],[Friday from 2],[Friday to 2]," +
                        "[Saturday from 1],[Saturday to 1],[Saturday from 2],[Saturday to 2]," +
                        "coverLunch,lunchLength,breakLength,maxHoursBeforeLunch) " +
                        "values ('" + currentStore.nameOfStore + "','" + currentStore.maxDailyHours + "','" +
                        currentStore.maxWeeklyHours[0] + "','" + currentStore.maxWeeklyHours[1] + "','" +
                        currentStore.maxWeeklyHours[2] + "','" + currentStore.maxWeeklyHours[3];

                for (int i = 0; i < 7; i++)
                {
                    cmd.CommandText = cmd.CommandText + "','" + currentStore.storeHours.fromOne[i].ToShortTimeString();
                    cmd.CommandText = cmd.CommandText + "','" + currentStore.storeHours.toOne[i].ToShortTimeString();
                    cmd.CommandText = cmd.CommandText + "','" + currentStore.storeHours.fromTwo[i].ToShortTimeString();
                    cmd.CommandText = cmd.CommandText + "','" + currentStore.storeHours.toTwo[i].ToShortTimeString();
                }
                cmd.CommandText = cmd.CommandText + "','" + currentStore.lunchBreak + "','" + currentStore.lunchDuration + "','" +
                                    currentStore.breakLength + "','" + currentStore.maxBeforeLunch + "')";
                cmd.ExecuteNonQuery();

                for (int i = 0; i < 14; i++)
                {
                    cmd.CommandText = "CREATE TABLE [" + currentStore.nameOfStore + daysOfWeek[i] + "]([employee] NCHAR (30) NOT NULL," +
                        "[beginning] NCHAR(10) NOT NULL,[ending] NCHAR(10) NOT NULL)";
                    cmd.ExecuteNonQuery();
                }





                /*cmd.CommandText = "insert into [storeTable] (storeName,maxEmployeeDailyHours," +
                        "maxHoursWeekOne,maxHoursWeekTwo,maxHoursWeekThree,maxHoursWeekFour," +
                        "'[Sunday from 1]','[Sunday to 1]','[Sunday from 2]','[Sunday to 2]'," +
                        "'[Monday from 1],'[Monday to 1]','[Monday from 2]','[Monday to 2]'," +
                        "'[Tuesday from 1]','[Tuesday to 1]','[Tuesday from 2]','[Tuesday to 2]'," +
                        "'[Wednesday from 1]','[Wednesday to 1]','[Wednesday from 2]','[Wednesday to 2]'," +
                        "'[Thursday from 1]','[Thursday to 1]','[Thursday from 2]','[Thursday to 2]'," +
                        "'[Friday from 1]','[Friday to 1]','[Friday from 2]','[Friday to 2]'," +
                        "'[Saturday from 1]','[Saturday to 1]','[Saturday from 2]','[Saturday to 2]'," +
                        "coverLunch,lunchLength,breakLength,maxHoursBeforeLunch) " +
                        "values ('" + currentStore.nameOfStore + "','" + currentStore.maxDailyHours + "','" +
                        currentStore.maxWeeklyHours[0] + "','" + currentStore.maxWeeklyHours[1] + "','" +
                        currentStore.maxWeeklyHours[2] + "','" + currentStore.maxWeeklyHours[3];

                for (int i = 0; i < 7; i++)
                {
                    cmd.CommandText = cmd.CommandText + "','" + currentStore.storeHours.fromOne[i].ToShortTimeString();
                    cmd.CommandText = cmd.CommandText + "','" + currentStore.storeHours.toOne[i].ToShortTimeString();
                    cmd.CommandText = cmd.CommandText + "','" + currentStore.storeHours.fromTwo[i].ToShortTimeString();
                    cmd.CommandText = cmd.CommandText + "','" + currentStore.storeHours.toTwo[i].ToShortTimeString();
                }
                cmd.CommandText = cmd.CommandText + "','" + currentStore.lunchBreak + "','" + currentStore.lunchDuration + "','" +
                                    currentStore.breakLength + "','" + currentStore.maxBeforeLunch + "')";
                cmd.ExecuteNonQuery();*/
            }
            catch
            {

            }
            connection.Close();
            display_Data();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(GetAllStores());
        }

        private void button34_Click(object sender, EventArgs e)
        {

            if (selectedStore == null)
            {

            }
            else
            {
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DROP TABLE [" + selectedStore + "]";
                cmd.ExecuteNonQuery();
                for (int i = 0; i < 14; i++)
                {
                    cmd.CommandText = "DROP TABLE [" + selectedStore.Trim() + daysOfWeek[i] + "]";
                    cmd.ExecuteNonQuery();
                }
                int rowNumber = findStore(selectedStore);
                cmd.CommandText = "delete from [storeTable] where Id = '" + dataGridView1.Rows[rowNumber].Cells[0].Value + "'";
                cmd.ExecuteNonQuery();
                connection.Close();
                display_Data();
                comboBox1.Items.Clear();
                
                /*connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "delete from [" + nameOfStore + "] where employeeId = '" + dataGridView1.SelectedCells[0].Value + "'";
            cmd.ExecuteNonQuery();
            connection.Close();
            textBox1.Text = "";
            textBox2.Text = "";
            display_Data();*/
                
                comboBox1.Items.AddRange(GetAllStores());
                comboBox1.SelectedItem = null;
                selectedStore = null;
                Button buttonToEnable;
                foreach (Control ctrl in Controls)
                {
                    if (ctrl.Name.StartsWith("button"))
                    {
                        buttonToEnable = (Button)ctrl;
                        buttonToEnable.Enabled = false;

                    }
                }
                button32.Enabled = true;
                button33.Enabled = true;

            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            dailySchedule saturdayTwo = new dailySchedule();
            this.Hide();
            saturdayTwo.setUpInfo(button17.Text,  selectedStore, "SaturdayTwo");
            saturdayTwo.ShowDialog();
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dailySchedule sundayOne = new dailySchedule();
            this.Hide();
            sundayOne.setUpInfo(button1.Text, selectedStore, "SundayOne");
            sundayOne.ShowDialog();
            this.Show();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            dailySchedule fridayTwo = new dailySchedule();
            this.Hide();
            fridayTwo.setUpInfo(button16.Text,  selectedStore, "FridayTwo");
            fridayTwo.ShowDialog();
            //cmd.CommandText = "UPDATE " + selectedStore.Trim() + " SET PAY = 111 WHERE employeeId = 1";
            this.Show();

        }

        private void button18_Click(object sender, EventArgs e)
        {
            button4_Click(sender, e);
            bool[] closed = new bool[7];
            int storeIndex = findStore(selectedStore);
            DateTime startOfDay = DateTime.Now;
            DateTime endOfDay = DateTime.Now;
            TimeSpan[] difference = new TimeSpan[7];
            difference[0] = endOfDay.Subtract(startOfDay);
            float weekHours = float.Parse(dataGridView1.Rows[storeIndex].Cells[3].Value.ToString());
            float hoursDivisor = 0;

            for (int i = 0; i < 7; i++)
            {
                startOfDay = DateTime.Parse(dataGridView1.Rows[storeIndex].Cells[7 + i * 4].Value.ToString());
                endOfDay = DateTime.Parse(dataGridView1.Rows[storeIndex].Cells[8 + i * 4].Value.ToString());
                difference[i] = endOfDay.Subtract(startOfDay);
                hoursDivisor += float.Parse(difference[i].TotalHours.ToString());
                if (startOfDay == endOfDay)
                {
                    closed[i] = true;
                }

            }

            for (int i = 0; i < 7; i++)
            {
                if (closed[i] == true)
                {

                }
                else
                {
                    if (difference[i].Hours < 8)
                    {
                        setDaySchedule(daysOfWeek[i * 2], (float.Parse(difference[i].TotalHours.ToString()) / hoursDivisor) * weekHours,
                            float.Parse(difference[i].TotalHours.ToString()));
                    }
                    else
                    {
                        setDaySchedule(daysOfWeek[i * 2], (float.Parse(difference[i].TotalHours.ToString()) / hoursDivisor) * weekHours, 8);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dailySchedule mondayOne = new dailySchedule();
            this.Hide();
            mondayOne.setUpInfo(button2.Text,  selectedStore, "MondayOne");
            mondayOne.ShowDialog();
            this.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            dailySchedule tuesdayOne = new dailySchedule();
            this.Hide();
            tuesdayOne.setUpInfo(button6.Text,  selectedStore, "TuesdayOne");
            tuesdayOne.ShowDialog();
            this.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dailySchedule wednesdayOne = new dailySchedule();
            this.Hide();
            wednesdayOne.setUpInfo(button7.Text,  selectedStore, "WednesdayOne");
            wednesdayOne.ShowDialog();
            this.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dailySchedule thursdayOne = new dailySchedule();
            this.Hide();
            thursdayOne.setUpInfo(button8.Text,  selectedStore, "ThursdayOne");
            thursdayOne.ShowDialog();
            this.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            dailySchedule fridayOne = new dailySchedule();
            this.Hide();
            fridayOne.setUpInfo(button9.Text,  selectedStore, "FridayOne");
            fridayOne.ShowDialog();
            this.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            dailySchedule saturdayOne = new dailySchedule();
            this.Hide();
            saturdayOne.setUpInfo(button10.Text,  selectedStore, "SaturdayOne");
            saturdayOne.ShowDialog();
            this.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            dailySchedule sundayTwo = new dailySchedule();
            this.Hide();
            sundayTwo.setUpInfo(button11.Text,  selectedStore, "SundayTwo");
            sundayTwo.ShowDialog();
            this.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            dailySchedule mondayTwo = new dailySchedule();
            this.Hide();
            mondayTwo.setUpInfo(button12.Text,  selectedStore, "MondayTwo");
            mondayTwo.ShowDialog();
            this.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            dailySchedule tuesdayTwo = new dailySchedule();
            this.Hide();
            tuesdayTwo.setUpInfo(button13.Text,  selectedStore, "TuesdayTwo");
            tuesdayTwo.ShowDialog();
            this.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            dailySchedule wednesdayTwo = new dailySchedule();
            this.Hide();
            wednesdayTwo.setUpInfo(button14.Text,  selectedStore, "WednesdayTwo");
            wednesdayTwo.ShowDialog();
            this.Show();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            dailySchedule thursdayTwo = new dailySchedule();
            this.Hide();
            thursdayTwo.setUpInfo(button15.Text,  selectedStore, "ThursdayTwo");
            thursdayTwo.ShowDialog();
            this.Show();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            for (int i = 1; i < 14; i+=2)
            {
                cmd.CommandText = "DELETE from " + selectedStore.Trim() + daysOfWeek[i];
                cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < dataGridView2.RowCount; i++)
            {
                cmd.CommandText = "UPDATE " + selectedStore.Trim() + " SET weekTwoHours = 0 WHERE employeeId = " + i.ToString();
                cmd.ExecuteNonQuery();
            }

            connection.Close();
            displayEmployeeData();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            button20_Click(sender, e);
            bool[] closed = new bool[7];
            int storeIndex = findStore(selectedStore);
            DateTime startOfDay = DateTime.Now;
            DateTime endOfDay = DateTime.Now;
            TimeSpan[] difference = new TimeSpan[7];
            difference[0] = endOfDay.Subtract(startOfDay);
            float weekHours = float.Parse(dataGridView1.Rows[storeIndex].Cells[4].Value.ToString());
            float hoursDivisor = 0;

            for (int i = 0; i < 7; i++)
            {
                startOfDay = DateTime.Parse(dataGridView1.Rows[storeIndex].Cells[7 + i * 4].Value.ToString());
                endOfDay = DateTime.Parse(dataGridView1.Rows[storeIndex].Cells[8 + i * 4].Value.ToString());
                difference[i] = endOfDay.Subtract(startOfDay);
                hoursDivisor += float.Parse(difference[i].TotalHours.ToString());
                if (startOfDay == endOfDay)
                {
                    closed[i] = true;
                    
                }

            }

            for (int i = 0; i < 7; i++)
            {
                if (closed[i] == true)
                {

                }
                else
                {
                    if (difference[i].Hours < 8)
                    {
                        setDaySchedule(daysOfWeek[1 +(i * 2)], (float.Parse(difference[i].TotalHours.ToString())/hoursDivisor) * weekHours,
                            float.Parse(difference[i].TotalHours.ToString()));
                    }
                    else
                    {
                        setDaySchedule(daysOfWeek[1 + (i* 2)], (float.Parse(difference[i].TotalHours.ToString()) / hoursDivisor) * weekHours, 8);
                    }
                }
            }
        }
    }
}
