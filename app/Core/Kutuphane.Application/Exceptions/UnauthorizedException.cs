using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Exceptions;

public class UnauthorizedException:Exception
{
    public UnauthorizedException(string message):base(message)
    {
        
    }
    public UnauthorizedException():base("You are not authorized to perform this action")
    {
        
    }
}
