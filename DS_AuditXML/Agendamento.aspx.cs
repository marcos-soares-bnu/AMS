using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
//

namespace DS_AuditXML
{
    public partial class Agendamento : System.Web.UI.Page
    {
        public MembershipUser               currentUser;
        public static MPSfwk.Model.Audits   aud;
        public static string                XMLATcmds   = ConfigurationManager.AppSettings["XMLData"] + "ListATcmds.xml";
        public static byte[]                bytes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Pega o usuário logado...
                currentUser = Membership.GetUser();

                if  (   (currentUser.UserName.ToUpper().IndexOf("F0FP186") >= 0) ||
                        (currentUser.UserName.ToUpper().IndexOf("MSOARES") >= 0) ||
                        (currentUser.UserName.ToUpper().IndexOf("UMPSOAR") >= 0)
                    )

                {
                    Button1.Text = "Atualiza - " + "AT_" + currentUser.UserName.Replace("\\", "_") + ".txt" + " !";
                    Button1.Visible = true;
                    Button2.Text = "LIST_USERS";
                    Button2.Visible = true;
                }

                panAviso.Visible = true;
                dtSel1.Visible = false;
                dtSel2.Visible = false;
                dtSel3.Visible = false;
                divMessage.InnerHtml = "";
                txtHoraIni.Text = DateTime.Now.ToString("yyyyMMddHHmm");
                txtHH.Text = DateTime.Now.ToString("HH");
                txtmm.Text = DateTime.Now.ToString("mm");

                //Chama a rotina para gerar a lista de Cmds e destacar os ativos...
                DS_AuditXML.Util_list.lstBox_BindRefresh(false, true, XMLATcmds, lstATcmds);

