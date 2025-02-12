﻿using TestApiJWT.Models;

namespace TestApiJWT.Services
{
    public interface IAuthService
    {
         Task<AuthModel> RegisterAsync(RegisterModel model);
         Task<AuthModel> GetTokenAsync(TokenRequestModel model);
         Task<string> AddRoleAsync(AddRoleModel model);
        Task<AuthModel> RefreshtokenAsync(string refreshToken);
        Task<bool> RevoketokenAsync(string refreshToken);
    }
}
