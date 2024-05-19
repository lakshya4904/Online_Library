using Entities.Members;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

#pragma warning disable 1591


namespace Entities
{
    [DataContract]
    [Serializable]

    public class PagedResults<T>
    {
        [DataMember]
        public List<T> Items
        {
            get; set;
        }

        [DataMember]
        public int CurrentPage
        {
            get; set;
        }

        [DataMember]
        public int PageCount => (int)Math.Ceiling((double)TotalCount / PageSize);

        [DataMember]
        public int TotalCount
        {
            get; set;
        }

        [DataMember]
        public int PageSize
        {
            get; set;
        }
        [DataMember]
        public double OrderTotal
        {
            get; set;
        }
        [DataMember]
        public double CreditTotal
        {
            get; set;
        }


        public PagedResults()
        {
            Items = new List<T>();
            PageSize = 10;
            OrderTotal = 0;
            CreditTotal = 0;
        }
    }
}
