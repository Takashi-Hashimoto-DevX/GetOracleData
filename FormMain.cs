using System;
using System.Windows.Forms;

namespace GetOracleData
{
    public partial class FormMain: Form
    {
        private readonly OracleDB oraDB;

        public FormMain()
        {
            InitializeComponent();
            oraDB = new OracleDB();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (oraDB.Open())
            {
                MessageBox.Show("Database Open Success");
            }
            else
            {
                MessageBox.Show("Database Open Error");
            }
        }
    }
}
