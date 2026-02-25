using AppCore.Models;

namespace AppCore.Interfaces;

public interface ICustomerService
{
    public IEnumerable<Customer> GetCustomers();
    public Task<IEnumerable<Customer>> GetCustomersAsync();
}
