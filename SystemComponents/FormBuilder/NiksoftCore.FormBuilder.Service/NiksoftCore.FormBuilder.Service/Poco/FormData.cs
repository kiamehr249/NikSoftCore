using System;

namespace NiksoftCore.FormBuilder.Service
{
    public class FormData
    {
        public int Id { get; set; }
        public string AnswerObject { get; set; }
        public int FormId { get; set; }
        public int? UserId { get; set; }
        public string UserCookie { get; set; }
        public string UserIP { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Form Form { get; set; }
        public virtual FormUser User { get; set; }
    }
}
