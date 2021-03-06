﻿using CatalogAppMVC.Models.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CatalogAppMVC.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }


        

        //Методы для работы с БД

        public static List<Category> GetCategoriesForWrite(int userID)
        {
            List<Category> list = new List<Category>();
            try
            {
                IRepository repository = new Repository();
                int userRoleId = repository.GetUserRole(userID).Id;

                IQueryable<WorkLinqToSql.AccessCatalogCategories> access = from acc in repository.Access where (acc.RoleID == userRoleId) && (acc.W == true) select acc;
                foreach(WorkLinqToSql.AccessCatalogCategories acc in access)
                {
                    list.Add(repository.ToCategory(acc.CatalogCategory));
                }
            }
            catch { }
            return list;
        }

        public static void DeleteCategory(int categoryID)
        {
            IRepository repository = new Repository();
            repository.RemoveCatalogCategories(categoryID);
        }
        public static void EditCategory(Category categoryNew)
        {
            IRepository repository = new Repository();
            repository.UpdateCatalogCategories(categoryNew);
        }
        public static List<Category> GetOpenCategory(int userID)
        {
            List<Category> list = new List<Category>();
            try
            {
                IRepository repository = new Repository();
                int userRoleId = repository.GetUserRole(userID).Id;

                IQueryable<WorkLinqToSql.AccessCatalogCategories> access = from acc in repository.Access where (acc.RoleID == userRoleId) && (acc.R == true) select acc;
                foreach (WorkLinqToSql.AccessCatalogCategories acc in access)
                {
                    list.Add(repository.ToCategory(acc.CatalogCategory));
                }
            }
            catch { }
            return list;
        }
        public static List<Category> GetAllCategory()
        {
            IRepository repository = new Repository();
            List<Category> list = new List<Category>();
            var categoriesFromBase = from cat in repository.CatalogCategories select cat;
            foreach(WorkLinqToSql.CatalogCategory c in categoriesFromBase)
            {
                list.Add(repository.ToCategory(c));
            }
            return list;
        }
        public static Category GetCategory(int categoryID)
        {
            IRepository repository = new Repository();
            try
            {
                var category = (from cat in repository.CatalogCategories where cat.Id == categoryID select cat).Single();
                return repository.ToCategory(category);
            }
            catch
            {

            }
            return null;
        }

        public void AddToBase()
        {
            IRepository repository = new Repository();
            ID = repository.CreateCatalogCategories(this);
            if(ID != 0)
            {
                WorkLinqToSql.AspNetRole role = repository.GetUserRole(Access.ADMINID);
                if (role != null)
                {
                    AccessRoleCategory access = new AccessRoleCategory(1, ID, true, true, true);
                    repository.CreateAccess(access);
                }

            }
        }
    }
}