﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CatalogAppMVC.Models
{
    public class AccessRoleCategory
    {
        public int RoleID { get; private set; }
        public int CategoryID { get; private set; }
        bool CanRead { get; private set; }
        bool CanWrite { get; private set; }
        bool CanDownloadFile { get; private set; }

        public AccessRoleCategory(int roleID, int categoryID, bool read, bool write, bool file)
        {
            RoleID = roleID;
            CategoryID = categoryID;
            CanRead = read;
            CanWrite = write;
            CanDownloadFile = file;
        }
    }
}