using DevExpress.LookAndFeel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace VeriTabanı
{
    public partial class Form1 : Form
    {
        string db;
        string tbl;
        string rep_name;
        DataTable data = new DataTable();
        private BindingSource bindingSource;
        XRDesignMdiController mdiController;


        public Form1()
        {
            InitializeComponent();
            Form_Load();
        }


        private void Form_Load()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "127.0.0.1\\SQLEXPRESS",
                UserID = "sa",
                Password = "asd123",
            };

            string connectionString = builder.ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();


            SqlCommand databases = new SqlCommand("SELECT name FROM sys.databases;", connection);
            SqlDataReader reader = databases.ExecuteReader();

            DataTable database_table = new DataTable();
            database_table.Load(reader);

            comboBox1.DataSource = database_table;
            comboBox1.DisplayMember = "name";

            connection.Close();

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                if (comboBox1.SelectedValue != null)
                {
                    db = comboBox1.Text;
                }

                string connectionString = $"Data Source=127.0.0.1\\SQLEXPRESS;Database={db};User ID=sa;Password=asd123";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string table = $@"
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';";

                using (SqlCommand tables = new SqlCommand(table, connection))
                {
                    SqlDataReader reader = tables.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    comboBox2.DataSource = dataTable;
                    comboBox2.DisplayMember = "TABLE_NAME";
                }

                connection.Close();


            }));


        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            this.BeginInvoke(new Action(() =>
            {
                if (comboBox1.SelectedValue != null)
                {
                    tbl = comboBox2.Text;
                }


                string connectionString = $"Data Source=127.0.0.1\\SQLEXPRESS;Database={db};User ID=sa;Password=asd123";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                string table = $@"
SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = '{tbl}';
";


                using (SqlCommand tables = new SqlCommand(table, connection))
                {
                    SqlDataReader reader = tables.ExecuteReader();

                    DataTable table_table = new DataTable();
                    table_table.Load(reader);

                    bindingSource = new BindingSource();
                    data = table_table.Copy();
                    bindingSource.DataSource = data;

                    table_table.Rows.Add("*");




                    checkedListBox1.DataSource = table_table;
                    checkedListBox1.DisplayMember = "COLUMN_NAME";

                    comboBox4.DataSource = bindingSource;
                    comboBox4.DisplayMember = "COLUMN_NAME";
                    comboBox4.ValueMember = "DATA_TYPE";

                }

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                }
                connection.Close();


            }));



        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            string connectionString = $"Data Source=127.0.0.1\\SQLEXPRESS;Database={db};User ID=sa;Password=asd123";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            if (comboBox3.Text != null)
            {
                string str = (string)comboBox3.SelectedValue;
                str = str.Substring(0, str.Length - 5);
                string sql = $@"SELECT * FROM [KumasKontrol].[dbo].[QBAR_tblData_ERPIDs] WHERE {str}";

                using (SqlCommand querry = new SqlCommand(sql, connection))
                {
                    SqlDataReader reader = querry.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    dataGridView1.DataSource = dataTable;

                }
            }
            connection.Close();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = $"Data Source=127.0.0.1\\SQLEXPRESS;Database={db};User ID=sa;Password=asd123";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            if (checkedListBox1.CheckedItems.Count > 0)
            {
                string get = "SELECT ";

                if (textBox1.Text.Equals(""))
                {

                    foreach (DataRowView item in checkedListBox1.CheckedItems)
                    {
                        get += item["COLUMN_NAME"] + ", ";
                    }
                    get = get.TrimEnd(',', ' ');
                    get += $@" FROM {tbl}";
                }

                else
                {
                    switch (comboBox4.SelectedValue)
                    {

                        case "int":

                            foreach (DataRowView item in checkedListBox1.CheckedItems)
                            {
                                get += item["COLUMN_NAME"] + ", ";
                            }
                            get = get.TrimEnd(',', ' ');

                            get += $@" FROM {tbl} WHERE {comboBox4.Text} = {textBox1.Text}";

                            break;

                        case "nvarchar":

                            foreach (DataRowView item in checkedListBox1.CheckedItems)
                            {
                                get += item["COLUMN_NAME"] + ", ";
                            }
                            get = get.TrimEnd(',', ' ');

                            get += $@" FROM {tbl} WHERE {comboBox4.Text} = '{textBox1.Text}'";

                            break;

                        case "bit":

                            foreach (DataRowView item in checkedListBox1.CheckedItems)
                            {
                                get += item["COLUMN_NAME"] + ", ";
                            }
                            get = get.TrimEnd(',', ' ');

                            get += $@" FROM {tbl} WHERE {comboBox4.Text} = {textBox1.Text}";

                            break;

                    }


                }

                using (SqlCommand data = new SqlCommand(get, connection))
                {
                    SqlDataReader reader = data.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    dataGridView2.DataSource = dataTable;
                }
            }
            comboBox4.SelectedIndex = -1;
            comboBox4.Text = "";
            textBox1.Clear();

        }
        private void tab_changed(object sender, EventArgs e)
        {

            string connectionString = $"Data Source=127.0.0.1\\SQLEXPRESS;Database={db};User ID=sa;Password=asd123";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            if (tabControl1.SelectedIndex == 1)
            {
                string sql = $@"SELECT ISLEMID, TYPE_DEFINATION, ROLLNUMBER, DETAIL1, DETAIL2 FROM [KumasKontrol].[dbo].[QBAR_tblData_ERPIDs];";


                using (SqlCommand querry = new SqlCommand(sql, connection))
                {
                    SqlDataReader reader = querry.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    DataTable list = new DataTable();
                    list.Columns.Add("Show");
                    list.Columns.Add("String");

                    try
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            string str = "";
                            string show = "";
                            foreach (DataColumn column in dataTable.Columns)
                            {
                                if (column.ColumnName == "ISLEMID") { str += column.ColumnName + " = " + row[column.ColumnName] + "AND "; }
                                str += column.ColumnName + " = '" + row[column.ColumnName] + "' AND ";
                                show += column.ColumnName + " " + row[column.ColumnName] + " ";
                            }
                            list.Rows.Add(show, str);
                        }
                    }
                    catch { }

                    comboBox3.DataSource = list;
                    comboBox3.ValueMember = "String";
                    comboBox3.DisplayMember = "Show";

                }
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                string sql = $@"SELECT [ID],[RAPORBASLIK] FROM [KumasKontrol].[dbo].[REP_tblREPORTs]";

                using (SqlCommand querry = new SqlCommand(sql, connection))
                {
                    SqlDataReader reader = querry.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    comboBox5.DataSource = dataTable;
                    comboBox5.ValueMember = "ID";
                    comboBox5.DisplayMember = "RAPORBASLIK";
                }
            }
            connection.Close();


        }
        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            string filterText = comboBox4.Text;
            int selectionStart = comboBox4.SelectionStart;

            if (filterText.Equals("")) { return; }
            bindingSource.Filter = $"COLUMN_NAME LIKE '%{filterText}%'";

            comboBox4.Text = filterText;
            comboBox4.SelectionStart = selectionStart;

            if (bindingSource.Count > 0)
            {
                comboBox4.DroppedDown = true;
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                comboBox2.SelectedIndex -= 1;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {


            if (comboBox2.Items.Count > comboBox2.SelectedIndex + 1)
            {
                comboBox2.SelectedIndex += 1;
            }

        }
        private void button4_Click(object sender, EventArgs e)
        {
            var id = comboBox5.SelectedValue;

            string connectionString = $"Data Source=127.0.0.1\\SQLEXPRESS;User ID=sa;Password=asd123";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            byte[] binaryData = null;

            string sql = $@"SELECT [REPORT], [DATASOURCE] FROM [KumasKontrol].[dbo].[REP_tblREPORTs] WHERE ID={id};";

            using (SqlCommand querry = new SqlCommand(sql, connection))
            {
                using (SqlDataReader reader = querry.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        binaryData = (byte[])reader["REPORT"];
                    }
                }
            }

            XRDesignForm form = new XRDesignForm();
            form.Name = comboBox5.Text;
            rep_name = comboBox5.Text;
            mdiController = form.DesignMdiController;
            UserLookAndFeel.Default.SetSkinStyle("Office 2016 Dark");

            mdiController.DesignPanelLoaded += new DesignerLoadedEventHandler(mdiController_DesignPanelLoaded);


            if (binaryData != null)
            {

                Stream report_stream = new MemoryStream(binaryData);

                XtraReport report = XtraReport.FromXmlStream(report_stream);

                mdiController.OpenReport(report);
            }
            else
            {
                MessageBox.Show("No binary data retrieved from the database.");
            }

            form.ShowDialog();
            if (mdiController.ActiveDesignPanel != null)
            {
                mdiController.ActiveDesignPanel.CloseReport();
            }

            connection.Close();
        }
        void mdiController_DesignPanelLoaded(object sender, DesignerLoadedEventArgs e)
        {
            XRDesignPanel panel = (XRDesignPanel)sender;
            panel.AddCommandHandler(new SaveCommandHandler(panel, rep_name));
        }
        private void button5_Click(object sender, EventArgs e)
        {

            if (textBox2.Text.Equals(""))
            {
                MessageBox.Show("Rapor Adını Giriniz!");
                return;
            }
            string connectionString = $"Data Source=127.0.0.1\\SQLEXPRESS;User ID=sa;Password=asd123";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string sql = $@"INSERT INTO [KumasKontrol].[dbo].[REP_tblREPORTs] ([RAPORBASLIK]) VALUES ('{textBox2.Text}'  )";

            using (SqlCommand querry = new SqlCommand(sql, connection))
            {
                querry.ExecuteNonQuery();
            }

            XRDesignForm form = new XRDesignForm();
            form.Name = textBox2.Text;
            rep_name = textBox2.Text;
            mdiController = form.DesignMdiController;
            UserLookAndFeel.Default.SetSkinStyle("Office 2016 Dark");

            mdiController.DesignPanelLoaded +=
                new DesignerLoadedEventHandler(mdiController_DesignPanelLoaded);

            mdiController.OpenReport(new XtraReport());

            form.ShowDialog();
            if (mdiController.ActiveDesignPanel != null)
            {
                mdiController.ActiveDesignPanel.CloseReport();
            }

            connection.Close();
        }
    }
}
