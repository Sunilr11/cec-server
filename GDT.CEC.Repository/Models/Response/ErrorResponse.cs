
namespace GDT.CEC.Repository.Models.Response
{
    public class ErrorResponse
    {
        public List<string> Errors { get; set; }
        public ErrorResponse(List<string> Errors)
        {
            this.Errors = Errors;
        }

    }
}
