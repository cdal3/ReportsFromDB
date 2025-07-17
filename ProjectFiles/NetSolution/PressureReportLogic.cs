#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.DataLogger;
using FTOptix.Report;
using FTOptix.OPCUAServer;
#endregion

public class PressureReportLogic : BaseNetLogic
{
    private Report myReport;
    private ResourceUri reportPath;
    private IUAVariable latestReportPath;
    private string reportLocale = "en-US";
    private Store myStore;
    private IUAVariable status;

    public override void Start()
    {
        
    }

    public override void Stop()
    {
        myReport.OnGeneratePdfCompleted -= MyReport_OnGeneratePdfCompleted;
    }

    [ExportMethod]
    public void QueryReportData()
    {
        string reportName = Owner.BrowseName.ToString();
        myReport = Project.Current.Get<Report>($"Reports/BatchReports/{reportName}");
        latestReportPath = Project.Current.GetVariable("Model/LatestReportPath");
        myStore = Project.Current.Get<Store>("DataStores/BatchDataStore");
        status = Owner.GetVariable("Status");
        
        var startTime = Owner.GetVariable("StartTime");
        var endTime = Owner.GetVariable("EndTime");
        var duration = Owner.GetVariable("Duration");
        var batchID = Owner.GetVariable("BatchID");
        var pressureMax = Owner.GetVariable("PressureMax");
        var pressureMin = Owner.GetVariable("PressureMin");

        Object[,] ResultSet;
        String[] Header;

        myStore.Query("SELECT MIN(LocalTimestamp) FROM BatchDataLogger WHERE BatchID = '" + batchID.Value + "'", out Header, out ResultSet);
        startTime.Value = Convert.ToDateTime(ResultSet[0, 0]);
        myStore.Query("SELECT MAX(LocalTimestamp) FROM BatchDataLogger WHERE BatchID = '" + batchID.Value + "'", out Header, out ResultSet);
        endTime.Value = Convert.ToDateTime(ResultSet[0, 0]);
        duration.Value = ((DateTime)endTime.Value - (DateTime)startTime.Value).TotalMilliseconds;
        myStore.Query("SELECT MAX(Pressure) FROM BatchDataLogger WHERE BatchID = '" + batchID.Value + "'", out Header, out ResultSet);
        pressureMax.Value = Convert.ToSingle(ResultSet[0, 0]);
        myStore.Query("SELECT MIN(Pressure) FROM BatchDataLogger WHERE BatchID = '" + batchID.Value + "'", out Header, out ResultSet);
        pressureMin.Value = Convert.ToSingle(ResultSet[0, 0]);

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string batchIDValue = batchID.Value;
        reportPath = ResourceUri.FromProjectRelativePath($"BatchReport_{batchIDValue}_{timestamp}.pdf");
        myReport.GeneratePdf(reportPath, reportLocale, out Guid operationId);
        myReport.OnGeneratePdfCompleted += MyReport_OnGeneratePdfCompleted;
        status.Value =  $"Report generation started";
    }

    private void MyReport_OnGeneratePdfCompleted(object sender, GeneratePdfCompletedEvent e)
    {
        Log.Info("PdfReportLogic", $"Report generation completed with result: {e.Result}");
        if (e.Result == GeneratePdfCompletedResult.PdfSuccessfullyGenerated)
        {
            latestReportPath.Value = reportPath.Uri;
            status.Value = "Report successfully generated";
        }
        else
        {
            status.Value = $"Report generation failed, error: {e.Result}";
        }
    }
}
