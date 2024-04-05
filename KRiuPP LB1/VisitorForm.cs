using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KRiuPP_LB1
{
    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }
    public partial class VisitorForm : Form
    {
        DataBase dataBase = new DataBase();

        int selectedRow;
        public VisitorForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void VisitorForm_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView);
        }

        private void CreateColumns()
        {
            dataGridView.Columns.Add("ID_Visitor", "ID_Visitor");
            dataGridView.Columns.Add("FIO", "FIO");
            dataGridView.Columns.Add("Adress", "Adress");
            dataGridView.Columns.Add("PhoneNumber", "PhoneNumber");
            dataGridView.Columns.Add("Email", "Email");
            dataGridView.Columns.Add("IsNew", String.Empty);

            dataGridView.Columns[5].Visible = false;
            dataGridView.Columns[0].Width = 65;
            dataGridView.Columns[1].Width = 150;
            dataGridView.Columns[4].Width = 140;
        }

        public void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetString(2), record.GetString(3), record.GetString(4), RowState.ModifiedNew);
            dataGridView.AllowUserToAddRows = false;
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"select * from [dbo].[Visitors]";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
        }

        private void dataGridView_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[selectedRow];

                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
                textBox3.Text = row.Cells[2].Value.ToString();
                textBox4.Text = row.Cells[3].Value.ToString();
                textBox5.Text = row.Cells[4].Value.ToString();
            }
        }

        private void refresh_button_Click_1(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView);
        }
        //Удаление записи из БД.
        private void deleteRow()
        {
            int index = dataGridView.CurrentCell.RowIndex;

            dataGridView.Rows[index].Visible = false;

            if (dataGridView.Rows[index].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView.Rows[index].Cells[5].Value = RowState.Deleted;
                return;
            }
        }

        private void delete_button_Click(object sender, EventArgs e)
        {
            deleteRow();
        }

        private void Update()
        {
            dataBase.openConnection();

            for (int index = 0; index < dataGridView.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView.Rows[index].Cells[5].Value;

                if (rowState == RowState.Existed)
                    continue;

                if (rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView.Rows[index].Cells[0].Value);

                    var deleteQuery = $"DELETE FROM Visitors WHERE ID_Visitor = {id}";

                    var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }


                if (rowState == RowState.Modified)
                {
                    var ID_Visitor = dataGridView.Rows[index].Cells[0].Value.ToString();
                    var FIO = dataGridView.Rows[index].Cells[1].Value.ToString();
                    var Adress = dataGridView.Rows[index].Cells[2].Value.ToString();
                    var PhoneNumber = dataGridView.Rows[index].Cells[3].Value.ToString();
                    var Email = dataGridView.Rows[index].Cells[4].Value.ToString();

                    var changeQuery = "UPDATE Visitors SET FIO = @FIO, Adress = @Adress, PhoneNumber = @PhoneNumber, Email = @Email WHERE ID_Visitor = @ID_Visitor";

                    using (var command = new SqlCommand(changeQuery, dataBase.getConnection()))
                    {
                        command.Parameters.AddWithValue("@ID_Visitor", ID_Visitor);
                        command.Parameters.AddWithValue("@FIO", FIO);
                        command.Parameters.AddWithValue("@Adress", Adress);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@Email", Email);

                        command.ExecuteNonQuery();
                    }
                }
            }

            dataBase.closeConnection();
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            Update();
        }

        //Изменение записей
        private void Change()
        {
            var selectedRowIndex = dataGridView.CurrentCell.RowIndex;

            var ID_Visitor = textBox1.Text;
            var FIO = textBox2.Text;
            var Adress = textBox3.Text;
            var PhoneNumber = textBox4.Text;
            var Email = textBox5.Text;

            if (dataGridView.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView.Rows[selectedRowIndex].SetValues(ID_Visitor, FIO, Adress, PhoneNumber, Email);
                dataGridView.Rows[selectedRowIndex].Cells[5].Value = RowState.Modified;
            }
        }

        private void change_button_Click(object sender, EventArgs e)
        {
            Change();
        }
        private void add_button_Click_1(object sender, EventArgs e)
        {

        }

        public DataTable queryReturnData(string query, DataTable dataTable)
        {
            SqlConnection myCon = new SqlConnection(@"Data Source=MIKHAILPC1;Initial Catalog=LibraryDB;Integrated Security=True");
            myCon.Open();

            SqlDataAdapter SDA = new SqlDataAdapter(query, myCon);
            SDA.SelectCommand.ExecuteNonQuery();

            SDA.Fill(dataTable);
            return dataTable;
        }
    }
}
