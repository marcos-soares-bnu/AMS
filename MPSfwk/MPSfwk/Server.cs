using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace MPSfwk
{
    public class Server
    {
        public Model.Server GerarObjeto(string srv, string usr, string psw)
        {

            Model.Server s = new MPSfwk.Model.Server();

            if (srv != "")
            {
                s.IPHOST = srv;
            }
            if (usr != "")
            {
                s.USUARIO = usr;
            }
            if (psw != "")
            {
                s.SENHA = psw;
            }

            return s;
        }
    }
}
