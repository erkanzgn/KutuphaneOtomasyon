using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Exceptions
{
    public class MemberNotEligibleException : Exception
    {
        public MemberNotEligibleException()
        {

        }

        public MemberNotEligibleException(string? message) : base(message)
        {
        }
    }
}
