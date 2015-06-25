using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace MPSfwk
{
    public static class SharePoint
    {

        public static string UploadAttachment(string UserName, string Password, string filePath, string listName, string listItemId)
        {
            ListsWebSrv.Lists listService = retListWebSrv(UserName, Password);
            //
            var fileName = System.IO.Path.GetFileName(filePath);
            var fileContent = System.IO.File.ReadAllBytes(filePath);
            return listService.AddAttachment(listName, listItemId, fileName, fileContent);
        }


        private static ListsWebSrv.Lists retListWebSrv(string UserName, string Password)
        {
            // Seta o nome e atributos da lista...
            //
            ListsWebSrv.Lists listService = new ListsWebSrv.Lists();
            listService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            listService.Url = "http://sts237wk8/sites/SDU/_vti_bin/Lists.asmx";
            listService.Credentials = new System.Net.NetworkCredential(UserName.Substring((UserName.IndexOf("\\") + 1)), Password, UserName.Substring(0, UserName.IndexOf("\\")));
            //
            return listService;
        }


        public static string updListaSDC(string lstSDC, string regs, string UserName, string Password)
        {
            ListsWebSrv.Lists listService = retListWebSrv(UserName, Password);
            //
            System.Xml.XmlNode ndListView = listService.GetListAndView(lstSDC, "");
            string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;
            string strViewID = ndListView.ChildNodes[1].Attributes["Name"].Value;
            //
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlElement batchElement = doc.CreateElement("Batch");
            batchElement.SetAttribute("OnError", "Continue");
            batchElement.SetAttribute("ListVersion", "1");
            batchElement.SetAttribute("ViewName", strViewID);

            batchElement.InnerXml = regs; //"<Method ID='0' Cmd='New'><Field Name='Title'>Added item</Field></Method>";

            try
            {
                XmlNode nodeListItems = listService.UpdateListItems(strListID, batchElement);
                var owsID = nodeListItems.SelectSingleNode("//@ows_ID").Value;
                //
                return owsID;
            }
            catch (Exception)
            {
                return "-1";
            }

        }



        public static DataTable retDtListaSDC(string lstSDC, string qryINN, string UserName, string Password)
        {
            try
            {
                ListsWebSrv.Lists listService = retListWebSrv(UserName, Password);
                //
                System.Xml.XmlNode ndListView = listService.GetListAndView(lstSDC, "");
                string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;
                string strViewID = ndListView.ChildNodes[1].Attributes["Name"].Value;
                //
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                string rowLimit = "25000";
                System.Xml.XmlElement query = xmlDoc.CreateElement("Query");
                System.Xml.XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
                System.Xml.XmlElement queryOptions = xmlDoc.CreateElement("QueryOptions");

                // Seta os filtros enviados pelas interfaces...
                // 
                if (qryINN == "")
                    query.InnerXml = "<Where><Or><IsNull> <FieldRef Name='Title' /></IsNull><IsNotNull><FieldRef Name='Title' /></IsNotNull></Or></Where>";
                else
                    query.InnerXml = qryINN;

                // Chama a rotina para converter os campos numa DataTable...
                //
                XmlNode nodeListItems = listService.GetListItems(lstSDC, strViewID, query, viewFields, rowLimit, queryOptions, null);
                return XmlNodeToDataTable(nodeListItems);
            }
            catch (Exception e)
            {
                Console.WriteLine("|!| retDtListaSDC Error - " + e.Message);
                return null;
            }

        } //OK MPS - 04/11/2014


        public static DataTable XmlNodeToDataTable(XmlNode myXmlNodeObject)
        {
            //Create a DataSet To Bind To
            DataSet ds = new DataSet();
            ds.Tables.Add("XmlDataSet");

            //Preenche a tabela com o conteudo do BAT...
            DataTable dt = new DataTable("XmlDataSet");

            //Get Column Names as String Array
            XmlDocument XMLDoc = new XmlDocument();
            XMLDoc.LoadXml(myXmlNodeObject.InnerXml);
            int colCount = Convert.ToInt32(XMLDoc.ChildNodes.Item(0).Attributes[0].Value);
            bool ehCabec = true;

            //Get Data Row By Row to populate the DataSet.Rows
            foreach (XmlNode RowNode in XMLDoc.ChildNodes.Item(0).ChildNodes)
            {
                if (ehCabec) //Preenche Cabec...
                {
                    for (int i = 0; i < RowNode.Attributes.Count; i++)
                    {
                        try { dt.Columns.Add(RowNode.Attributes[i].Name); }
                        catch { continue; }
                    }
                    ehCabec = false;
                }

                DataRow dr = dt.NewRow();
                //Preenche Cabec...
                for (int i = 0; i < RowNode.Attributes.Count; i++)
                {
                    try { dr[RowNode.Attributes[i].Name] = RowNode.Attributes[i].Value; }
                    catch { continue; }
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }


    }
}
