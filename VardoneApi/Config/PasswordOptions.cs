using System;
using System.Text;

namespace VardoneApi.Config
{
    internal static class PasswordOptions
    {
        public const string KEY = "DRhOiWfF&!m(ib+7SB$oGqS4LK145)skJ$KQeZ-FtAtVQ~tKbItT1GLvyT^XX6!$=BtC23G5d0yglj1poy-xi6T_I#bq6xmU2#SNQUe1sNkJr~&cnzTg6D%y2u3&6OnP0=g{_!459b9Jz)ie-n(&3tSJzir$pLJToi3r{QJ9yYi*Gwpi+IS9S7K3NCVE+$NO$9u_Bu65MBTTtEs&8704lY0i_D0WDMvP)Wh$lfM_E69laHm8+iV5iks&K$z414S";
        public static readonly byte[] IV = GetIV();

        private static byte[] GetIV()
        {
            var iv = new byte[16];
            Array.Copy(Encoding.ASCII.GetBytes(KEY), iv, 16);
            return iv;
        }
    }
}