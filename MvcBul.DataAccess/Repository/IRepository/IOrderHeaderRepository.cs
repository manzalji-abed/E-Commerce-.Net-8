using Microsoft.AspNetCore.Mvc.Rendering;
using MvcBul.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcBul.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository :  IRepository<OrderHeader>
    {
        void Update (OrderHeader orderHeader);
        void UpdateStatus(int id, string orderStatus, string paymentStatus = null);
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}
