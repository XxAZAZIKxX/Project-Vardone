using System.IO;
using System.Text;
using Newtonsoft.Json;
using VardoneEntities.Models.GeneralModels.Users;

namespace Vardone.Core
{
    public abstract class JsonTokenWorker
    {
        private static readonly string FilePath = MainWindow.PATH + @"\token.json";
        public static string GetToken() => !File.Exists(FilePath) ? null : Encoding.Default.GetString(File.ReadAllBytes(FilePath));

        public static void SetToken(string token)
        {
            if (!File.Exists(FilePath)) File.Create(FilePath).Close();
            File.WriteAllBytes(FilePath, Encoding.Default.GetBytes(token??""));
        }
    }
}