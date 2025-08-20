using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Domain
{
    public class AuxiliaryDbContext : DbContext
    {
        private readonly ILogger<AuxiliaryDbContext> _logger;

        public AuxiliaryDbContext(DbContextOptions<AuxiliaryDbContext> options, ILoggerFactory loggerFactory) : base(options)
        {
            _logger = loggerFactory.CreateLogger<AuxiliaryDbContext>();
        }

    }
}
