using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;

namespace MPSfwk
{
    public static class WMI
    {
        public static ConnectionOptions wmInstConOpt(string UserName, string Password)
        {
            ConnectionOptions oConn = new ConnectionOptions();
            oConn.Username = UserName;
            oConn.Password = Password;
            oConn.EnablePrivileges = true;

            return oConn;
        }


        public static ManagementPath wmInstPath(string MachineName, string ClassName, string NameSpace)
        {
            ManagementPath path = new ManagementPath();
            path.Server         = MachineName;
            path.ClassName      = ClassName;
            path.NamespacePath  = NameSpace;

            return path;
        }


        public static ManagementScope wmInstScope(string UserName, ManagementPath path, ConnectionOptions oConn)
        {
            ManagementScope scope;
            if (UserName == "")
            { scope = new ManagementScope(path); }
            else
            { scope = new ManagementScope(path, oConn); }

            return scope;
        }


        public static string ExecProcesso(Model.Server s, ConnectionOptions oConn, string localSrv, string commandLine)
        {
            ManagementPath path2 = wmInstPath(s.IPHOST, "Win32_Process", "root\\CIMV2");
            ManagementScope scopeProcess = wmInstScope(s.USUARIO, path2, wmInstConOpt(s.USUARIO, s.SENHA));

            using (ManagementClass process = new ManagementClass(scopeProcess, path2, null))
            {
                using (ManagementBaseObject inParams = process.GetMethodParameters("Create"))
                {
                    inParams["CommandLine"] = commandLine;
                    inParams["CurrentDirectory"] = localSrv;//DriveLetter + @":\\";
                    inParams["ProcessStartupInformation"] = null;
                    using (ManagementBaseObject outParams = process.InvokeMethod("Create", inParams, null))
                    {
                        int retVal = Convert.ToInt32(outParams.Properties["ReturnValue"].Value);
                        return retVal.ToString();
                    }
                }
            }
        }


        public static string CriaPastaSite(string localSrv, string remoteDir, Model.Server s)
        {
            string TempName = remoteDir;
            int Index = TempName.IndexOf(":");
            string DriveLetter = "C";
            if (Index != -1)
            {
                string[] arr = TempName.Split(new char[] { ':' });
                DriveLetter = arr[0];
                TempName = TempName.Substring(Index + 2);
            }

            try
            {
                ManagementPath myPath = wmInstPath(s.IPHOST, "", "root\\CIMV2");
                ConnectionOptions oConn = wmInstConOpt(s.USUARIO, s.SENHA);
                ManagementScope scope = wmInstScope(s.USUARIO, myPath, oConn);
                scope.Connect();

                //without next strange manipulation, the os.Get().Count will throw the "Invalid query" exception
                remoteDir = remoteDir.Replace("\\", "\\\\");
                ObjectQuery oq = new ObjectQuery("select Name from Win32_Directory where Name = '" + remoteDir + "'");
                using (ManagementObjectSearcher os = new ManagementObjectSearcher(scope, oq))
                {
                    if (os.Get().Count == 0)      //It don't exist, so create it!
                    {
                        string commandLine = String.Format(@"cmd /C  mkdir {0} ", TempName);
                        return ExecProcesso(s, oConn, localSrv, commandLine);
                    }
                    else
                        return "O usuário já possui um perfil neste Servidor!";
                }
            }
            catch (Exception ex)
            {
                return "Ocorreu um erro: " + ex.Source + "\nDetail: " + ex.Message;
            }
        
        } //OK MPS - 09/10/2014


