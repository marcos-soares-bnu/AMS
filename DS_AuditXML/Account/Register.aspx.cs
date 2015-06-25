using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;
using System.Management;

namespace DS_AuditXML.Account
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterUser.ContinueDestinationPageUrl = Request.QueryString["ReturnUrl"];
        }


        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
            FormsAuthentication.SetAuthCookie(RegisterUser.UserName, false /* createPersistentCookie */);

            string continueUrl = RegisterUser.ContinueDestinationPageUrl;
            if (String.IsNullOrEmpty(continueUrl))
            {
                continueUrl = "~/";
            }

            //
            string pathSrv = ConfigurationManager.AppSettings["XMLData"];
            string remoteDir = RegisterUser.UserName.Replace("\\","_");
            string msg;

            //=== MPS 09/OUT - Refatorado código ==================================================
            //    Cria Instancia Server
            //----------------------------------------------------
            MPSfwk.Model.Server s = new MPSfwk.Model.Server();
            s.IPHOST = txtServer.Text;
            s.USUARIO = txtUser.Text;
            s.SENHA = txtPass.Text;

            if (chkRemoto.Checked != true)
            {
                s.USUARIO = "";
            }

            //Chama a rotina para criar a pasta...
            msg = MPSfwk.WMI.CriaPastaSite(pathSrv, remoteDir, s);
            //----------------------------------------------------
            //    Fim
            //=== MPS 09/OUT - Refatorado código ==================================================

            if (msg != "0")
            {
                lblStatus1.Visible = true;
                lblStatus2.Text = "";
                chkRemoto.Checked = false;
                txtServer.Visible = false;
                txtUser.Visible = false;
                txtPass.Visible = false;
                lblStatus1.Text = msg;
            }
            else
            { Response.Redirect(continueUrl); }
        }


        private void highlightCell(TableRow tr)
        {
            tr.Attributes.Add("class", "border");
            tr.Cells[0].Attributes.Add("class", "left");
            tr.Cells[tr1.Cells.Count - 1].Attributes.Add("class", "right");
        }


        protected void chkRemoto_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRemoto.Checked == true)
            {
                //highlightCell(tr2);
                //highlightCell(tr3);
                txtServer.Visible = true;
                txtUser.Visible = true;
                txtPass.Visible = true;
                lblStatus1.Text = "(Host/IP):";
                lblStatus2.Text = "Usuário/Senha:";
            }
            else
            {
                txtServer.Visible = false;
                txtUser.Visible = false;
                txtPass.Visible = false;
                lblStatus1.Text = "";
                lblStatus2.Text = "";
            }
        }

    }

}
