using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Members
{
    public  class Genres : IDisposable
    {

        [DataMember]
        public int ID
        {
            get; set;
        }

        [DataMember]
        public string Name { get; set; }
        public void Dispose()
        {
        }
    }
}
