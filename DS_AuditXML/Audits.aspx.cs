using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;

namespace DS_AuditXML
{
    public partial class Audits : System.Web.UI.Page
    {
        public MembershipUser               currentUser;
        public static MPSfwk.Model.Audits   aud;
        public static string                XMLServers  = ConfigurationManager.AppSettings["XMLData"] + "ListHosts.xml";
        public static string                XMLClasses  = ConfigurationManager.AppSettings["XMLData"] + "ListClasses.xml";
        public static string[]              ordBY       = { "DESC", "ASC" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Chama a rotina para gerar a lista de Servidores/Classes e destacar os ativos...
                //true = faz o Bind...
                Label1.Text = Label1.Text + "  (" + DS_AuditXML.Util_list.lstBox_BindRefresh(true, true, XMLServers, lstHost) + ") Ativos";
                Label2.Text = Label2.Text + "  (" + DS_AuditXML.Util_list.lstBox_BindRefresh(false, true, XMLClasses, lstTipoClasse) + ") Ativos";

                // Atualiza a lista de datas de geracao...
                LimpaSetDatas();
                panAviso.Visible = true;

                //MPS 27/10/2014
                //Mostra a ultima Semana referente aos Ativos, o que não não foi gerado...
                showTreeFalhas();
            }
            else
            {
                //Chama a rotina para gerar a lista de Servidores/Classes e destacar os ativos...
                //false = não faz o Bind...
                DS_AuditXML.Util_list.lstBox_BindRefresh(true, false, XMLServers, lstHost);
                DS_AuditXML.Util_list.lstBox_BindRefresh(false, false, XMLClasses, lstTipoClasse);
            }

        } //OK MPS - 27/10/2014


        protected void showTreeFalhas()
        {
            //Variaveis...
            List<MPSfwk.Model.Audits> _lstNOK;
            aud = new MPSfwk.Model.Audits();

            //Inicia o TreeView...
            TreeView1.Nodes.Clear();
            TreeView1.Visible   = true;
            TreeNode raiz       = new TreeNode("_______________________________Conteudo________________________________");
            raiz.SelectAction   = TreeNodeSelectAction.Expand;
            TreeView1.Nodes.Add(raiz);
            //Seta os descendentes...
            TreeNode logini = null;
            TreeNode servers = null;
            TreeNode resNOK = null;

            //Seta Cabecalho do TreeView...
            DateTime aux_dtIni = DateTime.Now;
            dtSel3.Visible = true;
            dtSel3.Text = String.Format("LOG das Gerações Auditadas para os Ativos (Ultima Semana): {0} e {1}", aux_dtIni.ToString("dd/MM/yyyy"), aux_dtIni.AddDays(-7).ToString("dd/MM/yyyy"));

            //Seta os nos das Geracoes... Loop 7 ultimos dias...
            for (int i = 1; i <= 8; i++)
            {
                logini = new TreeNode("Geração: (" + aux_dtIni.ToString("dd/MM/yyyy") + ")");
                logini.SelectAction = TreeNodeSelectAction.Expand;
                raiz.ChildNodes.Add(logini);

                //------------------------------------------------
                //Chama a rotina para ver as falhas...
                aud.IDGeracao = aux_dtIni.ToString("yyyyMMdd");
                _lstNOK = SqlServer.AuditXML.lstAudits(aud, 4, ordBY[0]);

                //
                resNOK = new TreeNode("FALHA: (" + _lstNOK.Count + ")");
                resNOK.SelectAction = TreeNodeSelectAction.Expand;
                resNOK.Text = "<div style='color:#FF3300;font-size:Small;font-weight:bold;'>" + resNOK.Text + "</div>";
                resNOK.ToolTip = "Por gentileza, verifique os LOGs dos Agendamentos para identificar a falha!";
                logini.ChildNodes.Add(resNOK);
                //
                foreach (MPSfwk.Model.Audits ax in _lstNOK)
                {
                    servers = new TreeNode("Servidor: (" + ax.IDServer.Trim() + ")");
                    servers.SelectAction = TreeNodeSelectAction.Expand;
                    resNOK.ChildNodes.Add(servers);
                }
                resNOK.CollapseAll();
                //
                //------------------------------------------------

                //Chama a rotina para ver as OK...
                showTreeOK(logini, aux_dtIni);

                aux_dtIni = DateTime.Now.AddDays(-i);
            }

        } //OK MPS - 29/10/2014


