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
    public class ApplicationBankParamRepository : IRepository<ApplicationBankParam>
    {
        private readonly BankPaymentContext context;
        private readonly DbSet<ApplicationBankParam> entity;

        public ApplicationBankParamRepository(BankPaymentContext context)
        {
            this.context = context;
            entity = context.Set<ApplicationBankParam>();
        }

        public async Task<ApplicationBankParam> Get(Guid id)
        {
            return await entity.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ApplicationBankParam>> GetAll()
        {
            return await entity.ToListAsync();
        }

        public async Task Add(ApplicationBankParam obj)
        {
            context.Entry(obj).State = EntityState.Added;
            await context.SaveChangesAsync();
        }

        public async Task Update(ApplicationBankParam obj)
        {
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var obj = await Get(id);
            obj.IsDeleted = true;
            await Update(obj);
        }

        public async Task<IEnumerable<ApplicationBankParam>> GetAllBy(
            Expression<Func<ApplicationBankParam, bool>> predicate)
        {
            return await entity.Where(predicate).ToListAsync();
        }

        public async Task<ApplicationBankParam> GetFirstBy(Expression<Func<ApplicationBankParam, bool>> predicate)
        {
            return await entity.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task DeletePermanently(Guid id)
        {
            await entity.Where(x => x.Id == id).DeleteAsync();
        }

        public async Task<List<ApplicationBankParam>> GetByApplicationBank(Guid id)
        {
            return await entity.Where(x => x.ApplicationBankId == id).ToListAsync();
        }
    }
}