using SitePlugin;
using System.Collections.Generic;

namespace YouTubeLiveSitePlugin
{
    public class Metadata : IMetadata
    {
        public string Title { get; set; }
        public string Elapsed { get; set; }
        public string CurrentViewers { get; set; }
        public string Active { get; set; }
        public string TotalViewers { get; set; }
        public bool? IsLive { get; set; }
        public string Others { get; set; }
        public override string ToString()
        {
            var list = new List<string>();
            if (Title != null)
            {
                list.Add($"Title:{Title}");
            }
            if (Elapsed != null)
            {
                list.Add($"Elapsed:{Elapsed}");
            }
            if (CurrentViewers != null)
            {
                list.Add($"CurrentViewers:{CurrentViewers}");
            }
            var s = string.Join(" ", list);
            return s;
        }
    }
}
