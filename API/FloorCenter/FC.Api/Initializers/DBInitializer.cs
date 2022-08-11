using FC.Api.Helpers;
using FC.Api.Services.Users;
using FC.Core.Domain.Companies;
using FC.Core.Domain.Items;
using FC.Core.Domain.Sizes;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using System;
using System.Linq;

namespace FC.Api.Initializers
{
    public static class DBInitializer
    {

        public static void CreateDefaultData(DataContext context)
        {
            context.Database.EnsureCreated();

            AddDefaultCompanies(context);

            AddDefaultWarehouses(context);

            AddDefaultStores(context);

            AddDefaultUser(context);

            AddDefaultSizes(context);

            AddDefaultItemCategories(context);

            AddDefaultItems(context);

        }

        private static void AddDefaultUser(DataContext context)
        {
            // Look for any users
            if (context.Users.Any())
            {
                // DB has been seeded
                return;
            }

            var service = new UserService(context);

            var mainUser = new User
            {
                FullName = "FC Administrator",
                UserName = "floorcenter",
                UserType = UserTypeEnum.Administrator,
                Assignment = AssignmentEnum.MainOffice
            };
            service.InsertUser(mainUser, "administrator");

            var warehouseUser = new User
            {
                FullName = "FC Warehouse",
                UserName = "warehouse",
                UserType = UserTypeEnum.Administrator,
                Assignment = AssignmentEnum.Warehouse,
                WarehouseId = 1
            };
            service.InsertUser(warehouseUser, "warehouse");

            var storeUser = new User
            {
                FullName = "FC Store",
                UserName = "store",
                UserType = UserTypeEnum.Administrator,
                Assignment = AssignmentEnum.Store,
                StoreId = 1
            };
            service.InsertUser(storeUser, "store");

            var logisticsUser = new User
            {
                FullName = "FC Logistics",
                UserName = "logistics",
                UserType = UserTypeEnum.Administrator,
                Assignment = AssignmentEnum.Logistics
            };
            service.InsertUser(logisticsUser, "logistics");


        }

        private static void AddDefaultWarehouses(DataContext context)
        {
            // Look for any warehouses
            if (context.Warehouses.Any())
            {
                // DB has been seeded
                return;
            }

            var warehouses = new Warehouse[]
            {
                new Warehouse { Code = "MANILA", Name = "Lusterplus Inc.", DateCreated = DateTime.Now, Vendor = false },
                new Warehouse { Code = "CDO", Name = "Artemisia Corp.", DateCreated = DateTime.Now, Vendor = false  },
                new Warehouse { Code = "CEBU", Name = "Verduco Trading Inc. - Cebu", DateCreated = DateTime.Now, Vendor = false  },
                new Warehouse { Code = "DAVAO", Name = "Verduco Trading Inc. - Davao", DateCreated = DateTime.Now, Vendor = false  },
                new Warehouse { Code = "SAKRETE", Name = "Sakrete", DateCreated = DateTime.Now, Vendor = true  },
                new Warehouse { Code = "STONEPRO", Name = "Stonepro", DateCreated = DateTime.Now, Vendor = true  }
            };

            foreach (Warehouse warehouse in warehouses)
            {
                context.Warehouses.Add(warehouse);
            }

            context.SaveChanges();
        }

        private static void AddDefaultCompanies(DataContext context)
        {
            // Look for any companies
            if (context.Companies.Any())
            {
                // DB has been seeded
                return;
            }

            var companies = new Company[]
            {
                //new Company{ Code = "", Name = "FLOOR CENTER (MANILA, PAMPANGA, BATANGAS & CAVITE BRANCHES)", DateCreated = DateTime.Now },
                new Company{ Code = "F3J2", Name = "JBW", DateCreated = DateTime.Now },
                new Company{ Code = "F3T4", Name = "FC TILES DEPOT", DateCreated = DateTime.Now },
                new Company{ Code = "F3F4", Name = "FASTBUILD 1 (COMPANY OWNED)", DateCreated = DateTime.Now },
                new Company{ Code = "F3F2", Name = "FASTBUILD 2 (PREVIOUSLY DEALER)", DateCreated = DateTime.Now },
                //new Company{ Code = "", Name = "SOUTH MILANDIA, INC. (MINDANAO BRANCHES)", DateCreated = DateTime.Now },
                new Company{ Code = "F3S2", Name = "SMI", DateCreated = DateTime.Now },
                new Company{ Code = "F3G1", Name = "FC GENSAN", DateCreated = DateTime.Now },
                new Company{ Code = "F3S3", Name = "SARGEN FLOOR CENTER, INC.", DateCreated = DateTime.Now }
            };

            foreach (Company company in companies)
            {
                context.Companies.Add(company);
            }

            context.SaveChanges();
        }

