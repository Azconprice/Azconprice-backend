using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Statistics
    {
        public int TotalWorkRequests { get; set; }
        public int ThisYearWorkRequests { get; set; }
        public int ThisMonthWorkRequests { get; set; }
        public int TodayWorkRequests { get; set; }
        public int AcceptedWorkRequest { get; set; }
        public int ComlpetedWorks { get; set; }
    }
}
