using System;
using System.Collections.Generic;
using uStora.Common.Services.Int32;
using uStora.Data.Infrastructure;
using uStora.Data.Repositories;
using uStora.Model.Models;

namespace uStora.Service
{
    public interface ITagService : ICrudService<Tag>, IGetDataService<Tag>
    {
        IEnumerable<Tag> GetAllTags();


    }

    public class TagService : ITagService
    {
        private ITagRepository _tagRepository;
        private IUnitOfWork _unitOfWork;

        public TagService(ITagRepository TagRepository, IUnitOfWork unitOfWork)
        {
            this._tagRepository = TagRepository;
            this._unitOfWork = unitOfWork;
        }

        public Tag Add(Tag tag)
        {
          
            return _tagRepository.Add(tag);
        }

        public void Update(Tag tag)
        {
          
            _tagRepository.Update(tag);
        }

        public void Delete(int id)
        {
            _tagRepository.Delete(id);
        }

        public IEnumerable<Tag> GetAll()
        {
       
                return _tagRepository.GetMulti(x=>x.IsDeleted==false);

        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }



        public Tag FindById(int id)
        {
            return _tagRepository.GetSingleById(id);
        }
        public IEnumerable<Tag> GetAllTags()
        {
            return _tagRepository.GetMulti(x=>x.IsDeleted==false);
        }

        public IEnumerable<Tag> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
                return _tagRepository.GetMulti(x => x.Name.Contains(keyword) && x.IsDeleted == false);
            else
                return _tagRepository.GetMulti(x=>x.IsDeleted==false);
        }

        //public void IsDeleted(int id)
        //{
        //    var category = FindById(id);
        //    category.IsDeleted = true;
        //    SaveChanges();
        //}
    }
}