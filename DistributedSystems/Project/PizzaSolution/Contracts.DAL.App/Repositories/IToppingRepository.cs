﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.DAL.Base.Repositories;
using Domain;
using PublicApi.DTO.v1;

namespace Contracts.DAL.App.Repositories
{
    public interface IToppingRepository : IBaseRepository<Topping>
    {
        Task<ToppingDTO> SelectDTO();
        Task<IEnumerable<ToppingDTO>> SelectAllDTO();
    }
}