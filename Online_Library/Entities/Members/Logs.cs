using System;
using System.Runtime.Serialization;

namespace Entities.Members;

public class Logs : IDisposable
{
    [DataMember]
    public int Id
    {
        get; set;
    }
    [DataMember]
    public string Level
    {
        get; set;
    }
    [DataMember]
    public string CallSite
    {
        get; set;
    }
    [DataMember]
    public string Type
    {
        get; set;
    }
    [DataMember]
    public string Message
    {
        get; set;
    }
    [DataMember]
    public string StackTrace
    {
        get; set;
    }
    [DataMember]
    public string InnerException
    {
        get; set;
    }
    [DataMember]
    public string AdditionalInfo
    {
        get; set;
    }
    [DataMember]
    public DateTime LoggedOnDate
    {
        get => DateTime.Parse("1/1/1970");
        set { }
    }

    public void Dispose()
    {
    }
}
