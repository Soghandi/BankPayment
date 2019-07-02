﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Adin.BankPayment.Domain.Context;
using Adin.BankPayment.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Adin.BankPayment.Service
{
    public class BankRepository : IRepository<Bank>
    {
        private readonly BankPaymentContext context;
        private readonly DbSet<Bank> entity;

        public BankRepository(BankPaymentContext context)
        {
            this.context = context;
            entity = context.Set<Bank>();
        }

        public async Task<Bank> Get(Guid id)
        {
            return await entity.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Bank>> GetAll()
        {
            return await entity.ToListAsync();
        }

        public async Task Add(Bank obj)
        {
            context.Entry(obj).State = EntityState.Added;
            await context.SaveChangesAsync();
        }

        public async Task Update(Bank obj)
        {
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var obj = await Get(id);
            obj.IsDeleted = true;
            await Update(obj);
        }

        public async Task<IEnumerable<Bank>> GetAllBy(Expression<Func<Bank, bool>> predicate)
        {
            return await entity.Where(predicate).ToListAsync();
        }

        public async Task<Bank> GetFirstBy(Expression<Func<Bank, bool>> predicate)
        {
            return await entity.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task DeletePermanently(Guid id)
        {
            await entity.Where(x => x.Id == id).DeleteAsync();
        }
    }
}