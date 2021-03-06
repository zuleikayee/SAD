﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApplication1
{
    public partial class Adopt : Form
    {
        public MySqlConnection conn;
        public int dogID;
        public int adminID;
        int[] empids;
        public Dog dog { get; set; } 
        public Adopt()
        {
            InitializeComponent();
            conn = new MySqlConnection("Server=localhost;Database=dogpound;Uid=root;Pwd=root;");
        }
        //n mnbnm

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x20000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }


        private void Adopt_Load(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                MySqlCommand comm = new MySqlCommand("SELECT breed, color, size, gender, description, CONCAT(timeStart, '-', timeEnd) AS time, SUBSTRING(date, 1, 11) AS date FROM (dogoperation INNER JOIN dogprofile ON dogprofile.operationID = dogoperation.operationID) INNER JOIN location on dogoperation.locationID = location.locationID WHERE dogID = " + dogID, conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(comm);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                breeds.Text = dt.Rows[0]["breed"].ToString();
                color.Text = dt.Rows[0]["color"].ToString();
                size.Text = dt.Rows[0]["size"].ToString();
                gender.Text = dt.Rows[0]["gender"].ToString();
                location.Text = dt.Rows[0]["description"].ToString();
                date.Text = dt.Rows[0]["date"].ToString() + "\n" + dt.Rows[0]["time"].ToString();


                conn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                conn.Close();
            }
        }

        private void txtName_Enter(object sender, EventArgs e)
        {
            label1.Visible = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            dog.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            String fname = tbfname.Text;
            String mname = tbmname.Text;
            String lname = tblname.Text;
            String add = tbadd.Text;
            String idnum = tbIDnum.Text;
            String idtype = tbIDtype.Text;
            String num = tbnumber.Text;
            String date = DateTime.Now.ToString("yyyy-MM-dd");
            String month = (cbMonth.SelectedIndex + 1).ToString();
            String day = tbDay.Text;
            String year = tbDay.Text;
            String bday = year + '-' + month + '-' + day;
            if (tbfname.Text != "" && tbmname.Text != "" && tblname.Text != "" && tbadd.Text != "" && tbIDnum.Text != "" && tbIDtype.Text != "" && cbMonth.Text != "" && tbDay.Text != "" && tbYear.Text != "Year")
            {
                if (cbVaccine.Checked && cbVaccEmp.Text == "Employee")
                {
                    MessageBox.Show("Please enter required fields");
                }
                else
                {
                    int vaccine = 0;
                    if (cbVaccine.Checked)
                    {
                        vaccine = 1;
                    }
                    try
                    {
                        conn.Open();
                        MySqlCommand comm;
                        string messbox = "";
                        if (vaccine == 1)
                        {
                            
                            MySqlCommand commm = new MySqlCommand("SELECT quantity FROM items WHERE description = 'Vaccine' OR description = 'Syringe'", conn);
                            MySqlDataAdapter adpp = new MySqlDataAdapter(commm);
                            DataTable dtt = new DataTable();
                            adpp.Fill(dtt);
                            if (int.Parse(dtt.Rows[0]["quantity"].ToString()) > 0 && int.Parse(dtt.Rows[1]["quantity"].ToString()) > 0) {
                                adopt();
                                comm = new MySqlCommand("UPDATE items SET quantity=quantity-1 WHERE description = 'Vaccine'", conn);
                                comm.ExecuteNonQuery();
                                comm = new MySqlCommand("UPDATE items SET quantity=quantity-1 WHERE description = 'Syringe'", conn);
                                comm.ExecuteNonQuery();
                                string datenow = DateTime.Now.ToString("yyyy-MM-dd");
                                comm = new MySqlCommand("INSERT INTO stocktransaction(stockID, quantity, date, reason, employeeID, type) VALUES(1, 1, '" + datenow + "', 'Adopt', " + empids[cbVaccEmp.SelectedIndex] + ", 'Out')", conn);
                                comm.ExecuteNonQuery();
                                comm = new MySqlCommand("INSERT INTO stocktransaction(stockID, quantity, date, reason, employeeID, type) VALUES(5, 1, '" + datenow + "', 'Adopt', " + empids[cbVaccEmp.SelectedIndex] + ", 'Out')", conn);
                                comm.ExecuteNonQuery();
                                comm = new MySqlCommand("INSERT INTO activity(date, employeeID, type) VALUES('" + date + "', " + empids[cbVaccEmp.SelectedIndex] + ", 'Vaccination')", conn);
                                comm.ExecuteNonQuery();
                                comm = new MySqlCommand("SELECT quantity FROM items WHERE description = 'Vaccine'", conn);
                                MySqlDataAdapter adppt = new MySqlDataAdapter(comm);
                                DataTable dttt = new DataTable();
                                adpp.Fill(dttt);
                                string nl = Environment.NewLine;
                                int quantity = int.Parse(dttt.Rows[0]["quantity"].ToString());
                                messbox = "Successfully Adopted and Vaccinated!" + nl + "Vaccine Quantity is now: " + quantity.ToString();
                                comm = new MySqlCommand("SELECT quantity FROM items WHERE description = 'Syringe'", conn);
                                MySqlDataAdapter adpt = new MySqlDataAdapter(comm);
                                DataTable dtttt = new DataTable();
                                adpt.Fill(dtttt);
                                int quan = int.Parse(dtttt.Rows[0]["quantity"].ToString());
                                messbox = messbox + nl + "Syringe Quantity is now: " + quan.ToString();
                                MessageBox.Show(messbox);
                                preview();
                                dog.Show();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Running Out of Vaccine/Syringe. Cannot Vaccinate Dog", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            adopt();
                            messbox = "Successfully Adopted!";
                            MessageBox.Show(messbox);
                            preview();
                            dog.Show();
                            this.Close();
                        }
                        
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        conn.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter required fields");
            }
        }

        private void adopt()
        {
            String fname = tbfname.Text;
            String mname = tbmname.Text;
            String lname = tblname.Text;
            String add = tbadd.Text;
            String idnum = tbIDnum.Text;
            String idtype = tbIDtype.Text;
            String num = tbnumber.Text;
            String date = DateTime.Now.ToString("yyyy-MM-dd");
            String month = (cbMonth.SelectedIndex + 1).ToString();
            String day = tbDay.Text;
            String year = tbDay.Text;
            String bday = year + '-' + month + '-' + day;
            int vaccine = 0;
            if (cbVaccine.Checked)
            {
                vaccine = 1;
            }
            MySqlCommand comm = new MySqlCommand("INSERT INTO profile(firstname, middlename, lastname, contactNumber, address, birthdate) VALUES('" + fname + "', '" + mname + "','" + lname + "', '" + num + "', '" + add + "', '" + bday + "')", conn);
            comm.ExecuteNonQuery();

            comm = new MySqlCommand("SELECT MAX(personID) FROM profile", conn);
            MySqlDataAdapter adp = new MySqlDataAdapter(comm);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            int personID = int.Parse(dt.Rows[0]["MAX(personID)"].ToString());

            comm = new MySqlCommand("INSERT INTO client(personID, validIDType, validIDNumber) VALUES(" + personID + ", '" + idtype + "', '" + idnum + "')", conn);
            comm.ExecuteNonQuery();

            comm = new MySqlCommand("INSERT INTO dogtransaction(personID, dogID, date, payment, vaccine, type) VALUES(" + personID + ", " + dogID + ", '" + date + "', " + "0" + ", " + vaccine + ", '" + "adopt" + "')", conn);
            comm.ExecuteNonQuery();

            comm = new MySqlCommand("UPDATE dogprofile SET status = 'adopted' WHERE dogID = " + dogID, conn);
            comm.ExecuteNonQuery();
            string messbox;
        }

        private void Adopt_FormClosing(object sender, FormClosingEventArgs e)
        {
            dog.refreshAdoption();
        }
        
        private void printDocumentAdt_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("Davao City Dog Pound", new Font("Arial", 20, FontStyle.Bold), Brushes.Black, new Point(25, 100));
            e.Graphics.DrawString("Adopter's Details", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, new Point(25, 160));
            e.Graphics.DrawString("Adopter's Name: " + tbfname.Text + " " + tbmname.Text + " " + tblname.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 190));
            e.Graphics.DrawString("Contact Number: " + tbnumber.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 210));
            e.Graphics.DrawString("Birthdate: " + cbMonth.Text + " " + tbDay.Text + "," + tbYear.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 230));
            e.Graphics.DrawString("Address: " + tbadd.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 250));
            e.Graphics.DrawString("Valid ID Type: " + tbIDtype.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 270));
            e.Graphics.DrawString("Valid ID Number: " + tbIDnum.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 290));

            if (cbVaccine.Checked)
            {
                e.Graphics.DrawString("**Availed Vaccine, Vaccinated by: " + cbVaccEmp.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 430));
            }
            else
            {
                e.Graphics.DrawString("**No Vaccine", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 430));
            }
            // LINE SEPARATOR
            Pen black = new Pen(Color.Black, 3);
            Point p1 = new Point(25, 340);
            Point p2 = new Point(800, 340);
            e.Graphics.DrawLine(black, p1, p2);
            e.Graphics.DrawString("Dog Details ", new Font("Arial", 14, FontStyle.Bold), Brushes.Black, new Point(400, 160));
            e.Graphics.DrawString("Breed: " + breeds.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(400, 190));
            e.Graphics.DrawString("Color:" + color.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(400, 210));
            e.Graphics.DrawString("Size: " + size.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(400, 230));
            e.Graphics.DrawString("Gender: " + gender.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(400, 250));
            e.Graphics.DrawString("Location: " + location.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(400, 270));
            e.Graphics.DrawString("Date and Time Caught: " + date.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 380));
            e.Graphics.DrawString("Date: " + DateTime.Now, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(300, 600));
        }
        
        private void tbYear_TextChanged(object sender, EventArgs e)
        {
            if (tbYear.Text.Length == 4) cbMonth.Enabled = true;
        }

        private void cbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbDay.Enabled = true;
            tbDay.Text = "Day";
            tbDay.Items.Clear();
            responsiveDay();
        }

        private void tbYear_Enter(object sender, EventArgs e)
        {
            tbYear.Text = "";
        }

        //FUNCTIONS

        private void preview()
        {
            PrintPreviewDialog dlg = new PrintPreviewDialog();
            dlg.Document = printDocumentAdt;
            ((Form)dlg).WindowState = FormWindowState.Maximized;
            dlg.ShowDialog();
        }

        private void responsiveDay()
        {
            if (int.Parse(tbYear.Text) % 4 == 0 && cbMonth.Text == "February") { loopDay(29); }
            else if (int.Parse(tbYear.Text) % 4 != 0 && cbMonth.Text == "February") { loopDay(28); }
            else if (cbMonth.Text == "January" || cbMonth.Text == "March" || cbMonth.Text == "May" || cbMonth.Text == "July" || cbMonth.Text == "August" || cbMonth.Text == "October" || cbMonth.Text == "December") { loopDay(31); }
            else { loopDay(30); }
        }

        private void loopDay(int x)
        {
            int i = 1;
            while (i <= x)
            {
                tbDay.Items.Add(i.ToString());
                i++;
            }
        }

        private void refreshEmps()
        {
            cbVaccEmp.Items.Clear();
            try
            {
                conn.Open();
                MySqlCommand comm = new MySqlCommand("SELECT personID, CONCAT(lastname, ', ', firstname, ' ', SUBSTRING(middlename, 1, 1), '.') AS name FROM profile INNER JOIN employee ON employee.employeeID = profile.personID WHERE employee.status = 'Active'", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(comm);
                System.Data.DataTable dt = new System.Data.DataTable();
                adp.Fill(dt);
                empids = new int[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    empids[i] = int.Parse(dt.Rows[i]["personID"].ToString());
                    cbVaccEmp.Items.Add(dt.Rows[i]["name"].ToString());
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cbVaccine_CheckedChanged(object sender, EventArgs e)
        {
            if (cbVaccine.Checked)
            {
                cbVaccEmp.Visible = true;
                refreshEmps();
            }
            else
            {
                cbVaccEmp.Visible = false;
            }
        }

        private void printPreviewDialogAdt_Load(object sender, EventArgs e)
        {

        }

        private void cbMonth_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void tbYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && !((Keys)e.KeyChar == Keys.Back);
        }

        private void tbfname_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbfname_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !((Keys)e.KeyChar == Keys.Space) && !((Keys)e.KeyChar == Keys.Back);
        }

        private void tbmname_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !((Keys)e.KeyChar == Keys.Space) && !((Keys)e.KeyChar == Keys.Back);
        }

        private void tblname_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !((Keys)e.KeyChar == Keys.Space) && !((Keys)e.KeyChar == Keys.Back);
        }

        private void printPreviewDialogAdt_Load_1(object sender, EventArgs e)
        {

        }

        private void tbnumber_TextChanged(object sender, EventArgs e)
        {
            tbnumber.MaxLength = 11;
        }
    }
}
