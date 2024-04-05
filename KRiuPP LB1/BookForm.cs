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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KRiuPP_LB1
{
    public partial class BookForm : Form
    {
        DataBase dataBase = new DataBase();

        int selectedRow;

        public BookForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void BookForm_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView);
        }

        private void CreateColumns()
        {
            dataGridView.Columns.Add("ID_Book", "ID_Book");
            dataGridView.Columns.Add("ID_Author", "ID_Author");
            dataGridView.Columns.Add("Name", "Name");
            dataGridView.Columns.Add("DateOfPublishing", "DateOfPublishing");
            dataGridView.Columns.Add("IsNew", String.Empty);

            dataGridView.Columns[4].Visible = false;
        }

        public void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetInt32(1), record.GetString(2), record.GetData(3), RowState.ModifiedNew);
            dataGridView.AllowUserToAddRows = false;
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"select * from [dbo].[Books]";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }

            reader.Close();
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView.Rows[selectedRow];

                textBox1.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
                textBox3.Text = row.Cells[2].Value.ToString();
                textBox4.Text = row.Cells[3].Value.ToString();
            }
        }

        private void refresh_button_Click(object sender, EventArgs e)
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
                dataGridView.Rows[index].Cells[4].Value = RowState.Deleted;
                return;
            }
        }

        private void delete_button_Click_1(object sender, EventArgs e)
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

                    var deleteQuery = $"DELETE FROM Books WHERE ID_Book = {id}";

                    var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }


                if (rowState == RowState.Modified)
                {
                    var ID_Book = dataGridView.Rows[index].Cells[0].Value.ToString();
                    var ID_Author = dataGridView.Rows[index].Cells[1].Value.ToString();
                    var Name = dataGridView.Rows[index].Cells[2].Value.ToString();
                    var DateOfPublishing = dataGridView.Rows[index].Cells[3].Value.ToString();

                    var changeQuery = "UPDATE Authors SET ID_Author = @ID_Author, Name = @Name, DateOfPublishing = @DateOfPublishing WHERE ID_Book = @ID_Book";

                    using (var command = new SqlCommand(changeQuery, dataBase.getConnection()))
                    {
                        command.Parameters.AddWithValue("@ID_Book", ID_Book);
                        command.Parameters.AddWithValue("@ID_Author", ID_Author);
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@DateOfPublishing", DateOfPublishing);

                        command.ExecuteNonQuery();
                    }
                }
            }

            dataBase.closeConnection();
        }

        private void save_button_Click_1(object sender, EventArgs e)
        {
            Update();
        }

        //Изменение записей
        private void Change()
        {
            var selectedRowIndex = dataGridView.CurrentCell.RowIndex;

            var ID_Book = textBox1.Text;
            var ID_Author = textBox2.Text;
            var Name = textBox3.Text;
            var DateOfPublishing = textBox4.Text;

            if (dataGridView.Rows[selectedRowIndex].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView.Rows[selectedRowIndex].SetValues(ID_Book, ID_Author, Name, DateOfPublishing);
                dataGridView.Rows[selectedRowIndex].Cells[4].Value = RowState.Modified;
            }
        }

        private void change_button_Click_1(object sender, EventArgs e)
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
