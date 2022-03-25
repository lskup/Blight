﻿using Blight.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Blight.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Blight.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Blight.Repository
{
    public class GenericRepository2<T> : IGenericRepository2<T> where T : class
    {
        protected readonly BlightDbContext _blightDbContext;
        internal DbSet<T> _dbSet;
        private readonly IMapper _mapper;

        public GenericRepository2(BlightDbContext blightDbContext, IMapper mapper)
        {
            _blightDbContext = blightDbContext;
            _dbSet = _blightDbContext.Set<T>();
            _mapper = mapper;
        }

        public virtual async Task<T> Create(IDto dto)
        {
            var mappedObject =_mapper.Map<T>(dto);

            var result = await _dbSet.AddAsync(mappedObject);

            if (result.State != EntityState.Added)
            {
                throw new DataBaseException("Something went wrong");
            }

            await _blightDbContext.SaveChangesAsync();

            return mappedObject;
        }

        public virtual async Task Delete(int id)
        {
            var entity = await GetById(id);

            var result = _dbSet.Remove(entity);

            if (result.State != EntityState.Deleted)
            {
                throw new DataBaseException("Something went wrong");
            }
            await _blightDbContext.SaveChangesAsync();
        }

        public virtual async Task<T> FindElement(Expression<Func<T, bool>> predicate)
        {
            var result = await _dbSet.SingleOrDefaultAsync(predicate);

            return result;
        }

        public virtual async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> predicate)
        {
            if (predicate is null)
            {
                return await _dbSet
                    .AsNoTracking()
                    .ToListAsync();
            }

            return await _dbSet
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<T> GetById(int id)
        {
            var result = await _dbSet.FindAsync(id);
            if (result is null)
            {
                throw new NotFoundException("Element not Found");
            }
            
            return result;
        }

        public virtual Task<T> Update(int id, IDto dto)
        {
            throw new NotImplementedException();
        }

    }
}
