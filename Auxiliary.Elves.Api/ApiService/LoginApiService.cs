using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.ApiService
{
    public class LoginApiService : ILoginApiService
    {
        public ApiResponse<string> Login(AccountRequestDto accountRequest)
        {
            throw new NotImplementedException();
        }

        public bool Register(string userFeatureCode)
        {
            throw new NotImplementedException();
        }
    }
}
