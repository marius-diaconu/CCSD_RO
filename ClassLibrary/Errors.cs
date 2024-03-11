using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Errors : Database
    {
        // class properties
        protected string _table = "errors";
        private string _error_dir = "errors";

        // table properties
        public int id { get; set; }
        public string filepath { get; set; }
        public string filename { get; set; }
        public string created_at { get; set; }

        public Errors()
        {
            if (!Directory.Exists(this.error_dir))
            {
                try
                {
                    Directory.CreateDirectory(this.error_dir);
                }
                catch (UnauthorizedAccessException)
                {
                    Utility.ChangeDirectorySecurity(Utility.app_dir);
                    Directory.CreateDirectory(this.error_dir);
                }
            }
        } // end of Logs constructor

        public string error_dir
        {
            get { return Path.Combine(Utility.app_dir, this._error_dir); }
        }

        public string table
        {
            get { return this._table; }
        }

        public Errors First(string order = "asc")
        {
            var result = this.GetErrors("SELECT * FROM `" + this.table + "` ORDER BY `id` " + order.ToUpper() + " LIMIT 1");
            return this.Instantiate(result);
        } // end of First method

        public Errors FindByDate(DateTime date)
        {
            var result = this.GetErrors("SELECT * FROM `" + this.table + "` WHERE DATE(`created_at`) = '" +
                date.ToString("yyyy-MM-dd") + "' LIMIT 1");
            return this.Instantiate(result);
        } // end of FindByDate method

        public Errors FindById(int id)
        {
            var result = this.GetErrors("SELECT * FROM `" + this.table + "` WHERE `id` = " + id);
            return this.Instantiate(result);
        } // end of FindById method

        public List<Errors> Get(string order = "asc")
        {
            return this.GetErrors("SELECT * FROM `" + this.table + "` ORDER BY `id` " + order.ToUpper());
        } // end of Get method

        public bool Exists()
        {
            return Convert.ToBoolean(this.id);
        } // end of Exists method

        public bool Save()
        {
            return Convert.ToBoolean(this.id) ? this.Update() : this.Create();
        } // end of Save method

        public bool Delete()
        {
            try
            {
                this.ExecuteQuery("DELETE FROM `" + this.table + "` WHERE `id` = " + this.id);
                return true;
            }
            catch (Exception e)
            {
                Utility.InsertError(e.Message);
                return false;
            }
        } // end of Delete method

        private bool Create()
        {
            this.created_at = Utility.GetTimestamp();
            this.TrimValues();

            var sql = "INSERT INTO `" + this.table + "` (" +
                "`filepath`, " +
                "`filename`, " +
                "`created_at`) " +
                "VALUES (" +
                "'" + this.filepath + "', " +
                "'" + this.filename + "', " +
                "'" + this.created_at + "')";

            try
            {
                this.ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        } // end of Create method

        private bool Update()
        {
            this.TrimValues();

            var sql = "UPDATE `" + this.table + "` SET " +
                "`filepath` = '" + this.filepath + "', " +
                "`filename` = '" + this.filename + "', " +
                "`created_at` = '" + this.created_at + "' WHERE `id` = " + this.id;

            try
            {
                this.ExecuteQuery(sql);
                return true;
            }
            catch (Exception e)
            {
                Utility.InsertError(e.Message);
                return false;
            }
        } // end of Update method

        private Errors Instantiate(List<Errors> result)
        {
            if (result.Count > 0)
            {
                this.id = result[0].id;
                this.filepath = result[0].filepath;
                this.filename = result[0].filename;
                this.created_at = result[0].created_at;
            }

            return this;
        } // end of Instantiate method

        private void TrimValues()
        {
            this.filepath = this.TrimValue(this.filepath);
            this.filename = this.TrimValue(this.filename);
            this.created_at = this.TrimValue(this.created_at);
        } // end of TrimValues method
    }
}
