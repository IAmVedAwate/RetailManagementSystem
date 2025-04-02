using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailManagementSystem.Models.Models.DTO
{
    public class FeedbackDTO
    {
        public string FeedbackType { get; set; }
        public string FeedbackContent { get; set; }
        public DateTime DateSubmitted { get; set; }
    }
}
