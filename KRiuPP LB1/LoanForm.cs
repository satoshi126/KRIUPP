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
    public partial class LoanForm : Form
    {
        DataBase dataBase = new DataBase();

        int selectedRow;

        public LoanForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void LoanForm_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView);
        }

        private void CreateColumns()
        {
            dataGridView.Columns.Add("ID_Loan", "ID_Loan");
            dataGridView.Columns.Add("ID_Visitor", "ID_Visitor");
            dataGridView.Columns.Add("ID_Book", "ID_Book");
            dataGridView.Columns.Add("DateOFIssue", "DateOFIssue");
            dataGridView.Columns.Add("ReturnDate", "ReturnDate");
            dataGridView.Columns.Add("Status", "Status");
            dataGridView.Columns.Add("IsNew", String.Empty);

            dataGridView.Columns[6].Visible = false;
        }

        public void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetInt32(1), record.GetInt32(2), record.GetData(3), record.GetData(4), record.GetString(5), RowState.ModifiedNew);
            dataGridView.AllowUserToAddRows = false;
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"select * from [dbo].[Loans]";

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
                textBox6.Text = row.Cells[5].Value.ToString();
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
                dataGridView.Rows[index].Cells[6].Value = RowState.Deleted;
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

                    var deleteQuery = $"DELETE FROM Authors WHERE ID_Author = {id}";

                    var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }


                if (rowState == RowState.Modified)
                {
                    var ID_Author = dataGridView.Rows[index].Cells[0].Value.ToString();
                    var FIO = dataGridView.Rows[index].Cells[1].Value.ToString();
                    var DateOfBitrthday = dataGridView.Rows[index].Cells[2].Value.ToString();
                    var Country = dataGridView.Rows[index].Cells[3].Value.ToString();

                    var changeQuery = "UPDATE Authors SET FIO = @FIO, DateOfBitrthday = @DateOfBitrthday, Country = @Country WHERE ID_Author = @ID_Author";

                    using (var command = new SqlCommand(changeQuery, dataBase.getConnection()))
                    {
                        command.Parameters.AddWithValue("@ID_Author", ID_Author);
                        command.Parameters.AddWithValue("@FIO", FIO);
                        command.Parameters.AddWithValue("@DateOfBitrthday", DateOfBitrthday);
                        command.Parameters.AddWithValue("@Country", Country);

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

            var ID_Loan = textBox1.Text;
            var ID_Visitor = textBox2.Text;
            var ID_Book = textBox3.Text;
            var DateOFIssue = textBox4.Text;
            var ReturnDate = textBox5.Text;
            var Status = textBox6.Text;

            if (dataGridView.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView.Rows[selectedRowIndex].SetValues(ID_Loan, ID_Visitor, ID_Book, DateOFIssue, ReturnDate, Status);
                dataGridView.Rows[selectedRowIndex].Cells[6].Value = RowState.Modified;
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
