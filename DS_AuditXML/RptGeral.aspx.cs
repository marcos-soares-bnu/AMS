using Excel = Microsoft.Office.Interop.Excel;
using HtmlAgilityPack;
using Microsoft.XmlDiffPatch;
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;

namespace DS_AuditXML
{
    public partial class RptGeral : System.Web.UI.Page
    {
        public MembershipUser               currentUser;
        public static MPSfwk.Model.Audits   aud;
        public static string                XMLServers  = ConfigurationManager.AppSettings["XMLData"] + "ListHosts.xml";
        public static string                XMLClasses  = ConfigurationManager.AppSettings["XMLData"] + "ListClasses.xml";
        public static string[]              ordBY       = { "DESC", "ASC" };
        public XmlDiff                      diff        = new XmlDiff();//Classe p/ Comp. xmls...
        public Random                       r           = null;
        public string                       diffFile    = null;
        public string                       HTMFile     = null;
        public string                       startupPath = "";
        public static byte[]                bytes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Chama a rotina para gerar a lista de Servidores/Classes e destacar os ativos...
                //true = faz o Bind...
                chkSrv.Text = "(" + DS_AuditXML.Util_list.lstBox_BindRefresh(true, true, ConfigurationManager.AppSettings["XMLData"] + "ListHosts.xml", lstHost) + ") Ativos";
                chkCls.Text = "(" + DS_AuditXML.Util_list.lstBox_BindRefresh(false, true, ConfigurationManager.AppSettings["XMLData"] + "ListClasses.xml", lstTipoClasse) + ") Ativos";
                
                //Chama a rotina para varrer a pasta de relatórios pendentes de geração...
                lstRPTs();

                //Cria e carrega a lista de Dts disponiveis...
                aud = new MPSfwk.Model.Audits();

                // Atualiza a lista de datas de geracao...
                LimpaSetDatas();
            }
            else 
            {
                //Chama a rotina para gerar a lista de Servidores/Classes e destacar os ativos...
                //false = não faz o Bind...
                chkSrv.Text = "(" + DS_AuditXML.Util_list.lstBox_BindRefresh(true, false, ConfigurationManager.AppSettings["XMLData"] + "ListHosts.xml", lstHost) + ") Ativos";
                chkCls.Text = "(" + DS_AuditXML.Util_list.lstBox_BindRefresh(false, false, ConfigurationManager.AppSettings["XMLData"] + "ListClasses.xml", lstTipoClasse) + ") Ativos";
            }

        } //OK MPS - 03/10/2014

        protected void LimpaSetDatas()
        {
            //Pega o usuário logado / cria instancia Audits...
            currentUser = Membership.GetUser("DC\\f0fp186");
            aud = new MPSfwk.Model.Audits();

            //Seta as definicoes iniciais...
            lblDatas.Text = "0";
            lbl_txtDtIni.Text = "-";
            lbl_txtDtIni.Visible = false;
            lbl_txtDtFim.Text = "-";
            lbl_txtDtFim.Visible = false;
            Calendar1.SelectedDates.Clear();
            chkCompara.Text = "Intervalo [Inicio-Fim]: ";

            panAviso.Visible = true;
            dtSel1.Visible = false;
            dtSel2.Visible = false;
            divMessage.InnerHtml = "";

        } //OK MPS - 03/10/2014

        protected void lstRPTs()
        {
            try
            {
                string[] lstRPTs;
                string[] lin;
                string rptPath = ConfigurationManager.AppSettings["XMLData"] + "RPTs";
                string txtAux;
                var table = new DataTable("HTMs");
                //
                table.Columns.Add("Intervalo");
                table.Columns.Add("Classe");
                table.Columns.Add("Servidor");
                table.Columns.Add("HTM_file (Arquivo de Comparação)");
                //
                lstRPTs = Directory.GetFiles(rptPath, "*.htm");

                foreach (var item in lstRPTs)
                {
                    txtAux = item.Substring(rptPath.Length + 1);
                    lin = txtAux.Split('_');
                    if (lin.Length == 4)
                    {
                        lin[3] = item;
                        table.Rows.Add(lin);
                    }
                }

                if (table.Rows.Count != 0)
                { 
                    //Mostrar os arquivos de Comparação HTM gerados...
                    //escreve o RPT para XML temp..
                    table.WriteXml(ConfigurationManager.AppSettings["XMLData"] + "RPTs\\RptGeral_tmp.xml");
                    grdRptGeral.DataSource = table;
                    grdRptGeral.DataBind();

                    divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>Filtro efetuado com sucesso! Clique em Gerar Relatório para visualizar as informações... </span>"; 
                }
                else
                { 
                    //Verifica se existe algum arquivo gerado para mostrar...
                    if (File.Exists(ConfigurationManager.AppSettings["XMLData"] + "RPTs\\RptGeral_tmp.xml"))
                    {
                        DS_AuditXML.Util_list.bindGrid(ConfigurationManager.AppSettings["XMLData"] + "RPTs\\RptGeral_tmp.xml", grdRptGeral);
                    }
                }
            }
            catch (Exception ex)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>Ocorreu um erro - " + ex.Message + "</span>";
            }

        } //OK MPS - 03/10/2014

        protected void lstHost_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Atualiza a lista de datas de geracao...
            LimpaSetDatas();

        } //OK MPS - 03/10/2014

        protected void lstTipoClasse_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Atualiza a lista de datas de geracao...
            LimpaSetDatas();

        } //OK MPS - 03/10/2014

        protected void chkCls_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCls.Checked)
            { 
                DS_AuditXML.Util_list.setListSelONOFF(  true, 
                                                        lstTipoClasse, 
                                                        DS_AuditXML.Util_list.setLista(false, ConfigurationManager.AppSettings["XMLData"] + "ListClasses.xml")); 
            }
            else
            { 
                DS_AuditXML.Util_list.setListSelONOFF(  false, 
                                                        lstTipoClasse, 
                                                        DS_AuditXML.Util_list.setLista(false, ConfigurationManager.AppSettings["XMLData"] + "ListClasses.xml"));
            }
            // Atualiza a lista de datas de geracao...
            LimpaSetDatas();

        } //OK MPS - 03/10/2014

        protected void chkSrv_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSrv.Checked)
            { 
                DS_AuditXML.Util_list.setListSelONOFF(  true, 
                                                        lstHost, 
                                                        DS_AuditXML.Util_list.setLista(true, ConfigurationManager.AppSettings["XMLData"] + "ListHosts.xml")); 
            }
            else 
            { 
                DS_AuditXML.Util_list.setListSelONOFF(  false, 
                                                        lstHost, 
                                                        DS_AuditXML.Util_list.setLista(true, ConfigurationManager.AppSettings["XMLData"] + "ListHosts.xml")); 
            }
            // Atualiza a lista de datas de geracao...
            LimpaSetDatas();

        } //OK MPS - 03/10/2014


