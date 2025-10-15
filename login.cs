using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace loginApp
{
    public class Form1 : Form
    {
        private ComboBox cmbUsers;
        private TextBox txtOldPassword, txtNewPassword, txtConfirmPassword;
        private Button btnChangePassword, btnToggleTheme;
        private Label lblOldPassword, lblNewPassword, lblConfirmPassword, lblUser;
        private Panel headerPanel;
        private bool isDarkMode = false;
        private static string saUser = "sa";
        private static string saPassword = "password sa";
        private DataTable serversTable;

        public Form1()
        {
            // إعدادات عامة
            this.Text = "loginApp - الجمهورية الجزائرية الديمقراطية الشعبية";
            this.Size = new Size(650, 460);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 🔹 تحميل الخلفية من الموارد Resources
            try
            {
                this.BackgroundImage = Properties.Resources.AlgeriaFlag; // ✅ من الموارد
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch
            {
                MessageBox.Show("⚠️ لم يتم تحميل الخلفية من الموارد.");
            }

            // ===== لوحة العنوان =====
            headerPanel = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.FromArgb(200, Color.White)
            };

            Label lblTitle = new Label()
            {
                Text = "🇩🇿 الجمهورية الجزائرية الديمقراطية الشعبية",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 45
            };

            Label lblSubtitle = new Label()
            {
                Text = "تطبيق تغيير كلمة المرور في الشبكة",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 35
            };

            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(lblTitle);
            this.Controls.Add(headerPanel);

            // ===== زر التبديل بين الوضعين =====
            btnToggleTheme = new Button()
            {
                Text = "🌙 الوضع الليلي",
                Left = 10,
                Top = 10,
                Width = 140,
                Height = 35,
                BackColor = Color.DarkGray,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnToggleTheme.Click += ToggleTheme;
            this.Controls.Add(btnToggleTheme);

            // ===== عناصر الواجهة =====
            lblUser = new Label() { Text = "👤 المستخدم:", Left = 100, Top = 130, Width = 160, BackColor = Color.Transparent };
            cmbUsers = new ComboBox() { Left = 270, Top = 130, Width = 230 };

            lblOldPassword = new Label() { Text = "🔑 كلمة المرور القديمة:", Left = 100, Top = 180, Width = 160, BackColor = Color.Transparent };
            txtOldPassword = new TextBox() { Left = 270, Top = 180, Width = 230, PasswordChar = '*' };

            lblNewPassword = new Label() { Text = "🔒 كلمة المرور الجديدة:", Left = 100, Top = 230, Width = 160, BackColor = Color.Transparent };
            txtNewPassword = new TextBox() { Left = 270, Top = 230, Width = 230, PasswordChar = '*' };

            lblConfirmPassword = new Label() { Text = "✅ تأكيد كلمة المرور:", Left = 100, Top = 280, Width = 160, BackColor = Color.Transparent };
            txtConfirmPassword = new TextBox() { Left = 270, Top = 280, Width = 230, PasswordChar = '*' };

            btnChangePassword = new Button()
            {
                Text = "💾 تغيير كلمة المرور في جميع الخوادم",
                Left = 200,
                Top = 340,
                Width = 260,
                Height = 40,
                BackColor = Color.DarkGreen,
                ForeColor = Color.White
            };
            btnChangePassword.Click += BtnChangePassword_Click;

            // إضافة العناصر
            this.Controls.Add(lblUser);
            this.Controls.Add(cmbUsers);
            this.Controls.Add(lblOldPassword);
            this.Controls.Add(txtOldPassword);
            this.Controls.Add(lblNewPassword);
            this.Controls.Add(txtNewPassword);
            this.Controls.Add(lblConfirmPassword);
            this.Controls.Add(txtConfirmPassword);
            this.Controls.Add(btnChangePassword);

            // تحسين الخطوط والألوان
            ApplyLightMode();

            // تحميل المستخدمين
            LoadServersAndUsers();
        }

        // ===== الوضع الفاتح =====
        private void ApplyLightMode()
        {
            Font mainFont = new Font("Segoe UI", 13, FontStyle.Bold);
            this.BackColor = Color.White;

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.Font = mainFont;
                    lbl.ForeColor = Color.Black;
                    lbl.BackColor = Color.FromArgb(200, Color.White);
                }
                else if (ctrl is TextBox txt)
                {
                    txt.Font = new Font("Segoe UI", 13);
                    txt.ForeColor = Color.Black;
                    txt.BackColor = Color.FromArgb(245, 245, 245);
                }
                else if (ctrl is ComboBox cmb)
                {
                    cmb.Font = new Font("Segoe UI", 13);
                    cmb.ForeColor = Color.Black;
                    cmb.BackColor = Color.White;
                }
                else if (ctrl is Button btn && btn != btnToggleTheme)
                {
                    btn.Font = new Font("Segoe UI", 13, FontStyle.Bold);
                    btn.ForeColor = Color.White;
                    btn.BackColor = Color.FromArgb(0, 120, 215);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }
            }

            btnToggleTheme.Text = "🌙 الوضع الليلي";
            btnToggleTheme.BackColor = Color.DarkGray;
            btnToggleTheme.ForeColor = Color.Black;
            isDarkMode = false;
        }

        // ===== الوضع الليلي =====
        private void ApplyDarkMode()
        {
            Font mainFont = new Font("Segoe UI", 13, FontStyle.Bold);
            this.BackColor = Color.FromArgb(30, 30, 30);

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.Font = mainFont;
                    lbl.ForeColor = Color.White;
                    lbl.BackColor = Color.FromArgb(60, 60, 60);
                }
                else if (ctrl is TextBox txt)
                {
                    txt.Font = new Font("Segoe UI", 13);
                    txt.ForeColor = Color.White;
                    txt.BackColor = Color.FromArgb(45, 45, 45);
                }
                else if (ctrl is ComboBox cmb)
                {
                    cmb.Font = new Font("Segoe UI", 13);
                    cmb.ForeColor = Color.White;
                    cmb.BackColor = Color.FromArgb(45, 45, 45);
                }
                else if (ctrl is Button btn && btn != btnToggleTheme)
                {
                    btn.Font = new Font("Segoe UI", 13, FontStyle.Bold);
                    btn.ForeColor = Color.White;
                    btn.BackColor = Color.FromArgb(0, 90, 190);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }
            }

            btnToggleTheme.Text = "☀️ الوضع الفاتح";
            btnToggleTheme.BackColor = Color.DimGray;
            btnToggleTheme.ForeColor = Color.White;
            isDarkMode = true;
        }

        private void ToggleTheme(object sender, EventArgs e)
        {
            if (isDarkMode)
                ApplyLightMode();
            else
                ApplyDarkMode();
        }

        private void LoadServersAndUsers()
        {
            try
            {
                SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
                if (instance == null)
                {
                    MessageBox.Show("⚠️ هذه البيئة لا تدعم البحث التلقائي عن خوادم SQL.");
                    return;
                }

                serversTable = instance.GetDataSources();
                if (serversTable.Rows.Count == 0)
                {
                    MessageBox.Show("⚠️ لم يتم العثور على أي خوادم SQL في الشبكة.");
                    return;
                }

                foreach (DataRow row in serversTable.Rows)
                {
                    string serverName = row["ServerName"].ToString();
                    string instanceName = row["InstanceName"].ToString();
                    string fullServerName = string.IsNullOrEmpty(instanceName) ? serverName : $"{serverName}\\{instanceName}";
                    string connectionString = $"Data Source={fullServerName};Initial Catalog=databasename;User ID={saUser};Password={saPassword}";

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("SELECT utilisateur FROM utilisateurs", conn);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                string user = reader["utilisateur"].ToString();
                                if (!cmbUsers.Items.Contains(user))
                                    cmbUsers.Items.Add(user);
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠️ خطأ أثناء البحث عن الخوادم:\n" + ex.Message);
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            string selectedUser = cmbUsers.Text;
            string oldPassword = txtOldPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(selectedUser) || string.IsNullOrEmpty(oldPassword) ||
                string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("⚠️ الرجاء ملء جميع الحقول!");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("❌ كلمة المرور الجديدة وتأكيدها غير متطابقتين!");
                return;
            }

            if (serversTable == null || serversTable.Rows.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد خوادم متصلة.");
                return;
            }

            int successCount = 0;
            int failCount = 0;

            foreach (DataRow row in serversTable.Rows)
            {
                string serverName = row["ServerName"].ToString();
                string instanceName = row["InstanceName"].ToString();
                string fullServerName = string.IsNullOrEmpty(instanceName) ? serverName : $"{serverName}\\{instanceName}";
                string connectionString = $"Data Source={fullServerName};Initial Catalog=databasename;User ID={saUser};Password={saPassword}";

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM utilisateurs WHERE utilisateur=@user AND passwor=@oldPass", conn);
                        checkCmd.Parameters.AddWithValue("@user", selectedUser);
                        checkCmd.Parameters.AddWithValue("@oldPass", oldPassword);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count == 1)
                        {
                            SqlCommand updateCmd = new SqlCommand("UPDATE utilisateurs SET passwor=@newPass WHERE utilisateur=@user", conn);
                            updateCmd.Parameters.AddWithValue("@newPass", newPassword);
                            updateCmd.Parameters.AddWithValue("@user", selectedUser);
                            updateCmd.ExecuteNonQuery();
                            successCount++;
                        }
                        else
                        {
                            failCount++;
                        }
                    }
                }
                catch
                {
                    failCount++;
                }
            }

            MessageBox.Show($"✅ تم التحديث بنجاح في {successCount} خادم، وفشل في {failCount} خادم.");
        }
    }
}
