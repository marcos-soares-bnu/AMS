using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Xml;

namespace DS_AuditXML_cmd
{
    class Program
    {
        static byte[] bytes;
        static string PathServer = System.Configuration.ConfigurationManager.AppSettings["PathServer"];
        static string PathMoveServer = System.Configuration.ConfigurationManager.AppSettings["PathMoveServer"];
        static string ArqconnServer = System.Configuration.ConfigurationManager.AppSettings["ArqconnServer"];
        static string ArqcoMvServer = System.Configuration.ConfigurationManager.AppSettings["ArqcoMvServer"];
        static string[] xmls = { "lists.txt" };
        static string[] connServer = { "." };
        static string[] coMvServer = { "." };
        static DateTime _dthrmmss = DateTime.Now;

        public static string[] ListaEXTDir(string path, string ext)
        {
            if (!Directory.Exists(path))
            {
                return null;
            }
            return Directory.GetFiles(path, ext);
        }

        public static bool MoveSrvXML(bool ehRemoto, string local, string ExtFile)
        {
            //-----------------------------------------------------------------------
            // 09/12/2014 - Adicionar caminhos relativos C:\ pelos Arquivos params...
            //              Se achar .. no local, concatena o caminho do exe + params
            //-----------------------------------------------------------------------
            if (local.IndexOf("..") >= 0)
            {
                local = System.IO.Directory.GetCurrentDirectory() + local.Replace("..", "\\") + "\\";
            }

            // Testa e faz as conexoes ao Servidor Remoto (move)...
            //
            if (MPSfwk.WMI.netToServer(coMvServer[0], coMvServer[1], PathMoveServer, 8000))
            {
                //-----------------------------------------------------------------|
                // 24/SET-24/OUT - Testar, caso seja mover do Server para o Local...
                //
                int KMAX_ms = 5000;
                var directory = Path.GetDirectoryName(local).Trim();
                if (!directory.EndsWith("\\"))
                    local = directory + "\\";

                string[] lstDir = null;

                if (!ehRemoto)
                { 
                    lstDir = ListaEXTDir(PathMoveServer, ExtFile);
                    if (lstDir == null)
                    {
                        Console.WriteLine("|!| O Local informado - " + local + ", nao foi encontrado!");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("\n|----------------------------------------------------===================|");
                        Console.WriteLine("|Info| Movendo " + ExtFile + "'s | " + PathMoveServer + " ( " + lstDir.Length + ") arquivo(s) encontrado(s) para: " + local);
                        MPSfwk.WMI.MoveExtfileToServer(PathMoveServer + ExtFile, local, (lstDir.Length * KMAX_ms)); 
                    }
                }
                else
                { 
                    lstDir = ListaEXTDir(local, ExtFile);
                    if (lstDir == null)
                    {
                        Console.WriteLine("|!| O Local informado - " + local + ", nao foi encontrado!");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("\n|----------------------------------------------------===================|");
                        Console.WriteLine("|Info| Movendo " + ExtFile + "'s | " + local + " ( " + lstDir.Length + ") arquivo(s) encontrado(s) para: " + PathMoveServer);
                        MPSfwk.WMI.MoveExtfileToServer(local + ExtFile, PathMoveServer, (lstDir.Length * KMAX_ms));
                    }
                }
                return true;
            }
            else { return false; }
        }


