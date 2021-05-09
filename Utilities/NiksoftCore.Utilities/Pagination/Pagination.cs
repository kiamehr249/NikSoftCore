using System;

namespace NiksoftCore.Utilities
{
    public class Pagination
    {
        public int TotalSize { get; set; }
        public int PageSize { get; set; }
        public int Part { get; set; }
        public int StartIndex { get; set; }
        public int MaxShow { get; set; }
        public int TotalParts { get; set; }

        public Pagination(int totalSize, int pageSize, int part)
        {
            TotalSize = totalSize;
            PageSize = pageSize;
            Part = part == 0 ? 1 : part;
            StartIndex = GetSkip();
            MaxShow = 10;
            TotalParts = GetTotalParts();
        }

        public int GetSkip()
        {
            if (Part == 1 || Part == 0)
            {
                return 0;
            }
            return (Part - 1) * PageSize;
        }

        public int GetTotalParts()
        {
            return Convert.ToInt32(Math.Ceiling((double)TotalSize / PageSize));
        }

        public int GetStartShow()
        {
            if (MaxShow > Part)
            {
                return 1;
            }
            else
            {
                return Convert.ToInt32(Math.Floor((double)Part / MaxShow)) * MaxShow;
            }
        }

        public int GetEndShow()
        {
            
            if (Part >= (TotalParts - MaxShow))
            {
                return TotalParts;
            }
            else
            {
                return GetStartShow() + MaxShow;
            }

        }

        public int GetBackRang()
        {
            return GetStartShow() - MaxShow;
        }
    }
}
