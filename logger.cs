using NLog;
using System;
using System.Windows.Forms;

namespace GetOracleData
{
#pragma warning disable IDE1006
    public static class logger
#pragma warning restore IDE1006
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Error(string message, bool ShowMessageBox = false)
        {
            _logger.Error(message);
           
            if (ShowMessageBox)
            {
                MessageBox.Show($"エラー: {message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Error(Exception ex, bool ShowMessageBox = false)
        {
            _logger.Error(ex);

            if (ShowMessageBox)
            {
                MessageBox.Show($"エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