        public static bool zipListArq(string lstArq, string[] fileCompressList, string targetCompressName)
        {
            try
            {
                string PZipPath = "7za.exe";
                if (!File.Exists(PZipPath))
                {
                    Console.WriteLine("|!| O arquivo 7za.exe, nao foi encontrado na pasta atual!");
                    return false;
                }
                if (fileCompressList.Length == 0)
                {
                    Console.WriteLine("|!| Nenhum arquivo na pasta informada!");
                    return false;
                }

                // Cria a list em arquivo...list.txt
                StreamWriter fileList = new StreamWriter(lstArq, true);
                foreach (string filename in fileCompressList)
                {
                    if ((File.Exists(filename)) && (filename != lstArq))
                    {
                        fileList.WriteLine(filename);
                    }
                }
                fileList.Close();

                if (!File.Exists(lstArq))
                {
                    Console.WriteLine("|!| O arquivo - " + lstArq + ", nao foi encontrado na pasta atual!");
                    return false;
                }

                ProcessStartInfo pCompress = new ProcessStartInfo();
                pCompress.FileName = PZipPath;
                pCompress.Arguments = "a -tzip \"" + targetCompressName + "\" " + "@" + lstArq + " -mx=9";
                pCompress.WindowStyle = ProcessWindowStyle.Hidden;
                pCompress.UseShellExecute = false;
                pCompress.RedirectStandardOutput = false;
                Process x = Process.Start(pCompress);
                x.WaitForExit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("|!| Ocorreu um Erro no zipListArq - " + e.Message);
                return false;
            }
        }
        

        public static bool findNetPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            string pathRoot = Path.GetPathRoot(path);
            if (string.IsNullOrEmpty(pathRoot)) return false;
            ProcessStartInfo pinfo = new ProcessStartInfo("net", "use");
            pinfo.CreateNoWindow = true;
            pinfo.RedirectStandardOutput = true;
            pinfo.UseShellExecute = false;
            string output;
            using (Process p = Process.Start(pinfo))
            {
                output = p.StandardOutput.ReadToEnd();
            }
            foreach (string line in output.Split('\n'))
            {
                if (line.Contains(pathRoot) && line.Contains("OK"))
                {
                    return true; // shareIsProbablyConnected
                }
            }
            return false;
        }


        public static bool netToServer(string usr, string pwd, string srvPath, int timeout)
        {
            if (!findNetPath(srvPath))
            {
                var directory = Path.GetDirectoryName(srvPath).Trim();
                var command = "NET USE " + directory + " /user:" + usr + " " + pwd;
                ExecuteCommand(command, timeout, "C:\\");
                return true;
            }
            return true; // se já existe a conexao, apenas utiliza...
        }


        public static bool delToServer(string srvPath, int timeout)
        {
            if (findNetPath(srvPath))
            {
                var directory = Path.GetDirectoryName(srvPath).Trim();
                var command = "NET USE " + directory + " /delete";
                ExecuteCommand(command, timeout, "C:\\");
                return true;
            }
            return true;
        }


        //MPS 25/OUT - mover por ext - vários arquivos...
        public static void MoveExtfileToServer(string fromPathExt, string toPathExt, int timeout)
        {
            var command = " move /Y \"" + fromPathExt + "\"  \"" + toPathExt + "\"";
            ExecuteCommand(command, timeout, "C:\\");
        }
        //MPS 25/OUT - mover por ext - vários arquivos...


        public static void MovefileToServer(string filePath, string savePath, int timeout)
        {
            var directory = Path.GetDirectoryName(savePath).Trim();
            var filenameToSave = Path.GetFileName(savePath);

            if (!directory.EndsWith("\\"))
                filenameToSave = "\\" + filenameToSave;

            var command = " move /Y \"" + filePath + "\"  \"" + directory + filenameToSave + "\"";
            ExecuteCommand(command, timeout, "C:\\");
        }

        
        public static void SaveACopyfileToServer(string filePath, string savePath, int timeout)
        {
            var directory = Path.GetDirectoryName(savePath).Trim();
            var filenameToSave = Path.GetFileName(savePath);

            if (!directory.EndsWith("\\"))
                filenameToSave = "\\" + filenameToSave;

            var command = " copy /Y \"" + filePath + "\"  \"" + directory + filenameToSave + "\"";
            ExecuteCommand(command, timeout, "C:\\");
        }


