using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Crosscuts.Json;

namespace OneBeyond.Studio.Obelisk.WebApi.Extensions;

internal static class MvcJsonOptionsExtension
{
    public static MvcNewtonsoftJsonOptions AddPrivateSettersSerialization(this MvcNewtonsoftJsonOptions @this)
    {
        @this.SerializerSettings.ContractResolver = new PrivateSetterCamelCasePropertyNamesContractResolver();
        return @this;
    }
}
