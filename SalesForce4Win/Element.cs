using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesForce4Win
{
    class Element
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    class Field
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
    }
}
