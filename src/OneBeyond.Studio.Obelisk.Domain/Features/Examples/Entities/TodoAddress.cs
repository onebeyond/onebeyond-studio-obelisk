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

    //NOTE! In case if we want to do bulk insert, bulk updatable properties MUST HAVE A PRIVATE SETTER

    public int HouseNo { get; private set; }
    public string City { get; private set; }
    public string ZipCode { get; private set; }
}
