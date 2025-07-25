using BE;
using BE.ViewModel;
using DAL;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class Interaction_BLL
    {
        Interaction_DAL interaction_DAL = new Interaction_DAL();     
        public int Create(Interaction interaction) => interaction_DAL.Create(interaction);
        public List<Interaction> GetAllInteractions() => interaction_DAL.GetAllInteractions();
        public List<BE.ViewModel.InteractionDto> GetInteractionsForListView()
        {
            var interactions = interaction_DAL.GetAllInteractions(); // فرض بر اینکه این متد شامل Join با Client است
            var result = interactions.Select(i => new BE.ViewModel.InteractionDto
            {
                Id = i.Id,
                ClientName = i.Client.FullName,
                Type = i.Type,
                InteractionDate = i.InteractionDate,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public Interaction GetInteractionById(int id) => interaction_DAL.GetInteractionById(id);
        public string UpdateInteraction(Interaction updated) => interaction_DAL.UpdateInteraction(updated);
        public List<Interaction> SearchByClientName(string clientName) => interaction_DAL.SearchByClientName(clientName);
        public List<BE.ViewModel.InteractionDto> SearchInteractionForDGV(string clientName)
        {
            var interactions = interaction_DAL.SearchByClientName(clientName); // فرض بر اینکه این متد شامل Join با Client است
            var result = interactions.Select(i => new BE.ViewModel.InteractionDto
            {
                Id = i.Id,
                ClientName = i.Client.FullName,
                Type = i.Type,
                InteractionDate = i.InteractionDate,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public List<Interaction> GetTodayReminders() => interaction_DAL.GetTodayReminders();
        public List<InteractionDto> GetReminders()
        {
            var interactions = interaction_DAL.GetReminders(); // فرض بر اینکه این متد شامل Join با Client است
            var result = interactions.Select(i => new BE.ViewModel.InteractionDto
            {
                Id = i.Id,
                ClientName = i.Client.FullName,
                Type = i.Type,
                InteractionDate = i.InteractionDate,
                IsReminderSet = i.IsReminderSet ? "✅ دارد" : "❌ ندارد",
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
    }
}