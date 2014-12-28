﻿using CatalogAppMVC.Models;
using CatalogAppMVC.Models.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using CatalogAppMVC.Models.TESTCODE;
using System.Configuration;
using Ninject;
using CatalogAppMVC.Models.LinqToSqlMdl;
using CatalogAppMVC.Models.Identity;


namespace CatalogAppMVC.Controllers
{
    public class RecordsController : Controller
    {
        #region newRecord

        [Inject]
        public IRepository repo { get; set; }

        [HttpGet]
        public ActionResult AddRecord()
        {
            Record record = Session["Record"] as Record;
            return View(record);
        }

        [HttpPost]
        public ActionResult AddRecord(Record recordNew, string TagsString)
        {
            Record record = Session["Record"] as Record;
            record.LoadFromPage(recordNew);
            record.LoadTagsFromString(TagsString);

            Session["Record"] = record;
            return RedirectToAction("AddRecordFiles");
        }




        [HttpGet]
        public ActionResult AddRecordCategory()
        {
            IMyAppAuthentication user = new ApplicationAuthentication(HttpContext);
            ViewBag.Categories = new SelectList(TestICategory.GetCategoriesForWrite(user), "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult AddRecordCategory(int CategorySelected)
        {
            IMyAppAuthentication user = new ApplicationAuthentication(HttpContext);
            //TODO Добавить проверку, правда ли пользователь выбрал одну из доступных ему категорий
            Session["Record"] = new Record(user, CategorySelected);
            return RedirectToAction("AddRecord");
        }




        [HttpGet]
        public ActionResult AddRecordFiles()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddRecordFiles(object obl)
        {
            Record record = Session["Record"] as Record;
           
            return View("Добавлено");
        }


        #endregion

        #region OneRecordView

        public ActionResult RecordView(Record record)
        {
            record = TESTRecords.GetRecord(111);
            return View(record);
        }
        public ActionResult TryDownloadFile(int fileID)
        {
            IMyAppAuthentication user = new ApplicationAuthentication(HttpContext);
            if (Access.CanDownloadFile(user, fileID))
            {
                return RedirectToAction("DownloadFile", new { FileID = fileID });
            }
            return View("DownloadFileError");
        }

        public FileResult DownloadFile(int FileID)
        {
            CatalogAppMVC.Models.File file = TESTDocuments.GetFile(FileID);
            return File(file.GetPatchToFile(), CatalogAppMVC.Models.File.TYPEFILES, file.GetFileName());
        }


        #endregion

        public ActionResult TwoRecordsView(IEnumerable<Record> records)
        {
            //TODO Удалить заглушку
            List<Record> records1 = new List<Record>();
            records1.Add(TESTRecords.GetRecord(1));
            records1[0].Name = "Запись 1";
            records1.Add(TESTRecords.GetRecord(2));
            records1[1].Name = "Запись 2";
            records1[0].Specifications.Add(new Specification() { Name = "цвет1", Value = "зеленый" });
            records1[0].Specifications.Add(new Specification() { Name = "цвет2", Value = "синий" });
            records1[0].Specifications.Add(new Specification() { Name = "цвет3", Value = "желтый" });

            records1[1].Specifications.Add(new Specification() { Name = "цвет2", Value = "фиолетовый" });
            records1[1].Specifications.Add(new Specification() { Name = "цвет4", Value = "красный" });
            records1[1].Specifications.Add(new Specification() { Name = "цвет5", Value = "голубой" });
            records = records1;
            //---
            records1[0].Tags.Add(new Tag() { Name = "tag1" });
            records1[0].Tags.Add(new Tag() { Name = "tag2" });
            records1[0].Tags.Add(new Tag() { Name = "tag3" });
            records1[0].Tags.Add(new Tag() { Name = "tag2" });
            records1[0].AddToDataBase();


            MultipleRecordsForCompare multRecords = new MultipleRecordsForCompare(records);
            return View(multRecords);
        }

        public ActionResult RecordsOfCategory(int categoryID)
        {
            IMyAppAuthentication user = new ApplicationAuthentication(HttpContext);
            if (Access.CanReadCategory(user, categoryID))
            {
                return View(TESTRecords.GetRecords());
            }
            return RedirectToAction("Index", "Home");
        }
    }
}