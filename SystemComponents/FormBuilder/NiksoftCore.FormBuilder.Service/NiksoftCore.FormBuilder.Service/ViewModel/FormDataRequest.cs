namespace NiksoftCore.FormBuilder.Service
{
    public class FormDataRequest
    {
        public int Id { get; set; }
        public string AnswerObject { get; set; }
        public int FormId { get; set; }
        public int? UserId { get; set; }
        public string UserCookie { get; set; }
        public string UserIP { get; set; }
    }
}
