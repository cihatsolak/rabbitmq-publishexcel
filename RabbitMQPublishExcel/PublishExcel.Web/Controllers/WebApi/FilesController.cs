using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublishExcel.Web.Models.Contexts;
using PublishExcel.Web.Models.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PublishExcel.Web.Controllers.WebApi
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Upload(IFormFile file, string fileId)
        {
            if (file is { Length: < 0 })
                return BadRequest();

            var userFile = await _context.UserFiles.FindAsync(fileId);
            if (userFile is null)
                return NotFound();

            //deneme.xlsx
            string fileNameAndExtension = userFile.FileName + Path.GetExtension(file.FileName);

            //wwwroot/files
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", fileNameAndExtension);

            await using FileStream fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = FileStatus.Completed;

            _context.UserFiles.Update(userFile);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
