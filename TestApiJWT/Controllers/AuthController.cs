using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestApiJWT.Models;
using TestApiJWT.Services;

namespace TestApiJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(model);

            if(!result.IsAuthenticated)
                return BadRequest(result.Message);
            
           SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            

            return Ok(new {token = result.Token , /*expiresOn = result.ExpiresOn*/});
        }
        [HttpPost("GetToken")]
        public async Task<IActionResult> GetTokenAsync([FromBody]TokenRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.GetTokenAsync(model);

            if(!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }

            return Ok(new {token = result.Token , /*expiresOn = result.ExpiresOn*/});
        } 
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRoleAsync([FromBody]AddRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.AddRoleAsync(model);

            if(string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _authService.RefreshtokenAsync(refreshToken);

            if(!result.IsAuthenticated)
                return BadRequest(result);
            SetRefreshTokenInCookie(result.RefreshToken , result.RefreshTokenExpiration);

            return Ok(result);
        }
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken revoke)
        {
            var token = revoke.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token Is Required");

            var result = await _authService.RevoketokenAsync(token);

            if(!result)
                return BadRequest("Token Is InValid");

            return Ok();


        }

        private void SetRefreshTokenInCookie(string refreshToken , DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };
            Response.Cookies.Append("refreshToken" , refreshToken , cookieOptions);

        }
        

    }
}
