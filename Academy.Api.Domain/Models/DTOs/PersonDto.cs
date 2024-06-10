using System.ComponentModel.DataAnnotations.Schema;

namespace Academy.Api.Domain.Models.DTOs{
    public class PersonDto{
        public string? Name {get; set;}
        public string? Height {get; set;}
        public string? Mass {get; set;}
        public string? Hair_Color {get; set;}
        public string? Skin_Color {get; set;}
        public string? Eye_Color {get; set;}
        public string? Birth_Name {get; set;}
        public string? Gender {get; set;}
        public string? Homeworld {get; set;}
    }
}