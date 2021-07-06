using System;

namespace NiksoftCore.SystemBase.Service
{
    public class SystemFile
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Extention { get; set; }
        public FileType FileType { get; set; }
        public string ContentId { get; set; }
        public string ContentKey { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
