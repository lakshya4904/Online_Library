using System;

namespace Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryParamIgnoreAttribute : Attribute
    {
    }
}
