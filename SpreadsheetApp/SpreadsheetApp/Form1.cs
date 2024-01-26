using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetApp
{
    public partial class Form1 : Form
    {

        static int rows = 0, cols = 0;
        private SharableSpreadSheet spreadsheet = new SharableSpreadSheet(rows, cols);
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

       

     

        private void LoadBtn_Click(object sender, EventArgs e)
        {
            if (textBox1.Text !=null && textBox1.Text != "File Path")
            {
                spreadsheet.load(textBox1.Text);
                if (spreadsheet.getCell(0, 0) != null)
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Columns.Clear();
                    dataGridView1.Refresh();
                    for (int cols = 0; cols < spreadsheet.getSize().Item2; cols++)
                    { 
                        dataGridView1.Columns.Add("", "");
                    }
                    for (int rows = 0; rows < spreadsheet.getSize().Item1; rows++)
                    {
                        dataGridView1.Rows.Add();
                    }
                    for (int cols = 0; cols < spreadsheet.getSize().Item2; cols++)
                    {
                        for (int rows = 0; rows < spreadsheet.getSize().Item1; rows++)
                        {
                            dataGridView1[cols, rows].Value = spreadsheet.getCell(rows, cols);
                        }
                    }
                    
                }
                else
                    MessageBox.Show("Error, Load Didnt Success.");
            }
            else
                MessageBox.Show("Enter Valid File Path");
            
        }

        private void SearchBtn_Click_1(object sender, EventArgs e)
        {
            Tuple<int, int> Sol = spreadsheet.searchString(textBox2.Text);
            if (Sol != null)
                MessageBox.Show("String Found ! Located in Row :[" + Sol.Item1 + "], Col :[" + Sol.Item2+"]");
            else
                MessageBox.Show("String Not Found");
        }
        private void RefreshGrid()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            for (int cols = 0; cols < spreadsheet.getSize().Item2; cols++)
            {
                dataGridView1.Columns.Add("", "");
            }
            for (int rows = 0; rows < spreadsheet.getSize().Item1; rows++)
            {
                dataGridView1.Rows.Add();
            }
            for (int cols = 0; cols < spreadsheet.getSize().Item2; cols++)
            {
                for (int rows = 0; rows < spreadsheet.getSize().Item1; rows++)
                {
                    dataGridView1[cols, rows].Value = spreadsheet.getCell(rows, cols);
                }
            }
        }

        private void SetBtn_Click_1(object sender, EventArgs e)
        {
            string text = textBox3.Text;
            spreadsheet.setCell(Convert.ToInt32(textBox5.Text), Convert.ToInt32(textBox4.Text), text);
            if (spreadsheet.getCell(Convert.ToInt32(textBox5.Text), Convert.ToInt32(textBox4.Text)) == text)
            {
                RefreshGrid();
                MessageBox.Show("String Placed Succesfully ! ");
            }
            else
                MessageBox.Show("String did not Set Succesfully");
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            spreadsheet.exchangeCols(Convert.ToInt32(textBox8.Text), Convert.ToInt32(textBox9.Text));
            {
                for (int i = 0; i < spreadsheet.getSize().Item1; i++)
                {
                    for (int j = 0; j < spreadsheet.getSize().Item2; j++)
                    {
                        dataGridView1[j, i].Value = spreadsheet.getCell(i, j);
                    }
                }
            }
            RefreshGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {
                spreadsheet.save("spreadsheet.txt");
                MessageBox.Show("Saved succesfully");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            spreadsheet.exchangeRows(Convert.ToInt32(textBox6.Text), Convert.ToInt32(textBox7.Text));
            {
                for (int i = 0; i < spreadsheet.getSize().Item1; i++)
                {
                    for (int j = 0; j < spreadsheet.getSize().Item2; j++)
                    {
                        dataGridView1[j, i].Value = spreadsheet.getCell(i, j);
                    }
                }
            }
            RefreshGrid();
        }

       
    }


            
                

        }

    