//MPS 11-18/NOV - correção **************************************************************************************************

        private string[] LeHtm(string dat, string cls, string srv, string htmfile)
        {
            string[] lin = { dat, cls, srv, "", "", "", "", htmfile };
            string[] aux = { dat, cls, srv, "", "", "", "" };
            DateTime dti = DateTime.ParseExact(dat.Substring(1, 11), "dd-MMM-yyyy", null);
            DateTime dtf = DateTime.ParseExact(dat.Substring(13, 11), "dd-MMM-yyyy", null);
            // Le o Arquivo HTM com as diferencas apontadas... 
            //
            var doc = new HtmlDocument();
            doc.Load(htmfile);
            var nodes = doc.DocumentNode.SelectNodes("//table/tr");
            //
            // Cria 3 vetores com as diferencas, sendo
            // ("BACKGROUND-COLOR: LIGHTGREEN")  > 0 = Alterado
            // ("BACKGROUND-COLOR: RED")         > 0 = Deletado
            // ("BACKGROUND-COLOR: YELLOW")      > 0 = Adicionado
            //
            var rowsAlt = nodes.Skip(2).Select ( tr => tr
                                                .Elements   ("td")
                                                .Where      (   td => td.InnerHtml.ToString().ToUpper().IndexOf("BACKGROUND-COLOR: LIGHTGREEN") > 0)
                                                .Select     (   td => td.InnerText.Trim() )
                                                .ToArray()  );
            //
            var rowsDel = nodes.Skip(2).Select ( tr => tr
                                                .Elements   ("td")
                                                .Where      (td => td.InnerHtml.ToString().ToUpper().IndexOf("BACKGROUND-COLOR: RED") > 0)
                                                .Select     (td => td.InnerText.Trim() )
                                                .ToArray()  );
            //
            var rowsAdd = nodes.Skip(2).Select ( tr => tr
                                                .Elements   ("td")
                                                .Where      (td => td.InnerHtml.ToString().ToUpper().IndexOf("BACKGROUND-COLOR: YELLOW") > 0)
                                                .Select     (td => td.InnerText.Trim() )
                                                .ToArray()  );
            //

            //Loop Alt
            int iAlt = 0;
            foreach (var row in rowsAlt)
            {
                if (row.Length >= 1)
                {
                    if (cls.IndexOf("Usuarios dos Grupos Locais - SYS") >= 0)
                    {
                        string[] nodegr = nodes[iAlt - 1].InnerHtml.Split('>');
                        string grupo = nodegr[2].Replace("</font", "");
                        aux = trataAlt("", row[0], row[1], grupo, srv, dti, dtf);
                    }
                    else
                    {
                        string[] nodetg = nodes[iAlt + 1].InnerHtml.Split('>');
                        string tag = nodetg[2].Replace("</font", "").Replace("&lt;", "");
                        aux = trataAlt(tag, row[0], row[1], "", "", DateTime.Now, DateTime.Now); 
                    }
                    //
                    lin[3] = lin[3] + Convert.ToChar(13) + aux[3];
                    lin[4] = lin[4] + Convert.ToChar(13) + aux[4];
                    lin[5] = lin[5] + Convert.ToChar(13) + aux[5];
                    lin[6] = lin[6] + Convert.ToChar(13) + aux[6];
                }
                iAlt++;
            }

            //Loop Del
            string strDel = "";
            foreach (var row in rowsDel)
            { if (row.Length >= 1) { strDel = strDel + "|" + row[0]; } }
            //
            if (strDel.Length > 0)
            {
                aux     = trataDel(strDel.Split('|'));
                lin[3]  = lin[3] + Convert.ToChar(13) + aux[3];
                lin[4]  = lin[4] + Convert.ToChar(13) + aux[4];
                lin[5]  = lin[5] + Convert.ToChar(13) + aux[5];
                lin[6]  = lin[6] + Convert.ToChar(13) + aux[6];
            }
            //

            //Loop Add
            string strAdd = "";
            foreach (var row in rowsAdd)
            { if (row.Length >= 1) { strAdd = strAdd + "|" + row[0]; } }
            //
            if (strAdd.Length > 0)
            {
                aux = trataAdd(strAdd.Split('|'));
                lin[3] = lin[3] + Convert.ToChar(13) + aux[3];
                lin[4] = lin[4] + Convert.ToChar(13) + aux[4];
                lin[5] = lin[5] + Convert.ToChar(13) + aux[5];
                lin[6] = lin[6] + Convert.ToChar(13) + aux[6];
            }
            //
            //
            return lin;
        }
        //
        protected string[] trataAlt(string tag, string ini_lin, string fim_lin, string grupo, string srv, DateTime dtini, DateTime dtfim)
        {
            string[] aux        = { "0", "1", "2", "", "", "", "[Verificar com responsável!]" };
            IEnumerable<string> vet_aux;
            string[] vet_auxi   = ini_lin.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] vet_auxf   = fim_lin.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string str_aux      = "";
            string str_mot      = "";

            // Se tiver tag <> "", mostra a tag e ambas as diferencas
            if (tag != "")
            {
                aux[3] = "[" + tag + "] / " + ini_lin;
                aux[4] = "[" + tag + "] / " + fim_lin;
                aux[5] = "[Alterado Valores]";
                aux[6] = "- ";
            }
            else
            {
                // Trata os casos de Grupos x Usuarios + busca SharePoint
                if (vet_auxi.Count() > vet_auxf.Count())    
                {
                    vet_aux = vet_auxi.Except(vet_auxf, StringComparer.OrdinalIgnoreCase);
                    foreach (string s in vet_aux) { str_aux = str_aux + s + "; "; }
                    str_mot = "[Deletado] / ";
                    aux[3] = "[" + grupo + "] / " + str_aux;
                    aux[4] = "-";
                }
                else                                    
                {
                    vet_aux = vet_auxf.Except(vet_auxi, StringComparer.OrdinalIgnoreCase);
                    if (vet_aux.Count() > 0)
                    {
                        foreach (string s in vet_aux) { str_aux = str_aux + s + "; "; }
                        str_mot = "[Adicionado] / ";
                        aux[3] = "-";
                        aux[4] = "[" + grupo + "] / " + str_aux;
                    }
                }

                // Faz o Loop nos usuarios e busca no SharePoint se ha registros...
                str_aux = "";
                foreach (string s in vet_aux)   { str_aux = str_aux + checkSPUser(srv, s.Trim(), dtini, dtfim) + "; "; }
                //
                if (str_aux != "")              { aux[5] = str_mot + str_aux; }
                else                            
                { 
                    aux[5] = "[Alterado Formatos]";
                    aux[6] = "- ";
                }
            }
            //
            return aux;
        }
        //
        protected string[] trataDel(string[] del_lin)
        {
            string[] aux = { "0", "1", "2", "", "", "", "[Verificar com responsável!]" };
            //
            string str_aux = "";

            //Testa se eh apenas uma linha ou um bloco de tags...
            //
            if (del_lin.Count() == 2)
            {   str_aux = str_aux + " [" + del_lin[1].Trim() + "]"; }
            else
            {
                for (int i = 0; i < del_lin.Count(); i++)
                {
                    if  (   (   (del_lin[i].ToUpper().IndexOf("&GT;")       >   0)  &&
                                (del_lin[i].ToUpper().IndexOf("&LT;/")      == -1)
                            )                                                       &&
                            (   (del_lin[i].ToUpper().IndexOf("CAPTION")    >   0)  ||
                                (del_lin[i].ToUpper().IndexOf("NAME")       >   0)
                            )
                        )
                        str_aux = str_aux + " [" + del_lin[i].Replace("&lt;", "").Replace("&gt;", "").Trim() + "] / [" + del_lin[i + 1].Trim() + "]";
                }
            }
            //
            aux[3] = str_aux;
            aux[4] = "-";
            aux[5] = "[Deletado]";

            return aux;
        }
        //
        protected string[] trataAdd(string[] add_lin)
        {
            string[] aux = { "0", "1", "2", "", "", "", "[Verificar com responsável!]" };
            //
            string str_aux = "";
            //Testa se eh apenas uma linha ou um bloco de tags...
            //
            if (add_lin.Count() == 2)
            { str_aux = str_aux + " [" + add_lin[1].Trim() + "]"; }
            else
            {
                for (int i = 0; i < add_lin.Count(); i++)
                {
                    if  (   (   (add_lin[i].ToUpper().IndexOf("&GT;")       >   0)  &&
                                (add_lin[i].ToUpper().IndexOf("&LT;/")      == -1)
                            )                                                       &&
                            (   (add_lin[i].ToUpper().IndexOf("CAPTION")    >   0)  ||
                                (add_lin[i].ToUpper().IndexOf("NAME")       >   0)
                            )
                        )
                        str_aux = str_aux + " [" + add_lin[i].Replace("&lt;", "").Replace("&gt;", "").Trim() + "] / [" + add_lin[i + 1].Trim() + "]";
                }
            }
            //
            aux[3] = "-";
            aux[4] = str_aux;
            aux[5] = "[Adicionado]";

            return aux;
        }

        // Rotina para a leitura no SharePoint...
        //
        protected string checkSPUser(string srv, string usr, DateTime dtIni, DateTime dtFim)
        {
            try
            {
                DataTable dt = MPSfwk.SharePoint.retDtListaSDC("Deploy - Desvio de Acesso",
                                                                "<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + usr + "</Value></Eq></Where>",
                                                                currentUser.UserName,
                                                                currentUser.GetPassword());
                //
                if (dt != null)
                {
                    foreach (DataRow DRow in dt.Rows)
                    {
                        //Seta as datas/srv de intervalo...
                        DateTime _dti;
                        DateTime _dtf;
                        string _srv;
                        string _usr;
                        string ax = "";
                        //
                        //*********************************************************** Rev. 21/NOV
                        _usr = getSPFieldValue(DRow, "ows_Title");
                        _srv = getSPFieldValue(DRow, "ows_Servidor");
                        //
                                        ax      = getSPFieldValue(DRow, "ows_StartDate");
                        if (ax != "")   _dti    = Convert.ToDateTime(ax);
                        else            _dti    = DateTime.Now;
                        //
                                        ax      = getSPFieldValue(DRow, "ows_Fim_x0020_do_x0020_Acesso_x0020_");
                        if (ax != "")   _dtf    = Convert.ToDateTime(ax);
                        else            _dtf    = DateTime.Now;
                        //
                        //***********************************************************
                        if (((_usr.Trim().ToUpper().IndexOf(usr.Trim().ToUpper()) >= 0) &&
                                    (_srv.Trim().ToUpper().IndexOf(srv.Trim().ToUpper()) >= 0)
                                ) &&
                                ((((dtIni >= _dti) && (dtIni <= _dtf)) ||
                                        ((dtFim >= _dti) && (dtFim <= _dtf))
                                    )
                                )
                            )
                            return DRow[3].ToString();
                    }
                    //
                    return "[Não Identificado SDC]";
                }
                else { return "[Não Identificado SDC]"; }
            }
            catch
            { return "[Não Identificado SDC]"; }
        }


        protected string getSPFieldValue(DataRow DRow, string Field)
        {
            string str_aux = "";
            //
            for (int i = 0; i < DRow.Table.Columns.Count; i++)
            {
                if (DRow.Table.Columns[i].ColumnName == Field)
                {
                    if (DRow[i] == System.DBNull.Value)
                    {
                        if (DRow.Table.Columns[i].DataType.Name.Equals("Datetime"))
                        {
                            return DateTime.Now.ToString();
                        }
                    }
                    else { return DRow[i].ToString(); }
                }
            }

            return str_aux;
        }

