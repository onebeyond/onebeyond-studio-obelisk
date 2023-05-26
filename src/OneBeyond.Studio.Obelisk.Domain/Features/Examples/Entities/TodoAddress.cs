using OneBeyond.Studio.Domain.SharedKernel.Entities;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Examples.Entities;

public sealed class TodoAddress : ValueObject
{
    public TodoAddress(
        int houseNo,
        string city,
        string zipCode)
    {
        HouseNo = houseNo;
        City = city;
        ZipCode = zipCode;
    }

    public int HouseNo { get; }
    public string City { get; }
    public string ZipCode { get; }
}
