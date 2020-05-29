﻿using Covid19Radar.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Covid19Radar.Api.DataAccess
{
    public interface IUserRepository
    {
        Task<UserModel?> GetById(string id);

        Task Create(UserModel user);

        Task<bool> Exists(string id);

        Task<bool> Delete(IUser user);
    }
}
