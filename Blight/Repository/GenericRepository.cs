using Blight.Interfaces;
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
using Blight.Models;
using Blight.Interfaces.MethodsProvider;

namespace Blight.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class,IDto
    {
        protected readonly BlightDbContext _blightDbContext;
        internal DbSet<T> _dbSet;
        private readonly IMapper _mapper;
        private readonly IPaginating _pagination;

        public GenericRepository(BlightDbContext blightDbContext, IMapper mapper, IPaginating pagination)
        {
            _blightDbContext = blightDbContext;
            _dbSet = _blightDbContext.Set<T>();
            _mapper = mapper;
            _pagination = pagination;
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
            var entity = await GetById(id) as T;

            var result = _dbSet.Remove(entity);

            if (result.State != EntityState.Deleted)
            {
                throw new DataBaseException("Something went wrong");
            }
            await _blightDbContext.SaveChangesAsync();
        }

        public virtual async Task<T> FindElement(Expression<Func<T, bool>> predicate)
        {
            var result = await _dbSet
                .FirstOrDefaultAsync(predicate);

            return result;
        }

        public virtual async Task<IPagedResult<IDto>> GetAllPaginated(IPaginationObj paginationQuery)
        {
            var paginationObj = paginationQuery as PaginationQuery;

            var list = _dbSet
                .AsNoTracking()
                .AsQueryable();

            var pagedResult = _pagination.Paginate(list, paginationQuery);

            return pagedResult;
        }

        public virtual async Task<IDto> GetById(int id)
        {
            var result = await _dbSet.FindAsync(id);
            if (result is null)
            {
                throw new NotFoundException("Element not found");
            }

            return result;
        }

        public async Task<P> GetById<P>(int id) where P : IDto
        {
            var iDto = await GetById(id);

            var mappedDto = _mapper.Map<P>(iDto);

            return mappedDto;
        }


        public virtual async Task<IDto> Update(int id, IDto dto)
        {
            var objExistingInDb = await GetById(id);

            var dtoProperties = dto.GetType()
                .GetProperties();

            var existingObjProperties = objExistingInDb.GetType()
                .GetProperties();

            for (int i = 0; i < existingObjProperties.Length; i++)
            {
                for (int j = 0; j < dtoProperties.Length; j++)
                {
                    if (existingObjProperties[i].Name == dtoProperties[j].Name)
                    {
                        object? propertyValue = dtoProperties[j].GetValue(dto);

                        if (propertyValue != null)
                        {
                            existingObjProperties[i].SetValue(objExistingInDb, propertyValue);
                        }
                    }
                }
            }

            _blightDbContext.Entry(objExistingInDb)
                .CurrentValues
                .SetValues(objExistingInDb);

            await _blightDbContext.SaveChangesAsync();

            return objExistingInDb;

        }

    }
}
