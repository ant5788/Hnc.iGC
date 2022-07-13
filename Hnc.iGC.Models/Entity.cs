using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hnc.iGC.Models
{
    public abstract class Entity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Display(Name = "创建时间")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationTime { get; set; } = DateTime.Now;

        [Display(Name = "最后修改时间")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? LastModificationTime { get; set; }
    }
}