        private static void AddDefaultStores(DataContext context)
        {
            // Look for any stores
            if (context.Stores.Any())
            {
                // DB has been seeded
                return;
            }

            var stores = new Store[]
            {
                new Store { Code="F3J2-1", Name="1125", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-3", Name="ANGELES", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-4", Name="COMMONWEALTH", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-5", Name="LIBIS", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-6", Name="MC-ARTHUR", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-7", Name="MEXICO", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-8", Name="MUÑOZ", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-9", Name="QUEZON AVENUE", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-10", Name="ROSARIO", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-11", Name="SAN FERNANDO", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-12", Name="SM-NORTH EDSA", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-15", Name="SACOP", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-16", Name="PROJECT MUÑOZ", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-17", Name="VISAYAS AVENUE", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3J2-18", Name="1252", CompanyId=1, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3T4-1", Name="ROXAS BLVD", CompanyId=2, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3T4-2", Name="DIVERSION", CompanyId=2, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3T4-3", Name="ANTIPOLO", CompanyId=2, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F4-1", Name="BINONDO", CompanyId=3, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F4-2", Name="CAINTA", CompanyId=3, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F4-3", Name="LEMERY", CompanyId=3, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F4-4", Name="LIPA", CompanyId=3, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F2-1", Name="PROJECT ROXAS", CompanyId=4, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F2-3", Name="IMUS", CompanyId=4, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F2-4", Name="KAWIT", CompanyId=4, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F2-5", Name="MAKATI", CompanyId=4, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F2-6", Name="MOLINO", CompanyId=4, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3F2-8", Name="PROJECT MAKATI", CompanyId=4, DateCreated=DateTime.Now,WarehouseId = 1 },
                new Store { Code="F3S2-1", Name="BAJADA", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-2", Name="BUHANGIN", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-3", Name="BULUA", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-4", Name="BUTUAN", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-7", Name="MATINA", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-9", Name="VALENCIA", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-10", Name="LAPASAN", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-11", Name="ECOLAND", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-13", Name="TETUAN", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-14", Name="COTABATO", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-15", Name="VETERANS", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-16", Name="ILIGAN", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-17", Name="MANDAUE", CompanyId=5, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-5", Name="GENERAL SANTOS", CompanyId=6, DateCreated=DateTime.Now,WarehouseId = 4 },
                new Store { Code="F3S2-6", Name="SARGEN MARBEL", CompanyId=7, DateCreated=DateTime.Now,WarehouseId = 4 }
            };

            foreach (Store store in stores)
            {
                context.Stores.Add(store);
            }

            context.SaveChanges();

        }

        private static void AddDefaultSizes(DataContext context)
        {

            // Look for any sizes
            if (context.Sizes.Any())
            {
                // DB has been seeded
                return;
            }

            var sizes = new Size[]
            {
                new Size { Name="20x100", DateCreated = DateTime.Now },
                new Size { Name="80X80", DateCreated = DateTime.Now },
                new Size { Name="60x60", DateCreated = DateTime.Now },
                new Size { Name="50x50", DateCreated = DateTime.Now },
                new Size { Name="40x40", DateCreated = DateTime.Now },
                new Size { Name="30x60", DateCreated = DateTime.Now },
                new Size { Name="30x45", DateCreated = DateTime.Now },
                new Size { Name="30x30", DateCreated = DateTime.Now },
                new Size { Name="25x33", DateCreated = DateTime.Now },
                new Size { Name="20x60", DateCreated = DateTime.Now },
                new Size { Name="20x23", DateCreated = DateTime.Now },
                new Size { Name="20x30", DateCreated = DateTime.Now },
                new Size { Name="20x20", DateCreated = DateTime.Now },
                new Size { Name="15x80", DateCreated = DateTime.Now },
                new Size { Name="15x60", DateCreated = DateTime.Now },
                new Size { Name="15x15", DateCreated = DateTime.Now },
                new Size { Name="15x30", DateCreated = DateTime.Now },
                new Size { Name="11.2x45", DateCreated = DateTime.Now },
                new Size { Name="7x20", DateCreated = DateTime.Now },
                new Size { Name="2.5x20", DateCreated = DateTime.Now },
                new Size { Name="10x20", DateCreated = DateTime.Now },
                new Size { Name="4x45", DateCreated = DateTime.Now },
                new Size { Name="7x30", DateCreated = DateTime.Now },
                new Size { Name="4.5x45", DateCreated = DateTime.Now },
                new Size { Name="5x60", DateCreated = DateTime.Now },
                new Size { Name="6x30", DateCreated = DateTime.Now },
                new Size { Name="6x45", DateCreated = DateTime.Now },
                new Size { Name="7x45", DateCreated = DateTime.Now },
                new Size { Name="8x30", DateCreated = DateTime.Now },
                new Size { Name="3x30", DateCreated = DateTime.Now },
                new Size { Name="5x20", DateCreated = DateTime.Now },
                new Size { Name="6x20", DateCreated = DateTime.Now },
                new Size { Name="4x30", DateCreated = DateTime.Now },
                new Size { Name="8x33", DateCreated = DateTime.Now },
                new Size { Name="5x30", DateCreated = DateTime.Now },
                new Size { Name="4.5X60", DateCreated = DateTime.Now },
                new Size { Name="6x60", DateCreated = DateTime.Now },
                new Size { Name="7x60", DateCreated = DateTime.Now },
                new Size { Name="7.5x7.5", DateCreated = DateTime.Now },
                new Size { Name="2x30", DateCreated = DateTime.Now },
                new Size { Name="10x30", DateCreated = DateTime.Now },
                new Size { Name="10x10", DateCreated = DateTime.Now },
                new Size { Name="8x8", DateCreated = DateTime.Now },
                new Size { Name="9.8X30", DateCreated = DateTime.Now },
                new Size { Name="9.8X60", DateCreated = DateTime.Now },
                new Size { Name="12X30", DateCreated = DateTime.Now },
                new Size { Name="60x120", DateCreated = DateTime.Now },
                new Size { Name="59.5x59.5", DateCreated = DateTime.Now },
                new Size { Name="30x30x1.2mm x 0.1mm", DateCreated = DateTime.Now },
                new Size { Name="6'x36'x2.00mm", DateCreated = DateTime.Now },
                new Size { Name="305x305x2mm", DateCreated = DateTime.Now },
                new Size { Name="6inx36inx2.00mm", DateCreated = DateTime.Now },
                new Size { Name="7.3'x48.3'x3.2mm", DateCreated = DateTime.Now },
                new Size { Name="7.3inx48.3inx3.2mm", DateCreated = DateTime.Now },
                new Size { Name="457x457", DateCreated = DateTime.Now },
                new Size { Name="18'x18'x2.0mm", DateCreated = DateTime.Now },
                new Size { Name="6'x36'x2.0mm", DateCreated = DateTime.Now },
                new Size { Name="152x914 (6'x36') 2mm", DateCreated = DateTime.Now },
                new Size { Name="457x457(3mm)", DateCreated = DateTime.Now },
                new Size { Name="457x457(2mm)", DateCreated = DateTime.Now },
                new Size { Name="152x914(2mm)", DateCreated = DateTime.Now },
                new Size { Name="457x457(18'x18') 2-3mm", DateCreated = DateTime.Now },
                new Size { Name="30.5x30.5", DateCreated = DateTime.Now },

                new Size { Name="N/A", DateCreated = DateTime.Now },

                new Size { Name="0", DateCreated = DateTime.Now },

                new Size { Name="10MM", DateCreated = DateTime.Now },
                new Size { Name="9MM", DateCreated = DateTime.Now },
                new Size { Name="8MM", DateCreated = DateTime.Now },
                new Size { Name="7.5MM", DateCreated = DateTime.Now },

                new Size { Name="1220x2440x3mm ACP - PE", DateCreated = DateTime.Now },
                new Size { Name="1220x2440x4mm ACP - PVDF", DateCreated = DateTime.Now },
                new Size { Name="1220x2440x4mm ACP - NANO", DateCreated = DateTime.Now },
                new Size { Name="1220x2440x4mm ACP - Nano-PVDF", DateCreated = DateTime.Now },

                new Size { Name="152x914 (2mm)", DateCreated = DateTime.Now },
                new Size { Name="457x457 (3mm)", DateCreated = DateTime.Now },
                new Size { Name="457x457 (2mm)", DateCreated = DateTime.Now },
                new Size { Name="6inx36inx2.0mm", DateCreated = DateTime.Now }
                
            };

            foreach (Size size in sizes)
            {
                context.Sizes.Add(size);
            }

            context.SaveChanges();
        }

        private static void AddDefaultItems(DataContext context)
        {

            // Look for any sizes
            if (context.Items.Any())
            {
                // DB has been seeded
                return;
            }

            var items = new Item[]
            {

            };

            foreach (Item item in items)
            {
                context.Items.Add(item);
            }

            context.SaveChanges();
        }

        private static void AddDefaultItemCategories(DataContext context)
        {
            // Look for any category parents
            if (context.CategoryParents.Any())
            {
                // DB has been seeded
                return;
            }


            var parents = new CategoryParent[]
            {
                new CategoryParent
                {
                    Name="DOUBLE LOADED", DateCreated = DateTime.Now,
                    Children = new CategoryChild[]
                    {
                        new CategoryChild
                        {
                            Name ="SOLUBLE SALT", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm - ABA Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="SUPER WHITE", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="MICROLITE", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm - AK Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - C Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="PILATE", DateCreated = DateTime.Now,
                            GrandChildren =new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 - AK and C", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="POLISHED PORCELAIN", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="80x80 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="80x80 cm - CJ Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - C Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="RUSTIC", DateCreated = DateTime.Now
                        },
                    }
                },

                new CategoryParent
                {
                    Name ="FULL BODY", DateCreated = DateTime.Now,
                    Children = new CategoryChild[]
                    {
                        new CategoryChild
                        {
                            Name ="POLISHED", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - AC Series (Eagle)", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - SC Series (Eagle)", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - MT Series", DateCreated = DateTime.Now },
                            }

                        },
                        new CategoryChild
                        {
                            Name ="SUPER BLACK", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 & 30x60 - AK Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="MATTE", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - AM Series (Eagle)", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - SM Series (Eagle)", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - Alfa series (LM)", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="40x40 cm - Deluxe Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - VLM series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - T3 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="RUSTIC", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm - AE Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x20 cm - KDY Series", DateCreated = DateTime.Now },
                            }
                        }
                    }
                },

                new CategoryParent
                {
                    Name ="GLAZE", DateCreated = DateTime.Now,
                    Children  = new CategoryChild[]
                    {
                        new CategoryChild
                        {
                            Name ="SOLUBLE SALT", DateCreated = DateTime.Now,
                            GrandChildren  = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm - ASA Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - V Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="50x50 cm - ASA Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="40x40 cm - ASA Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="RUSTIC - FLOOR", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - CL Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - ME Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - BT Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - M Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="40x40 cm ", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="40x40 cm - F40 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="40x40 cm - I Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - I Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - 3 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - L Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - F30 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - I Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x100 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x20 cm - 2000 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x20 cm - P22 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="15x60 cm - AI0 Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="RUSTIC - WALL", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="30x60 cm - QF Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - Q3 Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="MATTE - WALL", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="20x30 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x60 cm - S26 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - Q Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - KY Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="SEMI-POLISHED - WALL", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="30x60 cm - VP Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="MATTE - WALL/FLOOR", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="15x60 cm - A1 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="15x80 cm - AM Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - Y Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - G0KA series (Eagle)", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="MATTE - FLOOR", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - M Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - P0 series (new)", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="40x40 cm - A4 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="40x40 cm - F4 Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="GLOSSY - FLOOR", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="40x40 cm - S Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - 0T Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="GLOSSY - WALL", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="40x40 cm - S Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - KY Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - I0 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - Milan Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - Y3 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - GQ Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - Roma Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x60 cm - I-26 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x60 cm - S2 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x60 cm - Y26 Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x60 cm ", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x30 cm - I Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x30 cm - Other series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="20x20 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="10x20 cm", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="POLISHED PORCELAIN", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="60x60 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm - MT Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="60x60 cm", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x60 cm - GQ Series", DateCreated = DateTime.Now },
                            }
                        },
                        new CategoryChild
                        {
                            Name ="POLISHED CERAMIC", DateCreated = DateTime.Now,
                            GrandChildren = new CategoryGrandChild[]
                            {
                                new CategoryGrandChild { Name="30x30 cm - K Series", DateCreated = DateTime.Now },
                                new CategoryGrandChild { Name="30x30 cm - W Series", DateCreated = DateTime.Now },
                            }
                        },
                    }
                },

