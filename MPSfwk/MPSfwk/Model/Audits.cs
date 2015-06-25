using System;
using System.Collections.Generic;
using System.Text;

namespace MPSfwk.Model
{
    public class Audits : IDisposable
    {
        private String idServer;
        public String IDServer
        {
            get { return idServer; }
            set { idServer = value; }
        }

        private String idClasse;
        public String IDClasse
        {
            get { return idClasse; }
            set { idClasse = value; }
        }

//MPS - 26/SET - ini
        private String dtGeracaoIni;
        public String DTGeracaoIni
        {
            get { return dtGeracaoIni; }
            set { dtGeracaoIni = value; }
        }
        private String dtGeracaoFim;
        public String DTGeracaoFim
        {
            get { return dtGeracaoFim; }
            set { dtGeracaoFim = value; }
        }
        private String cvGeracao;
        public String CVGeracao
        {
            get { return cvGeracao; }
            set { cvGeracao = value; }
        }
//MPS - 26/SET - fim

        private String idGeracao;
        public String IDGeracao
        {
            get { return idGeracao; }
            set { idGeracao = value; }
        }

        private DateTime dataUltimaAcao;
        public DateTime DataUltimaAcao
        {
            get { return dataUltimaAcao; }
            set { dataUltimaAcao = value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.ReRegisterForFinalize(this);
        }

        #endregion
    }
}
