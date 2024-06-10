using Academy.Api.Domain.Models.DTOs;

namespace Academy.Api.Repositories.Interfaces
{
    /// <summary>
    /// This is the interface to expose publicly the ExampleRepository
    /// All repositories should always have an interface so they can be properly exposed in the Depencendy Injection
    /// with the interface rather than directly. This improves extensibility of the solution because if all components
    /// reference repositories with their interfaces we can easily swap one repository with an equivalent one without making
    /// changes if not in the Program.cs
    /// </summary>
    public interface IExampleRepository
    {
        ExampleDto? GetExample(int id);
        public Task DeleteExampleMethod(int exampleId);
        public List<ExampleDto> GetExamplesByName(string name);
    }
}