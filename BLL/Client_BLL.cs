using BE;
using BE.ViewModel;
using DAL;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class Client_BLL
    {
        Client_DAL client_DAL = new Client_DAL();
        public string Create(Client client)
        {
            return client_DAL.Create(client);
        }
        public bool IsExist(Client client)
        {
            return client_DAL.IsExist(client);
        }
        public List<Client> GetAllClients()
        {
            return client_DAL.GetAllClients();
        }
        public List<BE.ViewModel.ClientDto> GetClientsForListView()
        {
            var clients = client_DAL.GetAllClients(); // فرض بر اینکه این متد شامل Join با Client است
            var result = clients.Select(i => new BE.ViewModel.ClientDto
            {
                Id = i.Id,
                FullName = i.FullName,
                FatherName = i.FatherName,
                NationalCode = i.NationalCode,
                PhoneNumber = i.PhoneNumber,
                Gender = i.Gender,
                UserRole = i.UserRole
            }).ToList();
            return result;
        }
        public List<Client> SearchClients(string keyword, string gender)
        {
            return client_DAL.SearchClients(keyword, gender);
        }
        public List<BE.ViewModel.ClientDto> SearchForListView(string keyword, string gender)
        {
            var clients = client_DAL.SearchClients(keyword, gender);
            var result = clients.Select(i => new BE.ViewModel.ClientDto
            {
                Id = i.Id,
                FullName = i.FullName,
                FatherName = i.FatherName,
                NationalCode = i.NationalCode,
                PhoneNumber = i.PhoneNumber,
                Gender = i.Gender,
                UserRole = i.UserRole,
            }).ToList();
            return result;
        }
        public Client GetClientById(int Id)
        {
            return client_DAL.GetClientById(Id);
        }
        public bool UpdateClientContactInfo(string nationalCode, string phoneNumber, string email, string address)
        {
            return client_DAL.UpdateClientContactInfo(nationalCode, phoneNumber, email, address);
        }        
        public string Delete(int Id)
        {
            return client_DAL.Delete(Id);
        }
        public List<string> ReadFullNames()
        {
            return client_DAL.ReadFullNames();
        }
        public Client ReadByFullName(string FullName)
        {
            return client_DAL.ReadByFullName(FullName);
        }
        public List<Client> ReadClientsList()
        {
            return client_DAL.ReadClientsList();
        }
    }
}