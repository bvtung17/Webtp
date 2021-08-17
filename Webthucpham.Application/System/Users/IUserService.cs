 using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Webthucpham.ViewModels.Common;
using Webthucpham.ViewModels.System.Users;

namespace Webthucpham.Application.System.Users
{
    public interface IUserService
    {
        Task<string> Authencate(LoginRequest request);
        Task<bool> Register(RegisterRequest request);

        Task<PagedResult<UserVm>> GetUsersPaging(GetUserPagingRequest request); 

    }
}
