using BE;
using BE.ViewModel;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class Meeting_BLL
    {
        Meeting_DAL meeting_dal = new Meeting_DAL();
        public int Create(Meeting meeting) => meeting_dal.Create(meeting);
        public List<Meeting> GetAllMeetings() => meeting_dal.GetAllMeetings();
        public List<BE.ViewModel.MeetingDto> GetAllMeetingsForListView()
        {
            var meetings = meeting_dal.GetAllMeetings(); // فرض بر اینکه این متد شامل Join با Client است
            var result = meetings.Select(i => new BE.ViewModel.MeetingDto
            {
                Id = i.Id,
                ClientName = i.Client.FullName,
                MeetingSubject = i.MeetingSubject,
                MeetingDateTime = i.MeetingDateTime,
                IsReminderNextMeetingSet = i.IsReminderMeetingSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public Meeting GetMeetingById(int id) => meeting_dal.GetMeetingById(id);
        public List<Meeting> SearchMeeting(string searchTerm) => meeting_dal.SearchMeeting(searchTerm);
        public List<BE.ViewModel.MeetingDto> SearchMeetingForDGV(string clientName = null, DateTime? meetingDate = null)
        {
            var meetings = meeting_dal.SearchMeeting(clientName, meetingDate);
            var result = meetings.Select(i => new BE.ViewModel.MeetingDto
            {
                Id = i.Id,
                ClientName = i.Client.FullName,
                MeetingSubject = i.MeetingSubject,
                MeetingDateTime = i.MeetingDateTime,
                IsReminderNextMeetingSet = i.IsReminderMeetingSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public List<Meeting> GetTodayAndPreReminderMeetings() => meeting_dal.GetTodayAndPreReminderMeetings();
        public List<MeetingDto> GetReminders()
        {
            var meetings = meeting_dal.GetReminders();
            var result = meetings.Select(i => new BE.ViewModel.MeetingDto
            {
                Id = i.Id,
                ClientName = i.Client.FullName,
                MeetingSubject = i.MeetingSubject,
                MeetingDateTime = i.MeetingDateTime,
                IsReminderNextMeetingSet = i.IsReminderMeetingSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
    }
}