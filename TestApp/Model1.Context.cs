﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestApp
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ElectroShopBDEntities : DbContext
    {
        private static ElectroShopBDEntities _context;
        public ElectroShopBDEntities()
            : base("name=ElectroShopBDEntities")
        {
        }

        public static ElectroShopBDEntities GetContext()
        {
            if (_context == null)
            {
                _context = new ElectroShopBDEntities();
            }
            return _context;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Вид_товара> Вид_товара { get; set; }
        public virtual DbSet<Данные_пользователя> Данные_пользователя { get; set; }
        public virtual DbSet<Заказ> Заказ { get; set; }
        public virtual DbSet<Заказ_Товар> Заказ_Товар { get; set; }
        public virtual DbSet<Пользователь> Пользователь { get; set; }
        public virtual DbSet<Пункт_Выдачи> Пункт_Выдачи { get; set; }
        public virtual DbSet<Роль_пользователя> Роль_пользователя { get; set; }
        public virtual DbSet<Статус_заказа> Статус_заказа { get; set; }
        public virtual DbSet<Товар> Товар { get; set; }
    }
}
