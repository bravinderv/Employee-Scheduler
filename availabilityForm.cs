using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scheduler
{
    public partial class availabilityForm : Form
    {
        WeeklyHours empAvail;
        public availabilityForm()
        {
            InitializeComponent();
            empAvail.fromOne = new DateTime[7];
            empAvail.toOne = new DateTime[7];
            empAvail.fromTwo = new DateTime[7];
            empAvail.toTwo = new DateTime[7];
            empAvail.open = new bool[7];
        }
        public void setText(String title, String prompt, String allDay)
        {
            label52.Text = title;
            label53.Text = prompt;
            label54.Text = allDay;
        }

        public void setEditHours(WeeklyHours store)
        {
            ComboBox[] timeBoxes = new ComboBox[84];
            CheckBox[] closed = new CheckBox[7];
            int i = 84;
            int j = 7;

            /*for (i = 0; i < 7; i++)
            {
                if (DateTime.Compare(store.fromOne[i], store.toOne[i]) == 0)
                {
                    closed[i] = true;
                }
            }*/

            foreach (Control ctrl in Controls)
            {
                if (ctrl.Name.StartsWith("comboBox"))
                {
                    i--;
                    timeBoxes[i] = (ComboBox)ctrl;
                }

                if (ctrl.Name.StartsWith("checkBox"))
                {
                    j--;
                    closed[j] = (CheckBox)ctrl;
                    if (DateTime.Compare(store.fromOne[j], store.toOne[j]) == 0)
                    {
                        closed[j].Checked = true;
                    }
                    else
                    {
                        closed[j].Checked = false;
                    }
                }

            }

            for (i = 0; i < 56; i+=8)
            {
                if (closed[i / 8].Checked == true)
                {

                }
                else
                {
                    timeBoxes[i].SelectedItem = store.fromOne[i / 8].ToString("hh");
                    timeBoxes[i + 1].SelectedItem = store.fromOne[i / 8].ToString("mm");
                    timeBoxes[i + 2].SelectedItem = store.toOne[i / 8].ToString("hh");
                    timeBoxes[i + 3].SelectedItem = store.toOne[i / 8].ToString("mm");

                    if (DateTime.Compare(store.fromTwo[i / 8], store.toTwo[i / 8]) == 0)
                    {

                    }
                    else
                    {
                        timeBoxes[i + 4].SelectedItem = store.fromTwo[i / 8].ToString("hh");
                        timeBoxes[i + 5].SelectedItem = store.fromTwo[i / 8].ToString("mm");
                        timeBoxes[i + 6].SelectedItem = store.toTwo[i / 8].Hour.ToString("hh");
                        timeBoxes[i + 7].SelectedItem = store.toTwo[i / 8].Minute.ToString("mm");
                    }
                }
            }

            for (i = 56; i < 84; i+=4)
            {
                if (closed[(i - 56) / 4].Checked == true)
                {

                }
                else
                {
                    timeBoxes[i].SelectedItem = store.fromOne[(i - 56) / 4].ToString("tt");
                    timeBoxes[i + 1].SelectedItem = store.toOne[(i - 56) / 4].ToString("tt");
                    timeBoxes[i + 2].SelectedItem = store.fromTwo[(i - 56) / 4].ToString("tt");
                    timeBoxes[i + 3].SelectedItem = store.toTwo[(i - 56) / 4].ToString("tt");
                }
            }
            
        }

        public void storeSetUp()
        {
            ComboBox[] timeBoxes = new ComboBox[84];
            CheckBox tempCheckBox;
            Label tempLabel;
            int i = 84;

            foreach (Control ctrl in Controls)
            {
                if (ctrl.Name.StartsWith("comboBox"))
                {
                    i--;
                    timeBoxes[i] = (ComboBox)ctrl;
                }

                if (ctrl.Name.StartsWith("checkBox"))
                {
                    tempCheckBox = (CheckBox)ctrl;
                    tempCheckBox.Checked = false;
                    tempCheckBox.Location = new Point(540, tempCheckBox.Location.Y);

                }

                if (ctrl.Name.StartsWith("label"))
                {
                    tempLabel = (Label)ctrl;
                    if (tempLabel.Text == ":" || tempLabel.Text == "TO")
                    {
                        tempLabel.Hide();
                    }

                    
                }
            }

            for (i = 4; i < 56; i += 8)
            {
                timeBoxes[i].Hide();
                timeBoxes[i + 1].Hide();
                timeBoxes[i + 2].Hide();
                timeBoxes[i + 3].Hide();              

            }

            for (i = 58; i < 84; i += 4)
            {
                timeBoxes[i].Hide();
                timeBoxes[i + 1].Hide();
            }

            label29.Hide();
            label30.Hide();
            label54.Location = new Point(490, label54.Location.Y);
            button1.Location = new Point(227, button1.Location.Y);
            button2.Location = new Point(383, button2.Location.Y);

        }
        public bool correctlyFormatted()
        {
            bool formatted = true;
            int i = 84;
            int j = 7;
            int t = 56;
            int test = 0;
            ComboBox[] timeBoxes = new ComboBox[84];
            CheckBox[] checkBoxes = new CheckBox[7];
            DateTime first = new DateTime();
            DateTime second = new DateTime();

            foreach (Control ctrl in Controls)
            {
                if (ctrl is CheckBox)
                {
                    j--;
                    checkBoxes[j] = (CheckBox)ctrl;
                }

                if (ctrl.Name.StartsWith("comboBox"))
                {
                    i--;
                    timeBoxes[i] = (ComboBox)ctrl;
                }
            }

            while (i < 84)
            {
                if (timeBoxes[i].SelectedItem == null)
                {
                    timeBoxes[i].SelectedItem = "";
                }
                i++;
            }

            i = 0;

            while (i < 56 && formatted)
            {
                if (timeBoxes[i].SelectedItem.ToString() == "" && (timeBoxes[i + 1].SelectedItem.ToString() != "" ||
                    timeBoxes[i + 2].SelectedItem.ToString() != "" || timeBoxes[i + 3].SelectedItem.ToString() != ""))
                {
                    formatted = false;
                }
                else if (timeBoxes[i].SelectedItem.ToString() != "" && (timeBoxes[i + 1].SelectedItem.ToString() == "" ||
                    timeBoxes[i + 2].SelectedItem.ToString() == "" || timeBoxes[i + 3].SelectedItem.ToString() == ""))
                {
                    formatted = false;
                }
                i += 4;
            }

            while (i < 84 && formatted)
            {
                if (timeBoxes[i].SelectedItem.ToString() == "" && timeBoxes[i + 1].SelectedItem.ToString() != "")
                {
                    formatted = false;
                }
                else if (timeBoxes[i].SelectedItem.ToString() != "" && timeBoxes[i + 1].SelectedItem.ToString() == "")
                {
                    formatted = false;
                }
                i += 2;
            }

            i = 0;

            while (i < 56 && formatted)
            {
                if (checkBoxes[j].Checked == true)
                {
                    j++;
                    i += 8;
                    t += 4;
                }
                else
                {


                    if (timeBoxes[i].SelectedItem.ToString() == "")
                    {

                    }
                    else
                    {
                        first = DateTime.Parse(timeBoxes[i].SelectedItem.ToString() + ":" + timeBoxes[i + 1].SelectedItem.ToString() + " "
                        + timeBoxes[t].SelectedItem.ToString());


                        second = DateTime.Parse(timeBoxes[i + 2].SelectedItem.ToString() + ":" + timeBoxes[i + 3].SelectedItem.ToString() + " "
                            + timeBoxes[t + 1].SelectedItem.ToString());

                        test = DateTime.Compare(first, second);
                    }

                    if (test >= 0)
                    {
                        formatted = false;
                    }

                    i += 4;
                    t += 2;

                    if (timeBoxes[i].SelectedItem.ToString() == "")
                    {

                    }
                    else
                    {
                        first = DateTime.Parse(timeBoxes[i].SelectedItem.ToString() + ":" + timeBoxes[i + 1].SelectedItem.ToString() + " "
                        + timeBoxes[t].SelectedItem.ToString());


                        second = DateTime.Parse(timeBoxes[i + 2].SelectedItem.ToString() + ":" + timeBoxes[i + 3].SelectedItem.ToString() + " "
                            + timeBoxes[t + 1].SelectedItem.ToString());

                        test = DateTime.Compare(first, second);
                    }

                    i += 4;
                    t += 2;

                    if (test > 0)
                    {
                        formatted = false;
                    }

                    j++;

                }
            }

            return formatted;
        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = 84;
            int j = 0;
            int k = 7;
            int l = 56;
            ComboBox[] timeBox = new ComboBox[84];
            CheckBox[] openCheckBox = new CheckBox[7];

            if (!correctlyFormatted())
            {
                label55.Text = "INCORRECTLY FORMATTED";
                return;
            }
            label55.Text = "";

            foreach (Control ctrl in Controls)
            {
                if (ctrl.Name.StartsWith("comboBox"))
                {
                    i--;
                    timeBox[i] = (ComboBox)ctrl;

                }

                if (ctrl.Name.StartsWith("checkBox"))
                {
                    k--;
                    openCheckBox[k] = (CheckBox)ctrl;
                }
            }

            while (j < 7)
            {
                if (openCheckBox[k].Checked)
                {
                    empAvail.fromOne[j] = DateTime.Parse("0" + ":" + "0");
                    empAvail.toOne[j] = DateTime.Parse("0" + ":" + "0");
                    empAvail.fromTwo[j] = DateTime.Parse("0" + ":" + "0");
                    empAvail.toTwo[j] = DateTime.Parse("0" + ":" + "0");
                    j++;
                    i += 8;
                    k++;
                    l += 4;
                }
                else
                {
                    empAvail.fromOne[j] = DateTime.Parse(timeBox[i].SelectedItem.ToString() + ":"
                        + timeBox[i + 1].SelectedItem.ToString() + timeBox[l].SelectedItem.ToString());

                    i += 2;
                    l++;

                    empAvail.toOne[j] = DateTime.Parse(timeBox[i].SelectedItem.ToString() + ":"
                        + timeBox[i + 1].SelectedItem.ToString() + timeBox[l].SelectedItem.ToString());

                    i += 2;
                    l++;

                    if (timeBox[i].SelectedItem.ToString() == "")
                    {
                        empAvail.fromTwo[j] = DateTime.Parse("0" + ":" + "0");
                        empAvail.toTwo[j] = DateTime.Parse("0" + ":" + "0");
                        i += 4;
                        l += 2;
                    }
                    else
                    {
                        empAvail.fromTwo[j] = DateTime.Parse(timeBox[i].SelectedItem.ToString() + ":"
                        + timeBox[i + 1].SelectedItem.ToString() + timeBox[l].SelectedItem.ToString());

                        i += 2;
                        l++;

                        empAvail.toTwo[j] = DateTime.Parse(timeBox[i].SelectedItem.ToString() + ":"
                            + timeBox[i + 1].SelectedItem.ToString() + timeBox[l].SelectedItem.ToString());

                        i += 2;
                        l++;
                    }
                    j++;
                    k++;

                }
            }
            
            j = 0;
        }

        public void sendBackHours(ref WeeklyHours hours)
        {
            hours = empAvail;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void availabilityForm_Load(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label53_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            label53.Text = correctlyFormatted().ToString();
        }

        private void label55_Click(object sender, EventArgs e)
        {

        }

        private void comboBox19_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox33_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label52_Click(object sender, EventArgs e)
        {

        }

        private void comboBox20_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            label55.Text = empAvail.fromOne[5].ToShortTimeString();
            label1.Text = empAvail.toOne[5].ToShortTimeString();
        }

        private void button3_Click_2(object sender, EventArgs e)
        {
            int i = 84;
            int k = 7;
            ComboBox[] timeBox = new ComboBox[84];
            CheckBox[] openCheckBox = new CheckBox[7];

            foreach (Control ctrl in Controls)
            {
                if (ctrl.Name.StartsWith("comboBox"))
                {
                    i--;
                    timeBox[i] = (ComboBox)ctrl;

                }

                if (ctrl.Name.StartsWith("checkBox"))
                {
                    k--;
                    openCheckBox[k] = (CheckBox)ctrl;
                }
            }

            while (i < 56)
            {
                timeBox[i].SelectedItem = timeBox[0].SelectedItem;
                timeBox[i+1].SelectedItem = timeBox[1].SelectedItem;
                timeBox[i+2].SelectedItem = timeBox[2].SelectedItem;
                timeBox[i+3].SelectedItem = timeBox[3].SelectedItem;
                timeBox[i+4].SelectedItem = timeBox[4].SelectedItem;
                timeBox[i+5].SelectedItem = timeBox[5].SelectedItem;
                timeBox[i+6].SelectedItem = timeBox[6].SelectedItem;
                timeBox[i+7].SelectedItem = timeBox[7].SelectedItem;
                i += 8;
            }

            while (i < 84)
            {
                timeBox[i].SelectedItem = timeBox[56].SelectedItem;
                timeBox[i+1].SelectedItem = timeBox[57].SelectedItem;
                timeBox[i+2].SelectedItem = timeBox[58].SelectedItem;
                timeBox[i+3].SelectedItem = timeBox[59].SelectedItem;
                i += 4;
            }

            while (k < 7)
            {
                openCheckBox[k].Checked = openCheckBox[0].Checked;
                k++;
            }

        }
    }
}
