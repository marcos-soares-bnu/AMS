using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
using System.Data;
using MPSfwk.Model;

namespace SqlServer
{
    public static class AuditXML
    {

        public static List<Audits> lstAudits(MPSfwk.Model.Audits aud_param, int tipLista, string ordBY)
        {
            SqlCommand comm = new SqlCommand();
            if (tipLista == 0)
            {
                comm.CommandText = @"SELECT
                                            ServerName,
                                            ClasseName,
                                            GeracaoDate,
                                            UltimaAcaoDate
                                       FROM ASPNETDB.dbo.ds_audit_xml" + MontaWhere(aud_param) +
                                    " ORDER BY GeracaoDate DESC";
            }
            else if (tipLista == 1)
            {
                comm.CommandText = @"SELECT
                                            ServerName,
                                            ClasseName,
                                            GeracaoDate,
                                            convert(datetime,stuff(stuff(stuff(GeracaoDate, 9, 0, ' '), 12, 0, ':'), 15, 0, ':')) ConvGeracaoDate 
                                       FROM ASPNETDB.dbo.ds_audit_xml" + MontaWhere(aud_param) +
                                    " GROUP BY ServerName, ClasseName, GeracaoDate" +
                                    " ORDER BY convert(datetime,stuff(stuff(stuff(GeracaoDate, 9, 0, ' '), 12, 0, ':'), 15, 0, ':')) " + ordBY;
            }
            else if (tipLista == 2)
            {
                comm.CommandText = @"( SELECT DISTINCT ServerName, ClasseName, MAX(GeracaoDate)
                                         FROM ASPNETDB.dbo.ds_audit_xml 
                                        WHERE GeracaoDate like '%" + aud_param.DTGeracaoFim + "%'" +
                                    "   GROUP BY ServerName, ClasseName, GeracaoDate " +
                                    ")  ORDER BY ServerName, ClasseName " + ordBY;
            }
            else if (tipLista == 3)
            {
                comm.CommandText = @"SELECT
                                            ServerName,
                                            ClasseName,
                                            GeracaoDate,
                                            UltimaAcaoDate
                                       FROM ASPNETDB.dbo.vw_ds_audit_xml_Ativos7days
                                      WHERE GeracaoDate like '" + aud_param.IDGeracao + "%' " +
                                    " ORDER BY ClasseName ASC, ServerName ASC";
            }
            else if (tipLista == 4)
            {
                comm.CommandText = @"SELECT
                                            VALUE + ' ' + SEL
                                       FROM ASPNETDB.dbo.ds_ListHosts
                                      WHERE CHK = '1'
                                        AND VALUE NOT IN (SELECT DISTINCT ServerName
                                                            FROM ASPNETDB.dbo.vw_ds_audit_xml_Ativos7days
                                                           WHERE GeracaoDate like '" + aud_param.IDGeracao + "%')";
            }


            comm.CommandType = CommandType.Text;

            using (SqlDataReader dataReader = SQLServer.DataAccess.ExecuteReader(comm))
            {
                List<Audits> _lst = new List<Audits>();
                while (dataReader.Read())
                {
                    Audits aud = new Audits();
                    aud.IDServer = dataReader.GetString(0);
                    if (tipLista != 4)
                    {
                        aud.IDClasse = dataReader.GetString(1);
                        aud.IDGeracao = dataReader.GetString(2);
                    }
                    if ((tipLista == 0) || (tipLista == 3))
                    { aud.DataUltimaAcao = dataReader.GetDateTime(3); }
                    else if (tipLista == 1)
                    { aud.CVGeracao = dataReader.GetDateTime(3).ToString(); }
                    _lst.Add(aud);
                }
                return _lst;
            }
        }

