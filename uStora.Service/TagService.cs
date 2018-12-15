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
       
                return _tagRepository.GetAll();

        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        //public IEnumerable<TagCategory> GetAllByParentID(int parentID)
        //{
        //    return _tagCategoryRepository.GetMulti(x => x.Status && x.ParentID == parentID);
        //}

        public Tag FindById(int id)
        {
            return _tagRepository.GetSingleById(id);
        }

        public IEnumerable<Tag> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
                return _tagRepository.GetMulti(x => x.Name.Contains(keyword));
            else
                return _tagRepository.GetAll();
        }

        //public void IsDeleted(int id)
        //{
        //    var category = FindById(id);
        //    category.IsDeleted = true;
        //    SaveChanges();
        //}
    }
}