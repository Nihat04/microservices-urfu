using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0.00m),
                    stock_quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    booked_quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.CheckConstraint("ck_products_booked_quantity_non_negative", "booked_quantity >= 0");
                    table.CheckConstraint("ck_products_booked_quantity_valid", "booked_quantity <= stock_quantity");
                    table.CheckConstraint("ck_products_price_non_negative", "price >= 0");
                    table.CheckConstraint("ck_products_stock_quantity_maximum", "stock_quantity <= 10000");
                    table.CheckConstraint("ck_products_stock_quantity_non_negative", "stock_quantity >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "ix_products_created_at",
                table: "products",
                column: "created_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_products_name",
                table: "products",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_products_price",
                table: "products",
                column: "price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
