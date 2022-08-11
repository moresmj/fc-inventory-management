using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Users
{
    public class UserDealer : User
    {
        public IList<int> StoresHandled { get; set; }

        public List<Store> Handled { get; set; }
    }
}
