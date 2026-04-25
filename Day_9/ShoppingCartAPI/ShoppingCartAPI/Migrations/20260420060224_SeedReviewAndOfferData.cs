using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedReviewAndOfferData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Offers",
                columns: new[] { "Id", "CouponCode", "DiscountPercentage", "IsActive", "ProductId" },
                values: new object[,]
                {
                    { 1, null, 10m, true, 1 },
                    { 2, "SAVE20", 20m, true, 1 },
                    { 3, null, 5m, true, 2 },
                    { 4, "AUDIO15", 15m, true, 3 },
                    { 5, "EXPIRED50", 50m, false, 3 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "ProductId", "Rating" },
                values: new object[,]
                {
                    { 1, "Great phone!", 1, 5 },
                    { 2, "Battery life is okay.", 1, 4 },
                    { 3, "Nice laptop for the price.", 2, 4 },
                    { 4, "Sound quality is amazing.", 3, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Offers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Offers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Offers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Offers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Offers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
