using Azure.Storage.Blobs;
using GDT.CEC.Service.DTOs;
using GDT.CEC.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace GDT.CEC.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }
        [HttpPost("uploadimage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile)
        {

            string imgUrl = await _imageService.UploadImageToBlobAsync(formFile);
            return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<string>
            {
                StatusCode = 1,
                Message = "Lab details updated successfully",
                Data = imgUrl
            });
        }
        [HttpPost("uploadimage64")]
        public async Task<IActionResult> UploadImage64([FromForm] ImageUploadDTO fileImage)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(fileImage.Base64);
                string imgUrl = await _imageService.UploadImageToBlobAsync(bytes, fileImage.FileName, fileImage.ContentType);
                return StatusCode((int)HttpStatusCode.OK, new ReturnResponse<string>
                {
                    StatusCode = 1,
                    Message = "Lab details updated successfully",
                    Data = imgUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new Response() { Message = AppConstants.GENERIC_EXCEPTION_MESSAGE });
            }
        }
    }
}