using API.DTOs;
using Core.Entities;

namespace API.Extensions;

public static class AddressMappingExtension
{
   public static AddressDTO? ToDTO(this Address? address)
    {
        if(address is null) return null;

        return new AddressDTO
        {
             Line1 = address.Line1,
             Line2 = address.Line2,
             City = address.City,
             PostalCode = address.PostalCode,
             State = address.PostalCode,
             Country = address.Country
        };
    }

    public static Address ToEntity(this AddressDTO addressDTO)
    {
        if(addressDTO is null) throw new ArgumentNullException(nameof(addressDTO));

        return new Address
        {
             Line1 = addressDTO.Line1,
             Line2 = addressDTO.Line2,
             City = addressDTO.City,
             PostalCode = addressDTO.PostalCode,
             State = addressDTO.PostalCode,
             Country = addressDTO.Country
        };
    }

    public static void UpdateFromDTO(this Address address, AddressDTO addressDTO)
    {
        if(addressDTO is null) throw new ArgumentNullException(nameof(addressDTO));
        if(address is null) throw new ArgumentNullException(nameof(address));

        address.Line1 = addressDTO.Line1;
        address.Line2 = addressDTO.Line2;
        address.City = addressDTO.City;
        address.PostalCode = addressDTO.PostalCode;
        address.State = addressDTO.PostalCode;
        address.Country = addressDTO.Country;
    }
}
