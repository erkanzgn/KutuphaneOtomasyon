using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Dtos.AuthDtos;

public class AuthResultDto
{
    public UserDto User { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
}
