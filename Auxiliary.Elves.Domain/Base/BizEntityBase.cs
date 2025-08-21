using Auxiliary.Elves.Infrastructure.Config.Domain;
using Auxiliary.Elves.Infrastructure.Config.Snowflake;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Domain.Base
{
    public class BizEntityBase : IEntity<long>, IUpdateTimeEntity
    {    /// <summary>
         /// 主键
         /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; } = IdWorker.NewDefaultId;


        public string CreateUser { get; set; } = "admin";
        public virtual string UpdateUser { get; set; } = "admin";

        [Column(TypeName = "datetime")]
        public virtual DateTime UpdateTime { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public virtual DateTime CreateTime { get; set; } = DateTime.Now;

        public object[] GetKeys()
        {
            throw new NotImplementedException();
        }
    }
}
