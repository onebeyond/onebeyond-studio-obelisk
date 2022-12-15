using Microsoft.AspNetCore.Mvc;
using OneBeyond.Studio.Domain.SharedKernel.Entities.Dto;

namespace OneBeyond.Studio.Obelisk.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/v{version:apiVersion}")]
public abstract class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
{
    protected ControllerBase()
    {
    }

    protected string QueryString
    {
        get
        {
            var queryString = Request.QueryString.ToUriComponent();
            if (queryString.Length > 0)
            {
                queryString = queryString.Substring(1);
            }
            return queryString;
        }
    }

    protected IActionResult Json(object value)
        => new JsonResult(value);

    protected FileStreamResult File(FileContentDto dto)
        => File(dto.Content, dto.ContentType, dto.Name);
}