                new CategoryParent
                {
                    Name ="RAK COLLECTION", DateCreated = DateTime.Now,
                    Children = new CategoryChild[]
                    {
                        new CategoryChild { Name="59.5x59.5 cm", DateCreated = DateTime.Now },
                        new CategoryChild { Name="20x23 cm - Hex", DateCreated = DateTime.Now },
                    }
                },

                new CategoryParent
                {
                    Name ="MOSAICS", DateCreated = DateTime.Now,
                    Children = new CategoryChild[]
                    {
                        new CategoryChild { Name="SOLUBLE SALT", DateCreated = DateTime.Now },
                    }
                }
            };


            foreach (CategoryParent parent in parents)
            {
                context.CategoryParents.Add(parent);
            }

            context.SaveChanges();

        }

        private static void AddCategoryParentChildren(DataContext context, int Id)
        {
            CategoryChild[] childs = null;

            if (Id == 10)
            {
                childs = new CategoryChild[]
                {
                    new CategoryChild { CategoryParentId = Id, Name="SOLUBLE SALT", DateCreated = DateTime.Now },
                    new CategoryChild { CategoryParentId = Id, Name="SUPER WHITE", DateCreated = DateTime.Now },
                    new CategoryChild { CategoryParentId = Id, Name="MICROLITE", DateCreated = DateTime.Now },
                    new CategoryChild { CategoryParentId = Id, Name="PILATE", DateCreated = DateTime.Now },
                    new CategoryChild { CategoryParentId = Id, Name="POLISHED PORCELAIN", DateCreated = DateTime.Now },
                    new CategoryChild { CategoryParentId = Id, Name="RUSTIC", DateCreated = DateTime.Now },
                };
            }
            else if (Id == 20)
            {
                childs = new CategoryChild[]
                {
                    new CategoryChild { Id = 201, CategoryParentId = Id, Name="POLISHED", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 202, CategoryParentId = Id, Name="SUPER BLACK", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 203, CategoryParentId = Id, Name="MATTE", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 204, CategoryParentId = Id, Name="RUSTIC", DateCreated = DateTime.Now }
                };
            }
            else if (Id == 30)
            {
                childs = new CategoryChild[]
                {
                    new CategoryChild { Id = 301, CategoryParentId = Id, Name="SOLUBLE SALT", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 302, CategoryParentId = Id, Name="RUSTIC - FLOOR", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 303, CategoryParentId = Id, Name="RUSTIC - WALL", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 304, CategoryParentId = Id, Name="MATTE - WALL", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 305, CategoryParentId = Id, Name="SEMI-POLISHED - WALL", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 306, CategoryParentId = Id, Name="MATTE - WALL/FLOOR", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 307, CategoryParentId = Id, Name="MATTE - FLOOR", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 308, CategoryParentId = Id, Name="GLOSSY - FLOOR", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 309, CategoryParentId = Id, Name="GLOSSY - WALL", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 3010, CategoryParentId = Id, Name="POLISHED PORCELAIN", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 3011, CategoryParentId = Id, Name="POLISHED CERAMIC", DateCreated = DateTime.Now },
                };
            }
            else if (Id == 40)
            {
                childs = new CategoryChild[]
                {
                    new CategoryChild { Id = 401, CategoryParentId = Id, Name="59.5x59.5 cm", DateCreated = DateTime.Now },
                    new CategoryChild { Id = 402, CategoryParentId = Id, Name="20x23 cm - Hex", DateCreated = DateTime.Now },
                };
            }
            else if (Id == 50)
            {
                childs = new CategoryChild[]
                {
                    new CategoryChild { Id = 501, CategoryParentId = Id, Name="SOLUBLE SALT", DateCreated = DateTime.Now },
                };
            }

            if (childs != null)
            {
                foreach (CategoryChild child in childs)
                {
                    context.CategoryChildren.Add(child);
                    context.SaveChanges();

                    AddCategoryChildrenGrandChildren(context, child.Id);
                }
            }
        }

        private static void AddCategoryChildrenGrandChildren(DataContext context, int CategoryChildId)
        {
            CategoryGrandChild[] grandChilds = null;

            #region ChildId from 101 - 105

            if (CategoryChildId == 101)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 1011, CategoryChildId = CategoryChildId, Name="60x60 cm - ABA Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 102)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 1021, CategoryChildId = CategoryChildId, Name="60x60 cm", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 103)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 1031, CategoryChildId = CategoryChildId, Name="60x60 cm - AK Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 1032, CategoryChildId = CategoryChildId, Name="60x60 cm - C Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 104)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 1041, CategoryChildId = CategoryChildId, Name="60x60 - AK and C", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 105)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 1051, CategoryChildId = CategoryChildId, Name="80x80 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 1052, CategoryChildId = CategoryChildId, Name="80x80 cm - CJ Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 1052, CategoryChildId = CategoryChildId, Name="60x60 cm - C Series", DateCreated = DateTime.Now },
                };
            }

            #endregion

            #region Child 201 - 204
            else if (CategoryChildId == 201)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 2011, CategoryChildId = CategoryChildId, Name="60x60 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2012, CategoryChildId = CategoryChildId, Name="60x60 cm - AC Series (Eagle)", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2013, CategoryChildId = CategoryChildId, Name="60x60 cm - SC Series (Eagle)", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2014, CategoryChildId = CategoryChildId, Name="60x60 cm - MT Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 202)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 2021, CategoryChildId = CategoryChildId, Name="60x60 & 30x60 - AK Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 203)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 2031, CategoryChildId = CategoryChildId, Name="60x60 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2032, CategoryChildId = CategoryChildId, Name="60x60 cm - AM Series (Eagle)", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2033, CategoryChildId = CategoryChildId, Name="60x60 cm - SM Series (Eagle)", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2034, CategoryChildId = CategoryChildId, Name="60x60 cm - Alfa series (LM)", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2035, CategoryChildId = CategoryChildId, Name="40x40 cm - Deluxe Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2036, CategoryChildId = CategoryChildId, Name="30x60 cm - VLM series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2037, CategoryChildId = CategoryChildId, Name="30x30 cm - T3 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2038, CategoryChildId = CategoryChildId, Name="30x30 cm", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 204)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 2041, CategoryChildId = CategoryChildId, Name="60x60 cm - AE Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 2042, CategoryChildId = CategoryChildId, Name="20x20 cm - KDY Series", DateCreated = DateTime.Now },
                };
            }
            #endregion

            #region Child 301 - 3011
            else if (CategoryChildId == 301)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 3011, CategoryChildId = CategoryChildId, Name="60x60 cm - ASA Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3012, CategoryChildId = CategoryChildId, Name="60x60 cm - V Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3013, CategoryChildId = CategoryChildId, Name="50x50 cm - ASA Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3014, CategoryChildId = CategoryChildId, Name="40x40 cm - ASA Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 302)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 3021, CategoryChildId = CategoryChildId, Name="60x60 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3022, CategoryChildId = CategoryChildId, Name="60x60 cm - CL Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3023, CategoryChildId = CategoryChildId, Name="60x60 cm - ME Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3024, CategoryChildId = CategoryChildId, Name="60x60 cm - BT Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3025, CategoryChildId = CategoryChildId, Name="60x60 cm - M Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3026, CategoryChildId = CategoryChildId, Name="40x40 cm ", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3027, CategoryChildId = CategoryChildId, Name="40x40 cm - F40 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3028, CategoryChildId = CategoryChildId, Name="40x40 cm - I Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3029, CategoryChildId = CategoryChildId, Name="30x60 cm - I Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30210, CategoryChildId = CategoryChildId, Name="30x30 cm - 3 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30211, CategoryChildId = CategoryChildId, Name="30x30 cm - L Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30212, CategoryChildId = CategoryChildId, Name="30x30 cm - F30 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30213, CategoryChildId = CategoryChildId, Name="30x30 cm - I Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30214, CategoryChildId = CategoryChildId, Name="20x100 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30215, CategoryChildId = CategoryChildId, Name="20x20 cm - 2000 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30216, CategoryChildId = CategoryChildId, Name="20x20 cm - P22 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30217, CategoryChildId = CategoryChildId, Name="15x60 cm - AI0 Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 303)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 3031, CategoryChildId = CategoryChildId, Name="30x60 cm - QF Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3032, CategoryChildId = CategoryChildId, Name="30x60 cm - Q3 Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 304)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 3041, CategoryChildId = CategoryChildId, Name="20x30 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3042, CategoryChildId = CategoryChildId, Name="20x60 cm - S26 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3043, CategoryChildId = CategoryChildId, Name="30x60 cm - Q Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3044, CategoryChildId = CategoryChildId, Name="30x60 cm - KY Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 305)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 3051, CategoryChildId = CategoryChildId, Name="30x60 cm - VP Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 306)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 3061, CategoryChildId = CategoryChildId, Name="15x60 cm - A1 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3062, CategoryChildId = CategoryChildId, Name="15x80 cm - AM Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3063, CategoryChildId = CategoryChildId, Name="30x30 cm - Y Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3064, CategoryChildId = CategoryChildId, Name="60x60 cm - G0KA series (Eagle)", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 307)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 3071, CategoryChildId = CategoryChildId, Name="60x60 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3072, CategoryChildId = CategoryChildId, Name="60x60 cm - M Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3073, CategoryChildId = CategoryChildId, Name="30x30 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3074, CategoryChildId = CategoryChildId, Name="30x30 cm - P0 series (new)", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3075, CategoryChildId = CategoryChildId, Name="40x40 cm - A4 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3076, CategoryChildId = CategoryChildId, Name="40x40 cm - F4 Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 308)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 3081, CategoryChildId = CategoryChildId, Name="40x40 cm - S Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3082, CategoryChildId = CategoryChildId, Name="30x30 cm - 0T Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 309)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 3091, CategoryChildId = CategoryChildId, Name="40x40 cm - S Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3092, CategoryChildId = CategoryChildId, Name="30x60 cm - KY Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3093, CategoryChildId = CategoryChildId, Name="30x60 cm - I0 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3094, CategoryChildId = CategoryChildId, Name="30x60 cm - Milan Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3095, CategoryChildId = CategoryChildId, Name="30x60 cm - Y3 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3096, CategoryChildId = CategoryChildId, Name="30x60 cm - GQ Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3097, CategoryChildId = CategoryChildId, Name="30x30 cm - Roma Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3098, CategoryChildId = CategoryChildId, Name="20x60 cm - I-26 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 3099, CategoryChildId = CategoryChildId, Name="20x60 cm - S2 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30910, CategoryChildId = CategoryChildId, Name="20x60 cm - Y26 Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30911, CategoryChildId = CategoryChildId, Name="20x60 cm ", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30912, CategoryChildId = CategoryChildId, Name="20x30 cm - I Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30913, CategoryChildId = CategoryChildId, Name="20x30 cm - Other series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30914, CategoryChildId = CategoryChildId, Name="20x20 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30915, CategoryChildId = CategoryChildId, Name="10x20 cm", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 3010)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 30101, CategoryChildId = CategoryChildId, Name="60x60 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30102, CategoryChildId = CategoryChildId, Name="60x60 cm - MT Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30103, CategoryChildId = CategoryChildId, Name="60x60 cm", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30104, CategoryChildId = CategoryChildId, Name="30x60 cm - GQ Series", DateCreated = DateTime.Now },
                };
            }
            else if (CategoryChildId == 3011)
            {
                grandChilds = new CategoryGrandChild[]
                {
                    new CategoryGrandChild { Id = 30111, CategoryChildId = CategoryChildId, Name="30x30 cm - K Series", DateCreated = DateTime.Now },
                    new CategoryGrandChild { Id = 30112, CategoryChildId = CategoryChildId, Name="30x30 cm - W Series", DateCreated = DateTime.Now },
                };
            }
            #endregion

            if (grandChilds != null)
            {
                foreach (CategoryGrandChild grandChild in grandChilds)
                {
                    context.CategoryGrandChildren.Add(grandChild);
                    context.SaveChanges();
                }
            }
        }

    }
}
