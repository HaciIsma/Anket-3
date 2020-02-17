using MetroFramework.Forms;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WindowsFormsApp33.Data;

namespace WindowsFormsApp33
{
    public partial class Form1 : MetroForm
    {
        User user = new User();
        List<Country> countries = null;
        AppDBContext AppDB = new AppDBContext();
        private delegate void UserAddSystem(User user);

        public Form1()
        {
            InitializeComponent();
        }

        #region Metods

        private void AddUserDB(User user)
        {
            AppDB.Add(user);
            AppDB.SaveChanges();
        }

        private void SetUserInfo(User user)
        {
            user.Name = nameBox.Text;
            user.LastName = lastNameBox.Text;
            user.MiddleName = middleNameBox.Text;
            if (comboBoxCountry.Text.Length > 0)
            {
                user.Country = countries[comboBoxCountry.SelectedIndex].Name;
            }
            else
            {
                user.Country = default;
            }
            user.City = cityBox.Text;
            user.Phone = phoneBox.Text;
            user.Birthday = dateTimePicker1.Value;
            if (male.Checked == true)
            {
                user.Gender = "Male";
            }
            else if (female.Checked == true)
            {
                user.Gender = "Female";
            }
        }

        private void CheckTextBoxs()
        {
            Regex letterCheck = new Regex("^[a-zA-Z]{3,25}$");
            if (!letterCheck.IsMatch(nameBox.Text))
            {
                MessageBox.Show("Name invalid");
            }
            if (!letterCheck.IsMatch(lastNameBox.Text))
            {
                MessageBox.Show("LastName invalid");
            }
            if (!letterCheck.IsMatch(middleNameBox.Text))
            {
                MessageBox.Show("Middle Name invalid");
            }
            if (!letterCheck.IsMatch(comboBoxCountry.Text))
            {
                MessageBox.Show("Country invalid");
            }
            if (!letterCheck.IsMatch(cityBox.Text))
            {
                MessageBox.Show("City invalid");
            }
            if (new Regex("^[0-9]$").IsMatch(phoneBox.Text))
            {
                MessageBox.Show("Phone invalid");
            }
        }

        private void Search()
        {
            user = AppDB.Users.FirstOrDefault(e => e.Name == textBox1.Text);
        }

        private void GetUser()
        {
            nameBox.Text = user.Name;
            lastNameBox.Text = user.LastName;
            middleNameBox.Text = user.MiddleName;
            comboBoxCountry.Text = user.Country;
            cityBox.Text = user.City;
            phoneBox.Text = user.Phone;
            dateTimePicker1.Value = user.Birthday;
            if (user.Gender == "Male")
            {
                male.Checked = true;
            }
            else if (user.Gender == "Female")
            {
                female.Checked = true;
            }
        }

        private void Clear()
        {
            nameBox.Text = default;
            lastNameBox.Text = default;
            middleNameBox.Text = default;
            comboBoxCountry.Text = default;
            cityBox.Text = default;
            phoneBox.Text = default;
            dateTimePicker1.Value = DateTime.Now;
            male.Checked = default;
            female.Checked = default;
        }

        #endregion

        #region Events

        private void Form1_Load(object sender, EventArgs e)
        {
            using (var db = new AppDBContext())
            {
                db.Database.EnsureCreated();
            }

            countries = JsonConvert.DeserializeObject<List<Country>>(File.ReadAllText(
                $@"{Directory.GetCurrentDirectory()}\JsonFile\Country\Code.json"));
            comboBoxCountry.Items.AddRange(countries.ToArray());
        }

        private void clear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void save_Click(object sender, EventArgs e)
        {
            UserAddSystem userAddSystem = SetUserInfo;
            userAddSystem += AddUserDB;
            userAddSystem.Invoke(new User());
            CheckTextBoxs();
        }

        private void load_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
                GetUser();
            }
            catch (Exception)
            {
                MessageBox.Show("User not faund");
            }
        }

        private void comboBoxCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCountry.SelectedIndex >= 0)
            {
                codeBox.Text = countries[comboBoxCountry.SelectedIndex].Code;
                codeBox.Enabled = false;
            }
        }

        #endregion
    }
}
