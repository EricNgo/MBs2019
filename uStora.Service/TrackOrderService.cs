using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uStora.Common.Services.Int32;
using uStora.Data.Infrastructure;
using uStora.Data.Repositories;
using uStora.Model.Models;

namespace uStora.Service
{
    public interface ITrackOrderService : ICrudService<TrackOrder>, IGetDataService<TrackOrder>
    {
        IEnumerable<TrackOrder> GetByUserId(string userId);
        IEnumerable<TrackOrder> GetByUserIdPaging(string userId, string keyword, int page, int pageSize, out int totalRow);

        IEnumerable<TrackOrder> GetLongLatByOrderId(int orderId);

        IEnumerable<TrackOrder> GetLocation(string cusId);
    }
    public class TrackOrderService : ITrackOrderService
    {
        private ITrackOrderRepository _trackOrderRepository;
        private IUnitOfWork _unitOfWork;
        public TrackOrderService(ITrackOrderRepository trackOrderRepository,
           IUnitOfWork unitOfWork)
        {
            _trackOrderRepository = trackOrderRepository;
            _unitOfWork = unitOfWork;
        }

        public TrackOrder Add(TrackOrder trackOrder)
        {
            return _trackOrderRepository.Add(trackOrder);
        }

        public void Delete(int id)
        {
            _trackOrderRepository.Delete(id);
        }

        public IEnumerable<TrackOrder> GetAll()
        {
            return _trackOrderRepository.GetAll(new string[] { "Order", "ApplicationUser"});
        }

        public IEnumerable<TrackOrder> GetAll(string keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    return _trackOrderRepository.GetMulti(x=>x.Status == true,new string[] { "Order", "ApplicationUser" }).OrderByDescending(x => x.Status);
                }
                else
                    return _trackOrderRepository.GetMulti(x => x.Status == true && x.Order.CustomerName.Contains(keyword)
                    || x.Order.CreatedDate.ToString().Contains(keyword),
                    new string[] { "Order", "ApplicationUser"}).OrderByDescending(x => x.Status);
            }
            catch
            {
                throw;
            }
        }

        public TrackOrder FindById(int id)
        {
            return _trackOrderRepository.GetSingleById(id);
        }

        //public IEnumerable<TrackOrder> GetByUserId(string userId)
        //{
        //    return _trackOrderRepository.GetMulti(x => x.UserId == userId && x.Status == true);
        //}
        public IEnumerable<TrackOrder> GetByUserId(string userId)
        {
            return _trackOrderRepository.GetMulti(x => x.UserId == userId && x.Status == true);
        }

        public IEnumerable<TrackOrder> GetByUserIdPaging(string userId,string keyword, int page, int pageSize, out int totalRow)
        {
            //return _trackOrderRepository.GetMulti(x => x.UserId == userId && x.Status == true);

            var query = _trackOrderRepository.GetMulti(x => x.UserId == userId && x.Status == true);
            if (string.IsNullOrEmpty(keyword))
            {
                totalRow = query.Count();
                return query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize);
            }
            else
            {
                query = _trackOrderRepository.GetMulti(x => x.Order.CustomerName.Contains(keyword) || x.Order.CustomerMobile.Contains(keyword), new string[] { "Order" });
                totalRow = query.Count();
                return query.OrderByDescending(x => x.CreatedDate).Skip((page - 1) * pageSize).Take(pageSize);
            }
        }

        public IEnumerable<TrackOrder> GetLongLatByOrderId(int orderId)
        {
            return _trackOrderRepository.GetLongLatByOrderId(orderId);
        }

        public IEnumerable<TrackOrder> GetLocation(string cusId)
        {
            return _trackOrderRepository.GetLocation(cusId);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Update(TrackOrder trackOrder)
        {
            _trackOrderRepository.Update(trackOrder);
        }

        public void IsDeleted(int id)
        {
            throw new NotImplementedException();
        }
    }
}
