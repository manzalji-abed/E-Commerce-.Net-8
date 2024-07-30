using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvcBul.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProps = null);
        T Get(Expression<Func<T,bool>> filter, string? includeProps = null, bool tracked = false);
        Task Add(T entity);
        Task Delete(T entity);
        Task DeleteRange(IEnumerable<T> entities);

    }
}
