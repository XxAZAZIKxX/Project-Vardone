using System.IO;
using System.Text;
using Newtonsoft.Json;
using VardoneEntities.Models.GeneralModels.Users;

namespace Vardone.Core
{
    public abstract class JsonTokenWorker
    {
        private static readonly string DllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        private static readonly string Path = System.IO.Path.GetDirectoryName(DllPath);
        private static readonly string FilePath = Path + @"\token.json";
        public static UserTokenModel GetToken()
        {
            if (!System.IO.File.Exists(FilePath)) return null;
            var s = Encoding.Default.GetString(File.ReadAllBytes(FilePath));
            return JsonConvert.DeserializeObject<UserTokenModel>(s);
        }
        public static void SetToken(UserTokenModel token)
        {
            if (!File.Exists(FilePath)) File.Create(FilePath).Close();
            File.WriteAllBytes(FilePath, Encoding.Default.GetBytes(JsonConvert.SerializeObject(token)));
        }
    }
}