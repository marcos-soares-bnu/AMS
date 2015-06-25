using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace DS_AuditXML
{
    public partial class Shares : System.Web.UI.Page
    {
        public string selHost = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                XDocument xmldoc = XDocument.Load(HttpContext.Current.Server.MapPath("~/Xml_Data/ListHosts.xml"));
                var items = (from i in xmldoc.Descendants("item")
                                select new { Item = i.Element("SEL").Value, Value = i.Element("VALUE").Value }).ToList();

                drpServidores.DataSource = items;
                drpServidores.DataTextField = "Value";
                drpServidores.DataValueField = "Item";
                drpServidores.DataBind();
            }
        }

        protected void btnVisualiza_Click(object sender, EventArgs e)
        {
            string xml = "DS_shares_" + drpServidores.SelectedItem + "_" + drpGeracoes.Text + ".xml";
            string scrText = "";
            scrText = scrText + "var Mleft = (screen.width/2)-(800/2);var Mtop = (screen.height/2)-(600/2);window.open( 'Util_list.aspx?rptTipo=Shares&xmlFile1=" + xml + "', null, 'height=600,width=800,status=yes,toolbar=no,scrollbars=yes,menubar=no,location=no,top=\'+Mtop+\', left=\'+Mleft+\'' );";
            // open a pop up window at the center of the page.
            ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", scrText, true);
        }

        protected void drpServidores_DataBound(object sender, EventArgs e)
        {
            updListas();
        }

        protected void updListas()
        {
            string[] lstDir;
            string txtAux;
            lstDir = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Xml_Data/"));
            string formatString = "yyyyMMddHHmmss";

            foreach (var item in lstDir)
            {
                if (item.IndexOf("DS_shares_") > 0)
                {
                    txtAux = item.Substring(item.IndexOf("DS_shares_"));

                    if (txtAux.IndexOf(drpServidores.SelectedItem.ToString()) > 0)
                    {
                        txtAux = item.Substring(item.IndexOf(drpServidores.SelectedItem.ToString()));
                        txtAux = txtAux.Substring(txtAux.IndexOf("_"));
                        txtAux = txtAux.Substring(1, txtAux.IndexOf(".xml") - 1);

                        DateTime dt = DateTime.ParseExact(txtAux, formatString, null);

                        drpGeracoes.Items.Add(txtAux);
                        Calendar1.SelectedDates.Add(dt);
                    }
                }
            }
            if (drpGeracoes.Text != "")
            { 
                btnVisualiza.Enabled = true;
                btnVisualiza.ForeColor = System.Drawing.Color.White;
            }
            else 
            { 
                btnVisualiza.Enabled = false;
                btnVisualiza.ForeColor = System.Drawing.Color.Silver;
            }
        }

        protected void drpServidores_TextChanged(object sender, EventArgs e)
        {
            btnCompara.Enabled = false;
            btnCompara.ForeColor = System.Drawing.Color.Silver;
            lblDatas.Text = "0";
            dtSel1.Text = "-";
            dtSel2.Text = "-";
            Calendar1.SelectedDates.Clear();
            selHost = drpServidores.SelectedItem.ToString();
            drpGeracoes.Items.Clear(); 
            updListas();
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt16(lblDatas.Text) <= 1)
            {
                DateTime dt = Calendar1.SelectedDates[0];
                if (Convert.ToInt16(lblDatas.Text) == 0)
                {
                    chkCompara.Text = "Selecionou a(s) data(s): ";
                    dtSel1.Text = dt.ToShortDateString();
                }
                else
                {
                    Calendar1.SelectedDates.Add(Convert.ToDateTime(dtSel1.Text));
                    dtSel2.Text = dt.ToShortDateString();
                    btnCompara.Enabled = true;
                    btnCompara.ForeColor = System.Drawing.Color.White;
                }

                lblDatas.Text = (Convert.ToInt16(lblDatas.Text) + 1).ToString();
            }
        }

        protected void chkCompara_CheckedChanged(object sender, EventArgs e)
        {
            lblDatas.Text = "0";
            dtSel1.Text = "-";
            dtSel2.Text = "-";
            Calendar1.SelectedDates.Clear();
            btnCompara.Enabled = false;
            btnCompara.ForeColor = System.Drawing.Color.Silver;
        }

        protected void btnCompara_Click(object sender, EventArgs e)
        {
            string xml1 = "DS_shares_10.134.99.38_20140618181800.xml";
            string xml2 = "DS_shares_10.134.99.38_20140616161600.xml";

            string scrText = "";
            scrText = scrText + "var Mleft = (screen.width/2)-(800/2);var Mtop = (screen.height/2)-(600/2);window.open( 'Util_list.aspx?rptTipo=SharesC&xmlFile1=" + xml1 + "&xmlFile2=" + xml2 + "', null, 'height=600,width=800,status=yes,toolbar=no,scrollbars=yes,menubar=no,location=no,top=\'+Mtop+\', left=\'+Mleft+\'' );";
            // open a pop up window at the center of the page.
            ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", scrText, true);
        }


    }
}