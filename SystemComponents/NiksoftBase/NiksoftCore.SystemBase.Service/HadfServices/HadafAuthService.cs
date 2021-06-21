using HadafAuthentication;
using HadafServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiksoftCore.SystemBase.Service
{
    public interface IHadafAuthService
    {
        Task<string> GetToken();
        Task<string> GetKey(string NCode, string Mobile, string Comment = "");
        Task<bool> CheckAuthService(string NCode, string Mobile, string Comment = "");
        Task<string> RegisterUser(string Key, string PublicCode, string FirstName, string LastName, string Gender, string MobileNumber);
    }

    public class HadafAuthService : IHadafAuthService
    {
        private readonly HadafServicesSoapClient service;

        public HadafAuthService()
        {
            service = new HadafServicesSoapClient(HadafServicesSoapClient.EndpointConfiguration.HadafServicesSoap);
        }

        public async Task<string> GetToken()
        {
            var token = await service.GetTokenAsync();
            return token.Body.GetTokenResult;
        }

        public async Task<string> GetKey(string NCode, string Mobile, string Comment = "")
        {
            var token = await GetToken();
            var authKey = new AuthenticationKey();
            return authKey.GenerateKey(NCode, Mobile, token, Comment);
        }

        public async Task<bool> CheckAuthService(string NCode, string Mobile, string Comment = "")
        {
            var key = await GetKey(NCode, Mobile, Comment);
            var authRes = await service.AuthenticateAsync(key);
            return bool.Parse(authRes.Body.AuthenticateResult);
        }

        public async Task<string> RegisterUser(string Key, string PublicCode, string FirstName, string LastName, string Gender, string MobileNumber)
        {
            var response = await service.RegisterUserAsync(Key, PublicCode, FirstName, LastName, Gender, MobileNumber);
            return response.Body.RegisterUserResult;
        }




    }
}
