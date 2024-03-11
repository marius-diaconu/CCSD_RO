using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class SqliteSequence : Database
    {
        private string table = "sqlite_sequence";
        public string name { get; set; }
        public long seq { get; set; }

        public bool Exists()
        {
            return Convert.ToBoolean(this.seq);
        } // end of Exists method

        public SqliteSequence First(string name)
        {
            try
            {
                var results = this.GetSqliteSequnces("SELECT * FROM `" + this.table + "` WHERE `name` = '" + name + "' LIMIT 1");
                return this.Instantiate(results);
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of First method

        public bool Update()
        {
            try
            {
                this.ExecuteQuery("UPDATE `" + this.table + "` SET `seq` = " + this.seq + " WHERE `name` = '" + this.name + "'");
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of Update method

        private SqliteSequence Instantiate(List<SqliteSequence> results)
        {
            if (results.Count > 0)
            {
                this.name = results[0].name;
                this.seq = results[0].seq;
            }

            return this;
        } // end of Instantiate method
    }
}
