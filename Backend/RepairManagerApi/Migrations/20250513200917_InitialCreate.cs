using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairManagerApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogPricing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemType = table.Column<string>(type: "TEXT", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountPercentage = table.Column<int>(type: "INTEGER", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogPricing", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address1 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address2 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address3 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address4 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Zip = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairWorkflows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StatusesJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairWorkflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CanManageUsers = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageRoles = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageInventory = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageCatalog = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanCreateWorkOrders = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanEditWorkOrders = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanDeleteWorkOrders = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManagePrograms = table.Column<bool>(type: "INTEGER", nullable: false),
                    CanManageWorkflows = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    CatalogItemType = table.Column<string>(type: "TEXT", nullable: false),
                    CatalogItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SKU = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ManufacturerId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductCategoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Devices_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RepairPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RepairWorkflowId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepairPrograms_RepairWorkflows_RepairWorkflowId",
                        column: x => x.RepairWorkflowId,
                        principalTable: "RepairWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Parts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SKU = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ManufacturerId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductCategoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parts_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parts_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parts_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SKU = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ServiceCategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Services_ServiceCategories_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalTable: "ServiceCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    RepairProgramId = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CustomerPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IssueDescription = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CurrentStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PartIdsJson = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkOrders_RepairPrograms_RepairProgramId",
                        column: x => x.RepairProgramId,
                        principalTable: "RepairPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "Description", "ManufacturerId", "Name", "ProductCategoryId", "SKU" },
                values: new object[] { 1, "Apple iPhone 13 smartphone", null, "iPhone 13", null, null });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "Description", "ManufacturerId", "Name", "ProductCategoryId", "SKU" },
                values: new object[] { 2, "Samsung Galaxy S21 smartphone", null, "Samsung Galaxy S21", null, null });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "Description", "ManufacturerId", "Name", "ProductCategoryId", "SKU" },
                values: new object[] { 3, "Apple MacBook Pro laptop", null, "MacBook Pro", null, null });

            migrationBuilder.InsertData(
                table: "Parts",
                columns: new[] { "Id", "Description", "DeviceId", "ManufacturerId", "Name", "ProductCategoryId", "SKU" },
                values: new object[] { 1, null, null, null, "iPhone 13 Screen", null, "SCR-IP13" });

            migrationBuilder.InsertData(
                table: "Parts",
                columns: new[] { "Id", "Description", "DeviceId", "ManufacturerId", "Name", "ProductCategoryId", "SKU" },
                values: new object[] { 2, null, null, null, "Samsung Galaxy S21 Screen", null, "SCR-SGS21" });

            migrationBuilder.InsertData(
                table: "Parts",
                columns: new[] { "Id", "Description", "DeviceId", "ManufacturerId", "Name", "ProductCategoryId", "SKU" },
                values: new object[] { 3, null, null, null, "iPhone Battery", null, "BAT-IP" });

            migrationBuilder.InsertData(
                table: "Parts",
                columns: new[] { "Id", "Description", "DeviceId", "ManufacturerId", "Name", "ProductCategoryId", "SKU" },
                values: new object[] { 4, null, null, null, "Samsung Battery", null, "BAT-SG" });

            migrationBuilder.InsertData(
                table: "RepairWorkflows",
                columns: new[] { "Id", "Name", "StatusesJson" },
                values: new object[] { 1, "Standard Repair Workflow", "[\"Pending\",\"In Progress\",\"Quality Check\",\"Completed\"]" });

            migrationBuilder.InsertData(
                table: "RepairWorkflows",
                columns: new[] { "Id", "Name", "StatusesJson" },
                values: new object[] { 2, "Express Repair Workflow", "[\"Pending\",\"In Progress\",\"Completed\"]" });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Description", "DeviceId", "Name", "SKU", "ServiceCategoryId" },
                values: new object[] { 1, "Replace damaged screen with a new one", null, "Screen Replacement", null, null });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Description", "DeviceId", "Name", "SKU", "ServiceCategoryId" },
                values: new object[] { 2, "Replace old or damaged battery", null, "Battery Replacement", null, null });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Description", "DeviceId", "Name", "SKU", "ServiceCategoryId" },
                values: new object[] { 3, "Repair water damage to device", null, "Water Damage Repair", null, null });

            migrationBuilder.InsertData(
                table: "RepairPrograms",
                columns: new[] { "Id", "Name", "RepairWorkflowId" },
                values: new object[] { 1, "Standard Phone Repair", 1 });

            migrationBuilder.InsertData(
                table: "RepairPrograms",
                columns: new[] { "Id", "Name", "RepairWorkflowId" },
                values: new object[] { 2, "Express Phone Repair", 2 });

            migrationBuilder.InsertData(
                table: "RepairPrograms",
                columns: new[] { "Id", "Name", "RepairWorkflowId" },
                values: new object[] { 3, "Premium Laptop Repair", 1 });

            migrationBuilder.InsertData(
                table: "WorkOrders",
                columns: new[] { "Id", "Code", "CreatedAt", "CurrentStatus", "CustomerName", "CustomerPhone", "DeviceId", "GroupId", "IssueDescription", "PartIdsJson", "RepairProgramId", "ServiceId", "UpdatedAt" },
                values: new object[] { 1, "WO00000", new DateTime(2025, 5, 11, 20, 9, 16, 775, DateTimeKind.Utc).AddTicks(5345), "In Progress", null, null, 1, null, null, "[1]", 1, 1, null });

            migrationBuilder.InsertData(
                table: "WorkOrders",
                columns: new[] { "Id", "Code", "CreatedAt", "CurrentStatus", "CustomerName", "CustomerPhone", "DeviceId", "GroupId", "IssueDescription", "PartIdsJson", "RepairProgramId", "ServiceId", "UpdatedAt" },
                values: new object[] { 2, "WO00000", new DateTime(2025, 5, 12, 20, 9, 16, 775, DateTimeKind.Utc).AddTicks(5354), "Pending", null, null, 2, null, null, "[4]", 2, 2, null });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ManufacturerId",
                table: "Devices",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ProductCategoryId",
                table: "Devices",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_GroupId",
                table: "InventoryItems",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_DeviceId",
                table: "Parts",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_ManufacturerId",
                table: "Parts",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_ProductCategoryId",
                table: "Parts",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairPrograms_RepairWorkflowId",
                table: "RepairPrograms",
                column: "RepairWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_DeviceId",
                table: "Services",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCategoryId",
                table: "Services",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                table: "Users",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_DeviceId",
                table: "WorkOrders",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_GroupId",
                table: "WorkOrders",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_RepairProgramId",
                table: "WorkOrders",
                column: "RepairProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ServiceId",
                table: "WorkOrders",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogPricing");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "Parts");

            migrationBuilder.DropTable(
                name: "StatusCodes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "RepairPrograms");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "RepairWorkflows");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "ServiceCategories");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "ProductCategories");
        }
    }
}
