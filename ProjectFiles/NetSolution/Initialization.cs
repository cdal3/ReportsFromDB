#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.DataLogger;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Report;
using FTOptix.OPCUAServer;
using FTOptix.Retentivity;
using FTOptix.Core;
using System.ComponentModel.Design;
#endregion

public class Initialization : BaseNetLogic
{
    public override void Start()
    {
        var myStore = Project.Current.Get<Store>("DataStores/BatchDataStore");
        var batchID = Project.Current.GetVariable("Model/BatchData/BatchID");
        var batchRunNumber = Project.Current.GetVariable("Model/BatchData/BatchRunNumber");
        string prefix = "BATCH";
        int maxBatchNumber = -1;

        Object[,] resultSet;
        String[] header;

        // Query all BatchID values
        myStore.Query("SELECT BatchID FROM BatchDataLogger", out header, out resultSet);

        if (resultSet != null && resultSet.GetLength(0) > 0)
        {
            for (int i = 0; i < resultSet.GetLength(0); i++)
            {
            var batchIDString = resultSet[i, 0]?.ToString();
            if (!string.IsNullOrEmpty(batchIDString) && batchIDString.StartsWith(prefix))
            {
                var numberPart = batchIDString.Substring(prefix.Length);
                if (int.TryParse(numberPart, out int batchNumber))
                {
                if (batchNumber > maxBatchNumber)
                    maxBatchNumber = batchNumber;
                }
            }
            }
            batchRunNumber.Value = maxBatchNumber + 1;
            batchID.Value = $"{prefix}{maxBatchNumber + 1}";
        }
        else
        {
            batchRunNumber.Value = 0;
            batchID.Value = $"{prefix}0";
        }
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

}
