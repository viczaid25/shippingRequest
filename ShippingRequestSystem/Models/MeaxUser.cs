using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingRequestSystem.Models;

[Table("meax_all_user")]
public class MeaxUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("pc_login_id")]
    [MaxLength(50)]
    public string? PcLoginId { get; set; }

    [Column("username")]
    [MaxLength(100)]
    public string? Username { get; set; }

    [Column("email")]
    [MaxLength(100)]
    public string? Email { get; set; }

    [Column("department")]
    [MaxLength(50)]
    public string? Department { get; set; }

    [Column("position")]
    [MaxLength(50)]
    public string? Position { get; set; }

    [Column("dep_2")]
    [MaxLength(50)]
    public string? Dep2 { get; set; }

    // auth es tinyint y no se usa — se ignora en AppDbContext
}