        private static string MontaWhere(MPSfwk.Model.Audits aud_param)
        {
            string Server = "  AND ServerName IN ({0}) ";
            string Classe = "  AND ClasseName IN ({0}) ";
            string Geracao = "  AND        convert(datetime,stuff(stuff(stuff(GeracaoDate, 9, 0, ' '), 12, 0, ':'), 15, 0, ':'))" +
                             "    BETWEEN  convert(datetime,stuff(stuff(stuff('{0}', 9, 0, ' '), 12, 0, ':'), 15, 0, ':'))" +
                             "      AND    convert(datetime,stuff(stuff(stuff('{1}', 9, 0, ' '), 12, 0, ':'), 15, 0, ':'))";

            System.Text.StringBuilder sbWhere = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(aud_param.IDServer))
                sbWhere.AppendFormat(Server, aud_param.IDServer.ToString());

            if (!string.IsNullOrEmpty(aud_param.IDClasse))
                sbWhere.AppendFormat(Classe, aud_param.IDClasse.ToString());

            if ((!string.IsNullOrEmpty(aud_param.DTGeracaoIni)) && 
                (!string.IsNullOrEmpty(aud_param.DTGeracaoFim)) &&
                (aud_param.DTGeracaoIni != "000000")            && 
                (aud_param.DTGeracaoFim != "235900") 
               )
                sbWhere.AppendFormat(Geracao, aud_param.DTGeracaoIni.ToString(), aud_param.DTGeracaoFim.ToString());

            if (sbWhere.Length > 0)
            {
                sbWhere.Remove(0, 5);
                sbWhere.Insert(1, " Where ", 1);
            }
            else
            {
                return string.Empty;
            }

            return sbWhere.ToString();
        }


        public static Boolean Gravar(string classe, string server, string geracao, XmlDocument xmlToSave)
        {
            SqlCommand comm = new SqlCommand();
            comm.CommandText = @"[ds_auditxml_InsertXML]";
            comm.CommandType = CommandType.StoredProcedure;

            //Transformar de XML para String
            String xml = xmlToSave.OuterXml;

            //Tipando o parâmetro para XML.
            SqlParameter param = new SqlParameter("@XmlFile", SqlDbType.Xml);
            param.Value = xml;
            comm.Parameters.Add(param);
            //
            comm.Parameters.Add(new SqlParameter("@ClasseName", classe));
            comm.Parameters.Add(new SqlParameter("@ServerName", server));
            comm.Parameters.Add(new SqlParameter("@GeracaoDate", geracao));
            //
            SQLServer.DataAccess.ExecuteNonQuery(comm);

            return true;
        }


        public static Boolean Atualizar(string classe, string server, string geracao, XmlDocument xmlToSave)
        {
            SqlCommand comm = new SqlCommand();
            comm.CommandText = @"[ds_auditxml_UpdateXML]";
            comm.CommandType = CommandType.StoredProcedure;

            //Transformar de XML para String
            String xml = xmlToSave.OuterXml;

            //Tipando o parâmetro para XML.
            SqlParameter param = new SqlParameter("@XmlFile", SqlDbType.Xml);
            param.Value = xml;
            comm.Parameters.Add(param);
            //
            comm.Parameters.Add(new SqlParameter("@ClasseName", classe));
            comm.Parameters.Add(new SqlParameter("@ServerName", server));
            comm.Parameters.Add(new SqlParameter("@GeracaoDate", geracao));
            //
            SQLServer.DataAccess.ExecuteNonQuery(comm);

            return true;

        } //MPS Add upd 08/10...


        public static XmlDocument LerXML(string classe, string server, string geracao)
        {
            XmlDocument xml = new XmlDocument();
            //
            String sql = @"SELECT XmlFile 
                             FROM dbo.ds_audit_xml
                            WHERE ClasseName  = '{0}'
                              AND ServerName  = '{1}'
                              AND GeracaoDate LIKE '%{2}%'
                         ORDER BY GeracaoDate DESC";

            SqlCommand comm = new SqlCommand();
            comm.CommandText = string.Format(sql, classe, server, geracao);
            comm.CommandType = CommandType.Text;

            string xmlDb = (String)SQLServer.DataAccess.ExecuteScalar(comm);

            if (!string.IsNullOrEmpty(xmlDb))
            {
                xml.LoadXml(xmlDb);
            }

            return xml;
        }

    }
}