//MPS 11-21/NOV - correção **************************************************************************************************


        protected void btnGerar_Click(object sender, EventArgs e)
        {
            try
            {
                //Pega o usuário f0fp186 para SharePoint / cria instancia Audits...
                currentUser = Membership.GetUser("DC\\f0fp186");
                string auxHTM = "";
                var table = new DataTable("RptGeral_HTMs");
                //
                table.Columns.Add("Intervalo");
                table.Columns.Add("Classe");
                table.Columns.Add("Servidor");
                table.Columns.Add("Diferenca Inicial");
                table.Columns.Add("Diferenca Final");
                table.Columns.Add("Motivo");
                table.Columns.Add("Acao");
                table.Columns.Add("HTMFile");

                for (int i = 0; i < grdRptGeral.Rows.Count; i++)
                {
                    //Chama a Rotina para mostrar apenas as diff de cada HTM...
                    //Testa para ver se foi gerado o htm, caso contrario desconsidera...
                    if (grdRptGeral.Rows[i].Cells[3].Text.IndexOf(".htm") > 0)
                    {
                        table.Rows.Add(LeHtm(grdRptGeral.Rows[i].Cells[0].Text, grdRptGeral.Rows[i].Cells[1].Text, grdRptGeral.Rows[i].Cells[2].Text, grdRptGeral.Rows[i].Cells[3].Text));
                        //
                        //Move o HTM para a pasta de gerados...
                        auxHTM = grdRptGeral.Rows[i].Cells[3].Text;
                        File.Move(auxHTM, auxHTM.Replace("RPTs\\", "RPTs\\gerados\\"));
                    }
                }
                //
                // Mostra o Relatório Final...
                // escreve o RPT para XML temp..
                table.WriteXml(ConfigurationManager.AppSettings["XMLData"] + "RPTs\\RptGeral_tmp.xml");
                //
                grdRptGeral.DataSource = table;
                grdRptGeral.DataBind();
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>Relatório gerado com sucesso! </span>";
            }
            catch (Exception ex)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>Ocorreu um erro - " + ex.Message + "</span>";
            }

        } //OK MPS - 04/11/2014


        protected void btnExportarXLS_Click(object sender, EventArgs e)
        {
            string scrText = "";
            scrText = scrText + "var Mleft = (screen.width/2)-(800/2);var Mtop = (screen.height/2)-(600/2);window.open( 'Util_list.aspx?flgComp=R&rptTipo=&xmlFile1=RptGeral_tmp.xml&xmlFile2=', null, 'height=600,width=800,status=yes,toolbar=no,scrollbars=yes,menubar=no,location=no,top=\'+Mtop+\', left=\'+Mleft+\'' );";
            // open a pop up window at the center of the page.
            ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", scrText, true);

        } //OK MPS - 04/11/2014


        protected void exeFiltrarRPT(bool ehSemanal)
        {
            int LenCal;
            DateTime dtCali;
            DateTime dtCalf;
            //
            //Seta as variaveis de Calendário caso não for Semanal...
            //
            if (ehSemanal)
            {
                LenCal = 2;
                dtCali = DateTime.Now.Subtract(TimeSpan.FromDays(7));
                lbl_txtDtIni.Text = dtCali.ToString("dd-MMM-yyyy");
                dtCalf = DateTime.Now;
                lbl_txtDtFim.Text = dtCalf.ToString("dd-MMM-yyyy");
                //
                chkCompara.Text = "Intervalo [" + lbl_txtDtIni.Text + "-" + lbl_txtDtFim.Text + "]";
            }
            else
            {
                LenCal = 2;
                dtCali = Calendar1.SelectedDates[0];
                dtCalf = Calendar1.SelectedDates[1];
            }
            //
            string param;
            string[] vCls;
            string[] vSrv;
            string[] lin = new string[4];
            //
            panAviso.Height = 600;
            panAviso.Visible = true;
            //
            var table = new DataTable("HTMs");
            //
            table.Columns.Add("Intervalo");
            table.Columns.Add("Classe");
            table.Columns.Add("Servidor");
            table.Columns.Add("HTM_file (Arquivo de Comparação)");
            
            //Se for Semanal, seleciona todos os ativos antes...
            //
            if (ehSemanal)
            { 
                //Seleciona todas as classes ativas...
                DS_AuditXML.Util_list.setListSelONOFF(  true,
                                                        lstTipoClasse,
                                                        DS_AuditXML.Util_list.setLista(false, ConfigurationManager.AppSettings["XMLData"] + "ListClasses.xml"));
                //Seleciona todos os Servidores ativos...
                DS_AuditXML.Util_list.setListSelONOFF(true,
                                                        lstHost,
                                                        DS_AuditXML.Util_list.setLista(true, ConfigurationManager.AppSettings["XMLData"] + "ListHosts.xml")); 
            }

            //
            param = String.Join("|", lstTipoClasse.Items
                        .Cast<ListItem>()
                        .Where(li => li.Selected)
                        .Select(li => li.Text)
                        .ToArray());
            vCls = param.Split('|');

            param = String.Join("|", lstHost.Items
                        .Cast<ListItem>()
                        .Where(li => li.Selected)
                        .Select(li => li.Text)
                        .ToArray());
            vSrv = param.Split('|');

            divMessage.InnerHtml = "";
            //Declara as variaveis a serem utilizadas...
            XmlDocument xmlDB_source = null;
            XmlDocument xmlDB_target = null;
            string _server = "";
            string _classe = "";
            string strdts = "";
            try
            {
                if (LenCal == 2)
                {
                    foreach (string _srv in vSrv)
                    {
                        foreach (string _cls in vCls)
                        {
                            //
                            HTMFile = "[Nenhuma diferença encontrada!]";
                            //
                            if (_srv.IndexOf(" [") > 0)
                                _server = _srv.Substring(0, _srv.IndexOf(" ["));
                            else
                                _server = _srv;

                            _classe = _cls;
                            // Busca na base de dados o XML com base nos filtros...
                            xmlDB_source = SqlServer.AuditXML.LerXML(_classe, _server, dtCali.ToString("yyyyMMdd"));
                            xmlDB_target = SqlServer.AuditXML.LerXML(_classe, _server, dtCalf.ToString("yyyyMMdd"));
                            //
                            //Testa se os params retornaram nulos...
                            if ((xmlDB_source.InnerXml == "") || (xmlDB_target.InnerXml == ""))
                            {
                                string msg = "Não foi possivel localizar o registro:" + "   " +
                                                _classe + " | " + _server + " | " +
                                                dtCali.ToString("yyyyMMdd") + " | " +
                                                dtCalf.ToString("yyyyMMdd");

                                divErro.InnerHtml = divErro.InnerHtml + "<br /><span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>Ocorreu um erro nos Filtros - " + msg + "</span>";
                            }
                            else
                            {
                                //
                                //Pega o intervalo...
                                strdts = "[" + lbl_txtDtIni.Text + "-" + lbl_txtDtFim.Text + "]";
                                //
                                bool ehEq = false;
                                ehEq = XmlDiffHtm(xmlDB_source.InnerXml, xmlDB_target.InnerXml, strdts, _server, _classe);
                                //
                                if (!ehEq)
                                {
                                    divMessage.InnerHtml = divMessage.InnerHtml + "<br />====================================================================================================";
                                    divMessage.InnerHtml = divMessage.InnerHtml + "<br />" + File.ReadAllText(HTMFile);
                                }
                                //
                                lin[0] = strdts;
                                lin[1] = _classe;
                                lin[2] = _server;
                                lin[3] = HTMFile;
                                //
                                table.Rows.Add(lin);
                            }
                        }
                    }
                    //Mostrar os arquivos de Comparação HTM gerados...
                    grdRptGeral.DataSource = table;
                    grdRptGeral.DataBind();

                    //Deleta os arquivos temporários...
                    foreach (FileInfo f in new DirectoryInfo(ConfigurationManager.AppSettings["XMLData"] + "RPTs").GetFiles("*.out"))
                    { f.Delete(); }
                }

            }
            catch (Exception eg)
            {
                string msg = "Parametros definidos:" + "\r" +
                                dtCali.ToString("yyyyMMdd") + "\r" +
                                dtCalf.ToString("yyyyMMdd") + "\r" +
                                _server + "\r" +
                                _classe + "\r";

                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>Ocorreu um erro nos Filtros - " + msg + eg.Message + "</span>";
            }

        } //OK MPS - 04/11/2014


        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            exeFiltrarRPT(false);

        } //OK MPS - 04/11/2014


        protected void chkSemanal_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSemanal.Checked)
            { exeFiltrarRPT(true); }

        } //OK MPS - 04/11/2014


        public bool XmlDiffHtm(string source, string target, string intervalo, string server, string classe)
        {
            // Randomiza para criar arquivos unicos de comparação
            r = new Random();
            bool isEqual = false;

            //Pega o usuário logado...
            MembershipUser currentUser = Membership.GetUser();

            //output diff file.
            startupPath = ConfigurationManager.AppSettings["XMLData"] + "\\RPTs";
            diffFile = startupPath + Path.DirectorySeparatorChar + "vxd" + r.Next() + ".out";
            //
            XmlTextWriter tw = new XmlTextWriter(new StreamWriter(diffFile));
            tw.Formatting = Formatting.Indented;

            try
            {
                XmlReader expected = XmlReader.Create(new StringReader(target));
                XmlReader actual = XmlReader.Create(new StringReader(source));

                XmlDiff diff = new XmlDiff( XmlDiffOptions.IgnoreChildOrder |
                                            XmlDiffOptions.IgnoreComments   |
                                            XmlDiffOptions.IgnoreXmlDecl    |
                                            XmlDiffOptions.IgnoreWhitespace);

                isEqual = diff.Compare(actual, expected, tw);
                tw.Close();

                //-----------------------------------------------------------------
                // Cria a comparação dos XMLs...
                XmlDiffView dv = new XmlDiffView();

                // Carrega o arquivo orig = source + o arquivo XML das Diff...
                XmlTextReader orig = new XmlTextReader(new StringReader(source)); //source
                XmlTextReader diffGram = new XmlTextReader(diffFile);
                dv.Load(orig,
                    diffGram);

                orig.Close();
                diffGram.Close();

                // Chama a função para gravar e formatar o conteudo + diff em HTM...
                if (!isEqual)
                { GrHtm(dv, intervalo, server, classe); }
                //
                return isEqual;
            }
            catch (XmlException xe)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>Ocorreu um erro de Comparação - " + xe.StackTrace + "</span>";
                return isEqual;
            }

        } //OK MPS - 04/11/2014


        public void GrHtm(XmlDiffView dv, string Dts, string Srv, string Cls)
        {
            string msg;
            StringBuilder msgHtm = new StringBuilder();
            //
            HTMFile = startupPath + Path.DirectorySeparatorChar + Dts + "_" + Cls + "_" + Srv + "_diff" + r.Next() + ".htm";
            StreamWriter sw1 = new StreamWriter(HTMFile);

            // Escreve o HTML com as diferenças para analise do LeHtm...
            
            // Cabecalho
            msg = "<html><body><table width='100%'>";
            msgHtm.Append(msg);
            sw1.Write(msg);
            
            // Legenda
            msg =   "<tr><td colspan='2' align='center'><b>"    +   Cls     +   "_"     +   Srv         +
                    " - Legenda:</b> <font style='background-color: yellow'"                            +
                    " color='black'>adicionado</font>&nbsp;&nbsp;<font style='background-color: red'"   +
                    " color='black'>removido</font>&nbsp;&nbsp;<font style='background-color: "         +
                    "lightgreen' color='black'>alterado</font>&nbsp;&nbsp;"                             +
                    "<font style='background-color: red' color='blue'>movido Ini</font>"                +
                    "&nbsp;&nbsp;<font style='background-color: yellow' color='blue'>movido Fim"        +
                    "</font>&nbsp;&nbsp;<font style='background-color: white' color='#AAAAAA'>"         +
                    "ignorado</font></td></tr>";
            msgHtm.Append(msg);
            sw1.Write(msg);

            // Arquivos comparados...
            msg = "<tr><td><b> File Name : ";
            msgHtm.Append(msg);
            sw1.Write(msg);
            //
            msg = Dts.Substring(1,11);
            msgHtm.Append(msg);
            sw1.Write(msg);
            //
            msg = "</b></td><td><b> File Name : ";
            msgHtm.Append(msg);
            sw1.Write(msg);
            //
            msg = Dts.Substring(13,11);
            msgHtm.Append(msg);
            sw1.Write(msg);
            //
            msg = "</b></td></tr>";
            msgHtm.Append(msg);
            sw1.Write(msg);

            // This gets the differences but just has the 
            //rows and columns of an HTML table
            dv.GetHtml(sw1);

            // Finaliza o HTML
            msg = "</table></body></html>";
            msgHtm.Append(msg);
            sw1.Write(msg);

            // Fecha arquivos
            sw1.Close();
            dv = null;

        } //OK MPS - 04/11/2014


        protected void chkCompara_CheckedChanged(object sender, EventArgs e)
        {
            LimpaSetDatas();

        } //OK MPS 24/10/2014


        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            if (    (Convert.ToInt16(lblDatas.Text) <= 1) &&
                    (chkCompara.Checked)
               )
            {
                DateTime dt = Calendar1.SelectedDates[0];
                if (Convert.ToInt16(lblDatas.Text) == 0)
                {
                    lbl_txtDtIni.Text = dt.ToString("dd-MMM-yyyy");
                    //escreve o Periodo...
                    string axcomp = "Intervalo [" + lbl_txtDtIni.Text + "-Fim]";
                    chkCompara.Text = axcomp;
                }
                else
                {
                    Calendar1.SelectedDates.Add(Convert.ToDateTime(lbl_txtDtIni.Text));
                    lbl_txtDtFim.Text = dt.ToString("dd-MMM-yyyy");
                    //escreve o Periodo...
                    string axcomp = "Intervalo [" + lbl_txtDtIni.Text + "-" + lbl_txtDtFim.Text + "]";
                    chkCompara.Text = axcomp;
                }
                lblDatas.Text = (Convert.ToInt16(lblDatas.Text) + 1).ToString();
            }

        } //OK MPS 24/10/2014


        protected void btnExportarSP_Click(object sender, EventArgs e)
        {
            try
            {
                string          strLstSP    = "http://sts237wk8/sites/SDU/Lists/Deploy%20%20Auditorias%20DS_AuditXML/AllItems.aspx";
                String          strUrl      = ConfigurationManager.AppSettings["DS_Url"];
                StringBuilder   sbMsg       = new StringBuilder();
                DateTime        dtf         = DateTime.Now;
                DateTime        dti         = DateTime.Now;
                int             iLin        = 0;
                //
                foreach (GridViewRow row in grdRptGeral.Rows)
                {
                    dtf                     = DateTime.ParseExact(row.Cells[0].Text.Substring(13, 11), "dd-MMM-yyyy", null);
                    dti                     = DateTime.ParseExact(row.Cells[0].Text.Substring(1, 11), "dd-MMM-yyyy", null);
                    string      vet_audf    = row.Cells[1].Text + ";" + row.Cells[2].Text + ";" + dti.ToString("yyyyMMdd") + ";F;" + dtf.ToString("yyyyMMdd");

                    // Busca os arquivos das diferencas localmente...
                    string      maskFile    = row.Cells[7].Text.Substring(row.Cells[7].Text.IndexOf("["));
                    var         HTMfile     = Directory.GetFiles(ConfigurationManager.AppSettings["XMLData"] + "RPTs\\", maskFile, SearchOption.AllDirectories).FirstOrDefault();
                    //
                    newRegSDCAudits (   iLin.ToString(),
                                        "DS_AuditXML",
                                        calcNumSemana(dtf),
                                        row.Cells[1].Text,
                                        row.Cells[2].Text,
                                        "", "",
                                        "", "",
                                        "[" + dti.ToString("dd/MMM/yyyy") + "-" + dtf.ToString("dd/MMM/yyyy") + "]", strUrl + "Util_list.aspx?" + vet_audf,
                                        row.Cells[5].Text,
                                        HTMfile.ToString()
                                    );
                    //
                    iLin++;
                }
                sbMsg.Append("<br /><br />Salvou o Relatório no Portal SDC - " + newRegSDCAudits((iLin + 1).ToString(), "DS_AuditXML_rpt", calcNumSemana(dtf), "", "", "", "", "", "", "", "", "Backup do Relatório no Periodo.", (ConfigurationManager.AppSettings["XMLData"] + "RPTs\\RptGeral_tmp.xml")));
                sbMsg.Append("<br />Salvou Lista SDC com sucesso!. Acesse o link: <a href=" + strLstSP + "> Deploy - Auditorias DS_AuditXML</a> para conferir!");
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + sbMsg.ToString() + "</span>";
            }
            catch (Exception ex)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>Ocorreu um erro SharePoint - " + ex.StackTrace + "</span>";
            }

        } //OK MPS - 02/12/2014


        protected string calcNumSemana(DateTime dtf)
        {
            //Calculo do Num da Semana...
            var cultureInfo         = System.Globalization.CultureInfo.CurrentCulture;
            var calendar            = cultureInfo.Calendar;
            var calendarWeekRule    = cultureInfo.DateTimeFormat.CalendarWeekRule;
            var firstDayOfWeek      = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            var lastDayOfWeek       = cultureInfo.LCID == 1033 //En-us
                                    ? DayOfWeek.Saturday
                                    : DayOfWeek.Sunday;
            var lastDayOfYear       = new DateTime(dtf.Year, 12, 31);
            var weekNumber          = calendar.GetWeekOfYear(dtf, calendarWeekRule, firstDayOfWeek);

            return                  weekNumber.ToString(); 
            
        } //OK MPS - 03/12/2014


        protected string newRegSDCAudits(   string str_ID,
                                            string str_Title,
                                            string str_Semana,
                                            string str_Classe,
                                            string str_Servidor,
                                            string str_Inicial,     string lnk_Inicial,
                                            string str_Final,       string lnk_Final,
                                            string str_Diferencas,  string lnk_Diferencas,
                                            string str_Motivo,      string file_Add
                                        )
        {
            //Pega o usuário logado...
            MembershipUser currentUser = Membership.GetUser("DC\\f0fp186");
            StringBuilder sb = new StringBuilder();
            string result;

            //---------------------------------------------------------------------------------------------
            // Escreve o batch por linha...
            //---------------------------------------------------------------------------------------------
            sb.Append("<Method ID='" + str_ID + "' Cmd='New'>");
            //
            if (str_Title       != "")  { sb.Append("<Field Name='Title'>"      + str_Title         + "</Field>"); }
            if (str_Semana      != "")  { sb.Append("<Field Name='Semana'>"     + str_Semana        + "</Field>"); }
            if (str_Classe      != "")  { sb.Append("<Field Name='Classe'>"     + str_Classe        + "</Field>"); }
            if (str_Servidor    != "")  { sb.Append("<Field Name='Servidor'>"   + str_Servidor      + "</Field>"); }
            if (str_Inicial     != "")  { sb.Append("<Field Name='Inicial'>"    + lnk_Inicial       + ", " + str_Inicial + "</Field>"); }
            if (str_Final       != "")  { sb.Append("<Field Name='Final'>"      + lnk_Final         + ", " + str_Final + "</Field>"); }
            if (str_Diferencas  != "")  { sb.Append("<Field Name='Diferencas'>" + lnk_Diferencas    + ", " + str_Diferencas + "</Field>"); }
            if (str_Motivo      != "")  { sb.Append("<Field Name='Motivo'>"     + str_Motivo        + "</Field>"); }
            //
            sb.Append("</Method>");
            //---------------------------------------------------------------------------------------------
            result = MPSfwk.SharePoint.updListaSDC("Deploy - Auditorias DS_AuditXML", sb.ToString(), currentUser.UserName, currentUser.GetPassword());
            if ((file_Add       != "")  && (result != ""))
                { result = MPSfwk.SharePoint.UploadAttachment(currentUser.UserName, currentUser.GetPassword(), file_Add, "Deploy - Auditorias DS_AuditXML", result); }

            return result;

        } //OK MPS - 03/12/2014


        protected void grdRptGeral_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //Pega o usuário logado...
            MembershipUser currentUser = Membership.GetUser();

            try
            {
                if (e.CommandName == "Del")
                {
                    //Chama a função para o Executar a Tarefa...
                    divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Deletou - " + "MPS" + " com sucesso! </span>";
                }
            }
            catch (Exception eg)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Ocorreu um erro: " + eg.Message + "  </span>";
            }

        } //OK MPS - 04/11/2014---em desenv-------------------------------------------------


        protected void grdRptGeral_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((ImageButton)e.Row.Cells[0].Controls[1]).OnClientClick = "return confirm('Voce tem certeza que deseja Deletar este Item do Portal SDC?');";
            }

        } //OK MPS - 04/11/2014---em desenv-------------------------------------------------


        //
        //*** MPS *** Desenv (TESTE) - fim.....................................................
        //string[] arrSelected = qry.Where(x => x.Selected).Select(x => x.Texto).ToArray();
        //string[] arrNotSelected = qry.Where(x => !x.Selected).Select(x => x.Texto).ToArray();
        //-------------------------------------------------------------
        // This method reads the diff options set on the 
        // menu and configures the XmlDiffOptions object.
        //XmlDiffOptions diffOptions = new XmlDiffOptions();
        //bool compareFragments = false;
        //diffOptions = diffOptions | XmlDiffOptions.IgnoreChildOrder;
        //diffOptions = diffOptions | XmlDiffOptions.IgnoreComments;
        //diffOptions = diffOptions | XmlDiffOptions.IgnoreDtd;
        //diffOptions = diffOptions | XmlDiffOptions.IgnoreNamespaces;
        //diffOptions = diffOptions | XmlDiffOptions.IgnorePrefixes;
        //diffOptions = diffOptions | XmlDiffOptions.IgnoreWhitespace;
        //diffOptions = diffOptions | XmlDiffOptions.IgnoreXmlDecl;
        //diff.Algorithm = XmlDiffAlgorithm.Auto;
        //diff.Algorithm = XmlDiffAlgorithm.Fast;
        //diff.Algorithm = XmlDiffAlgorithm.Precise;
        //-------------------------------------------------------------
    }
}