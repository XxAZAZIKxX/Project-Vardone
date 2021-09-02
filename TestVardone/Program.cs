using System;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core.Client;
using VardoneLibrary.Core.Client.Base;
using VardoneLibrary.VardoneEvents;

namespace TestVardone
{
    internal static class Program
    {
        private static void Main()
        {
            VardoneBaseApi.RegisterUser(new RegisterUserModel
            {
                Username = "test_user",
                Email = "q",
                Password = "q"
            });
            var client = new VardoneClient(VardoneBaseApi.GetUserToken("q", "q"));


            Console.ReadKey(true);
        }
    }
}
