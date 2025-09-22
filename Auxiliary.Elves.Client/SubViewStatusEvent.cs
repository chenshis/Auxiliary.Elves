using Auxiliary.Elves.Client.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Client
{
    public class SubViewStatusEvent : PubSubEvent<StatusMessage>
    {

    }

    // 定义要传递的消息类型
    public class StatusMessage
    {
        public AccountModel Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
