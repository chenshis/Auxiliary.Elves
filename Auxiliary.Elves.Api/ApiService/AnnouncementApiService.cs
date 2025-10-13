using Auxiliary.Elves.Api.Dtos;
using Auxiliary.Elves.Api.IApiService;
using Auxiliary.Elves.Domain;
using Auxiliary.Elves.Infrastructure.Config;
using Microsoft.Extensions.Logging;

namespace Auxiliary.Elves.Api.ApiService
{
    public class AnnouncementApiService : IAnnouncementApiService
    {
        private readonly AuxiliaryDbContext _dbContext;
        private readonly ILogger<AnnouncementApiService> _logger;


        public AnnouncementApiService(AuxiliaryDbContext dbContext, ILogger<AnnouncementApiService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public bool AddAnnouncement(string ment)
        {
            if (string.IsNullOrWhiteSpace(ment))
                return false;

            _dbContext.AnnouncementEntities.Add(new Domain.Entities.AnnouncementEntity
            {
                Announcement = ment
            });

            return _dbContext.SaveChanges() > SystemConstant.Zero;
        }

        public bool DeleteAnnouncement(long id)
        {
            if (id == null)
                return false;

            var announcementEntity = _dbContext.AnnouncementEntities.FirstOrDefault(t => t.Id == id);

            if (announcementEntity!=null)
            {
                _dbContext.AnnouncementEntities.Remove(announcementEntity);
            }

            return _dbContext.SaveChanges() > SystemConstant.Zero;
        }

        public List<AnnouncementDto> GetAnnouncementDto()
        {
            var announcementEntity = _dbContext.AnnouncementEntities;

            if (!announcementEntity.Any())
                return new List<AnnouncementDto>();

            return announcementEntity.Select(x=>new AnnouncementDto { Id=x.Id,
                Announcement=x.Announcement }).ToList();
        }

   

    }
}
