using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Members;

public class BookGenres : IDisposable
{
    [DataMember]
    public int ID { get; set; }  // New ID field

    [DataMember]
    public int BookID { get; set; }

    [DataMember]
    public int GenreID { get; set; }


    public void Dispose()
    {

    }
}
