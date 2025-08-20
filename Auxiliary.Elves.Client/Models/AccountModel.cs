using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Client.Models
{
    public class AccountModel
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string BindAccount { get; set; }
        public DateTime? ExpireTime { get; set; }
        public bool Status { get; set; }
    }
}
