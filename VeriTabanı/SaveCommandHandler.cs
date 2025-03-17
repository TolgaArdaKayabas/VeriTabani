using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;

namespace VeriTabanı
{

    public class SaveCommandHandler : ICommandHandler
    {
        XRDesignPanel panel;
        string name;

        public SaveCommandHandler(XRDesignPanel panel, string name)
        {
            this.panel = panel;
            this.name = name;
        }
        public void HandleCommand(ReportCommand command, object[] args)
        {
            // Save the report.
            Save();
        }
        public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler)
        {
            useNextHandler = !(command == ReportCommand.SaveFile ||
                command == ReportCommand.SaveFileAs);
            return !useNextHandler;
        }
        void Save()
        {
            
            MemoryStream stream = new MemoryStream();
            var report = panel.Report;

            string name = this.name;

            report.SaveLayoutToXml(stream);

            stream.Position = 0;
            byte[] data = stream.ToArray();

            string connectionString = $"Data Source=127.0.0.1\\SQLEXPRESS;User ID=sa;Password=asd123";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string sql = $@"UPDATE [KumasKontrol].[dbo].[REP_tblREPORTs] SET REPORT = @Data WHERE RAPORBASLIK = '{name}'";

            using(SqlCommand cmd = new SqlCommand(sql, connection))
            {

                try
                {
                    cmd.Parameters.Add("@Data", System.Data.SqlDbType.VarBinary, data.Length).Value = data;
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data stored successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            // Prevent the "Report has been changed" dialog from being shown.
            panel.ReportState = ReportState.Saved;
        }
    }
}
