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
    }
}
