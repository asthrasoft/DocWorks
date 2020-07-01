using System;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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

        public DataSet ClearData()
        {
            string dir = _env.WebRootPath + "/data/";
            if (Directory.Exists(dir))
                Directory.Delete(dir,true);

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
            dt.Rows.Add(dr);

            ds.WriteXml(dsfileName);
            return ds;
        }

        public DataSet DelPortfolio(string pID)
        {
            var ds = InitDatSet();
            DataTable dt = ds.Tables["portfolio"];

            ds.Tables["portfolio"].AcceptChanges();
            foreach (DataRow dr in ds.Tables["portfolio"].Rows)
            {
                if (dr["pID"].ToString() == pID)
                {
                    dr.Delete();
                }
            }
            ds.Tables["portfolio"].AcceptChanges();
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

        public void SaveFileProperties(string pID, string fID, docu3clist docs)
        {
            string binfile = _env.WebRootPath + "/data/" + pID + "/" + fID + ".bin";
            IFormatter formatter = new BinaryFormatter();
            if (File.Exists(binfile)) File.Delete(binfile);

            using (Stream stream = new FileStream(binfile, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, docs);
            }
        }

        public docu3clist GetFileProperties(string pID, string fID)
        {
            string binfile = _env.WebRootPath + "/data/" + pID + "/" + fID + ".bin";
            docu3clist docs = new docu3clist();
            if (!File.Exists(binfile)) return docs;

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(binfile, FileMode.Open, FileAccess.Read))
            {
                docs = (docu3clist)formatter.Deserialize(stream);
            }
            docs.html = docu3cAPIClient.SetDocHTML(docs);

            return docs;
        }

        public Dictionary<string, string> SaveFileClassification(string fID, string ftype)
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

        public string CheckFileProperties(string pID, string fID)
        {
            //for the given portfolio, get ALL OTHER file properties. 
            string _html = "";
            docu3clist selfile = GetFileProperties(pID, fID);

            var ds = InitDatSet();
            if (ds.Tables.Contains("file"))
            {
                docu3clist currfile;
                foreach (DataRow dr in ds.Tables["file"].Rows)
                {
                    if (dr["fID"].ToString() != fID && dr["fType"].ToString() != "UNKNOWN")
                    {
                        _html += "<h6 class='text-warning'>Comparing with : (" + dr["fType"]  + ")" + dr["fName"]  + "</h6>";
                        currfile = GetFileProperties(pID, dr["fID"].ToString());
                        var ret_html = CompareProperties(selfile, currfile);
                        if(ret_html == "ERROR")
                            _html += "<h6 class='text-warning'>Could not compare documents</h6>";
                        else
                            _html += ret_html;
                    }
                }
            }

            return _html;
        }

        private string CompareProperties(docu3clist sel, docu3clist curr)
        {
            string _html = "ERROR";
            if (sel.Count == 0|| curr.Count == 0) return _html;
            _html = "";
            var seldoc = sel[0]; var currdoc = curr[0];
            foreach (var key in seldoc.docProps.Keys)
            {
                if (key == "doc.type" || key == "org.name") continue;
                if (!currdoc.docProps.ContainsKey(key)) continue;
                if (!seldoc.docProps[key].Value.Replace(" ","").Equals(currdoc.docProps[key].Value.Replace(" ","")))
                {
                    _html += "<h6 class='text-warning'>Mismatch for " + key + ": <span class='text-success'>" + seldoc.docProps[key].Value.ToString() + " | vs | " + currdoc.docProps[key].Value.ToString() + "</span></h6>";
                }
            }

            if (_html == "") _html += "<h6 class='text-success'>Documents Match!!!</h6>";
            return _html;
        }

        public DataSet DeleteFile(string pID,string fID)
        {
            var ds = InitDatSet();
            if (ds.Tables.Contains("file"))
            {
                string _filename = ""; string _filepropname = "";
                ds.Tables["file"].AcceptChanges();
                foreach (DataRow dr in ds.Tables["file"].Rows)
                {
                    if (dr["fID"].ToString() == fID)
                    {
                        _filename = _env.WebRootPath + "/data/" + pID + "/" + dr["fName"].ToString();
                        _filepropname = _env.WebRootPath + "/data/" + pID + "/" + fID + ".bin";
                        dr.Delete();
                    }
                }
                ds.Tables["file"].AcceptChanges();
                ds.WriteXml(dsfileName);

                if (File.Exists(_filename))
                    File.Delete(_filename);

                if (File.Exists(_filepropname))
                    File.Delete(_filepropname);
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

    }
}
