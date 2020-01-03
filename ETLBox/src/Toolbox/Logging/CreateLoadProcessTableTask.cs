﻿
using ALE.ETLBox.ConnectionManager;
using ALE.ETLBox.ControlFlow;
using System;
using System.Collections.Generic;

namespace ALE.ETLBox.Logging
{
    /// <summary>
    /// Will create the default load process table for the default database logging.
    /// You can then use the `StartLoadProcessTask`, `AbortLoadProcessTask`, `EndLoadProcessTask`.
    /// These will generate entries in this table and populate the right fields.
    /// </summary>
    /// <see cref="StartLoadProcessTask"/>
    /// <see cref="EndLoadProcessTask" />
    /// <see cref="AbortLoadProcessTask" />
    public class CreateLoadProcessTableTask : GenericTask, ITask
    {
        /* ITask Interface */
        public override string TaskName => $"Create default etlbox load process table";
        public string LoadProcessTableName { get; set; } = "etlbox_load_process";
        public string Sql => LoadProcessTable.Sql;
        public CreateTableTask LoadProcessTable { get; private set; }
        public void Execute()
        {
            LoadProcessTable.CopyTaskProperties(this);
            LoadProcessTable.Create();
            ControlFlow.ControlFlow.CurrentLoadProcessTable = LoadProcessTableName;
        }

        public CreateLoadProcessTableTask(string loadProcessTableName)
        {
            this.LoadProcessTableName = loadProcessTableName;
            InitCreateTableTask();
        }

        public CreateLoadProcessTableTask(IConnectionManager connectionManager, string loadProcessTableName) : this(loadProcessTableName)
        {
            this.ConnectionManager = connectionManager;
        }

        private void InitCreateTableTask()
        {
            List<ITableColumn> lpColumns = new List<ITableColumn>() {
                    new TableColumn("load_process_id","INT", allowNulls: false, isPrimaryKey: true, isIdentity:true),
                    new TableColumn("start_date","DATETIME", allowNulls: false),
                    new TableColumn("end_date","DATETIME", allowNulls: true),
                    new TableColumn("source","NVARCHAR(20)", allowNulls: true),
                    new TableColumn("process_name","NVARCHAR(100)", allowNulls: false) { DefaultValue = "N/A" },
                    new TableColumn("start_message","NVARCHAR(4000)", allowNulls: true)  ,
                    new TableColumn("is_running","SMALLINT", allowNulls: false) { DefaultValue = "1" },
                    new TableColumn("end_message","NVARCHAR(4000)", allowNulls: true)  ,
                    new TableColumn("was_successful","SMALLINT", allowNulls: false) { DefaultValue = "0" },
                    new TableColumn("abort_message","NVARCHAR(4000)", allowNulls: true) ,
                    new TableColumn("was_aborted","SMALLINT", allowNulls: false) { DefaultValue = "0" }
                };
            LoadProcessTable = new CreateTableTask(LoadProcessTableName, lpColumns) { DisableLogging = true };
        }

        public static void Create(string loadProcessTableName = "etlbox_load_process")
            => new CreateLoadProcessTableTask(loadProcessTableName).Execute();
        public static void Create(IConnectionManager connectionManager, string loadProcessTableName = "etlbox_load_process")
            => new CreateLoadProcessTableTask(connectionManager, loadProcessTableName).Execute();

    }
}


