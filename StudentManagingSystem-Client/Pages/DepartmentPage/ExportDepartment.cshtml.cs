using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagingSystem_Client.Services;
using StudentManagingSystem_Client.ViewModel;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace StudentManagingSystem_Client.Pages.DepartmentPage
{
    [Authorize]
    public class ExportDepartmentModel : PageModel
    {
        [BindProperty]
        public string? Keyword { get; set; }
        [BindProperty]
        public bool? Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int TotalPage { get; set; }
        public async Task<IActionResult> OnGetAsync(string? keyword, bool? status, int pageIndex, int pagesize)
        {
            var client = new ClientService(HttpContext);
            Keyword = keyword;
            Status = status;
            if (pageIndex == 0) pageIndex = 1;
            PageIndex = pageIndex;
            pagesize = 1000;

            var requestModel = new DepartmentSearchRequest
            {
                keyword = keyword,
                status = status,
                page = pageIndex,
                pagesize = pagesize
            };
            var response = await client.Export("/api/Department/Export", requestModel);
            if (response.IsSuccessStatusCode)
            {
                // Read the file content from the API response
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                // Set the response headers for file download
                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    FileName = "Department.xlsx", // Replace with the desired file name
                    Inline = false
                };
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                // Return the file as a FileStreamResult
                return new FileStreamResult(new MemoryStream(fileBytes), "application/octet-stream");
            }
            else
            {
                // Handle the case when the API call fails
                return Content("Failed to download the Excel file");
            }
        }
    }
}
