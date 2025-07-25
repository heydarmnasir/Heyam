using System;

namespace BE.ViewModel
{
    public class ReminderDto
    {
        public string Title { get; set; } // مثلاً موضوع جلسه یا قسط یا لایحه
        public DateTime ReminderDate { get; set; }
        public string Description { get; set; } // توضیح یا خلاصه
    }
}