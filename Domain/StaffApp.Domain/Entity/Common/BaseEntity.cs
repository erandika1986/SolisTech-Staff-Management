using System.ComponentModel.DataAnnotations.Schema;

namespace StaffApp.Domain.Entity.Common
{
    public class BaseEntity
    {
        [Column(Order = 0)]
        public int Id { get; set; }
    }
}
