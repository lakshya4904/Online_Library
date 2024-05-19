using System;
using System.Runtime.Serialization;

namespace Entities.Members;

public class Book : IDisposable
{
    [DataMember]
    public int ID { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string FirstName { get; set; }

    [DataMember]
    public string LastName { get; set; }

    [DataMember]
    public DateTime? PublicationDate { get; set; }

    [DataMember]
    public string Publisher { get; set; }

    [DataMember]
    public string Edition { get; set; }

    [DataMember]
    public string ISBN { get; set; }

    [DataMember]
    public string LanguageCode { get; set; }

    [DataMember]
    public int? Pages { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Format { get; set; }

    [DataMember]
    public bool IsAvailable { get; set; }

    public void Dispose()
    {
        // Implement resource disposal logic if needed
    }
}

