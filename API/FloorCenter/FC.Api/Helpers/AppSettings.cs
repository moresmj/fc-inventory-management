using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Helpers
{
    public class AppSettings
    {

        public string Secret { get; set; }

        public int TokenEpiration { get; set; }

        public string Item_temp_image { get; set; }

        public string Item_image { get; set; }

        public int RecordDisplayPerPage { get; set; }

        public Dictionary<string, int?> StoreSeries { get; set;}

        public Dictionary<string, int?> StoreTransferSeries { get; set; }

        public string LogPath { get; set; }

    }
}
