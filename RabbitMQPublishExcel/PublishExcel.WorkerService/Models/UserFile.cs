using System;
using System.Collections.Generic;

#nullable disable

namespace PublishExcel.WorkerService.Models
{
    public partial class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int FileStatus { get; set; }
    }
}
