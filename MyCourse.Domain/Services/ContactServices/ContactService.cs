using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyCourse.Domain.Attributes;
using MyCourse.Domain.Data.Interfaces.Repositories;
using MyCourse.Domain.Data.Interfaces.Services;
using MyCourse.Domain.DTOs.ContactRequestDtos;
using MyCourse.Domain.Entities;
using MyCourse.Domain.Exceptions.ContactRequestEx;
using MyCourse.Domain.POCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MyCourse.Domain.Services.ContactServices
{
    [Injectable(ServiceLifetime.Scoped)]
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<ContactService> _logger;
        private readonly IMapper _mapper;
        private readonly SmtpSettings _smtpSettings;

        public ContactService(IContactRepository contactRepository, ILogger<ContactService> logger, IMapper mapper, SmtpSettings smtpSettings)
        {
            _contactRepository = contactRepository;
            _logger = logger;
            _mapper = mapper;
            _smtpSettings = smtpSettings;
        }

        public async Task<IEnumerable<ContactRequestDto>> GetAllContactRequestsAsync()
        {
            var contactRequests = await _contactRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ContactRequestDto>>(contactRequests);
        }

        public async Task<IEnumerable<ContactRequestDto>> GetUnansweredContactRequestsAsync()
        {
            var contactRequests = await _contactRepository.GetUnansweredAsync();
            return _mapper.Map<IEnumerable<ContactRequestDto>>(contactRequests);
        }

        public async Task<IEnumerable<ContactRequestDto>> GetAnsweredContactRequestsAsync()
        {
            var contactRequests = await _contactRepository.GetAnsweredAsync();
            return _mapper.Map<IEnumerable<ContactRequestDto>>(contactRequests);
        }

        public async Task<ContactRequestDto> GetContactRequestByIdAsync(int id)
        {
            var contactRequest = await _contactRepository.GetByIdAsync(id);
            if (contactRequest == null)
            {
                throw new ContactRequestNotFoundException(id);
            }

            return _mapper.Map<ContactRequestDto>(contactRequest);
        }

        public async Task CreateContactRequestAsync(ContactRequestCreateDto createDto)
        {
            if (createDto == null)
            {
                throw new ContactRequestValidationException(null, "Das Kontaktanfrage-Objekt darf nicht null sein.");
            }

            try
            {
                var contactRequest = _mapper.Map<ContactRequest>(createDto);
                contactRequest.DateCreated = DateTime.Now;
                contactRequest.IsAnswered = false;

                await _contactRepository.AddAsync(contactRequest);
                await _contactRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Erstellen der Kontaktanfrage.");
                throw;
            }
        }


        // Hilfsmethoden für die Validierung mit Switch-Ausdrücken
        private string ValidateName(string name) => name switch
        {
            null or "" or " " => "Der Name darf nicht leer sein.",
            _ => string.Empty
        };

        private string ValidateEmail(string email) => email switch
        {
            null or "" or " " => "Die E-Mail-Adresse darf nicht leer sein.",
            _ when !IsValidEmail(email) => "Die E-Mail-Adresse ist ungültig.",
            _ => string.Empty
        };

        private string ValidateSubject(string subject) => subject switch
        {
            null or "" or " " => "Der Betreff darf nicht leer sein.",
            _ => string.Empty
        };

        private string ValidateMessage(string message) => message switch
        {
            null or "" or " " => "Die Nachricht darf nicht leer sein.",
            _ => string.Empty
        };
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public async Task RespondToContactRequestAsync(ContactRequestRespondDto respondDto)
        {
            var contactRequest = await _contactRepository.GetByIdAsync(respondDto.Id);
            if (contactRequest == null)
            {
                _logger.LogWarning("ContactRequest with {id} not found", respondDto.Id);
                throw new ContactRequestNotFoundException(respondDto.Id);
            }

            if (contactRequest.IsAnswered)
            {
                throw new AlreadyAnsweredException(respondDto.Id);
            }

            try
            {
                var smtpClient = new SmtpClient(_smtpSettings.Server)
                {
                    Port = _smtpSettings.Port,
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                    Subject = $"Antwort auf Ihre Kontaktanfrage: {contactRequest.Subject}",
                    Body = respondDto.AnswerMessage,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(contactRequest.Email);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (ContactRequestEmailSendingException ex)
            {
                _logger.LogError(ex, "Fehler beim Senden der Antwort-E-Mail an {Email}", contactRequest.Email);
                throw new ContactRequestEmailSendingException(contactRequest.Id, ex.Message);
            }

            contactRequest.IsAnswered = true;
            contactRequest.AnswerDate = DateTime.Now;
            contactRequest.AnswerMessage = respondDto.AnswerMessage;

            _contactRepository.Update(contactRequest);
            await _contactRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactRequestDto>> SearchContactRequestsAsync(string searchTerm)
        {
            var contactRequests = await _contactRepository.SearchAsync(searchTerm);
            return _mapper.Map<IEnumerable<ContactRequestDto>>(contactRequests);
        }
    }
}
