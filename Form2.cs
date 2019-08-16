using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Scheduler
{
    public partial class scheduleSettingsForm : Form
    {
        Store newStore;
        bool edit = false;
        bool change = false;
        public scheduleSettingsForm()
        {
            InitializeComponent();
            newStore.maxWeeklyHours = new float[4];
            newStore.storeHours.fromOne = new DateTime[7];
            newStore.storeHours.toOne = new DateTime[7];
            newStore.storeHours.fromTwo = new DateTime[7];
            newStore.storeHours.toTwo = new DateTime[7];
            label2.Hide();
            label3.Hide();
            label4.Hide();
            checkBox1.Hide();
            comboBox1.Hide();
            textBox5.Hide();

        }

        public void setUpScheduleSettings(DataGridView store, int storeIndex)
        {
            textBox6.Text = store.Rows[storeIndex].Cells[1].Value.ToString();
            textBox1.Text = store.Rows[storeIndex].Cells[3].Value.ToString();
            textBox2.Text = store.Rows[storeIndex].Cells[4].Value.ToString();
            for (int i = 0; i < 7; i++)
            {
                newStore.storeHours.fromOne[i] = DateTime.Parse(store.Rows[storeIndex].Cells[7 + i * 4].Value.ToString());
                newStore.storeHours.toOne[i] = DateTime.Parse(store.Rows[storeIndex].Cells[8 + i * 4].Value.ToString());
                newStore.storeHours.fromTwo[i] = DateTime.Parse(store.Rows[storeIndex].Cells[9 + i * 4].Value.ToString());
                newStore.storeHours.toTwo[i] = DateTime.Parse(store.Rows[storeIndex].Cells[10 + i * 4].Value.ToString());
            }
            newStore.maxWeeklyHours[0] = float.Parse(store.Rows[storeIndex].Cells[3].Value.ToString());
            newStore.maxWeeklyHours[1] = float.Parse(store.Rows[storeIndex].Cells[4].Value.ToString());
            edit = true;
        }
        public void sendBackStoreInfo(ref Store storeInfo, ref bool changeStuff)
        {
            storeInfo = newStore;
            changeStuff = change;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox5.ReadOnly = false;
            }
            else
            {
                textBox5.ReadOnly = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            availabilityForm storeHours = new availabilityForm();
            storeHours.setText("Payroll Hours","Enter the times when you need workers scheduled","   Closed All Day");
            this.Hide();
            storeHours.storeSetUp();
            storeHours.Size = new System.Drawing.Size(650,800);
            if (edit)
            {
                storeHours.setEditHours(newStore.storeHours);
            }
            else
            {

            }
            storeHours.ShowDialog();
            storeHours.sendBackHours(ref newStore.storeHours);
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            newStore.nameOfStore = textBox6.Text;
            newStore.maxDailyHours = 8;

            newStore.maxWeeklyHours[0] = float.Parse(textBox1.Text, CultureInfo.InvariantCulture.NumberFormat);
            newStore.maxWeeklyHours[1] = float.Parse(textBox2.Text, CultureInfo.InvariantCulture.NumberFormat);
            newStore.maxWeeklyHours[2] = 0;
            newStore.maxWeeklyHours[3] = 0;

            /*newStore.maxWeeklyHours[0] = float.Parse(textBox1.Text, CultureInfo.InvariantCulture.NumberFormat);
            newStore.maxWeeklyHours[1] = float.Parse(textBox2.Text, CultureInfo.InvariantCulture.NumberFormat);
            newStore.maxWeeklyHours[2] = float.Parse(textBox3.Text, CultureInfo.InvariantCulture.NumberFormat);
            newStore.maxWeeklyHours[3] = float.Parse(textBox4.Text, CultureInfo.InvariantCulture.NumberFormat);*/
            if (checkBox1.Checked == true)
            {
                newStore.lunchBreak = true;
                newStore.lunchDuration = int.Parse(textBox5.Text);
                newStore.breakLength = 15;
                newStore.maxBeforeLunch = int.Parse(comboBox1.SelectedItem.ToString());
            }
            else
            {
                newStore.lunchBreak = false;
            }
            change = true;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
