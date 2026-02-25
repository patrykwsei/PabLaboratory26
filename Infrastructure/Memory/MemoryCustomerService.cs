using AppCore.Interfaces;
using AppCore.Models;

namespace Infrastructure.Memory;

public class MemoryCustomerService : ICustomerService
{
    public IEnumerable<Customer> GetCustomers()
    {
        return
        [
            new Customer()
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "a@wsei.edu.pl",
                Phone = "111-222-333",
                AddressId = 11

            },
            new Customer()
            {
                Id = 2,
                FirstName = "Anna",
                LastName = "Norek",
                Email = "b@wsei.edu.pl",
                Phone = "444-555-666",
                AddressId = 12
            }
        ];
    }

    public Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        throw new NotImplementedException();
    }
}