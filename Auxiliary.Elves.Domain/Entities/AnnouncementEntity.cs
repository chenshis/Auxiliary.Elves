using Auxiliary.Elves.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auxiliary.Elves.Domain.Entities
{
    [Table("sys_announcement")]
    public class AnnouncementEntity : BizEntityBase
    {
        /// <summary>
        /// 公告内容,;
        /// </summary>
        public string Announcement { get; set; }
    }
}