        protected void showTreeOK(TreeNode logini, DateTime aux_dtIni)
        {
            //Variaveis...
            List<MPSfwk.Model.Audits> _lstOK;
            aud = new MPSfwk.Model.Audits();

            //Seta os descendentes...
            TreeNode resOK = null;
            TreeNode classes = null;
            TreeNode servers = null;

            //------------------------------------------------
            //Chama a rotina para ver as OK...
            aud.IDGeracao = aux_dtIni.ToString("yyyyMMdd");
            _lstOK = SqlServer.AuditXML.lstAudits(aud, 3, ordBY[0]);

            //
            resOK = new TreeNode("OK: (" + _lstOK.Count + ")");
            resOK.SelectAction = TreeNodeSelectAction.Expand;
            resOK.Text = "<div style='color:#006600;font-size:Small;font-weight:bold;'>" + resOK.Text + "</div>";
            resOK.ToolTip = "Abaixo estão as Classes e Servidores Auditados com sucesso!";
            logini.ChildNodes.Add(resOK);
            //

            string aux_cls = "";
            int contOK = 0;
            foreach (MPSfwk.Model.Audits ax in _lstOK)
            {
                if (aux_cls != ax.IDClasse)
                {
                    if (aux_cls != "")
                    { classes.Text = classes.Text + " (" + contOK + ")"; }
                    classes = new TreeNode("Classe: (" + ax.IDClasse.Trim() + ")");
                    classes.SelectAction = TreeNodeSelectAction.Expand;
                    resOK.ChildNodes.Add(classes);

                    servers = new TreeNode("Servidor: (" + ax.IDServer.Trim() + ")");
                    servers.SelectAction = TreeNodeSelectAction.Expand;
                    classes.ChildNodes.Add(servers);
                    contOK = 1;
                }
                else
                {
                    servers = new TreeNode("Servidor: (" + ax.IDServer.Trim() + ")");
                    servers.SelectAction = TreeNodeSelectAction.Expand;
                    classes.ChildNodes.Add(servers);
                    contOK++;
                }

                aux_cls = ax.IDClasse;
            }
            if (aux_cls != "")
            { classes.Text = classes.Text + " (" + contOK + ")"; }
            if (servers != null)
                servers.CollapseAll();
            if (classes != null)
                classes.CollapseAll();
            resOK.CollapseAll();
            //
            //------------------------------------------------

        } //OK MPS - 29/10/2014


        protected void LimpaSetDatas()
        {
            //Pega o usuário logado / cria instancia Audits...
            currentUser = Membership.GetUser();
            aud = new MPSfwk.Model.Audits();

            //Seta as definicoes iniciais...
            lblDatas.Text = "0";
            dtSel1.Text = "-";
            dtSel1.Visible = false;
            dtSel2.Text = "-";
            dtSel2.Visible = false;
            Calendar1.SelectedDates.Clear();
            drpGeracoes.Items.Clear();
            btnCompara.Enabled = false;
            btnCompara.ForeColor = System.Drawing.Color.Silver;
            divMessage.InnerHtml = "";
            btnGeraClasse.Enabled = true;
            btnGeraClasse.ForeColor = System.Drawing.Color.White;
            GridView1.DataSource = null;
            GridView1.DataBind();
            GridView2.DataSource = null;
            GridView2.DataBind();
            btnCompara.Text = "Comparar";

            // Atualiza a lista de datas de geracao...
            DS_AuditXML.Util_list.ListaDTsGeracao(ordBY, aud, lstHost, lstTipoClasse, drpGeracoes, drpGeracoes);

        } //OK MPS - 03/10/2014

