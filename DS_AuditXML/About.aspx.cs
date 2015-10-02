using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//
using Excel = Microsoft.Office.Interop.Excel;
using HtmlAgilityPack;
using Microsoft.XmlDiffPatch;
//
using System.Configuration;
using System.Data;
using System.IO;
using System.Management;
using System.Text;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using System.Web.UI.DataVisualization.Charting;

namespace DS_AuditXML
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            DataTable dtaux = MPSfwk.SharePoint.retDtListaSDC("MPS - Archive Status",
                                                            "",
                                                            "DC\\F0FP186",
                                                            "Mpsoa0715");

            GridView1.DataSource = dtaux;
            GridView1.DataBind();

            //
            string[] x = new string[dtaux.Rows.Count];
            int[] y = new int[dtaux.Rows.Count];
            for (int i = 0; i < dtaux.Rows.Count; i++)
            {
                x[i] = dtaux.Rows[i][0].ToString();
                y[i] = Convert.ToInt32(dtaux.Rows[i][7]);
            }
            Chart1.Series[0].Points.DataBindXY(x, y);
            Chart1.Series[0].ChartType = SeriesChartType.Pie;
            Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            Chart1.Legends[0].Enabled = true;
            //
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //
            string file_Add = "";
            StringBuilder sb = new StringBuilder();
            string result;
            int iLin = 0;
            //
            string usr = "DC\\F0FP186";
            string pwd = "Mpsoa0715";

            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.Cells[1].Text.Contains("AD-DS-001"))
                {
                    //---------------------------------------------------------------------------------------------
                    // Escreve o batch por linha...
                    //---------------------------------------------------------------------------------------------
                    //sb.Append("<Method ID='" + iLin.ToString() + "' Cmd='New'>");
                    sb.Append("<Method ID='" + row.Cells[14].Text + "' Cmd='Update'>");
                    //
                    if (TextBox1.Text != "")
                    {
                        sb.Append("<Field Name='ID'>" + row.Cells[14].Text + "</Field>");
                        sb.Append("<Field Name='Title'>" + TextBox1.Text + "</Field>"); 
                    }
                    //if (str_Semana != "") { sb.Append("<Field Name='Semana'>" + str_Semana + "</Field>"); }
                    //if (str_Classe != "") { sb.Append("<Field Name='Classe'>" + str_Classe + "</Field>"); }
                    //if (str_Servidor != "") { sb.Append("<Field Name='Servidor'>" + str_Servidor + "</Field>"); }
                    //if (str_Inicial != "") { sb.Append("<Field Name='Inicial'>" + lnk_Inicial + ", " + str_Inicial + "</Field>"); }
                    //if (str_Final != "") { sb.Append("<Field Name='Final'>" + lnk_Final + ", " + str_Final + "</Field>"); }
                    //if (str_Diferencas != "") { sb.Append("<Field Name='Diferencas'>" + lnk_Diferencas + ", " + str_Diferencas + "</Field>"); }
                    //if (str_Motivo != "") { sb.Append("<Field Name='Motivo'>" + str_Motivo + "</Field>"); }
                    //
                    sb.Append("</Method>");
                    //---------------------------------------------------------------------------------------------

                    result = MPSfwk.SharePoint.updListaSDC("MPS - Archive Status", sb.ToString(), usr, pwd);
                    if ((file_Add != "") && (result != ""))
                    { result = MPSfwk.SharePoint.UploadAttachment(usr, pwd, file_Add, "MPS - Archive Status", result); }
                }
                //
                iLin++;
            }
        }

    }
}
