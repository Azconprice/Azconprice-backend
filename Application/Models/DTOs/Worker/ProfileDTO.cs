using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.DTOs.Worker
{
    public class ProfileDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int TotalRequestsCount { get; set;}
        public int InactiveRequestsCount { get; set; }
        public int ActiveRequestsCount { get; set; }
        public int CompletedRequestsCount { get; set; }
        public double Rating { get; set; }
    }
}
