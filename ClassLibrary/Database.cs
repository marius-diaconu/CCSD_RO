using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SQLite;
using Dapper;

namespace ClassLibrary
{
    public class Database
    {
        private string _db_dir = "database";
        private string _db_name = "sqlite.db";
        private string _db_path;
        protected SQLiteConnection sqlite;
        public bool is_open;

        public Database()
        {
            string db_dir = Path.Combine(Utility.app_dir,  this._db_dir);
            this.db_path = Path.Combine(db_dir, this.db_name);

            if (!Directory.Exists(@db_dir))
            {
                try
                {
                    Directory.CreateDirectory(@db_dir);
                }
                catch (UnauthorizedAccessException)
                {
                    Utility.ChangeDirectorySecurity(Utility.app_dir);
                    Directory.CreateDirectory(@db_dir);
                }
            }

            if (this.CheckIfDBExists())
            {
                this.sqlite = new SQLiteConnection("Data Source=" + this.db_path);
                if (this.OpenConn())
                {
                    this.is_open = true;
                }
            }
        } // end of construct method

        public string db_name
        {
            get { return _db_name; }
        }

        public string db_path
        {
            get { return _db_path; }
            set { _db_path = value; }
        }

        public bool OpenConn()
        {
            try
            {
                this.sqlite.Open();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of OpenConn method

        public bool CloseConn()
        {
            try
            {
                this.sqlite.Close();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of CloseConn method

        public bool ExecuteQuery(string sql)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, this.sqlite);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of ExecuteQuery method

        public bool CheckIfDBExists()
        {
            if (!File.Exists(this.db_path))
            {
                SQLiteConnection.CreateFile(this.db_path);
                return true;
            }
            else if (File.Exists(this.db_path))
            {
                return true;
            }
            return false;
        } // end of CheckIfDBExists method

        public void CheckIfTablesExists()
        {
            if (!this.HasTable("tasks"))
            {
                this.CreateTasksTable();
            }
            else
            {
                if (this.HasColumn("tasks", "task_frequency"))
                {
                    this.ExecuteQuery("ALTER TABLE `tasks` RENAME COLUMN `task_frequency` TO `task_interval`");
                }

                if (!this.HasColumn("tasks", "task_interval"))
                {
                    this.ExecuteQuery("ALTER TABLE `tasks` ADD COLUMN `task_interval` INTEGER UNSIGNED DEFAULT 0");
                }
            }

            if (!this.HasTable("logs"))
            {
                this.CreateLogsErrorsTable("logs");
            }

            if (!this.HasTable("errors"))
            {
                this.CreateLogsErrorsTable("errors");
            }
        } // end of CheckIfTablesExists method

        public void CheckTablesOverload()
        {
            if (this.HasTable("tasks"))
            {
                this.CheckTableOverload("tasks");
            }

            if (this.HasTable("logs"))
            {
                this.CheckTableOverload("logs");
            }

            if (this.HasTable("errors"))
            {
                this.CheckTableOverload("errors");
            }
        } // end of CheckTablesOverload method

        public bool HasColumn(string table_name, string column_name)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM pragma_table_info('" + table_name + "') " +
                    "WHERE name= '" + column_name + "'", this.sqlite);
                int response = Convert.ToInt32(command.ExecuteScalar());
                return Convert.ToBoolean(response);
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of HasColumn method

        public bool HasTable(string table_name)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand("SELECT name FROM `sqlite_master` WHERE type = 'table' " +
                    "AND name = '" + table_name + "'", this.sqlite);
                string response = (string)command.ExecuteScalar();
                return !string.IsNullOrEmpty(response);
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of HasTable method

        public int CountRows(string table_name)
        {
            switch (table_name)
            {
                case "tasks":
                    var tasks = this.GetTasks("SELECT * FROM `" + table_name + "`");
                    return tasks.Count;
                case "logs":
                    var logs = this.GetLogs("SELECT * FROM `" + table_name + "`");
                    return logs.Count;
                case "errors":
                    var errors = this.GetErrors("SELECT * FROM `" + table_name + "`");
                    return errors.Count;
                default: return 0; 
            }
        } // end of CountRows method

        public bool CreateTasksTable()
        {
            var sql = "CREATE TABLE IF NOT EXISTS `tasks` (" +
                "`id` INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "`task_uuid` TEXT NOT NULL, " +
                "`task_name` TEXT NOT NULL, " +
                "`source_path` TEXT NOT NULL, " +
                "`dest_path` TEXT NULL DEFAULT NULL, " +
                "`clear_dest` INTEGER UNSIGNED DEFAULT 0, " +
                "`sub_dirs` INTEGER UNSIGNED DEFAULT 0, " +
                "`task_action` INTEGER UNSIGNED DEFAULT NULL, " +
                "`task_start_date` TEXT NOT NULL, " +
                "`task_time` TEXT NOT NULL, " +
                "`task_last_run` TEXT NULL DEFAULT NULL, " +
                "`task_next_run` TEXT NULL DEFAULT NULL, " +
                "`task_end_date` TEXT NULL DEFAULT NULL, " +
                "`task_type` TEXT NOT NULL, " +
                "`task_repetable` INTEGER UNSIGNED DEFAULT 0, " +
                "`task_interval` INTEGER UNSIGNED DEFAULT 0, " +
                "`task_stopped` INTEGER UNSIGNED DEFAULT 0, " +
                "`created_at` TEXT NOT NULL, " +
                "`updated_at` TEXT NULL DEFAULT NULL)";

            return this.ExecuteQuery(sql);
        } // end of CreateTasksTable method

        public bool CreateLogsErrorsTable(string table_name)
        {
            var sql = "CREATE TABLE IF NOT EXISTS `" + table_name + "` (" +
                "`id` INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "`filepath` TEXT NOT NULL, " +
                "`filename` TEXT NOT NULL, " +
                "`created_at` TEXT NOT NULL)";

            return this.ExecuteQuery(sql);
        } // end of CreateLogsErrorsTable method

        public void CheckTableOverload(string table_name)
        {
            SqliteSequence sequence = new SqliteSequence();
            sequence = sequence.First(table_name);
            long maximum = (long)(Math.Pow(2, 32) - Math.Pow(2, 16));
            if (sequence.Exists() && sequence.seq > maximum)
            {
                var sql = "ALTER TABLE `" + table_name + "` RENAME TO `old_" + table_name + "`";
                if (this.ExecuteQuery(sql))
                {
                    if (table_name == "tasks")
                    {
                        string[] table_fields = {
                            "task_uuid",
                            "task_name",
                            "source_path",
                            "dest_path",
                            "clear_dest",
                            "sub_dirs",
                            "task_action",
                            "task_start_date",
                            "task_time",
                            "task_last_run",
                            "task_next_run",
                            "task_end_date",
                            "task_type",
                            "task_repetable",
                            "task_interval",
                            "task_stopped",
                            "created_at",
                            "updated_at"
                        };

                        if (this.CreateTasksTable())
                        {
                            sql = "INSERT INTO `" + table_name + "` (`" +
                                string.Join("`,`", table_fields) + "`) " +
                                "SELECT `" + string.Join("`,`", table_fields) + "` FROM `old_" + table_name + "`";

                            if (this.ExecuteQuery(sql))
                            {
                                if (this.ExecuteQuery("DROP TABLE `old_" + table_name + "`"))
                                {
                                    Tasks task = new Tasks();
                                    task = task.First("SELECT * FROM `" + task.table + "` ORDER BY `id` DESC LIMIT 1");
                                    sequence.seq = task.id;
                                    sequence.Update();
                                }
                            }
                        }
                    }
                    else
                    {
                        string[] table_fields = {
                            "filepath",
                            "filename",
                            "created_at"
                        };

                        if (this.CreateLogsErrorsTable(table_name))
                        {
                            sql = "INSERT INTO `" + table_name + "` (`" +
                                string.Join("`,`", table_fields) + "`) " +
                                "SELECT `" + string.Join("`,`", table_fields) + "` FROM `old_" + table_name + "`";

                            if (this.ExecuteQuery(sql))
                            {
                                if (this.ExecuteQuery("DROP TABLE `old_" + table_name + "`"))
                                {
                                    if (table_name == "logs")
                                    {
                                        Logs log = new Logs();
                                        log = log.First("desc");
                                        sequence.seq = log.id;
                                        sequence.Update();
                                    }

                                    if (table_name == "errors")
                                    {
                                        Errors error = new Errors();
                                        error = error.First("desc");
                                        sequence.seq = error.id;
                                        sequence.Update();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        } // end of CheckTableOverload method

        public string TrimValue(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                str = str.TrimStart(' ');
                str = str.TrimEnd(' ');
                return str.ToString();
            }
            return null;
        } // end of TrimValue method

        public List<Tasks> GetTasks(string sql)
        {
            try
            {
                return this.sqlite.Query<Tasks>(sql, new DynamicParameters()).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of GetTasks method

        public List<Logs> GetLogs(string sql)
        {
            try
            {
                return this.sqlite.Query<Logs>(sql, new DynamicParameters()).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of GetLogs method

        public List<Errors> GetErrors(string sql)
        {
            try
            {
                return this.sqlite.Query<Errors>(sql, new DynamicParameters()).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of GetErrors method

        public List<SqliteSequence> GetSqliteSequnces(string sql)
        {
            try
            {
                return this.sqlite.Query<SqliteSequence>(sql, new DynamicParameters()).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of GetSqliteSequnces method
    }
}
