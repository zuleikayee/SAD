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
    public partial class Viewemp : Form
    {
        Employee emp { get; set; }
        public MySqlConnection conn;
        public Viewemp()
        {
            InitializeComponent();
            conn = new MySqlConnection("Server=localhost;Database=dogpound;Uid=root;Pwd=root;");
        }

        private void Viewemp_Load(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                MySqlCommand com = new MySqlCommand("SELECT CONCAT(firstname, ' ', middlename, ' ', lastname) AS name, gender, birthdate, contactNumber, address, position, status FROM profile INNER JOIN employee ON employee.employeeID = profile.personID ORDER BY lastname, firstname", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(com);
                DataTable dt = new DataTable();
                adp.Fill(dt);


                dgvViewEmp.DataSource = dt;
                
                dgvViewEmp.Columns["name"].HeaderText = "Name";
                dgvViewEmp.Columns["gender"].HeaderText = "Gender";
                dgvViewEmp.Columns["birthdate"].HeaderText = "Birthdate";
                dgvViewEmp.Columns["contactNumber"].HeaderText = "Contact No.";
                dgvViewEmp.Columns["address"].HeaderText = "Address";
                dgvViewEmp.Columns["position"].HeaderText = "Position";
                dgvViewEmp.Columns["status"].HeaderText = "Status";

                dgvViewEmp.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvViewEmp.Columns["Gender"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvViewEmp.Columns["Birthdate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvViewEmp.Columns["ContactNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvViewEmp.Columns["Address"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvViewEmp.Columns["Position"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvViewEmp.Columns["Status"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                conn.Close();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        
    }
}
