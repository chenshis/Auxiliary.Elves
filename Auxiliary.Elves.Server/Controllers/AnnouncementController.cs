using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.AspNetCore.Mvc;

namespace Auxiliary.Elves.Server.Controllers
{
    public class AnnouncementController : AuxiliaryControllerBase
    {
        public IAnnouncementApiService  AnnouncementApiService { get; }

        public AnnouncementController(IAnnouncementApiService announcementApiService)
        {
            AnnouncementApiService = announcementApiService;
        }

        /// <summary>
        /// 新增公告
        /// </summary>
        /// <param name="ment">内容</param>
        /// <returns></returns>

        [HttpPost]
        [Route(SystemConstant.AddAnnouncementRoute)]
        public bool AddAnnouncement(string ment)
        {
            return AnnouncementApiService.AddAnnouncement(ment);
        }

        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.DelAnnouncementRoute)]    
        public bool RemoveAnnouncement(long id)
        {
            return AnnouncementApiService.DeleteAnnouncement(id);
        }

        /// <summary>
        /// 查询公告
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(SystemConstant.AnnouncementRoute)]
        public List<AnnouncementDto> GetAnnouncement()
        {
            return AnnouncementApiService.GetAnnouncementDto();
        }   
    }
}
