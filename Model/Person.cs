using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Person:EmailAddress
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
