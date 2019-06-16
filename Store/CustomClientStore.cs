﻿using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using auth.Interface;

namespace auth.Store
{
    public class CustomClientStore : IClientStore
    {
        protected IRepository _dbRepository;

        public CustomClientStore(IRepository repository)
        {
            _dbRepository = repository;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _dbRepository.Single<Client>(c => c.ClientId == clientId);

            return Task.FromResult(client);
        }
    }
}
