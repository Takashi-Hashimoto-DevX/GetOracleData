using System;
using System.Data;
using System.IO;
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

        private void B1_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM MY_TABLE";
            DataTable dataTable;


            if (!oraDB.Open())
            {
                logger.Error("データベース接続に失敗しました。", true);
                return;
            }

            try
            {
                if (!oraDB.Execute(sql,out dataTable))
                {
                    throw new Exception("データ取得に失敗しました。");
                }

                string filePath = Path.Combine(AppContext.BaseDirectory, "MY_TABLE.xlsx");

                if (Excel.Export(dataTable, filePath))
                {
                    MessageBox.Show("Excelファイルの出力しました。\n" + filePath, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, true);
            }
            finally
            {
                oraDB.Close();
            }
        }
    }
}
