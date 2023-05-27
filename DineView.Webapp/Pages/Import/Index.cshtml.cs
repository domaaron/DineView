using DineView.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Linq;

namespace DineView.Webapp.Pages.Import
{
    public class IndexModel : PageModel
    {
        private static string[] _allowedTextExtensions = { ".txt", ".csv" };
        private static string[] _allowedExcelExtensions = { ".xls", ".xlsx" };
        private readonly DishImportService _importService;

        public IndexModel(DishImportService importService)
        {
            _importService = importService;
        }

        [BindProperty]
        public IFormFile? UploadedFile { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; }
        [TempData]
        public string? Message { get; set; }
        public IActionResult OnPostCsvImport()
        {
            var (success, message) = CheckUploadedFile(_allowedTextExtensions);
            if (!success)
            {
                ErrorMessage = message;
                return RedirectToPage();
            }

            using var stream = UploadedFile!.OpenReadStream();
            (success, message) = _importService.LoadCsv(stream);
            if (!success)
            {
                ErrorMessage = message;
            } 
            else
            {
                Message = message;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostExcelImport()
        {
            var (success, message) = CheckUploadedFile(_allowedExcelExtensions);
            if (!success)
            {
                ErrorMessage = message;
                return RedirectToPage();
            }

            using var stream = UploadedFile!.OpenReadStream();
            (success, message) = _importService.LoadExcel(stream);
            if (!success)
            {
                ErrorMessage = message;
            }
            else
            {
                Message = message;
            }

            return RedirectToPage();
        }

        public void OnGet()
        {
        }

        private (bool success, string message) CheckUploadedFile(string[] allowedExtensions)
        {
            if (UploadedFile is null)
            {
                return (false, "No file was uploaded.");
            }

            var extension = Path.GetExtension(UploadedFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return (false, $"Only files with the extension {string.Join(",", allowedExtensions)} are allowed.");
            }

            if (UploadedFile.Length > 1 << 20)
            {
                return (false, "The file must not exceed 1 MB in size.");
            }
            return (true, string.Empty);
        }
    }
}
