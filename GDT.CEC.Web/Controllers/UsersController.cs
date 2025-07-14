using GDT.CEC.Service.DTOs;
using GDT.CEC.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Text;

namespace GDT.CEC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        #region Declarations
        private readonly IUsersService _usersService;
        private readonly ILogger<UsersController> _logger;
        #endregion
        public UsersController(IUsersService usersService, ILogger<UsersController> logger)
        {
            _usersService = usersService ?? throw new ArgumentNullException(nameof(IUsersService));
            _logger = logger;
        }


        [HttpPost("create")]
        [SwaggerResponse(200, "Ok", typeof(Response))]
        [SwaggerResponse(400, "Bad Request", typeof(Response))]
        [SwaggerResponse(500, "Internal Server Error", typeof(Response))]
        public async Task<IActionResult> CreateAsync([FromBody] TempUserRegister user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    List<string> errorMessage = new();

                    if (_usersService.IsUserEmailVerified(user.User.Email, user.RegistrationKey))
                    {
                        ReturnResponse<Response> resp = await _usersService.SaveUserAsync(user);
                        if (resp.StatusCode == 1)
                        {
                            return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<string>()
                            {
                                Message = AppConstants.USERS_ADDED_SUCCESSFULLY,
                                StatusCode = 1
                            });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<string>()
                            {
                                Message = resp.Message,
                                StatusCode = 1
                            });
                        }
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.Conflict, new ReturnResponse<Response>()
                        {
                            Message = AppConstants.USER_OTP_FAILED,
                            StatusCode = 0
                        });
                    }
                }
                else
                {
                    _logger.LogDebug(AppConstants.MODEL_VALIDATION_FAILED);
                    return await PopulateModelErrorsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ReturnResponse<string>()
                {
                    Message = AppConstants.GENERIC_EXCEPTION_MESSAGE,
                    StatusCode = 0
                });
            }
        }



        [HttpPost("verifyemail")]
        [SwaggerResponse(200, "Ok", typeof(Response))]
        [SwaggerResponse(400, "Bad Request", typeof(Response))]
        [SwaggerResponse(500, "Internal Server Error", typeof(Response))]
        public async Task<IActionResult> VerifyEmail([FromBody] UserEmailVerification user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    if (!Common.IsValidEmail(user.Email))
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new ReturnResponse<Response>()
                        {
                            Message = AppConstants.INVALID_EMAIL,
                            StatusCode = 0
                        });
                    }
                    if (_usersService.UserEmailExist(user.Email))
                    {
                        return StatusCode((int)HttpStatusCode.Conflict, new ReturnResponse<Response>()
                        {
                            Message = AppConstants.USER_ALREADY_EXIST,
                            StatusCode = 0
                        });
                    }
                    else
                    {
                        string key = _usersService.GenerateOTPSendEmail(user.Email);
                        return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<string>()
                        {
                            Message = "",
                            Data = key,
                            StatusCode = 1
                        });
                    }
                }
                else
                {
                    _logger.LogDebug(AppConstants.MODEL_VALIDATION_FAILED);
                    return await PopulateModelErrorsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ReturnResponse<string>() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE, StatusCode = 0 });
            }
        }


        [HttpPost("resendotp")]
        [SwaggerResponse(200, "Ok", typeof(Response))]
        [SwaggerResponse(400, "Bad Request", typeof(Response))]
        [SwaggerResponse(500, "Internal Server Error", typeof(Response))]
        public async Task<IActionResult> ResendOTP([FromBody] UserEmailVerification user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    if (_usersService.UserEmailExist(user.Email))
                    {
                        return StatusCode((int)HttpStatusCode.Conflict, new ReturnResponse<Response>()
                        {
                            Message = AppConstants.USER_ALREADY_EXIST,
                            StatusCode = 0
                        });
                    }
                    else
                    {
                        _usersService.RemoveMemoryCache(user.Key);
                        string key = _usersService.GenerateOTPSendEmail(user.Email);
                        return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<string>()
                        {
                            Message = "",
                            Data = key,
                            StatusCode = 1
                        });
                    }
                }
                else
                {
                    _logger.LogDebug(AppConstants.MODEL_VALIDATION_FAILED);
                    return await PopulateModelErrorsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ReturnResponse<string>() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE, StatusCode = 0 });
            }
        }



        [HttpPost("otpverification")]
        [SwaggerResponse(200, "Ok", typeof(Response))]
        [SwaggerResponse(400, "Bad Request", typeof(Response))]
        [SwaggerResponse(500, "Internal Server Error", typeof(Response))]
        public async Task<IActionResult> OTPVerification([FromBody] OTPVerificaton user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ReturnResponse<Response> returnResponse = _usersService.EmailOTPVerification(user.key, user.OTP);
                    if (returnResponse.StatusCode == 0)
                    {
                        return StatusCode((int)HttpStatusCode.Conflict, new ReturnResponse<Response>()
                        {
                            Message = AppConstants.USER_OTP_FAILED,
                            StatusCode = 0
                        });
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<string>()
                        {
                            Message = "Email Verified",
                            Data = "",
                            StatusCode = 1
                        });
                    }
                }
                else
                {
                    _logger.LogDebug(AppConstants.MODEL_VALIDATION_FAILED);
                    return await PopulateModelErrorsAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ReturnResponse<string>() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE, StatusCode = 0 });
            }
        }

        [HttpGet("getUserRegisterConfig")]
        public async Task<IActionResult> GetUserRegisterConfiguration()
        {
            try
            {
                var configs = await _usersService.GetUserRegisterConfiguration();

                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<UserRegisterConfig>()
                {
                    Message = "",
                    Data = configs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        [HttpGet("getUserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    return Unauthorized();
                }
                var tokenString = authorizationHeader;
                var jwtEncodedString = tokenString.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtEncodedString);
                var userOid = token.Claims.FirstOrDefault(claim => claim.Type == "oid")?.Value;


                var user = await _usersService.GetUserByOIDAsync(userOid);
                if (user.IsActive)
                {
                    return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<User>()
                    {
                        StatusCode = 1,
                        Message = "",
                        Data = user
                    });
                }
                else
                {
                    return Unauthorized(AppConstants.USER_INACTIVE);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        [HttpPost("activeUsers")]
        [SwaggerResponse(200, "Status updated successfully", typeof(ReturnResponse<ActiveDTO>))]
        [SwaggerResponse(404, "User not found", typeof(ReturnResponse<string>))]
        [SwaggerResponse(500, "Internal Server Error", typeof(ReturnResponse<string>))]
        public async Task<IActionResult> UpdateUserActive([FromBody] ActiveDTO activeDto)
        {
            try
            {
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    return Unauthorized();
                }
                var tokenString = authorizationHeader;
                var jwtEncodedString = tokenString.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtEncodedString);
                var userOid = token.Claims.FirstOrDefault(claim => claim.Type == "oid")?.Value;
                var user = await _usersService.GetUserByOIDAsync(userOid);
                if (user != null && (user.RoleId == AppConstants.ROLE_ID_ADMIN || user.RoleId == AppConstants.ROLE_ID_SUPERADMIN))
                {
                    if (user.IsActive)
                    {
                        bool result = await _usersService.UpdateUserActiveAsync(activeDto.Id, activeDto.IsActive, user.RoleId == AppConstants.ROLE_ID_SUPERADMIN);
                        if (result)
                        {
                            return Ok(new ReturnResponse<ActiveDTO>
                            {
                                Message = "User status updated successfully",
                                StatusCode = 1,
                                Data = activeDto
                            });
                        }
                        else
                        {
                            return NotFound(new ReturnResponse<string>
                            {
                                Message = "User not found",
                                StatusCode = 0
                            });
                        }
                    }
                    else
                    {
                        return Unauthorized(AppConstants.USER_INACTIVE);
                    }
                }
                else
                {
                    return Unauthorized(AppConstants.NO_USER_PERMISSION);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status");
                return StatusCode(500, new ReturnResponse<string>
                {
                    Message = "Internal server error",
                    StatusCode = 0
                });
            }
        }

        [Authorize]
        [HttpPost("roleUpdate")]
        [SwaggerResponse(200, "Status updated successfully", typeof(ReturnResponse<RoleDTO>))]
        [SwaggerResponse(404, "User not found", typeof(ReturnResponse<string>))]
        [SwaggerResponse(500, "Internal Server Error", typeof(ReturnResponse<string>))]
        public async Task<IActionResult> UpdateUser([FromBody] RoleDTO roleDto)
        {
            try
            {
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    return Unauthorized();
                }
                var tokenString = authorizationHeader;
                var jwtEncodedString = tokenString.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtEncodedString);
                var userOid = token.Claims.FirstOrDefault(claim => claim.Type == "oid")?.Value;
                var user = await _usersService.GetUserByOIDAsync(userOid);
                if (user != null && user.RoleId == AppConstants.ROLE_ID_SUPERADMIN)
                {
                    if (user.IsActive)
                    {
                        int result = await _usersService.UpdateUserRoleAsync(roleDto.Ids, roleDto.RoleId);

                        if (result > 0)
                        {
                            return Ok(new ReturnResponse<Response>
                            {
                                Message = $"Updated {result} users",
                                StatusCode = 1
                            });
                        }
                        else
                        {
                            return NotFound(new ReturnResponse<string>
                            {
                                Message = "User role not updated",
                                StatusCode = 0
                            });
                        }
                    }
                    else
                    {
                        return Unauthorized(AppConstants.USER_INACTIVE);
                    }
                }
                else
                {
                    return Unauthorized(AppConstants.NO_USER_PERMISSION);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status");
                return StatusCode(500, new ReturnResponse<string>
                {
                    Message =AppConstants.GENERIC_EXCEPTION_MESSAGE,
                    StatusCode = 0
                });
            }
        }

        [Authorize]
        [HttpPost("updateuser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            try
            {
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    return Unauthorized();
                }
                var tokenString = authorizationHeader;
                var jwtEncodedString = tokenString.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtEncodedString);
                var userOid = token.Claims.FirstOrDefault(claim => claim.Type == "oid")?.Value;

                var result = await _usersService.UpdateAsync(user, userOid);

                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<User>()
                {
                    StatusCode = 1,
                    Message = AppConstants.USERS_UPDATED_SUCCESSFULLY
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        [HttpPost("getusers")]
        public async Task<IActionResult> GetUsers(PagingModel model)
        {
            try
            {
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    return Unauthorized();
                }
                var tokenString = authorizationHeader;
                var jwtEncodedString = tokenString.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtEncodedString);
                var userOid = token.Claims.FirstOrDefault(claim => claim.Type == "oid")?.Value;

                var user = await _usersService.GetUserByOIDAsync(userOid);
                if (user != null && (user.RoleId == AppConstants.ROLE_ID_ADMIN || user.RoleId == AppConstants.ROLE_ID_SUPERADMIN))
                {
                    if (user.IsActive)
                    {
                        var users = await _usersService.GetUsersAsync(model.pagesize, model.pageno, model.sortdir, model.sortexpression, model.filters);
                        return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<UserPagingModel>()
                        {
                            Message = "",
                            Data = users
                        });
                    }
                    else
                    {
                        return Unauthorized(AppConstants.USER_INACTIVE);
                    }
                }
                else
                {
                    return Unauthorized(AppConstants.NO_USER_PERMISSION);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        [HttpGet("getadusers")]
        public async Task<IActionResult> GetAdUsers()
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return Unauthorized();
            }
            var tokenString = authorizationHeader;
            var jwtEncodedString = tokenString.ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtEncodedString);
            var userOid = token.Claims.FirstOrDefault(claim => claim.Type == "oid")?.Value;

            var user = await _usersService.GetUserByOIDAsync(userOid);
            if (user != null && (user.RoleId == AppConstants.ROLE_ID_ADMIN || user.RoleId == AppConstants.ROLE_ID_SUPERADMIN))
            {
                if (user.IsActive)
                {
                    var response = await _usersService.GetAllAdUsers();
                    if (response.StatusCode == 1)
                    {
                        return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<List<AzureADUserDTO>>()
                        {
                            Message = "",
                            Data = response.Data
                        });
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = response.Message });
                    }
                }
                else
                {
                    return Unauthorized(AppConstants.USER_INACTIVE);
                }
            }
            else
            {
                return Unauthorized(AppConstants.NO_USER_PERMISSION);
            }
        }
        [Authorize]
        [HttpGet("getuserstatistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            var response = await _usersService.GetUserStatisticsAsync();
            if (response.StatusCode == 1)
            {
                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<UserStatics>()
                {
                    Message = "",
                    Data = response.Data
                });
            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = response.Message });
            }
        }

        [Authorize]
        [HttpPost("importadusers")]
        public async Task<IActionResult> ImportAdUsers(List<AzureADUserDTO> users)
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return Unauthorized();
            }
            var tokenString = authorizationHeader;
            var jwtEncodedString = tokenString.ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtEncodedString);
            var userOid = token.Claims.FirstOrDefault(claim => claim.Type == "oid")?.Value;

            var user = await _usersService.GetUserByOIDAsync(userOid);
            if (user != null && (user.RoleId == AppConstants.ROLE_ID_ADMIN || user.RoleId == AppConstants.ROLE_ID_SUPERADMIN))
            {
                if (user.IsActive)
                {
                    var response = await _usersService.SaveADUserAsync(users);
                    if (response.StatusCode == 1)
                    {
                        return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<string>()
                        {
                            Message = AppConstants.USERS_ADDED_SUCCESSFULLY,
                            Data = response.Data
                        });
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = response.Message });
                    }
                }
                else
                {
                    return Unauthorized(AppConstants.USER_INACTIVE);
                }
            }
            else
            {
                return Unauthorized(AppConstants.NO_USER_PERMISSION);
            }
        }

        [HttpGet("getuserimportstatus")]
        public async Task GetUserImportStatus(string importkey)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                await Echo(webSocket, importkey);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }



        private async Task Echo(WebSocket webSocket, string key)
        {
            var buffer = new byte[1024 * 4];

            while (!webSocket.CloseStatus.HasValue)
            {
                UserImportStatus status = _usersService.GetUserImportStatus(key);

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(status);


                var serverMsg = Encoding.UTF8.GetBytes(json);
                await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                if (status != null && status.TotalCount == status.ProcessedCount)
                {
                    break;
                }
                System.Threading.Thread.Sleep(1000);

            }
        }



        [Authorize]
        [HttpPost("approveuser")]
        public async Task<IActionResult> ApproveRejectUser([FromBody] StatusUpdateModel model)
        {
            try
            {
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    return Unauthorized();
                }
                var tokenString = authorizationHeader;
                var jwtEncodedString = tokenString.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtEncodedString);
                var userOid = token.Claims.FirstOrDefault(claim => claim.Type == "oid")?.Value;
                var user = await _usersService.GetUserByOIDAsync(userOid);
                if (user != null && (user.RoleId == AppConstants.ROLE_ID_ADMIN || user.RoleId == AppConstants.ROLE_ID_SUPERADMIN))
                {
                    if (user.IsActive)
                    {
                        var response = await _usersService.ApproveRejectUser(model.Userid, model.Status, model.RejectReason);
                        return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<User>()
                        {
                            StatusCode = response.StatusCode,
                            Message = response.Message
                        });
                    }
                    else
                    {
                        return Unauthorized(AppConstants.USER_INACTIVE);
                    }
                }
                else
                {
                    return Unauthorized(AppConstants.NO_USER_PERMISSION);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }
    }
}
