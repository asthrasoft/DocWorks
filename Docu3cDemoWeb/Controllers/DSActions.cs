using System;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;

namespace Docu3cDemoWeb
{
    public class DSActions
    {
        private readonly IWebHostEnvironment _env;
        string dsfileName = "";
        public DSActions(IWebHostEnvironment env)
        {
            _env = env;
            dsfileName = _env.WebRootPath + "/data/docu3cds.ds";
        }

        public DataSet GetDataSet()
        {
            var ds = InitDatSet();
            return ds;
        }

        public DataSet SavePortfolio(string pName, string cName)
        {
            var ds = InitDatSet();
            DataTable dt = ds.Tables["portfolio"];

            DataRow dr = dt.NewRow();
            dr["pID"] = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            dr["pName"] = pName;
            dr["cName"] = cName;
            dr["docCount"] = 0;
            dr["Classify"] = 0;
            dr["Comply"] = 0;
            dr["Comprehend"] = 0;
            dt.Rows.Add(dr);

            ds.WriteXml(dsfileName);
            return ds;
        }

        public DataSet SaveFiles(string pID,IFormFileCollection files)
        {
            var ds = InitDatSet();
            var dt = ds.Tables["file"];
            long id = ((long)(DateTime.Now - new DateTime(2020, 1, 1)).TotalSeconds);
            foreach (var file in files)
            {
                id++;
                string dir = _env.WebRootPath + "/data/" + pID;
                string fil = _env.WebRootPath + "/data/" + pID + "/" + file.FileName;

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (!File.Exists(fil))
                {
                    using (var stream = System.IO.File.Create(fil))
                    {
                        file.CopyTo(stream);
                    }
                    DataRow dr = dt.NewRow();
                    dr["fID"] = id.ToString();
                    dr["pID"] = pID;
                    dr["fName"] = file.FileName;
                    dr["fType"] = "UNKNOWN";
                    dt.Rows.Add(dr);
                }
                else
                {
                    using (var stream = System.IO.File.Create(fil))
                    {
                         file.CopyTo(stream);
                    }
                }
            }
            ds.WriteXml(dsfileName);
            return ds;
        }

        public Dictionary<string,string> GetFileInfo(string fID)
        {
            Dictionary<string, string> fileinfo = new Dictionary<string, string>();
            var ds = InitDatSet();
            if (ds.Tables.Contains("file"))
            {
                DataRow[] drs = ds.Tables["file"].Select("fID = '" + fID + "'");
                fileinfo.Add("path", "/data/" + drs[0]["pID"].ToString() + "/" + drs[0]["fname"].ToString());
                fileinfo.Add("type", drs[0]["fType"].ToString());
            }

            return fileinfo;
        }

        public Dictionary<string, string> ClassifyFile(string fID, string ftype)
        {
            Dictionary<string, string> fileinfo = new Dictionary<string, string>();
            var ds = InitDatSet();
            if (ds.Tables.Contains("file"))
            {
                ds.Tables["file"].AcceptChanges();
                foreach (DataRow dr in ds.Tables["file"].Rows)
                {
                    if(dr["fID"].ToString() == fID)
                    {
                        dr["fType"] = ftype;
                        fileinfo.Add("path", "/data/" + dr["pID"].ToString() + "/" + dr["fname"].ToString());
                        fileinfo.Add("type", dr["fType"].ToString());
                    }
                }
                ds.Tables["file"].AcceptChanges();
                ds.WriteXml(dsfileName);
            }
            return fileinfo;
        }

        public DataSet DeleteFile(string pID,string fID)
        {
            var ds = InitDatSet();
            if (ds.Tables.Contains("file"))
            {
                string _filename = "";
                ds.Tables["file"].AcceptChanges();
                foreach (DataRow dr in ds.Tables["file"].Rows)
                {
                    if (dr["fID"].ToString() == fID)
                    {
                        _filename = _env.WebRootPath + "/data/" + pID + "/" + dr["fName"].ToString();
                        dr.Delete();
                    }
                }
                ds.Tables["file"].AcceptChanges();
                ds.WriteXml(dsfileName);

                if (File.Exists(_filename))
                    File.Delete(_filename);

            }


            return ds;
        }

        private DataSet InitDatSet()
        {
            DataSet ds = new DataSet("docu3c");

            string dir = _env.WebRootPath + "/data/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (System.IO.File.Exists(dsfileName))
                ds.ReadXml(dsfileName);

            DataTable dt = new DataTable();
            if (!ds.Tables.Contains("portfolio"))
            {
                dt = new DataTable("portfolio");
                DataColumn column;

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "pID";
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "pName";
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "cName";
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Int32");
                column.ColumnName = "docCount";
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Classify";
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Comply";
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Comprehend";
                dt.Columns.Add(column);

                ds.Tables.Add(dt);
            }

            if (!ds.Tables.Contains("file"))
            {
                dt = new DataTable("file");
                DataColumn column;

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "fID";
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "pID";
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "fName";
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "fType";
                dt.Columns.Add(column);

                ds.Tables.Add(dt);
            }

            return ds;
        }

        private string GenID()
        {
            //https://stackoverflow.com/questions/11313205/generate-a-unique-id
            //https://codereview.stackexchange.com/questions/233452/generate-unique-id-in-c
            //StringBuilder builder = new StringBuilder();
            //Enumerable
            //   .Range(65, 26)
            //    .Select(e => ((char)e).ToString())
            //    .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
            //    .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
            //    .OrderBy(e => Guid.NewGuid())
            //    .Take(6)
            //    .ToList().ForEach(e => builder.Append(e));
            //string id = builder.ToString();

            string id = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            return id;
        }
    }
}
