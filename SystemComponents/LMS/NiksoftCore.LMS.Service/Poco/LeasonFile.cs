using System;

namespace NiksoftCore.LMS.Service
{
    public class LeasonFile
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileSrc { get; set; }
        public FileType FileType { get; set; }
        public int LeasonId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CraeteBy { get; set; }

        public virtual Leason Leason { get; set; }
    }
}
