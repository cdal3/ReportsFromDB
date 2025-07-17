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

[CustomBehavior]
public class ReportObjectBehavior : BaseNetBehavior
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined behavior is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined behavior is stopped
    }

    [ExportMethod]
    public void GenerateReport()
    {
        Log.Info("Generating report...");
        var myScript = Project.Current.GetObject($"Reports/BatchReports/{Node.BrowseName}/{Node.BrowseName}Logic");
        Log.Info($"Reports/BatchReports/{Node.BrowseName}/{Node.BrowseName}Logic/QueryReportData being executed");
        myScript.ExecuteMethod("QueryReportData");
    }

#region Auto-generated code, do not edit!
    protected new ReportObject Node => (ReportObject)base.Node;
#endregion
}
