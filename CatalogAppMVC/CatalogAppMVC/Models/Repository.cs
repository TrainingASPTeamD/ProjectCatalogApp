﻿using CatalogAppMVC.Models.interfaces;
using CatalogAppMVC.Models.WorkLinqToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CatalogAppMVC.Models
{
    public class Repository : IRepository
    {


        public IQueryable<LinqToSqlMdl.CatalogCategories> CatalogCategories
        {
            get { throw new NotImplementedException(); }
        }

        public bool CreateCatalogCategories(LinqToSqlMdl.CatalogCategories instance)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCatalogCategories(LinqToSqlMdl.CatalogCategories instance)
        {
            throw new NotImplementedException();
        }

        public bool RemoveCatalogCategories(int idCatalogCategories)
        {
            throw new NotImplementedException();
        }











        public IQueryable<LinqToSqlMdl.Machinery> Machinerys
        {
            get { throw new NotImplementedException(); }
        }

        public int CreateMachinery(Record record)
        {
            Machinery machinery = new Machinery();
            try
            {
                CatalogDatabaseDataContext context = new WorkLinqToSql.CatalogDatabaseDataContext();

                machinery.title = record.Name;
                machinery.Description = record.Description;
                machinery.Status = (int)Record.StatusType.PREMODERATION;
                machinery.Category = record.CategoryID;
                machinery.UserAuthor = record.UserAuthorID;
                context.Machineries.InsertOnSubmit(machinery);
                context.Machineries.Context.SubmitChanges();

                foreach (Specification sp in record.Specifications)
                {
                    if (!CreateSpecifications(sp, machinery.Id))
                    {
                        throw new Exception("Невожно создать спецификацию");
                    }
                }
                if (record.Tags != null)
                {
                    foreach (Tag tag in record.Tags)
                    {
                        CreateTag(tag, machinery.Id);
                    }
                }

            }
            catch
            {
                return 0;
            }
            return machinery.Id;
        }

        public bool UpdateMachinery(Record record)
        {
            try
            {
                CatalogDatabaseDataContext context = new WorkLinqToSql.CatalogDatabaseDataContext();
                WorkLinqToSql.Machinery machinery = (from m in context.Machineries where m.Id == record.ID select m).Single<WorkLinqToSql.Machinery>();
                
                machinery.Description = record.Description;
                machinery.Status = (int)record.Status;
                machinery.Category = record.CategoryID;
                machinery.UserAuthor = record.UserAuthorID;
                context.Machineries.Context.SubmitChanges();

                foreach (Specification sp in record.Specifications)
                {
                    if (!UpdateSpecifications(sp))
                    {
                        throw new Exception("Ошибка обновления спецификации");
                    }
                }
                if (record.Tags != null)
                {
                    foreach (Tag tag in record.Tags)
                    {
                        UpdateTag(tag);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool RemoveMachinery(int recordID)
        {
            try
            {
                CatalogDatabaseDataContext context = new WorkLinqToSql.CatalogDatabaseDataContext();
                WorkLinqToSql.Machinery machinery = (from m in context.Machineries where m.Id == recordID select m).Single<WorkLinqToSql.Machinery>();

                var specificationsID = from s in context.MachineSpecifications where s.MachineID == machinery.Id select s.SpecificationID;
                if (specificationsID.Count() > 0)
                {
                    foreach (int spID in specificationsID)
                    {
                        RemoveSpecifications(spID);
                    }
                }
                var tagsID = from t in context.MachineTags where t.MachineID == machinery.Id select t.TagID;
                if (tagsID.Count() > 0)
                {
                    foreach (int tagID in tagsID)
                    {
                        RemoveTag(tagID, machinery.Id);
                    }
                }
                var filesID = from f in context.Documents where f.MachineID == machinery.Id select f.Id;
                if(filesID.Count() > 0)
                {
                    foreach(int fileID in filesID)
                    {
                        RemoveFile(fileID);
                    }
                }

                context.Machineries.DeleteOnSubmit(machinery);
                context.Machineries.Context.SubmitChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool UpdateStatusMachinery(Record.StatusType statusNew, int recordID)
        {
            try
            {
                WorkLinqToSql.CatalogDatabaseDataContext context = new CatalogDatabaseDataContext();
                WorkLinqToSql.Machinery machinery = (from m in context.Machineries where m.Id == recordID select m).Single<WorkLinqToSql.Machinery>();
                machinery.Status = (int)statusNew;
                context.Machineries.Context.SubmitChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }










        public IQueryable<WorkLinqToSql.Specification> Specifications
        {
            get { throw new NotImplementedException(); }
        }

        public bool CreateSpecifications(Specification specificationModel, int recordID)
        {
            try
            {
                CatalogDatabaseDataContext context = new WorkLinqToSql.CatalogDatabaseDataContext();
                WorkLinqToSql.Specification specification = new WorkLinqToSql.Specification();
                WorkLinqToSql.MachineSpecification machinespecification = new MachineSpecification();

                specification.Name = specificationModel.Name;
                specification.Value = specificationModel.Value;

                machinespecification.MachineID = recordID;
                machinespecification.Specification = specification;

                context.Specifications.InsertOnSubmit(specification);
                context.MachineSpecifications.InsertOnSubmit(machinespecification);


                context.Specifications.Context.SubmitChanges();
                context.MachineSpecifications.Context.SubmitChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool UpdateSpecifications(Specification specificationModel)
        {
            try
            {
                CatalogDatabaseDataContext context = new WorkLinqToSql.CatalogDatabaseDataContext();

                WorkLinqToSql.Specification specification = (from s in context.Specifications where s.Id == specificationModel.ID select s).Single<WorkLinqToSql.Specification>();
                specification.Name = specificationModel.Name;
                specification.Value = specificationModel.Value;

                context.Specifications.Context.SubmitChanges();

            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool RemoveSpecifications(int idSpecifications)
        {
            try
            {
                CatalogDatabaseDataContext context = new WorkLinqToSql.CatalogDatabaseDataContext();

                WorkLinqToSql.Specification specification = (from s in context.Specifications where s.Id == idSpecifications select s).Single<WorkLinqToSql.Specification>();

                var macnineSpecifications = from ms in context.MachineSpecifications where ms.SpecificationID == specification.Id select ms;

                foreach (WorkLinqToSql.MachineSpecification ms in macnineSpecifications)
                {
                    context.MachineSpecifications.DeleteOnSubmit(ms);
                }
                context.MachineSpecifications.Context.SubmitChanges();

                context.Specifications.DeleteOnSubmit(specification);
                context.Specifications.Context.SubmitChanges();

            }
            catch
            {
                return false;
            }

            return true;
        }






        public IQueryable<LinqToSqlMdl.Tags> Tags
        {
            get { throw new NotImplementedException(); }
        }

        public bool CreateTag(CatalogAppMVC.Models.Tag tagModel, int recordID)
        {
            try
            {
                CatalogDatabaseDataContext context = new WorkLinqToSql.CatalogDatabaseDataContext();
                WorkLinqToSql.Tag tag = new WorkLinqToSql.Tag();
                tag.Name = tagModel.Name;

                var tagsInBase = from t in context.Tags where t.Name == tagModel.Name select t;
                if (tagsInBase.Count() > 0)
                {
                    tag = tagsInBase.First();
                }
                else
                {
                    context.Tags.InsertOnSubmit(tag);
                    context.Tags.Context.SubmitChanges();
                }

                WorkLinqToSql.MachineTag machineTag = new MachineTag();

                machineTag.MachineID = recordID;
                machineTag.Tag = tag;

                context.MachineTags.InsertOnSubmit(machineTag);
                context.MachineTags.Context.SubmitChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool UpdateTag(CatalogAppMVC.Models.Tag tagModel)
        {
            try
            {
                CatalogDatabaseDataContext context = new WorkLinqToSql.CatalogDatabaseDataContext();
                WorkLinqToSql.Tag tag = (from t in context.Tags where t.Id == tagModel.ID select t).Single<WorkLinqToSql.Tag>();

                tag.Name = tagModel.Name;
                context.MachineTags.Context.SubmitChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool RemoveTag(int idTags, int recordID)
        {
            try
            {
                CatalogDatabaseDataContext context = new WorkLinqToSql.CatalogDatabaseDataContext();

                WorkLinqToSql.Tag tag = (from t in context.Tags where t.Id == idTags select t).Single<WorkLinqToSql.Tag>();

                var machineTag = from mt in context.MachineTags where (mt.TagID == tag.Id) && (mt.MachineID == recordID) select mt;

                foreach (WorkLinqToSql.MachineTag mt in machineTag)
                {
                    context.MachineTags.DeleteOnSubmit(mt);
                }
                context.MachineTags.Context.SubmitChanges();

                var otherTagMachine = from mt in context.MachineTags where mt.TagID == tag.Id select mt;
                if (otherTagMachine.Count() == 0)
                {
                    context.Tags.DeleteOnSubmit(tag);
                    context.Tags.Context.SubmitChanges();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }



        public IQueryable<Document> File
        {
            get { throw new NotImplementedException(); }
        }

        public bool CreateFile(Document tag)
        {
            throw new NotImplementedException();
        }

        public bool UpdateFile(Document tag)
        {
            throw new NotImplementedException();
        }

        public bool RemoveFile(int fileID)
        {
            return true;
        }
    }
}