        public static int ExecuteCommand(string command, int timeout, string workDir)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/C " + command)
                                {
                                    CreateNoWindow = true, 
                                    UseShellExecute = false,
                                    WorkingDirectory = workDir,
                                };
            var process = Process.Start(processInfo);
            //System.Threading.Thread.Sleep(timeout);
            process.WaitForExit(timeout);
            var exitCode = process.ExitCode;
            process.Close();
            return exitCode;
        }


        public static DataTable GroupMembers(string srv, string usr, string pwd)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                //
                MPSfwk.Model.Server s = new MPSfwk.Model.Server();
                s.IPHOST = srv;
                s.USUARIO = usr;
                s.SENHA = pwd;
                ManagementScope ms = scopeMgmt(false, s);
                ObjectGetOptions objectGetOptions = new ObjectGetOptions();
                //
                string targethost = "";
                string groupname = "";
                string aux_qry = "";
                if ((srv.IndexOf(".") == -1) && (srv.ToUpper() != "LOCALHOST"))
                {   aux_qry = "select * from Win32_Group Where Domain = '" + srv + "'"; }
                else
                { aux_qry = "select * from Win32_Group Where LocalAccount = True"; }
                //
                //MPS teste - 10/out
                //
                Console.WriteLine("DEBUG - aux_qry = " + aux_qry);
                //
                DataTable dt_aux = dtlistaClasse("Win32_Group",
                                                    aux_qry,
                                                    srv,
                                                    usr,
                                                    pwd);
                //
                //Cria tabela para preencher os campos
                DataTable dt1 = new DataTable();
                dt1.TableName = "GroupMembers";
                dt1.Columns.Add("Domain");
                dt1.Columns.Add("Group Name");
                dt1.Columns.Add("Users");
                //
                foreach (DataRow drow in dt_aux.Rows)
                {
                    //
                    DataRow dr = dt1.NewRow();
                    //
                    targethost = drow["Domain"].ToString();
                    groupname = drow["Name"].ToString();

                    StringBuilder qs = new StringBuilder();
                    qs.Append("SELECT PartComponent FROM Win32_GroupUser WHERE GroupComponent = \"Win32_Group.Domain='");
                    qs.Append(targethost);
                    qs.Append("',Name='");
                    qs.Append(groupname);
                    qs.AppendLine("'\"");
                    ObjectQuery query = new ObjectQuery(qs.ToString());
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(ms, query);
                    ManagementObjectCollection queryCollection = searcher.Get();
                    foreach (ManagementObject m in queryCollection)
                    {
                        ManagementPath path = new ManagementPath(m["PartComponent"].ToString());
                        {
                            String[] names = path.RelativePath.Split(',');
                            result.Append(names[0].Substring(names[0].IndexOf("=") + 1).Replace("\"", " ").Trim() + "\\");
                            result.AppendLine(names[1].Substring(names[1].IndexOf("=") + 1).Replace("\"", " ").Trim() + " ; ");
                        }
                    }
                    //Console.WriteLine("Domain =  " + targethost + " Name = " + groupname + " Users = " + result.ToString());
                    dr["Domain"] = targethost;
                    dr["Group Name"] = groupname;
                    dr["Users"] = result.ToString();
                    dt1.Rows.Add(dr);
                    //
                    result = new StringBuilder();
                    
                }
                return dt1;
                //
            }
            catch (Exception e)
            {
                Console.WriteLine("|!| GroupMembers Error - " + e.Message);
                return null;
            }
        }



        public static string InsertAT(  string srv, string usr, string pwd, 
                                        string inCMD, string inRPT, string inDOW, string inDOM, string inSTM)
        {
            try
            {
                MPSfwk.Model.Server s = new MPSfwk.Model.Server();
                s.IPHOST = srv;
                s.USUARIO = usr;
                s.SENHA = pwd;
                string strJobId = "";
                ManagementScope ms = scopeMgmt(false, s);
                ObjectGetOptions objectGetOptions = new ObjectGetOptions();
                ManagementPath managementPath = new ManagementPath("Win32_ScheduledJob");
                ManagementClass processClass = new ManagementClass(ms, managementPath, objectGetOptions);
                ManagementBaseObject inParams = processClass.GetMethodParameters("Create");
                inParams["Command"] = inCMD;
                inParams["InteractWithDesktop"] = "False";
                inParams["RunRepeatedly"] = inRPT;
                inParams["DaysOfMonth"] = inDOM;
                inParams["DaysOfWeek"] = inDOW;
                inParams["StartTime"] = inSTM + "00.000000-180";
                ManagementBaseObject outParams =
                        processClass.InvokeMethod("Create", inParams, null);

                strJobId = outParams["JobId"].ToString();

                return "Novo JobId (" + strJobId + ") criado com sucesso!";
            }
            catch (UnauthorizedAccessException uex)
            {
                return "Ocorreu um erro: " + uex.Message;
            }
            catch (ManagementException mex)
            {
                return "Ocorreu um erro: " + mex.Message;
            }
        }


        public static string DeleteAT(string srv, string JobID)
        {
            try
            {
                string strJobId = "";

                ManagementObject mo;
                ManagementPath path = ManagementPath.DefaultPath;
                path.RelativePath = "Win32_ScheduledJob.JobId=" + "\"" + JobID + "\"";
                path.Server = srv;
                mo = new ManagementObject(path);
                ManagementBaseObject inParams = null;
                // use late binding to invoke "Delete" method on "Win32_ScheduledJob" WMI class
                ManagementBaseObject outParams = mo.InvokeMethod("Delete", inParams, null);

                strJobId = outParams.Properties["ReturnValue"].Value.ToString();
                if (strJobId == "0") { return "O JobId ( " + JobID + " ) selecionado foi Apagado!"; }
                else { return "Out parameters: ReturnValue= " + strJobId; }
            }
            catch (UnauthorizedAccessException uex)
            {
                return "Ocorreu um erro: " + uex.Message;
            }
            catch (ManagementException mex)
            {
                return "Ocorreu um erro: " + mex.Message;
            }
        }


        public static DataTable dtlistaClasse(string cls, string cls_SEL, string srv, string usr, string pwd)
        {
            try
            {
                MPSfwk.Model.Server s = new MPSfwk.Model.Server();
                s.IPHOST = srv;
                s.USUARIO = usr;
                s.SENHA = pwd;

                ManagementScope ms = scopeMgmt(true, s);   //true = testa a conexao remota, senao
                                                            //       acaba retornando a local***
                                                            //       extrai as classes localmente...
                //teste de conexao...
                if (ms == null) { return null; }

                ManagementObjectSearcher srcd;
                //
                //testa se a Classe possui o Host ao inves do IP, se for muda o LocalAccount
                string aux_qry = "";
                if (    (srv.IndexOf(".") == -1)                                && 
                        (cls_SEL.ToUpper().IndexOf("LOCALACCOUNT = TRUE") > 0)  &&
                        (srv.ToUpper() != "LOCALHOST")
                   )
                {   aux_qry = cls_SEL.ToUpper().Replace("LOCALACCOUNT = TRUE", ("Domain = '" + srv.ToUpper() + "'")); }
                else
                {   aux_qry = cls_SEL; }
                //
                //MPS teste - 10/out
                Console.WriteLine("DEBUG - aux_qry = " + aux_qry);
                //
                srcd = new ManagementObjectSearcher(ms, new ObjectQuery(aux_qry));
                ManagementObjectCollection moc = srcd.Get();

                //Cria tabela para preencher os campos
                DataTable dt1 = new DataTable();
                dt1.TableName = cls;

                //teste...
                string aux_cls = "";
                string[] aux = cls_SEL.Split(' ');
                if (aux.Length == 3)
                { aux_cls = aux[3]; }
                else
                {
                    for (int i = 1; i < aux.Length; i++)
                    {
                        if (aux[i].ToUpper() == "FROM")
                        {
                            aux_cls = aux[i+1];
                            break;
                        }
                    }                
                }

                //Preenche o Grid com as colunas da classe WMI
                //(Caso haja campos determinados, seleciona somente os campos determinados...)
                //
                //ordena, conforme entrada..
                string[] ordem = null;
                if (cls_SEL.IndexOf("*") > 0)
                {
                    var wmiClasse = new ManagementClass(aux_cls);
                    foreach (var prop in wmiClasse.Properties)
                    { if ((cls_SEL.IndexOf(prop.Name) > 0) || (cls_SEL.IndexOf("*") > 0)) { dt1.Columns.Add(prop.Name); } }
                }
                else
                { 
                    int pos1 = cls_SEL.ToUpper().IndexOf("SELECT") + 6;
                    int pos2 = cls_SEL.ToUpper().IndexOf("FROM");
                    if (pos1 < pos2)
                    {
                        if (cls_SEL.IndexOf(",") > 0)
                        { ordem = cls_SEL.Substring(pos1, (pos2 - pos1)).Trim().Split(',', ' '); }
                        else
                        { ordem[0] = cls_SEL.Substring(pos1, (pos2 - pos1)); }
                        //
                        //Preenche as colunas com os campos determinados...
                        for (int i = 0; i < ordem.Length; i++)
                        {
                            if (ordem[i] != "")
                            { dt1.Columns.Add(ordem[i]); }
                        }
                    }
                }

                //Preenche o Grid com os valores da classe WMI
                foreach (ManagementObject mo in moc)
                {
                    DataRow dr = dt1.NewRow();

                    System.Management.PropertyDataCollection pdc = mo.Properties;
                    foreach (System.Management.PropertyData pd in pdc) { dr[pd.Name] = pd.Value; }

                    dt1.Rows.Add(dr);
                }
                //
                //
                return dt1;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
            catch (ManagementException)
            {
                return null;
            }
        }


        public static bool testeConnWMI(string srv, string usr, string pwd)
        {
            MPSfwk.Model.Server s = new MPSfwk.Model.Server();
            s.IPHOST = srv;
            s.USUARIO = usr;
            s.SENHA = pwd;
            //Chama a função para o Gerenciamento WMI remoto/local
            // se ocorre erro, tenta a conexão local...
            ManagementScope ms = scopeMgmt(true, s);
            //
            try
            {
                if (ms.IsConnected == true)
                { return true; }
                else
                { return false; }
            }
            catch (Exception)
            {
                return false;
            }

        }


        private static ManagementScope scopeMgmt(bool test, Model.Server s)
        {
            ManagementPath mp;
            ManagementScope ms = null;
            try
            {
                ConnectionOptions co = new ConnectionOptions();
                co.Impersonation = ImpersonationLevel.Impersonate;
                co.Authentication = AuthenticationLevel.Packet;
                co.Timeout = new TimeSpan(0, 0, 30);
                co.EnablePrivileges = true;
                co.Username = s.USUARIO;
                co.Password = s.SENHA;

                mp = new ManagementPath();
                mp.NamespacePath = @"\root\cimv2";
                mp.Server = s.IPHOST;

                ms = new ManagementScope(mp, co);
                //Se ocorrer erro com a conexão remota acima, tenta a local...
                ms.Connect();
            }
            catch (ManagementException me)
            {
                if (me.ErrorCode.ToString() != "LocalCredentials")
                {
                    if ((test) && (s.IPHOST.ToUpper() != "LOCALHOST"))
                    {
                        Console.WriteLine("|Info| Ocorreu um Erro de Gerenciamento - " + me.Message);
                        return null;
                    }
                }
                else
                {
                    mp = new ManagementPath();
                    mp.NamespacePath = @"\root\cimv2";
                    mp.Server = s.IPHOST;

                    ms = new ManagementScope(mp);
                    ms.Connect();
                }
            }
            catch (Exception eg)
            {
                Console.WriteLine("|Info| Ocorreu um Erro Geral - " + eg.Message);
                return null;
            }


            return ms;
        }

    }
}
