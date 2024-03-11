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
        public bool is_open;
        private string _db_name;
        private string _db_path;
        protected SQLiteConnection sqlite;

        public Database()
        {
            string db_dir = Path.Combine(Utility.root_dir,  "database");
            this.db_name = "sqlite.db";
            this.db_path = Path.Combine(db_dir, this.db_name);

            if (!Directory.Exists(@db_dir))
            {
                Directory.CreateDirectory(@db_dir);
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
            set { _db_name = value; }
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
            if (!HasTable("tasks"))
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
                    "`task_next_run` TEXT NULL DEFAULT NULL, " +
                    "`task_end_date` TEXT NULL DEFAULT NULL, " +
                    "`task_type` TEXT NOT NULL, " +
                    "`task_recursive` INTEGER UNSIGNED DEFAULT 0, " +
                    "`task_running` INTEGER UNSIGNED DEFAULT 0, " +
                    "`task_stopped` INTEGER UNSIGNED DEFAULT 0, " +
                    "`created_at` TEXT NOT NULL, " +
                    "`updated_at` TEXT NULL DEFAULT NULL)";
                
                ExecuteQuery(sql);
            }
        } // end of CheckIfTablesExists method

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

        public bool HasColumn(string table_name, string column_name)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM pragma_table_info('" + table_name + "') " +
                    "WHERE name= '" + column_name + "'", this.sqlite);
                string response = (string)command.ExecuteScalar();
                return !string.IsNullOrEmpty(response);
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

        public int Count(string table_name)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand("SELECT COUNT(`ID`) FROM `" + table_name + "`", this.sqlite);
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of Count method

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

        public List<Task> GetTasks(string sql)
        {
            try
            {
                return this.sqlite.Query<Task>(sql, new DynamicParameters()).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of GetTasks method
    }
}
