using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBeyond.Studio.Obelisk.Domain.Features.Users.Commands;
public class RequestResetPasswordDto
{
    public string? loginId { get; set; }

    public string? ResetPasswordPageUrl { get; set; }

}
