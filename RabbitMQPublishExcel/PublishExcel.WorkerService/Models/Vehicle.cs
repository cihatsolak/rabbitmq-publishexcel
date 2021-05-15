using System;
using System.Collections.Generic;

#nullable disable

namespace PublishExcel.WorkerService.Models
{
    public partial class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string SubModel { get; set; }
        public int? Year { get; set; }
        public string Color { get; set; }
        public decimal? Price { get; set; }
    }
}
