using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailsEcosystem.Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductFeaturedAndSoldCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SoldCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { true, 1240 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { true, 980 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { true, 760 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { true, 650 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { true, 520 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { true, 410 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { true, 385 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { true, 310 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 114,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 115,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 116,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 117,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 118,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 119,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 124,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 125,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 126,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 127,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 128,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 129,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 133,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 134,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 135,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 136,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 137,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 138,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 139,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 140,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 141,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 142,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 143,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 144,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 145,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 146,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 147,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 148,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 149,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 150,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 151,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 152,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 153,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 154,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 155,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 156,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 157,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 158,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 159,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 160,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 161,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 162,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 163,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 164,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 165,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 166,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 167,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 168,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 169,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 170,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 171,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 172,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 173,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 174,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 175,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 176,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 177,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 178,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 179,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 180,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 181,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 182,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 183,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 184,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 185,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 186,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 187,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 188,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 189,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 190,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 191,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 192,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 193,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 194,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 195,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 196,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 197,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 198,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 199,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 203,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 204,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 205,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 206,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 207,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 208,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 209,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 210,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 211,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 212,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 213,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 214,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 215,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 216,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 217,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 218,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 219,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 220,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 221,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 222,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 223,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 224,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 225,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 226,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 227,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 228,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 229,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 230,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 231,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 232,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 233,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 234,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 235,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 236,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 237,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 238,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 239,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 240,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 241,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 242,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 243,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 244,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 245,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 246,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 247,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 248,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 249,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 250,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 251,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 252,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 253,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 254,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 255,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 256,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 257,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 258,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 259,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 260,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 261,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 262,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 263,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 264,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 265,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 266,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 267,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 268,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 269,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 270,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 271,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 272,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 273,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 274,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 275,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 276,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 277,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 278,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 279,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 280,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 281,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 282,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 283,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 284,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 285,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 286,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 287,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 288,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 289,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 290,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 291,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 292,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 293,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 294,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 295,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 296,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 297,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 298,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 299,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 300,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 301,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 302,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 303,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 304,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 305,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 306,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 307,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 308,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 309,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 310,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 311,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 312,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 313,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 314,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 315,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 316,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 317,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 318,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 319,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 320,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 321,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 322,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 323,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 324,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 325,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 326,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 327,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 328,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 329,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 330,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 331,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 332,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 333,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 334,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 335,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 336,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 337,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 338,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 339,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 340,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 341,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 342,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 343,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 344,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 345,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 346,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 347,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 348,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 349,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 350,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 351,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 352,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 353,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 354,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 355,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 356,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 357,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 358,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 359,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 360,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 361,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 362,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 363,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 364,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 365,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 366,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 367,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 368,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 369,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 370,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 371,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 372,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 373,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 374,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 375,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 376,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 377,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 378,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 379,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 380,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 381,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 382,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 383,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 384,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 385,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 386,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 387,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 388,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 389,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 390,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 391,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 392,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 393,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 394,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 395,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 396,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 397,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 398,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 399,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 400,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 401,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 402,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 403,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 404,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 405,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 406,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 407,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 408,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 409,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 410,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 411,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 412,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 413,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 414,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 415,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 416,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 417,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 418,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 419,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 420,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 421,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 422,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 423,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 424,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 425,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 426,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 427,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 428,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 429,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 430,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 431,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 432,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 433,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 434,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 435,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 436,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 437,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 438,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 439,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 440,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 441,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 442,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 443,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 444,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 445,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 446,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 447,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 448,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 449,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 450,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 451,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 452,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 453,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 454,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 455,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 456,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 457,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 458,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 459,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 460,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 461,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 462,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 463,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 464,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 465,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 466,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 467,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 468,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 469,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 470,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 471,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 472,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 473,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 474,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 475,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 476,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 477,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 478,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 479,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 480,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 481,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 482,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 483,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 484,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 485,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 486,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 487,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 488,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 489,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 490,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 491,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 492,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 493,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 494,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 495,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 496,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 497,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 498,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 499,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 500,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 501,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 502,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 503,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 504,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 505,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 506,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 507,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 508,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 509,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 510,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 511,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 512,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 513,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 514,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 515,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 516,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 517,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 518,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 519,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 520,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 521,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 522,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 523,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 524,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 525,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 526,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 527,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 528,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 529,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 530,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 531,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 532,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 533,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 534,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 535,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 536,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 537,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 538,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 539,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 540,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 541,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 542,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 543,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 544,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 545,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 546,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 547,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 548,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 549,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 550,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 551,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 552,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 553,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 554,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 555,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 556,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 557,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 558,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 559,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 560,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 561,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 562,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 563,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 564,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 565,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 566,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 567,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 568,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 569,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 570,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 571,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 572,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 573,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 574,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 575,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 576,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 577,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 578,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 579,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 580,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 581,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 582,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 583,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 584,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 585,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 586,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 587,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 588,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 589,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 590,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 591,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 592,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 593,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 594,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 595,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 596,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 597,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 598,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 599,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 600,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 601,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 602,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 603,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 604,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 605,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 606,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 607,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 608,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 609,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 610,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 611,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 612,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 613,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 614,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 615,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 616,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 617,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 618,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 619,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 620,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 621,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 622,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 623,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 624,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 625,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 626,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 627,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 628,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 629,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 630,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 631,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 632,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 633,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 634,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 635,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 636,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 637,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 638,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 639,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 640,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 641,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 642,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 643,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 644,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 645,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 646,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 647,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 648,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 649,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 650,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 651,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 652,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 653,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 654,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 655,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 656,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 657,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 658,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 659,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 660,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 661,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 662,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 663,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 664,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 665,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 666,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 667,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 668,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 669,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 670,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 671,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 672,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 673,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 674,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 675,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 676,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 677,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 678,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 679,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 680,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 681,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 682,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 683,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 684,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 685,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 686,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 687,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 688,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 689,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 690,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 691,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 692,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 693,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 694,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 695,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 696,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 697,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 698,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 699,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 700,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 701,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 702,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 703,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 704,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 705,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 706,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 707,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 708,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 709,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 710,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 711,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 712,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 713,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 714,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 715,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 716,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 717,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 718,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 719,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 720,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 721,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 722,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 723,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 724,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 725,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 726,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 727,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 728,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 729,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 730,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 731,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 732,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 733,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 734,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 735,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 736,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 737,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 738,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 739,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 740,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 741,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 742,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 743,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 744,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 745,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 746,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 747,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 748,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 749,
                columns: new[] { "IsFeatured", "SoldCount" },
                values: new object[] { false, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SoldCount",
                table: "Products");
        }
    }
}