        protected void lstHost_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Atualiza a lista de datas de geracao...
            LimpaSetDatas();

        } //OK MPS - 01/10/2014

        protected void lstTipoClasse_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Atualiza a lista de datas de geracao...
            LimpaSetDatas();

        } //OK MPS - 01/10/2014

        protected void btnGeraClasse_Click(object sender, EventArgs e)
        {
            //Pega o usuário logado...
            currentUser = Membership.GetUser();
            DataTable dt1;

            //Testa a conexão com o Servidor Selecionado...
            //Se não existir informa o usuario...
            if (lstHost.SelectedItem == null)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Não foi selecionado o Servidor, por gentileza, selecione e tente novamente! </span>";
                return;
            }
            if (lstTipoClasse.SelectedItem == null)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Não foi selecionado a Classe, por gentileza, selecione e tente novamente! </span>";
                return;
            }
            //MPS 27/10/2014
            bool connOK = false;
            if (lstHost.SelectedItem.ToString().IndexOf(" [") > 0)
                connOK = MPSfwk.WMI.testeConnWMI(lstHost.SelectedItem.ToString().Substring(0, lstHost.SelectedItem.ToString().IndexOf(" [")), currentUser.UserName, currentUser.GetPassword());
            else
                connOK = MPSfwk.WMI.testeConnWMI(lstHost.SelectedItem.ToString(), currentUser.UserName, currentUser.GetPassword());
            
            if (!connOK)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>Não foi possivel se conectar ao Servidor selecionado! Talvez o IP/Host selecionado não foi encontrado, ou usuário sem privilégio.  </span>";
                return;
            }

            string classe = lstTipoClasse.SelectedItem.ToString();
            string classe_SEL = lstTipoClasse.SelectedValue.ToString();
            //MPS 27/10/2014
            string server= "";
            if (lstHost.SelectedItem.ToString().IndexOf(" [") > 0)
                server = lstHost.SelectedItem.ToString().Substring(0, lstHost.SelectedItem.ToString().IndexOf(" ["));
            else
                server = lstHost.SelectedItem.ToString();

            try
            {
                //Chama a função para o Gerenciamento WMI remoto/local
                // se ocorre erro, tenta a conexão local...
                if (classe_SEL == "Win32_GroupUser")
                {
                    dt1 = MPSfwk.WMI.GroupMembers(  server,
                                                    currentUser.UserName,
                                                    currentUser.GetPassword());
                }
                else
                {
                    dt1 = MPSfwk.WMI.dtlistaClasse( classe,
                                                    classe_SEL,
                                                    server,
                                                    currentUser.UserName,
                                                    currentUser.GetPassword());
                }

                GridView2.DataSource = dt1;
                GridView2.DataBind();

                string dthr = DateTime.Now.ToString("yyyyMMddHHmmss");
                string arq = ConfigurationManager.AppSettings["XMLData"] + currentUser.UserName.Replace("\\", "_") + "\\" + classe + "_" + server + "_" + dthr + ".xml";
                dt1.WriteXml(arq);

                //Grava o XML no banco de Dados...
                XmlDocument xmlSave = new XmlDocument();
                xmlSave.Load(arq);
                SqlServer.AuditXML.Gravar(classe, server, dthr, xmlSave);

                //Grava e disponibiliza o XML...
                dtSel2.Visible = true;
                dtSel2.Text = dthr;
                String strUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace("Audits.aspx", "");
                string str_lnk = currentUser.UserName.Replace("\\", "_") + "\\" + classe + "_" + server + "_" + dthr + ".xml";
                string str_href = strUrl + "Xml_Data\\" + str_lnk;
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" +
                                        "Geração efetuada com sucesso em: <a href=" + str_href.Replace(" ", "%20") + ">" + str_lnk +
                                        "</a>   </span>";

            }
            catch (UnauthorizedAccessException was)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Ocorreu um erro - Acesso não autorizado: " + was.Message + "  </span>";
            }
            catch (ManagementException ex)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Ocorreu um erro de gerenciamento: " + ex.Message + "  </span>";
            }
            catch (Exception eg)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Ocorreu um erro: " + eg.Message + "  </span>";
            }

            //Limpa e atualiza a ultima Data Gerada...
            drpGeracoes.Items.Clear();

            // Atualiza a lista de datas de geracao...
            DS_AuditXML.Util_list.ListaDTsGeracao(ordBY, aud, lstHost, lstTipoClasse, drpGeracoes, drpGeracoes);

        } //OK MPS - 03/10/2014

        protected void chkCompara_CheckedChanged(object sender, EventArgs e)
        {
            // Atualiza a lista de datas de geracao...
            LimpaSetDatas();

        } //OK MPS - 01/10/2014

        protected string getUltimaGeracao(string diaSel)
        {
            var selected = from i in drpGeracoes.Items.Cast<ListItem>()
                           where ((ListItem)i).Text.Contains(diaSel)
                           select i;
            if (selected.Count() > 0)
            { return selected.ToList()[selected.Count() - 1].Text; }
            else
            { return ""; }

        } //OK MPS - 02/10/2014

        protected void btnCompara_Click(object sender, EventArgs e)
        {
            if (btnCompara.Text.IndexOf("(Mostrar Tudo)") > 0)
            {
                //Chama a rotina para mostra as diff...
                DS_AuditXML.Util_list.setGridFull(GridView1);
                DS_AuditXML.Util_list.setGridFull(GridView2);
                btnCompara.Text = "Comparar (Mostrar Diferença)";
            }
            else if (btnCompara.Text.IndexOf("(Mostrar Diferença)") > 0)
            {
                //Chama a rotina para mostra tudo...
                DS_AuditXML.Util_list.setGridDiff(GridView1, System.Drawing.Color.LightGray);
                DS_AuditXML.Util_list.setGridDiff(GridView2, System.Drawing.Color.LightGray);
                btnCompara.Text = "Comparar (Mostrar Tudo)";
            }
            else
            {
                //Chama a rotina de comparação que destaca as linhas diferentes...
                DS_AuditXML.Util_list.compare(GridView1, GridView2, System.Drawing.Color.LightGreen, System.Drawing.Color.LightGray);
                btnCompara.Text = "Comparar (Mostrar Tudo)";
            }

        } //OK MPS - 02/10/2014

        protected void setDatasCalVerGrids(bool soh1, GridView _grd1, GridView _grd2)
        {
            try
            {
                DateTime d = DateTime.ParseExact(dtSel1.Text, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                aud.DTGeracaoIni = d.ToString("yyyyMMdd") + "000000";
                if (!soh1) { d = DateTime.ParseExact(dtSel2.Text, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); }
                aud.DTGeracaoFim = d.ToString("yyyyMMdd") + "235900";

                // Atualiza a lista de datas de geracao...
                string[] ordena = { "ASC", "DESC" };
                DS_AuditXML.Util_list.ListaDTsGeracao(ordena, aud, lstHost, lstTipoClasse, drpGeracoes, drpGeracoes);

                //Pega a ultima geração de cada data selecionada...
                string ultSel1 = "";
                string ultSel2 = "";
                if (dtSel1.Text.Length >= 10)
                { ultSel1 = getUltimaGeracao(dtSel1.Text.Substring(0, 10)); }
                if (dtSel2.Text.Length >= 10)
                { ultSel2 = getUltimaGeracao(dtSel2.Text.Substring(0, 10)); }
                if (ultSel1 == "")
                { divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Não foi encontrado nenhuma Geração para a Data: " + dtSel1.Text + "  </span><br />"; }
                else if ((ultSel2 == "") && (!soh1) )
                { divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Não foi encontrado nenhuma Geração para a Data: " + dtSel2.Text + "  </span><br />"; }
                else
                {
                    //Pega o usuário logado...
                    currentUser = Membership.GetUser();

                    //Limpa as Datas e copia as 2 ultimas encontradas p /comparacao...
                    drpGeracoes.Items.Clear();
                    if (!soh1)
                    { 
                        drpGeracoes.Items.Add(ultSel2);
                        btnCompara.Enabled = true;
                        btnCompara.ForeColor = System.Drawing.Color.White;
                    }
                    drpGeracoes.Items.Add(ultSel1);

                    //Define os campos Selecionados para o preenchimento dos Grids...
                    //Se não existir informa o usuario...
                    if (lstHost.SelectedItem == null)
                    {
                        divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Não foi selecionado o Servidor, por gentileza, selecione e tente novamente! </span>";
                        return;
                    }
                    if (lstTipoClasse.SelectedItem == null)
                    {
                        divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Não foi selecionado a Classe, por gentileza, selecione e tente novamente! </span>";
                        return;
                    }
                    //MPS 27/10/2014
                    aud.IDClasse = lstTipoClasse.SelectedItem.ToString();
                    if (lstHost.SelectedItem.ToString().IndexOf(" [") > 0)
                        aud.IDServer = lstHost.SelectedItem.ToString().Substring(0, lstHost.SelectedItem.ToString().IndexOf(" ["));
                    else
                        aud.IDServer = lstHost.SelectedItem.ToString();
                    d = DateTime.ParseExact(ultSel1, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    dtSel1.Text = d.ToString();
                    aud.IDGeracao = d.ToString("yyyyMMddHHmmss");
                    DS_AuditXML.Util_list.DbXMLBindGrid(currentUser, aud, GridView1);
                    //
                    if (!soh1)
                    {
                        d = DateTime.ParseExact(ultSel2, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        dtSel2.Text = d.ToString();
                        aud.IDGeracao = d.ToString("yyyyMMddHHmmss");
                        DS_AuditXML.Util_list.DbXMLBindGrid(currentUser, aud, GridView2);
                    }
                }
                if ((!chkCompara.Checked) || (Convert.ToInt16(lblDatas.Text) >= 2))
                {
                    //Zera o contador...
                    lblDatas.Text = "0";
                }

            }
            catch (Exception eg)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Ocorreu um erro: " + eg.Message + "  </span>";
            }
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt16(lblDatas.Text) <= 1)
            {
                DateTime dt = Calendar1.SelectedDates[0];
                if (Convert.ToInt16(lblDatas.Text) == 0)
                {
                    dtSel1.Text = dt.ToString("dd/MM/yyyy HH:mm:ss");
                    dtSel1.Visible = true;
                }
                else
                {
                    Calendar1.SelectedDates.Add(Convert.ToDateTime(dtSel1.Text));
                    dtSel2.Text = dt.ToString("dd/MM/yyyy HH:mm:ss");
                    dtSel2.Visible = true;
                }
                lblDatas.Text = (Convert.ToInt16(lblDatas.Text) + 1).ToString();
            }
            //Verifica se esta checado a Comparacao...
            if (    (Calendar1.SelectedDates.Count == 2) &&
                    (chkCompara.Checked))
                    { setDatasCalVerGrids(false, GridView1, GridView2); }
            else    { setDatasCalVerGrids(true, GridView1, GridView2); }

        } //OK MPS - 02/10/2014

    }
}