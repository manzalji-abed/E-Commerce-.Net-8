using Microsoft.AspNetCore.Mvc.Rendering;
using MvcBul.DataAccess.Data;
using MvcBul.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcBul.DataAccess.Repository
{
    public class UnitOfWork :IUnitOfWork
    {
        private ApplicationDbContext _context;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }

        public ICompanyRepository Company { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; set; }

        public IApplicationUserRepository ApplicationUser { get; set; }
        public IOrderHeaderRepository OrderHeader { get; set; }
        public IOrderDetailRepository OrderDetail { get; set; }

        public IProductImageRepository ProductImage { get; set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategotyRepository(context);
            Product = new ProductRepository(context);
            Company = new CompanyRepository(context);
            ShoppingCart = new ShoppingCartRespository(context);
            ApplicationUser = new ApplicationUserRepository(context);
            OrderHeader = new OrderHeaderRepository(context);
            OrderDetail = new OrderDetailRepository(context);
            ProductImage = new ProductImageRepository(context);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
