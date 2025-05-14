using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepairManagerApi.Migrations
{
    public partial class SchemaUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Manufacturers_ManufacturerId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Manufacturers_ManufacturerId",
                table: "Parts");

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RepairPrograms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorkOrders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkOrders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RepairPrograms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RepairPrograms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RepairWorkflows",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RepairWorkflows",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "CatalogItemId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CatalogPricing");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "CatalogPricing");

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Services",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Parts",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Parts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CatalogItemType",
                table: "InventoryItems",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "InventoryItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartId",
                table: "InventoryItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "InventoryItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Devices",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Devices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ItemType",
                table: "CatalogPricing",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "CatalogPricing",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartId",
                table: "CatalogPricing",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "CatalogPricing",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_DeviceId",
                table: "InventoryItems",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_PartId",
                table: "InventoryItems",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ServiceId",
                table: "InventoryItems",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPricing_DeviceId",
                table: "CatalogPricing",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPricing_PartId",
                table: "CatalogPricing",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogPricing_ServiceId",
                table: "CatalogPricing",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogPricing_Devices_DeviceId",
                table: "CatalogPricing",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogPricing_Parts_PartId",
                table: "CatalogPricing",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogPricing_Services_ServiceId",
                table: "CatalogPricing",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Manufacturers_ManufacturerId",
                table: "Devices",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Devices_DeviceId",
                table: "InventoryItems",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Parts_PartId",
                table: "InventoryItems",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Services_ServiceId",
                table: "InventoryItems",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Manufacturers_ManufacturerId",
                table: "Parts",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogPricing_Devices_DeviceId",
                table: "CatalogPricing");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogPricing_Parts_PartId",
                table: "CatalogPricing");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogPricing_Services_ServiceId",
                table: "CatalogPricing");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Manufacturers_ManufacturerId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Devices_DeviceId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Parts_PartId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Services_ServiceId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Manufacturers_ManufacturerId",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_DeviceId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_PartId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_ServiceId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogPricing_DeviceId",
                table: "CatalogPricing");

            migrationBuilder.DropIndex(
                name: "IX_CatalogPricing_PartId",
                table: "CatalogPricing");

            migrationBuilder.DropIndex(
                name: "IX_CatalogPricing_ServiceId",
                table: "CatalogPricing");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "PartId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "CatalogPricing");

            migrationBuilder.DropColumn(
                name: "PartId",
                table: "CatalogPricing");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "CatalogPricing");

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Services",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Parts",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Parts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "CatalogItemType",
                table: "InventoryItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "CatalogItemId",
                table: "InventoryItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Devices",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ManufacturerId",
                table: "Devices",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "ItemType",
                table: "CatalogPricing",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CatalogPricing",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "CatalogPricing",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Manufacturers_ManufacturerId",
                table: "Devices",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Manufacturers_ManufacturerId",
                table: "Parts",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id");
        }
    }
}