                //Visualiza a Lista de Tasks...
                ListTasks(false);
            }
            else
            {
                int indSel = lstATcmds.SelectedIndex;
                //Chama a rotina para gerar a lista de Cmds e destacar os ativos...
                DS_AuditXML.Util_list.lstBox_BindRefresh(false, true, XMLATcmds, lstATcmds);
                lstATcmds.SelectedIndex = indSel;
            }

        } //OK MPS - 06/10/2014

        protected string readfile_Click(string txtfile)
        {
            //Preenche a tabela com o conteudo do BAT...
            DataTable table = new DataTable();
            table.Columns.Add("Conteudo");
            string lin = "";
            string logfile = "";

            try
            {
                using (StreamReader sr = new StreamReader(txtfile))
                {
                    while (!sr.EndOfStream)
                    {
                        lin = sr.ReadLine();
                        table.Rows.Add(lin);
                        if (lin.IndexOf("> ") > 0)
                        {
                            logfile = lin.Substring(lin.IndexOf("> ") + 2);
                        }
                    }
                }
                GridView2.DataSource = table;
                GridView2.DataBind();
                return logfile;
            }
            catch (Exception eg)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Ocorreu um erro: " + eg.Message + "  </span>";
                return null;
            }

        } //OK MPS - 06/10/2014

        public string getDayWeek() //Int32
        {
            //Int32 sum = 0;
            string week = "";
            chkSemana.ToolTip = "";
            for (int i = 0; i < chkSemana.Items.Count; i++)
            { 
                if (chkSemana.Items[i].Selected) 
                { 
                    week = week + chkSemana.Items[i].Value + ",";
                    //sum += Convert.ToInt32(chkSemana.Items[i].Value); 
                } 
            }
            if (week != "")
            {
                chkSemana.ToolTip = "/SC WEEKLY /D " + week.Substring(0, (week.Length - 1));
            }
            return chkSemana.ToolTip;
            //return sum;

        } //OK MPS - 06/10/2014

        public string getDayMonth() //Int32
        {
            if (chkSemana.ToolTip == "")
            {
                string mont = "";
                //Int32 sum = 0;
                for (int i = 0; i < chkDiaMes.Items.Count; i++)
                { 
                    if (chkDiaMes.Items[i].Selected)
                    {
                        mont = mont + chkDiaMes.Items[i].Text + "-";
                        //sum += Convert.ToInt32(chkDiaMes.Items[i].Value); 
                    } 
                }
                if (mont != "")
                {
                    chkDiaMes.ToolTip = "/SC MONTHLY /D " + mont.Substring(0, mont.Length - 1);
                }
                //return sum;
            }
            return chkDiaMes.ToolTip;

        } //OK MPS - 06/10/2014

        protected string GeraBatAT(bool sohUpdArqUser)
        {
            try
            {
                //Pega o usuário logado...
                currentUser = Membership.GetUser();

                //Pega a chave de Criptografia...
                var Section = ConfigurationManager.GetSection("system.web/machineKey");
                string aux = ((System.Web.Configuration.MachineKeySection)(Section)).ValidationKey.ToString();
                bytes = ASCIIEncoding.ASCII.GetBytes(aux.Substring(0, 8));

                string pathCMD = ConfigurationManager.AppSettings["XMLData"] + "ATs\\";
                string nomeCMD = "";

                //MPS - 21/OUT---------------------------------------------
                //Testa se existe parametros nos cmds...
                if (lstATcmds.SelectedValue.ToString().IndexOf("[p1]") > 0)
                {
                    string findParam = "";
                    string findParamConv = "";
                    string[] stringSeparators = new string[] { "[", "]" };
                    string[] vetParam = lstATcmds.SelectedValue.ToString().Split(stringSeparators, StringSplitOptions.None);
                    string[] vetParamConc = txtParams.Text.Split(stringSeparators, StringSplitOptions.None);
                    if (vetParam.Length == vetParamConc.Length)
                    {
                        foreach (string s in vetParam)
                        {
                            if (s.IndexOf(">>") == -1)
                            {
                                if (s.ToUpper().IndexOf("P") >= 0)
                                { findParam = findParam + "[" + s + "] "; }
                            }
                        }
                        foreach (string s in vetParamConc)
                        {
                            if ((s != " ") && (s != ""))
                            { findParamConv = findParamConv + s + " "; }
                        }
                    }
                    else
                    { return "Erro"; }

                    nomeCMD = lstATcmds.SelectedValue.ToString().Replace(findParam, findParamConv);
                }
                else
                { nomeCMD = lstATcmds.SelectedValue.ToString(); }
                //
                //MPS - 21/OUT---------------------------------------------

                string param = currentUser.UserName.Replace("\\", "_");
                param += " ";
                param += currentUser.GetPassword();

                string cryptedparam = DS_AuditXML.Util_list.Encrypt(param, bytes);

                //Grava param criptografado em arquivo...
                string batCMD = pathCMD + "AT_" + currentUser.UserName.Replace("\\", "_") + "_" + txtHoraIni.Text + DateTime.Now.Second + ".bat";
                string arqParam = pathCMD + "AT_" + currentUser.UserName.Replace("\\", "_") + ".txt";

                //Cria/Atualiza o Arquivo de usuario para utilizar no app BATCH...
                using (StreamWriter writer = new StreamWriter(arqParam, false))
                { writer.WriteLine(cryptedparam); }

                if (!sohUpdArqUser)
                {
                    //Cria um Arquivo BAt na pasta das ATs para executar via AT...
                    using (StreamWriter writer = new StreamWriter(batCMD, false))
                    {
                        writer.WriteLine("cd " + pathCMD);
                        writer.WriteLine(nomeCMD);
                    }
                    return batCMD;
                }
                else
                {
                    return arqParam;
                }
            }
            catch (Exception ex)
            {
                return "Ocorreu um erro: " + ex.Message;
            }

        } //OK MPS - 21/10/2014

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //Pega o usuário logado...
            MembershipUser      currentUser = Membership.GetUser();

            //Seta os dados para conn WMI...
            MPSfwk.Model.Server s           = new MPSfwk.Model.Server();
            s.IPHOST                        = ConfigurationManager.AppSettings["ServerAT"];
            s.USUARIO                       = currentUser.UserName;
            s.SENHA                         = currentUser.GetPassword();

            //Pega o indice e a linha selecionada
            int                 index       = Convert.ToInt32(e.CommandArgument);
            GridViewRow         row         = GridView1.Rows[index];

            //Chama a rotina para ler o bat e mostrar no grid2...
            int                 indnom      = GetColumnIndexByName(GridView1, "Nome");
            string              nomSel      = row.Cells[indnom].Text;
            int                 posSp1      = nomSel.IndexOf(" (");
            int                 posSp2      = nomSel.IndexOf("C:");
            string              batSel      = nomSel.Substring(posSp2);
            nomSel                          = nomSel.Substring(0, posSp1);
            batSel                          = batSel.Substring(0, batSel.IndexOf(" )"));
            string              logbat      = readfile_Click(batSel);
            string              cmd         = "";
            int                 ret         = 0;
            //
            dtSel2.Visible                  = false;
            dtSel3.Visible                  = false;
            GridView2.Visible               = false;
            TreeView1.Visible               = false;

            try
            {
                if (e.CommandName == "Del")
                {
                    //Chama a função para o Executar a Tarefa...
                    cmd = "schtasks /delete /TN \"" + nomSel + "\" /F ";
                    ret = MPSfwk.WMI.ExecuteCommand(cmd, 5000, "");
                    //
                    if (File.Exists(batSel)) { File.Delete(batSel); }
                    //
                    ListTasks(true);
                    divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Deletou - " + nomSel + " com sucesso! </span>";
                }
                else if (e.CommandName == "Run")
                {
                    //Chama a função para o Executar a Tarefa...
                    cmd = "schtasks /run /TN \"" + nomSel + "\" ";
                    ret = MPSfwk.WMI.ExecuteCommand(cmd, 5000, "");
                    //
                    ListTasks(true);
                    divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Executou - " + nomSel + " com sucesso! </span>";
                }
                else if (e.CommandName == "View")
                {
                    //
                    dtSel2.Visible = true;
                    dtSel3.Visible = true;
                    GridView2.Visible = true;

                    //Chama a rotina para gerar a lista de Servidores/Classes/Cmds e destacar os ativos...
                    dtSel2.Text = "[" + batSel + "]";

                    //Visualiza os Logs do Sistema...(Treeview)
                    dtSel3.Text = "[" + logbat + "]";
                    showLogTv(logbat);
                }
            }
            catch (Exception eg)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Ocorreu um erro: " + nomSel + " | " + batSel + " | " + cmd + " | " + " | " + ret + " | " + eg.Message + "  </span>";
            }

        } //OK MPS - 21/10/2014

        private int GetColumnIndexByName(GridView grid, string name)
        {
            for (int i = 0; i < grid.HeaderRow.Cells.Count; i++)
            {
                if (grid.HeaderRow.Cells[i].Text.ToLower().Trim() == name.ToLower().Trim())
                {
                    return i;
                }
            }

            return -1;

        } //OK MPS - 09/10/2014

        protected void lnkHelp_Click(object sender, EventArgs e)
        {
            if (dtSel3.Visible == true)
            {
                lnkHelp.Text = "(Ver)?";
                dtSel1.Visible = true;
                GridView1.Visible = true;
                dtSel3.Visible = false;
                TreeView1.Visible = false;
            }
            else
            {
                lnkHelp.Text = "(Ocultar)?";
                dtSel1.Visible = false;
                GridView1.Visible = false;
                dtSel2.Visible = false;
                GridView2.Visible = false;
                //Visualiza os Logs do Sistema...(Treeview)
                string logbat = ConfigurationManager.AppSettings["XMLData"] + "ATs\\ds_ajuda-H.log";
                if (File.Exists(logbat))
                {
                    dtSel3.Text = "[" + logbat + "]";
                    dtSel3.Visible = true;
                    showLogTv(logbat);
                }
            }

        } //OK MPS - 21/10/2014

        protected void showLogTv(string logfile)
        {
            TreeView1.Visible = true;
            TreeView1.Nodes.Clear();
            TreeNode raiz = new TreeNode("_______________________________Conteudo________________________________");
            raiz.SelectAction = TreeNodeSelectAction.Expand;
            TreeView1.Nodes.Add(raiz);
            string linha = "";
            string[] vetlin = { "" };
            string cls = "";
            string srv = "";
            //
            TreeNode logini = null;
            TreeNode classes = null;
            TreeNode servers = null;

            using (StreamReader sr = new StreamReader(logfile))
            {
                while ((!sr.EndOfStream) ||
                        (linha != null)
                      )
                {
                    if ((linha.IndexOf("|Info|") >= 0) && (linha.IndexOf("Inicio do Processamento") >= 0))
                    {
                        cls = "";
                        srv = "";
                        //
                        logini = new TreeNode(linha);
                        logini.SelectAction = TreeNodeSelectAction.Expand;
                        raiz.ChildNodes.Add(logini);

                        while ((linha.IndexOf("Final do Processamento") == -1) &&
                                (linha.IndexOf("|!|") == -1) &&
                                (linha != null)
                              )
                        {
                            linha = sr.ReadLine();
                            if (linha != null)
                            {
                                if ((linha.IndexOf("|Info|") >= 0) && (linha.IndexOf("Inicio do Processamento") >= 0))
                                { break; }
                            }
                            else { break; }
                            //
                            if (    (linha.IndexOf("Info|") >= 0)   ||
                                    (linha.IndexOf("?|") >= 0)      ||
                                    (linha.IndexOf("!|") >= 0)
                               )
                            {
                                if (linha.IndexOf("Info| Server") >= 0)
                                {
                                    while ((linha.IndexOf("Info| Server") >= 0) ||
                                            (linha == "")
                                          )
                                    {
                                        if (linha == "") { linha = sr.ReadLine(); }
                                        if (linha.IndexOf("Info| Server") == -1) { linha = sr.ReadLine(); }
                                        if (linha.IndexOf("Info| Server") >= 0)
                                        {
                                            vetlin = linha.Split('|');
                                            if (cls != vetlin[3])
                                            {
                                                classes = new TreeNode(vetlin[3]);
                                                classes.SelectAction = TreeNodeSelectAction.Expand;
                                                logini.ChildNodes.Add(classes);

                                                if (srv != vetlin[2])
                                                {
                                                    servers = new TreeNode(vetlin[2] + "( " + vetlin[4].Substring(vetlin[4].IndexOf("[")) + " )");
                                                    servers.SelectAction = TreeNodeSelectAction.Expand;
                                                    classes.ChildNodes.Add(servers);
                                                }
                                            }
                                            else
                                            {
                                                servers = new TreeNode(vetlin[2] + "( " + vetlin[4].Substring(vetlin[4].IndexOf("[")) + " )");
                                                servers.SelectAction = TreeNodeSelectAction.Expand;
                                                classes.ChildNodes.Add(servers);
                                            }
                                        }
                                        //
                                        srv = vetlin[2];
                                        cls = vetlin[3];
                                        linha = sr.ReadLine();
                                    }
                                }
                                if (    (linha.IndexOf("Info|") >= 0)   ||
                                        (linha.IndexOf("?|") >= 0)      ||
                                        (linha.IndexOf("!|") >= 0)
                                   )
                                {
                                    //Caso não tenha a seq de Server, mostra a msg de info...
                                    TreeNode filhos = new TreeNode(linha);
                                    filhos.SelectAction = TreeNodeSelectAction.Expand;
                                    logini.ChildNodes.Add(filhos);
                                }
                            }
                        }
                    }
                    else { linha = sr.ReadLine(); } //Vai para a proxima...
                }
            }

        } //OK MPS - 21/10/2014

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((ImageButton)e.Row.Cells[2].Controls[1]).OnClientClick = "return confirm('Voce tem certeza que deseja Deletar esta Tarefa?');";
                //
                if ( (e.Row.Cells[4].Text.IndexOf("DS_AuditXML") == -1) &&
                     (e.Row.Cells[4].Text.IndexOf("Shell_AutoArch") == -1)
                   )
                {
                    e.Row.Visible = false;
                }
            }

        } //OK MPS - 21/10/2014

        protected void imbRefresh_Click(object sender, ImageClickEventArgs e)
        {
            //Pega o usuário logado...
            MembershipUser currentUser = Membership.GetUser();

            ListTasks(true);
            //dtSel3.Visible = true;
            //dtSel3.Text = "Deploy - Desvio de Acesso";
            //GridView3.Visible = true;
            //
            //GridView3.DataSource = MPSfwk.SharePoint.ListSPDeployAcesso(currentUser.UserName, currentUser.GetPassword());
            //GridView3.DataBind();

        } //OK MPS - 23/10/2014

        protected void imbAddVazio_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //Pega o user local\senha para criar a task...
                MembershipUser userTasks = Membership.GetUser("ds_auditxml_tasks", false);
                string passTasks = userTasks.GetPassword();

                string cmd      = "";
                string nomBAT   = GeraBatAT(false);
                string nomTSK   = lstATcmds.SelectedItem.ToString() + " - " + txtHoraIni.Text + DateTime.Now.Second;
                string HHmm     = txtHoraIni.Text.Substring(8, 2) + ":" + txtHoraIni.Text.Substring(10, 2);
                if (chkRepetir.Checked)
                {
                    cmd = "schtasks /Create " + getDayWeek() + getDayMonth() +
                                            " /RU " + userTasks.UserName + " /RP " + passTasks +
                                            " /TN  \"" + nomTSK + "\" " +
                                            " /TR  \"" + nomBAT + "\" " +
                                            " /ST " + HHmm + " ";
                }
                else
                {
                    cmd = "schtasks /Create   /SC ONCE " +
                                            " /RU " + userTasks.UserName + " /RP " + passTasks +
                                            " /TN  \"" + nomTSK + "\" " +
                                            " /TR  \"" + nomBAT + "\" " +
                                            " /ST " + HHmm + " ";
                }

                //Chama a função para o Exutar a Tarefa...
                int ret = MPSfwk.WMI.ExecuteCommand(cmd, 3000, "");

                ListTasks(true);
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + " Nova Tarefa - " + nomTSK + " criada com sucesso!  </span>";
            }
            catch (Exception eg)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + "Ocorreu um erro: " + eg.Message + "  </span>";
            }

        } //OK MPS - 21/10/2014

        public void ListTasks(bool ehNovo)
        {
            try
            {
                //Preenche a tabela com o conteudo do BAT...
                DataTable dt = new DataTable("Tasks");
                dtSel1.Text = "Lista de Tarefas Agendadas";
                dtSel1.Visible = true;
                GridView1.Visible = true;
                dt.Columns.Add("Nome");
                dt.Columns.Add("Status");
                dt.Columns.Add("Disparadores");
                dt.Columns.Add("Proxima Execucao");
                dt.Columns.Add("Ultima Execucao");
                dt.Columns.Add("Ultimo Resultado");
                dt.Columns.Add("Autor");
                bool ehCabec = false;
                string[] row = null;
                string[] stringSeparators = new string[] { "\",\"" };
                string msg = "";

                string nomTXT = ConfigurationManager.AppSettings["XMLData"] + "ListTasks.txt";
                string cmd = "schtasks /Query /FO CSV /V | findstr /V /C:\"TaskName\" > \"" + nomTXT + "\" ";

                //Chama a função para o Executar a Tarefa...
                //somente se o arquivo for > 4 horas...
                DateTime startTime = DateTime.Now;
                DateTime endTime;
                int ret = 0;
                endTime = System.IO.File.GetLastWriteTime(nomTXT);

                double diffH = startTime.Subtract(endTime).TotalHours;
                if ( (diffH > 1) || (ehNovo) )
                { ret = MPSfwk.WMI.ExecuteCommand(cmd, 5000, ""); }

                //
                using (StreamReader sr = new StreamReader(nomTXT, System.Text.Encoding.UTF8, true))
                {
                    while (!sr.EndOfStream)
                    {
                        row = sr.ReadLine().Split(stringSeparators, StringSplitOptions.None);
                        if (!ehCabec)
                        {
                            if (row[0].ToUpper().IndexOf("NOME DO HOST") > 0)
                            { ehCabec = true; }
                        }
                        else
                        {
                            if (row[0].ToUpper().IndexOf("NOME DO HOST") > 0)
                            { break; }
                            else
                            {
                                if (row.Length != 28)
                                { msg = msg + "Ocorreu um erro na leitura das Tasks! <br />"; }
                                else
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["Nome"] = row[1] + " (" + row[8] + ")";
                                    dr["Status"] = ConvEncString(row[3]);
                                    string disp = "";
                                    for (int i = 18; i < 28; i++)
                                    {
                                        if ((row[i].ToUpper().IndexOf("DESATIVADO") == -1) &&
                                                (row[i].ToUpper().IndexOf("N/A") == -1) &&
                                                (row[i].ToUpper().IndexOf("NENHUM") == -1)
                                           ) { disp = disp + row[i] + " "; }
                                    }
                                    dr["Disparadores"] = ConvEncString(disp);
                                    dr["Proxima Execucao"] = ConvEncString(row[2]);
                                    dr["Ultima Execucao"] = ConvEncString(row[5]);
                                    dr["Ultimo Resultado"] = row[6];
                                    dr["Autor"] = row[7];
                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }
                //
                msg = msg + " " + ret.ToString();
                GridView1.DataSource = dt;
                GridView1.DataBind();
                //
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + " Lista Tarefas - " + nomTXT + " ( " + System.IO.File.GetLastWriteTime(nomTXT) + " ) </span>";
            }
            catch (Exception ex)
            {
                divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'>" + " Ocorreu um erro na leitura das Tasks - " + ex.Message + "</span>";
            }

        } //OK MPS - 21/10/2014

        protected void Button2_Click(object sender, EventArgs e)
        {
            dtSel1.Visible = false;
            GridView1.Visible = false;
            dtSel3.Visible = false;
            TreeView1.Visible = false;
            //
            GridView2.DataSource = null;
            GridView2.DataBind();
            //
            dtSel2.Text = Button1.Text;
            GridView2.DataSource = Membership.GetAllUsers();
            GridView2.DataBind();
            GridView2.Visible = true;
            dtSel2.Visible = true;

        } //OK MPS - 23/10/2014

        protected void Button1_Click(object sender, EventArgs e)
        {
            string msg = GeraBatAT(true);
            divMessage.InnerHtml = "<span id='msg' style='color:#FF3300;font-size:Smaller;font-weight:bold;'> Arquivo - " + msg + " atualizado!  </span>";

        } //OK MPS - 21/10/2014


        protected string ConvEncString(string enc)
        {
            char[] originalString = enc.ToCharArray();
            StringBuilder asAscii = new StringBuilder(); // store final ascii string and Unicode points
            foreach (char c in originalString)
            {
                // test if char is ascii, otherwise convert to Unicode Code Point
                int cint = Convert.ToInt32(c);
                if (cint <= 127 && cint >= 0)
                    asAscii.Append(c);
                else
                    if (cint == 65533)
                        asAscii.Append('a');
                    else
                        asAscii.Append(String.Format("{0}", cint.ToString()).Trim());
            }

            return asAscii.ToString();

        } //OK MPS - 21/10/2014

        protected void GridView3_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void GridView3_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }


//MPS 09-21/OUT - BUGS ----------------------------------------------------------------

    }
}