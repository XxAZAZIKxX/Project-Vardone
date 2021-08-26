using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VardoneApi
{
    public static class TokenOptions
    {
        public const string ISSUER = "VardoneApi_C#";
        public const string AUDIENCE = "Vardone_WPF";
        private const string Key = "YRiGt^MQ3+5{4o#Zrmg(wROSiOn=Ujg.#=o18ERmdZaJ4W7T&8*~8g+s!8E6=#v(lOIAUzG-6Kr№6#JB}phg*G2Nu#B]FCSx-Hik~n№]?!EUL%aQa+uEsFo!sbnke!U#?[Z9cEk$+p3NuaS=75mJChlf3dPpR]IK%:QZ2(MeO=1Du-a!8KsPF^EGrIe~G%RE-,3nV2XIkiF[RMU-ZKf:.,*eO@N~7$Gt(|K9yajy*cd]Z=3f$-№OAI~GaHqL%s9)";
        public const int LIFETIME = 60;
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.ASCII.GetBytes(Key));
    }
}