using OneBeyond.Studio.Domain.SharedKernel.Entities;
using OneBeyond.Studio.Obelisk.Domain.Attributes;

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

    //NOTE! In case if we want to do bulk insert, bulk updatable properties must have a private setter

    //NOTE! BulkUpdateName should correspond to TodoItemConfiguration inInfrastructure!

    [BulkUpdateColumnName("HouseNo")]
    public int HouseNo { get; private set; }
    public string City { get; private set; }
    [BulkUpdateColumnName("Zip")]
    public string ZipCode { get; private set; }
}
