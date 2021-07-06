using System.Collections.Generic;

namespace NiksoftCore.FormBuilder.Service
{
    public class FormAnswer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AnswerValue { get; set; }
        public string AnswerText { get; set; }
        public bool AnswerBoolValue { get; set; }
        public int ControlType { get; set; }
        public int MaxValue { get; set; }
        public string MaxMessage { get; set; }
        public int MinValue { get; set; }
        public string MinMessage { get; set; }
        public bool IsRequired { get; set; }
        public string EmptyMessage { get; set; }
        public int OrderId { get; set; }
        public List<AnswerItem> Items { get; set; }
    }
}
