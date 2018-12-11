using System.Collections.Generic;
using System.Linq;
using uStora.Common.Services.Int32;
using uStora.Data.Infrastructure;
using uStora.Data.Repositories;
using uStora.Model.Models;

namespace uStora.Service
{
    public interface IManufactorService : ICrudService<Manufactor>, IGetDataService<Manufactor>
    {
    }

    public class ManufactorService : IManufactorService
    {
        private readonly IManufactorRepository _manufactorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ManufactorService(IManufactorRepository manufactorRepo,
            IUnitOfWork unitOfWork)
        {
            _manufactorRepo = manufactorRepo;
            _unitOfWork = unitOfWork;
        }

        public Manufactor Add(Manufactor entity)
        {
            return _manufactorRepo.Add(entity);
        }

        public void Delete(int id)
        {
            _manufactorRepo.Delete(id);
        }

        public Manufactor FindById(int id)
        {
           return _manufactorRepo.GetSingleById(id);
        }

        public IEnumerable<Manufactor> GetAll(string keyword = null)
        {
            if (string.IsNullOrEmpty(keyword))
                return _manufactorRepo.GetAll();
            else
            {
                return _manufactorRepo.GetAll().Where(x => x.Name.ToLower().Contains(keyword.ToLower()));
            }
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Update(Manufactor entity)
        {
            _manufactorRepo.Update(entity);
        }
    }
}
