using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kutuphane.Application.Exceptions
{
    public class DuplicateException : Exception
    {

        public DuplicateException(string? message) : base(message)
        {

        }
        public DuplicateException(string entity , string  entityProp , object key):base($"{entity} with {entityProp} already exists ")
        {
            
        }
    }
}
