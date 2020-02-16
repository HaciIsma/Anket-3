using MetroFramework.Forms;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
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

        private void GetUserDB(DbDataReader reader)
        {
            while (reader.Read())
            {
                nameBox.Text = reader.GetString(1);
                lastNameBox.Text = reader.GetString(2);
                middleNameBox.Text = reader.GetString(3);
                comboBoxCountry.Text = reader.GetString(4);
                cityBox.Text = reader.GetString(5);
                phoneBox.Text = reader.GetString(6);
                dateTimePicker1.Value = DateTime.Parse(reader.GetString(7));
                if (reader.GetString(8) == "Male")
                {
                    male.Checked = true;
                }
                else if (reader.GetString(8) == "Female")
                {
                    female.Checked = true;
                }
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

        private void SearchUserAndGetUser(string query)
        {
            using (var ctx = new AppDBContext())
            {
                using (var command = ctx.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    ctx.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        GetUserDB(result);
                    }
                }
            }
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
                SearchUserAndGetUser($@"SELECT * from Users WHERE name = '{textBox1.Text}'");
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
