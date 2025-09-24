using Auxiliary.Elves.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Api.IApiService
{
    public interface IAnnouncementApiService
    {
        /// <summary>
        /// 查询公告
        /// </summary>
        /// <returns></returns>
        List<AnnouncementDto> GetAnnouncementDto();

        /// <summary>
        /// 新增公告
        /// </summary>
        /// <param name="ment"></param>
        /// <returns></returns>
        bool AddAnnouncement(string ment);

        /// <summary>
        /// 删除公告
        /// </summary>
        /// <returns></returns>
        bool DeleteAnnouncement(long id);
    }
}
