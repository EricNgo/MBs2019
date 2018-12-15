using System;
using System.Collections.Generic;
using uStora.Common.Services.Int32;
using uStora.Data.Infrastructure;
using uStora.Data.Repositories;
using uStora.Model.Models;

namespace uStora.Service
{
    public interface ITagCategoryService : ICrudService<TagCategory>, IGetDataService<TagCategory>
    {

    }

    public class TagCategoryService : ITagCategoryService
    {
        private ITagCategoryRepository _tagCategoryRepository;
        private IUnitOfWork _unitOfWork;

        public TagCategoryService(ITagCategoryRepository TagCategoryRepository, IUnitOfWork unitOfWork)
        {
            this._tagCategoryRepository = TagCategoryRepository;
            this._unitOfWork = unitOfWork;
        }

        public TagCategory Add(TagCategory tagCategory)
        {

            return _tagCategoryRepository.Add(tagCategory);
        }

        public void Update(TagCategory tagCategory)
        {

            _tagCategoryRepository.Update(tagCategory);
        }

        public void Delete(int id)
        {
            _tagCategoryRepository.Delete(id);
        }

        public IEnumerable<TagCategory> GetAll()
        {

            return _tagCategoryRepository.GetAll();

        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        //public IEnumerable<TagCategory> GetAllByParentID(int parentID)
        //{
        //    return _tagCategoryRepository.GetMulti(x => x.Status && x.ParentID == parentID);
        //}

        public TagCategory FindById(int id)
        {
            return _tagCategoryRepository.GetSingleById(id);
        }

        public IEnumerable<TagCategory> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
                return _tagCategoryRepository.GetMulti(x => x.Name.Contains(keyword));
            else
                return _tagCategoryRepository.GetAll();
        }

        //public void IsDeleted(int id)
        //{
        //    var category = FindById(id);
        //    category.IsDeleted = true;
        //    SaveChanges();
        //}
    }
}