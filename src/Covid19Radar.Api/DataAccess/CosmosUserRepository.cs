﻿using Covid19Radar.Api.DataStore;
using Covid19Radar.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Covid19Radar.Api.DataAccess
{
    public class CosmosUserRepository : IUserRepository
    {
        const string SequenceName = "JumpConsistentSeed";
        private readonly ICosmos _db;
        private readonly ISequenceRepository _sequence;
        private readonly ILogger<CosmosUserRepository> _logger;

        public CosmosUserRepository(
            ICosmos db,
            ISequenceRepository sequence,
            ILogger<CosmosUserRepository> logger)
        {
            _db = db;
            _sequence = sequence;
            _logger = logger;
        }

        public async Task<UserModel?> GetById(string id)
        {
            var itemResult = await _db.User.ReadItemAsync<UserModel>(id, new PartitionKey(id));
            if (itemResult.StatusCode == HttpStatusCode.OK)
            {
                return itemResult.Resource;
            }

            return null;
        }

        public async Task Create(UserModel user)
        {
            user.JumpConsistentSeed = await _sequence.GetNextAsync(SequenceName, 1);
            var r = await _db.User.CreateItemAsync(user, new PartitionKey(user.PartitionKey));
            _logger.LogInformation($"{nameof(Create)} RequestCharge:{r.RequestCharge}");
        }

        public async Task<bool> Exists(string id)
        {
            bool userFound = false;
            try
            {
                var userResult = await _db.User.ReadItemAsync<UserModel>(id, new PartitionKey(id));
                if (userResult.StatusCode == HttpStatusCode.OK)
                {
                    userFound = true;
                }
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == HttpStatusCode.NotFound)
                {
                    userFound = false;
                }
            }

            return userFound;
        }

        public async Task<bool> Delete(IUser user)
        {
            bool userFound = false;
            try
            {
                var result = await _db.User.DeleteItemAsync<UserModel>(user.GetId(), new PartitionKey(user.GetId()));
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    userFound = true;
                }
            }
            catch (CosmosException cosmosException)
            {
                if (cosmosException.StatusCode == HttpStatusCode.NotFound)
                {
                    userFound = false;
                }
            }
            return userFound;
        }

    }
}
