using BE;
using BE.ViewModel;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class PersonalNote_BLL
    {
        PersonalNote_DAL PersonalNote_dal = new PersonalNote_DAL();
        public int Create(PersonalNote personalNote) => PersonalNote_dal.Create(personalNote);
        public List<PersonalNote> GetAllPersonalNotes() => PersonalNote_dal.GetAllPersonalNotes();
        public List<PersonalNotesDto> GetPersonalNotesForListView()
        {
            var PersonalNotes = PersonalNote_dal.GetAllPersonalNotes(); // فرض بر اینکه این متد شامل Join با Client است

            var result = PersonalNotes.Select(i => new PersonalNotesDto
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.Category,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                ReminderDate = i.ReminderDate ?? default(DateTime),
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public PersonalNote GetPersonalNoteId(int id) => PersonalNote_dal.GetPersonalNoteId(id);
        public List<PersonalNote> Search(string searchTerm) => PersonalNote_dal.Search(searchTerm);
        public List<PersonalNotesDto> SearchPersonalNotesForDGV(string searchTerm)
        {
            var PersonalNotes = PersonalNote_dal.Search(searchTerm); // فرض بر اینکه این متد شامل Join با Client است
            var result = PersonalNotes.Select(i => new BE.ViewModel.PersonalNotesDto
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.Category,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                ReminderDate = i.ReminderDate ?? default(DateTime),
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public string Delete(int notesId) => PersonalNote_dal.Delete(notesId);
        public List<PersonalNote> GetTodayReminders() => PersonalNote_dal.GetTodayReminders();
        public List<PersonalNotesDto> GetReminders()
        {
            var personalNotes = PersonalNote_dal.GetReminders(); // فرض بر اینکه این متد شامل Join با Client است
            var result = personalNotes.Select(i => new BE.ViewModel.PersonalNotesDto
            {
                Id = i.Id,
                Title = i.Title,
                Category = i.Category,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                ReminderDate = i.ReminderDate ?? default(DateTime),
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
    }
}