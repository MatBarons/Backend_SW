using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academy.Api.Repositories.EF;

/// <summary>
/// This is an example class that represents a table in the Database
///
/// SQL models are recommended to be kept with internal visibility, not public, because using EF objects in Business Logic is risky
/// Even an involutary single change to an EF object by the Business logic could be reflected to the database
/// So EF objects must never go out to Services and Web layers, but should always be mapped to other objects defined in Domain
/// </summary>
[Table("Example", Schema = "dbo")]
internal class Example
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    public string? Description { get; set; }
}