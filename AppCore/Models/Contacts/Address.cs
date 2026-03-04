using AppCore.Models.Contacts.Enums;

namespace AppCore.Models.Contacts;

public class Address
{
    public string Street { get; set; } = "";
    public string City { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string Country { get; set; } = "";
    public AddressType Type { get; set; } = AddressType.Main;
}