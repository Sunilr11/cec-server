using GDT.CEC.Repository.Models.PageTemplate;
using GDT.CEC.Service.DTOs;
using GDT.CEC.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace GDT.CEC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _templateService;
        private readonly IUsersService _usersService;
        private readonly ILogger _logger;

        public TemplateController(ITemplateService templateService,IUsersService usersService,ILogger<TemplateController> logger)
        {
           _templateService = templateService;
            _usersService = usersService;
            _logger = logger;
        }
        [HttpGet("getmenu")]
        public async Task<IActionResult> GetMenus()
        {
            try
            {


                var menus=await  _templateService.GetMenu();
                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<Menu>()
                {
                    Message = "",
                    Data = menus
                });
            }
            catch(Exception  ex) 
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }
        [HttpGet("getcategories")]
        public async Task<IActionResult> GetCategories(string type, string name)
        {
            try
            {
                var categories = await _templateService.GetCategories(type, name);
                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<List<Category>>()
                {
                    Message = "",
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }

        [Authorize]
        [HttpPost("createlabcategory")]
        public async Task<IActionResult> CreateLabCategory([Microsoft.AspNetCore.Mvc.FromBody] CategoryDTO category)
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
                        var result = await _templateService.CreateLabCategoryAsync(category);
                        if (result)
                        {
                            return StatusCode((int)HttpStatusCode.Created, new ReturnResponse<CategoryDTO>
                            {
                                StatusCode = 1,
                                Message = "Lab category created successfully"
                            });
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.BadRequest, new ReturnResponse<string>
                            {
                                StatusCode = 0,
                                Message = "Failed to create lab category"
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
                    Message = AppConstants.GENERIC_EXCEPTION_MESSAGE
                });
            }
        }

        [Authorize]
        [HttpPost("updatecategory")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDTO category)
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
                        var result = await _templateService.UpdateCategoryAsync(category);

                        if (result)
                        {
                            return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<CategoryDTO>
                            {
                                StatusCode = 1,
                                Message = "Category details updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new ReturnResponse<string>
                            {
                                StatusCode = 0,
                                Message = "Category not found"
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
                    Message = AppConstants.GENERIC_EXCEPTION_MESSAGE
                });
            }
        }

        [Authorize]
        [HttpPost("updatecategorytabs")]
        public async Task<IActionResult> UpdateCategoryTabs([FromBody] CategoryTabDTO category)
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
                        var result = await _templateService.UpdateCategoryTabsAsync(category);

                        if (result)
                        {
                            return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<CategoryDTO>
                            {
                                StatusCode = 1,
                                Message = "Category details updated successfully"
                            });
                        }
                        else
                        {
                            return NotFound(new ReturnResponse<string>
                            {
                                StatusCode = 0,
                                Message = "Category not found"
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
                    Message = AppConstants.GENERIC_EXCEPTION_MESSAGE
                });
            }
        }

        [HttpDelete("deletecategory {header}")]
        
        
        [SwaggerOperation(Summary = "Delete a category by its header")]
        public async Task<IActionResult> DeleteCategory(string header)
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
                    var result = await _templateService.DeleteCategoryAsync(header);

                    if (result)
                    {
                        return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<string>
                        {
                            StatusCode = 1,
                            Message = "Category details deleted successfully"
                        });
                    }
                    else
                    {
                        return NotFound(new ReturnResponse<string>
                        {
                            StatusCode = 0,
                            Message = "Category not found"
                        });
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
                    Message = AppConstants.GENERIC_EXCEPTION_MESSAGE
                });
            }
        }



        [HttpGet("gethometemplate")]
        public async Task<IActionResult> GetHomeTemplate()
        {
            try
            {
                var homeTemplate = await _templateService.GetHomeTemplate();
                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<HomeTemplate>()
                {
                    Message = "",
                    Data = homeTemplate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }
        [HttpGet("getMasterTemplate")]
        public async Task<IActionResult> GetMasterTemplate(string name)
        {
            try
            {
                var masterTemplate = await _templateService.GetMasterTemplate(name);
                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<MasterTemplate>()
                {
                    Message = "",
                    Data = masterTemplate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }

        [HttpGet("getabouttemplate")]
        public async Task<IActionResult> GetAboutTemplate()
        {
            try
            {
                var aboutTemplate = await _templateService.GetAboutTemplate();
                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<AboutTemplate>()
                {
                    Message = "",
                    Data = aboutTemplate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }

        [HttpGet("getMenudetails")]
        public async Task<IActionResult> GetMenuDetailsByRole(int roleId)
        {
            try
            {
                var menuDetails = await _templateService.GetMenuDetailsByRoleAsync(roleId);

                if (menuDetails == null || !menuDetails.Any())
                {
                    return NotFound(new { message = "No menus found for the specified role." });
                }

                return Ok(menuDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching menu details for roleId: {RoleId}", roleId);
                return StatusCode(500, new
                {
                    message = ex.Message,
                    statusCode = 1,
                    logMessage = ex.StackTrace
                });
            }
        }


    }
}