        public static void GeraXML(string server, string classe, string cls_SEL, string user, string pass)
        {
            try
            {
                // Chama as rotinas para a geração do DT...
                DataTable dt1;
                if (cls_SEL.Contains("Win32_GroupUser"))
                { dt1 = MPSfwk.WMI.GroupMembers(server, user, pass); }
                else
                { dt1 = MPSfwk.WMI.dtlistaClasse(classe, cls_SEL, server, user, pass); }
                //
                string path = System.IO.Directory.GetCurrentDirectory() + "\\XML_Data";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                //
                if (dt1 == null)
                {
                    Console.WriteLine("\n|----------------------------------------------------===================|");
                    Console.WriteLine("|!| Server - " + server + " | Classe - " + classe + " | Ocorreu um erro de Conexao! Tente outro <arqIDUser>");
                }
                else
                {
                    string arq = classe + "_" + server + "_" + _dthrmmss.ToString("yyyyMMddHHmmss") + ".xml";
                    dt1.WriteXml(path + "\\" + arq);
                    //
                    //add ao vetor xmls
                    Array.Resize(ref xmls, (xmls.Length + 1));
                    xmls[(xmls.Length - 1)] = path + "\\" + arq;
                    //
                    //Copia o arquivo para o JumpServer definido no app.config...
                    MPSfwk.WMI.SaveACopyfileToServer((path + "\\" + arq), (PathServer + arq), 1500);
                    //
                }
            }
            catch (Exception eg)
            {
                Console.WriteLine("\n|----------------------------------------------------===================|");
                Console.WriteLine("|!| Server - " + server + " | Classe - " + classe + " | Ocorreu um erro - " + eg.Message);
            }
        }


