using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Exceptions
{
    public class CopyNotAvailableException : Exception
    {
  
        public CopyNotAvailableException(string? message) : base(message)
        {
        } 
        public CopyNotAvailableException(object key):base($" Copy Id no : {key} , Copy  is not available")
        {
        }

    }
}
