using System;
using Adin.BankPayment.Domain.Cache;
using Adin.BankPayment.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Adin.BankPayment.Domain.Context
{
    public class BankPaymentContext : DbContext
    {
        public BankPaymentContext(DbContextOptions<BankPaymentContext> options) : base(options)
        {
            Init();
        }

        public BankPaymentContext()
        {
            Init();
        }

        public DbSet<Application> Applications { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Bank> Banks { get; set; }

        public DbSet<ApplicationBank> ApplicationBanks { get; set; }

        public DbSet<ApplicationBankParam> ApplicationBankParams { get; set; }

        protected internal virtual void Init()
        {
            //  this.InitializeDynamicFilters();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionStringGetter.ConStr);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Transaction>().HasQueryFilter(p => p.IsDeleted == false);
            modelBuilder.Entity<Transaction>().Property(p => p.Amount).HasColumnType("decimal(18, 2)");


            modelBuilder.Entity<Bank>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Bank>().HasQueryFilter(p => p.IsDeleted == false);

            modelBuilder.Entity<Application>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Application>().HasQueryFilter(p => p.IsDeleted == false);

            modelBuilder.Entity<ApplicationBank>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<ApplicationBank>().HasQueryFilter(p => p.IsDeleted == false);

            modelBuilder.Entity<ApplicationBankParam>().Property(x => x.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<ApplicationBankParam>().HasQueryFilter(p => p.IsDeleted == false);

            modelBuilder.Entity<Bank>().HasData(
                new Bank
                {
                    Code = 1,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    IsDeleted = false,
                    PostUrl = "https://sep.shaparak.ir/MobilePG/MobilePayment",
                    Title = "سامان",
                    Id = Guid.Parse("482a591e-7536-4f47-a544-e9d4342586bd"),
                    Status = 0
                },
                new Bank
                {
                    Code = 2,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    IsDeleted = false,
                    PostUrl = "https://pec.shaparak.ir/NewIPG?Token={0}",
                    Title = "پارسیان",
                    Id = Guid.Parse("ab3f226a-be56-4092-bbd0-2ae8ffbce131"),
                    Status = 0
                },
                new Bank
                {
                    Code = 3,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    IsDeleted = false,
                    PostUrl = "https://bpm.shaparak.ir/pgwchannel/startpay.mellat",
                    Title = "ملت",
                    Id = Guid.Parse("98504148-3d89-4abb-9fb5-281bed8714e3"),
                    Status = 0
                },
                new Bank
                {
                    Code = 4,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    IsDeleted = false,
                    PostUrl = "https://pf.efarda.ir/pf/api/ipg/purchase",
                    Title = "تجارت الکترونیکی ارتباط فردا",
                    Id = Guid.Parse("8c4d3794-983f-4610-bf88-abe8db1ad07d"),
                    Status = 0
                },
                new Bank
                {
                    Code = 5,
                    CreatedBy = 1,
                    CreationDate = DateTime.Now,
                    IsDeleted = false,
                    PostUrl = "https://pep.shaparak.ir/payment.aspx",
                    Status = (byte)0,
                    Title = "پاسارگاد",
                    Id = new Guid("e147830f-696d-4c4a-a7b4-b20722dd3ff6")
                });
        }
    }
}