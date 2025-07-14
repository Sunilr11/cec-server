using GDT.CEC.Repository.Models.AzureLabs;
using GDT.CEC.Service.DTOs;
using GDT.CEC.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace GDT.CEC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabController : ControllerBase
    {
        private readonly IAzureLabService _LabService;
        private readonly IUsersService _usersService;
        private readonly ILogger _logger;

        public LabController(IAzureLabService LabService,IUsersService usersService, ILogger<LabController> logger)
        {
            _LabService = LabService;
            _usersService = usersService;
            _logger = logger;
        }
        [HttpGet("getlabs")]
        public async Task<IActionResult> GetLabs(bool getactiveonly, bool detailed = false)
        {
            try
            {
                var labs = await _LabService.GetLabs(getactiveonly, detailed);
                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<List<AzureLab>>()
                {
                    Message = "",
                    Data = labs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }

        [Authorize()]
        [HttpGet("getlabsforuser")]
        public async Task<IActionResult> GetLabsforUser()
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
                if (user != null && user.IsActive)
                {
                    var labs = await _LabService.GetLabsForUser(userOid);
                    return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<List<AzureLab>>()
                    {
                        Message = "",
                        Data = labs
                    });
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

        [HttpGet("getlabdetail")]
        public async Task<IActionResult> GetLab(string id)
        {
            try
            {
                var lab = await _LabService.GetLab(id);
                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<LabDTO>()
                {
                    Message = "",
                    Data = lab
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        [HttpPost("createlab")]
        [SwaggerResponse(201, "Lab created successfully", typeof(Response))]
        [SwaggerResponse(400, "Bad Request", typeof(Response))]
        [SwaggerResponse(500, "Internal Server Error", typeof(Response))]
        public async Task<IActionResult> CreateLab(CreateLabDTO lab)
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
                        var result = await _LabService.CreateLabAsync(lab);
                        if (result)
                        {
                            return StatusCode((int)HttpStatusCode.Created, new ReturnResponse<CreateLabDTO>
                            {
                                StatusCode = 1,
                                Message = "Lab created successfully"
                            });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.BadRequest, new ReturnResponse<string>
                            {
                                StatusCode = 0,
                                Message = "Failed to create lab"
                            });
                        }
                    }
                    else
                    {
                        return Unauthorized(AppConstants.NO_USER_PERMISSION);
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
                return StatusCode((int)HttpStatusCode.InternalServerError, new ReturnResponse<string>
                {
                    Message = "An error occurred while creating the lab"
                });
            }
        }


        [Authorize]
        [HttpPost("labActive")]
        [SwaggerResponse(200, "Status updated successfully", typeof(ReturnResponse<LabActiveDTO>))]
        [SwaggerResponse(404, "Lab not found", typeof(ReturnResponse<string>))]
        [SwaggerResponse(500, "Internal Server Error", typeof(ReturnResponse<string>))]
        public async Task<IActionResult> UpdateLabActive([FromBody] LabActiveDTO labactiveDto)
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
                        bool result = await _LabService.UpdateLabActiveAsync(labactiveDto.Id, labactiveDto.IsActive);
                        if (result)
                        {
                            return Ok(new ReturnResponse<LabActiveDTO>
                            {
                                Message = "Lab status updated successfully",
                                StatusCode = 1,
                                Data = labactiveDto
                            });
                        }
                        else
                        {
                            return NotFound(new ReturnResponse<string>
                            {
                                Message = "Lab not found",
                                StatusCode = 0
                            });
                        }
                    }
                    else
                    {
                        return Unauthorized(AppConstants.NO_USER_PERMISSION);
                    }
                }
                else
                {
                   return  Unauthorized(AppConstants.NO_USER_PERMISSION);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lab status");
                return StatusCode(500, new ReturnResponse<string>
                {
                    Message = "Internal server error",
                    StatusCode = 0
                });
            }
        }


        [Authorize]
        [HttpPost("updatelab")]
        public async Task<IActionResult> UpdateLab(CreateLabDTO lab)
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
                        var result = await _LabService.UpdateLabAsync(lab);
                        if (result)
                        {
                            return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<CreateLabDTO>
                            {
                                StatusCode = 1,
                                Message = "Lab details updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new ReturnResponse<string>
                            {
                                StatusCode = 0,
                                Message = "Lab not found"
                            });
                        }
                    }
                    else
                    {
                        return Unauthorized(AppConstants.NO_USER_PERMISSION);
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
                return StatusCode((int)HttpStatusCode.InternalServerError, new ReturnResponse<string>
                {
                    Message = "An error occurred while updating the lab details"
                });

            }
        }
    }
}
