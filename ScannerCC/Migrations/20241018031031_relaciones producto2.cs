using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityScout.Migrations
{
    /// <inheritdoc />
    public partial class relacionesproducto2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoBotella",
                table: "ProductoDetalle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoBotella",
                table: "ProductoDetalle",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