        public static void GravaXML(string local)
        {
            //-----------------------------------------------------------------------
            // 09/12/2014 - Adicionar caminhos relativos C:\ pelos Arquivos params...
            //              Se achar .. no local, concatena o caminho do exe + params
            //-----------------------------------------------------------------------
            if (local.IndexOf("..") >= 0)
            {
                local = System.IO.Directory.GetCurrentDirectory() + local.Replace("..", "\\") + "\\";
            }

            string[] lstDir;
            string[] lstAux;
            string classe;
            string server;
            string geracao;
            bool retorno = false;
            lstDir = Directory.GetFiles(local,"*.xml");
            //
            Console.WriteLine("\n|----------------------------------------------------===================|");
            Console.WriteLine("|Info| Gravando XML | " + local + " ( " + lstDir.Length + ") arquivo(s) encontrado(s)...");
            //
            foreach (var item in lstDir)
            {
                //Grava o XML no banco
                XmlDocument xmlSave = new XmlDocument();
                xmlSave.Load(item);
                lstAux = item.Replace(local, "").Split('_');

                if (lstAux.Length == 3)
                {
                    classe = lstAux[0];
                    server = lstAux[1];
                    geracao = lstAux[2].Substring(0, (lstAux[2].Length - 4));
                    //
                    retorno = SqlServer.AuditXML.Gravar(classe, server, geracao, xmlSave);
                    //
                    if (retorno)
                    {
                        if (File.Exists(item))
                        {
                            File.Delete(item);
                            Console.WriteLine("\n|----------------------------------------------------===================|");
                            Console.WriteLine("|Info| Server - " + server + " | Classe - " + classe + " | Inserido na base de dados - Arquivo - " + item.Replace(local, ""));
                        }                    
                    }
                }
            }
            //
            Console.WriteLine("\n|----------------------------------------------------===================|");
            Console.WriteLine("|Info| DS_AuditXML_cmd -  Final do Processamento - | " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            Console.WriteLine("|----------------------------------------------------===================|");
        }


        public static string[] retVetor(string src, bool ehSEL)
        {
            FileStream fs = new FileStream(src, FileMode.Open, FileAccess.Read);
            string[] vetor = { "." };
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlNodeList xmlnode;
            int i = 0;
            int j = 0;
            string CHK, SEL, VALUE;
            //Carrega XML...
            xmldoc.Load(fs);
            xmlnode = xmldoc.GetElementsByTagName("item");
            for (i = 0; i <= xmlnode.Count - 1; i++)
            {
                xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                //
                CHK = xmlnode[i].ChildNodes.Item(0).InnerText.Trim();
                SEL = xmlnode[i].ChildNodes.Item(1).InnerText.Trim();
                VALUE = xmlnode[i].ChildNodes.Item(2).InnerText.Trim();
                //
                if (CHK == "1")
                {
                    if (ehSEL) { vetor[j] = SEL; }
                    else { vetor[j] = VALUE; }
                    //
                    j++;
                    Array.Resize(ref vetor, j + 1);
                }
            }            
            if (vetor != null)
            {
                Array.Resize(ref vetor, j);
                return vetor;
            }
            else
            { return null; }
        }


        static bool setArqconnServer()
        {
            //testa o usuario de conn ao Servidor para Copia dos XMLs...
            //
            string args_enc;
            using (StreamReader reader = new StreamReader(ArqconnServer))
            { args_enc = reader.ReadLine(); }
            //
            //descripta args_enc
            string args_desc = MPSfwk.Crypt.Decrypt(args_enc, bytes);
            //
            // Separa os parametros decriptados para setar os comandos
            connServer = args_desc.Replace("_","\\").Split(' ');

            //-----------------------------------------------------------
            //testa o usuario de connM ao Servidor para Copia dos XMLs...
            //
            using (StreamReader reader = new StreamReader(ArqcoMvServer))
            { args_enc = reader.ReadLine(); }
            //
            //descripta args_enc
            args_desc = MPSfwk.Crypt.Decrypt(args_enc, bytes);
            //
            // Separa os parametros decriptados para setar os comandos
            coMvServer = args_desc.Replace("_", "\\").Split(' ');

            // Teste se alimentou corretamente os param's...
            if ((connServer.Length < 2) || (coMvServer.Length < 2))
            {
                Console.WriteLine("\n|----------------------------------------------------===================|");
                Console.WriteLine("|!| ArqconnServer - Argumentos invalidos! Verifique o arquivo de Configuracao. - " + ArqconnServer);
                return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            try
            {
                bytes = ASCIIEncoding.ASCII.GetBytes(System.Configuration.ConfigurationManager.AppSettings["validationKey"].Substring(0, 8)); // ("AAA5998D")
                string[] param;
                string[] servers = { "." };
                string[] classes = { "." };
                string[] cls_SEL = { "." };
                string userlocal = "";
                string login = "";
                string ziparq = "";
                string dthrmmss = "";
                dthrmmss = _dthrmmss.ToString("yyyyMMddHHmmss");
                string path = System.IO.Directory.GetCurrentDirectory() + "\\XML_Data";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                ziparq = path + "\\Geracao_" + dthrmmss + ".zip";
                //
                Console.WriteLine("\n|----------------------------------------------------===================|");
                Console.WriteLine("|Info| DS_AuditXML_cmd - Inicio do Processamento - | " + _dthrmmss.ToString("dd/MM/yyyy HH:mm:ss"));
                Console.WriteLine("|Info| _loc.Copia - " + PathServer);
                Console.WriteLine("|Info| _loc.Move  - " + PathMoveServer);
                Console.WriteLine("|Info| _arq.Copia - " + ArqconnServer);
                Console.WriteLine("|Info| _arq.Move  - " + ArqcoMvServer);
                Console.WriteLine("|Info| _arq.ZIP   - " + "XML_Data\\Geracao_" + dthrmmss + ".zip");
                Console.WriteLine("|Info| Parametros : ");
                int cont = 1;
                foreach (string s in args)
                {
                    Console.WriteLine(String.Format("|Info| param [{0, 2}] = {1}", cont, s)); 
                    cont++;
                }
                Console.WriteLine("|----------------------------------------------------===================|");
                //

                if ((args == null) || (!setArqconnServer()))
                {
                    Console.WriteLine("\n|----------------------------------------------------===================|");
                    Console.WriteLine("|!| Argumentos invalidos! Tente novamente passando-os corretamente.");
                }
                else
                {
                    // Testa os param's e chama a rotina apropriada...
                    //
                    if      ((args.Length == 2) && (args[0].ToUpper() == "-G")) { GravaXML(args[1]); }
                    else if ((args.Length == 2) && (args[0].ToUpper() == "-M")) 
                    {
                        MoveSrvXML(true, args[1], "*.xml");
                        MoveSrvXML(true, args[1], "*.log");
                        //
                        Console.WriteLine("\n|----------------------------------------------------===================|");
                        Console.WriteLine("|Info| DS_AuditXML_cmd -  Final do Processamento - | " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        Console.WriteLine("|----------------------------------------------------===================|");
                    }
                    else if ((args.Length == 2) && (args[0].ToUpper() == "-L")) 
                    {
                        MoveSrvXML(false, args[1], "*.xml");
                        MoveSrvXML(false, args[1], "*.log");
                        //
                        Console.WriteLine("\n|----------------------------------------------------===================|");
                        Console.WriteLine("|Info| DS_AuditXML_cmd -  Final do Processamento - | " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        Console.WriteLine("|----------------------------------------------------===================|");
                    }
                    //
                    else
                    {
                        if (args[0].ToUpper() == "-H")
                        {
                            Console.WriteLine("|?| DS_AuditXML_cmd   - Aplicativo para o Controle dos arquivos XML's  ");
                            Console.WriteLine("|?|                     (fotos dos Servidores, inclusao Base de Dados)");
                            Console.WriteLine("\n|?| DS_AUDITXML_CMD.exe [-h]");
                            Console.WriteLine("|?|                     [-g] [pastaGravaXML]");
                            Console.WriteLine("|?|                     [-m] [pastaMoveXML]");
                            Console.WriteLine("|?|                     [-l] [pastaLocalXML]");
                            Console.WriteLine("|?|     <ArqDomUsrPwd>  [-u|-i|-x] [param1] [param2] [param3]");
                            Console.WriteLine("\n|?|  [pastaGravaXML]    Pasta contendo XMLs p/ inclusao Base de Dados.");
                            Console.WriteLine("|?|  <ArqDomUsrPwd>    Arquivo Criptografado com Dominio/Usuario/Senha.");
                            Console.WriteLine("|?|  [pastaMoveXML]     Pasta contendo XMLs p/ mover Local para Remoto.");
                            Console.WriteLine("|?|  [pastaLocalXML]    Pasta contendo XMLs p/ mover Remoto para Local.");
                            Console.WriteLine("|?|  [param1]           Lista de Servidores para Audit separados por (;).");
                            Console.WriteLine("|?|  [param2]           Lista de Classes para Audit sep. por (;) esp.(@).");
                            Console.WriteLine("|?|  [param3]           Conteudo Classes para Audit sep. por (;) esp.(@).");
                            Console.WriteLine("\n|?|  Lembrete :         Na pasta do app, devem estar os arquivos:");
                            Console.WriteLine("|?|  ----------         [ListHosts.xml] / [ListClasses.xml]");
                            Console.WriteLine("|?|                     Os Param's abaixo, estao no arquivo de config.");
                            Console.WriteLine("|?|  _loc.Copia         Caminho Remoto para copias backup dos XMLs.");
                            Console.WriteLine("|?|  _loc.Move          Caminho Remoto para mover XMLs para Local(PC).");
                            Console.WriteLine("|?|  _arq.Copia         Arquivo (criptografado) p/ Manipulacao de XML,");
                            Console.WriteLine("|?|  _arq.Move          com o usuario p/ acesso Remoto aos Servidores.");
                            Console.WriteLine("|?|  _arq.ZIP           Arquivo ZIP contendo os XML's gravados no DB.");
                        }
                        else
                        {
                            // Le a string criptografada no arquivo de entrada...
                            //
                            string arqParam = args[0];
                            string args_enc = "";
                            arqParam = arqParam.Replace("\r", "");
                            arqParam = arqParam.Replace("\n", "");
                            arqParam = arqParam.Replace("@", " ");
                            using (StreamReader reader = new StreamReader(arqParam))
                            { args_enc = reader.ReadLine(); }

                            // Descripta args_enc e separa os parametros para os comandos
                            //
                            string args_desc = MPSfwk.Crypt.Decrypt(args_enc, bytes);
                            param = args_desc.Split(' ');

                            if (param.Length < 2)
                            {
                                Console.WriteLine("\n|----------------------------------------------------===================|");
                                Console.WriteLine("|!| Argumentos invalidos! Tente novamente passando-os corretamente.");
                            }
                            else
                            {
                                //-----------------------------------------------------
                                //MPS - 08/OUT - Add servers/classes nos param... - Fim
                                //=====================================================
                                if (args.Length == 1)
                                {
                                    userlocal = param[0].Replace("_", "\\");
                                    login = param[1];
                                    servers = retVetor("ListHosts.xml", false);
                                    classes = retVetor("ListClasses.xml", false);
                                    cls_SEL = retVetor("ListClasses.xml", true);

                                    // Testa e faz as conexoes ao Servidor Remoto (copia)...
                                    //
                                    if (MPSfwk.WMI.netToServer(connServer[0], connServer[1], PathServer, 8000))
                                    {
                                        // Loop Server / Classes...
                                        //
                                        foreach (string s in servers)
                                        { for (int i = 0; i < classes.Length; i++) { GeraXML(s, classes[i], cls_SEL[i], userlocal, login); } }

                                        // Zipa os arquivos... / deleta os arquivos, caso tenha os zipados...
                                        //
                                        if (MPSfwk.WMI.zipListArq(xmls[0], xmls, ziparq)) { delxmls(); }
                                        //
                                        Console.WriteLine("\n|----------------------------------------------------===================|");
                                        Console.WriteLine("|Info| DS_AuditXML_cmd -  Final do Processamento - | " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                        Console.WriteLine("|----------------------------------------------------===================|");
                                        //
                                        MPSfwk.WMI.delToServer(PathServer, 8000);
                                    }
                                    //
                                } //---> MPS - 08/OUT
                                else
                                {

                                    //=====================================================
                                    //MPS - 08/OUT - Add servers/classes nos param... - Ini
                                    //-----------------------------------------------------
                                    if (args.Length < 5) // tem somente Servers
                                    {
                                        Console.WriteLine("|!| Argumentos invalidos - Tipo, Servidor(es), Classe(s) e classe(s) SEL, nao informados!");
                                        return;
                                    }
                                    else if (args.Length == 5) // tem -U + lstserver + lstclasses + lstSEL
                                    {
                                        servers = args[2].Split(';');
                                        classes = args[3].Replace("@", " ").Split(';');
                                        cls_SEL = args[4].Replace("-ALL-", "*").Replace("-all-", "*").Replace("@", " ").Split(';');
                                        //
                                        if (classes.Length != cls_SEL.Length)
                                        {
                                            Console.WriteLine("|!| Argumentos invalidos - Numero de classes diferente das de classes SEL!");
                                            return;
                                        }
                                        //
                                        if ((args[1].ToUpper() != "-U") && (args[1].ToUpper() != "-I") && (args[1].ToUpper() != "-X"))
                                        {
                                            Console.WriteLine("|!| Argumentos invalidos - Tipo informado invalido! (Validos: [-u] [-i])");
                                            return;
                                        }
                                        Console.WriteLine("\n|Info| Tipo       = " + args[1]);
                                        //
                                        foreach (string s in servers)
                                        {
                                            for (int i = 0; i < classes.Length; i++)
                                            {
                                                Console.WriteLine("\n|Info| Server     = " + s);
                                                Console.WriteLine("\n|Info| Classe     = " + classes[i]);
                                                Console.WriteLine("\n|Info| Classe_SEL = " + cls_SEL[i]);
                                                Console.WriteLine("\n|Info| Usr        = " + param[0].Replace("_","\\"));
                                                Console.WriteLine("\n|Info| Pwd        = " + param[1]);

                                                //chama a proc...
                                                if (args[1].ToUpper() == "-U") { updSrvCls(true, s, classes[i], cls_SEL[i], param[0].Replace("_", "\\"), param[1]); }
                                                if (args[1].ToUpper() == "-X") { updSrvCls(false, s, classes[i], cls_SEL[i], param[0].Replace("_", "\\"), param[1]); }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception eg)
            {
                Console.WriteLine("\n|----------------------------------------------------===================|");
                Console.WriteLine("|!| Ocorreu um Erro Generico - " + eg.Message);
                Console.WriteLine("\n|----------------------------------------------------===================|");
            }
        }

        public static void delxmls()
        {
            foreach (string s in xmls)
            {
                if (File.Exists(s)) { File.Delete(s); }
            }
        }

//==========================================================================================================
//MPS - 09/OUT_12/DEZ - Chamadas das novas procs...
//----------------------------------------------------------------------------------------------------------
        public static int updSrvCls(bool ehDel, string server, string classe, string cls_SEL, string user, string pass)
        {
            // Chama as rotinas para a geração do DT...
            DataTable dt1;
            string geracao = DateTime.Now.ToString("yyyyMMddhhmmss");
            string arq = classe + "_" + server + "_" + geracao + ".xml";
            string path = System.IO.Directory.GetCurrentDirectory() + "\\XML_Data";

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                dt1 = MPSfwk.WMI.dtlistaClasse(classe, cls_SEL, server, user, pass);
                if (dt1 == null)
                {
                    Console.WriteLine("\n|----------------------------------------------------===================|");
                    Console.WriteLine("|!| Server - " + server + " | Classe - " + classe + " | Ocorreu um erro de Conexao! Tente outros parametros.");
                }
                else
                {
                    dt1.WriteXml(path + "\\" + arq);

                    //Grava na base de dados o arquivo acima...
                    XmlDocument xmlSave = new XmlDocument();
                    xmlSave.Load(path + "\\" + arq);

                    if (ehDel)
                    {
                        bool retorno = SqlServer.AuditXML.Atualizar(classe, server, geracao, xmlSave);
                        //
                        if (retorno)
                        {
                            if (File.Exists(path + "\\" + arq))
                            {
                                File.Delete(path + "\\" + arq);
                                Console.WriteLine("\n|----------------------------------------------------===================|");
                                Console.WriteLine("|Info| Server - " + server + " | Classe - " + classe + " | Inserido na base de dados - Arquivo - " + (path + "\\" + arq));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n|----------------------------------------------------===================|");
                        Console.WriteLine("|Info| Server - " + server + " | Classe - " + classe + " | XML criado localmente - Arquivo - " + (path + "\\" + arq));
                    }
                }
            }
            catch (Exception eg)
            {
                if (File.Exists(path + "\\" + arq)) { File.Delete(path + "\\" + arq); }
                //
                Console.WriteLine("\n|----------------------------------------------------===================|");
                Console.WriteLine("|!| Server - " + server + " | Classe - " + classe + " | Ocorreu um erro - " + eg.Message);
            }

            return 0;
        }



        //---------------------------------------------------------------
        //-64bit//using (ZipFile zip = new ZipFile())
        //-64bit//{
        //-64bit//            zip.FlattenFoldersOnExtract = true;
        //-64bit//            zip.StatusMessageTextWriter = System.Console.Out;
        //-64bit//            zip.AddFile(xmls[xmls.Length - 1], "\\"); // recurses subdirectories
        //-64bit//    zip.Save(ziparq);
        //-64bit//}
        
        
        //*** Convertido para Framework .net2 ***//using System.Xml.Linq;
        //public static string[] retVetor(string src, bool ehSEL)
        //{
        //    string[] vetor = { "." };
        //    int i = 0;
        //    //
        //    XDocument lbSrc = XDocument.Load(src);
        //    foreach (XElement item in lbSrc.Descendants("item"))
        //    {
        //        if (item.Element("CHK").Value == "1")
        //        {
        //            if (ehSEL)  { vetor[i] = item.Element("SEL").Value; }
        //            else        { vetor[i] = item.Element("VALUE").Value; }
        //            //
        //            i++;
        //            Array.Resize(ref vetor, i+1);
        //        }
        //    }
        //    if (vetor != null)
        //    {
        //        Array.Resize(ref vetor, i);
        //        return vetor;
        //    }
        //    else
        //    { return null; }
        //}
    }
}
