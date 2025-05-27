using Application.Models.DTOs;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ClientService(UserManager<User> userManager, IProfessionRepository professionRepository) : IClientService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IProfessionRepository _professionRepository = professionRepository;
    }
}