using System;
using System.Collections.Generic;
using System.Text;

namespace MPSfwk.Model
{
    public class Server
    {
        private string IPhost;
        private string usuario;
        private string senha;

        public string IPHOST { get { return IPhost; } set { IPhost = value; } }
        public string USUARIO { get { return usuario; } set { usuario = value; } }
        public string SENHA { get { return senha; } set { senha = value; } }
    }
}

