using AppCore.Models.Contacts;
using AppCore.Repositories;

namespace Infrastructure.Memory.Repositories;

public class MemoryCompanyRepository : MemoryGenericRepository<Company>, ICompanyRepository
{
    public Task<IEnumerable<Company>> FindByNameAsync(string namePart)
    {
        namePart = namePart?.Trim() ?? "";
        var result = _data.Values
            .Where(c => c.Name.Contains(namePart, StringComparison.OrdinalIgnoreCase))
            .AsEnumerable();

        return Task.FromResult(result);
    }

    public Task<Company?> FindByNipAsync(string nip)
    {
        nip = nip?.Trim() ?? "";
        var result = _data.Values.FirstOrDefault(c =>
            !string.IsNullOrWhiteSpace(c.NIP) &&
            string.Equals(c.NIP.Trim(), nip, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(result);
    }

    public Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId)
    {
        if (!_data.TryGetValue(companyId, out var company))
            throw new KeyNotFoundException($"Company with Id={companyId} not found.");

        return Task.FromResult(company.Employees.AsEnumerable());
    }
}