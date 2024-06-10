using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Academy.Api.Repositories.EF;

[Table("people", Schema = "republic")]
internal class Person
{
    [Key]
    public string? Name {get; set;}
    public string? Height {get; set;}
    public string? Mass {get; set;}
    public string? Hair_Color {get; set;}
    public string? Skin_Color {get; set;}
    public string? Eye_Color {get; set;}
    public string? Birth_Name {get; set;}
    public string? Gender {get; set;}
    [ForeignKey("Planets")]
    public string? Homeworld {get; set;}
}