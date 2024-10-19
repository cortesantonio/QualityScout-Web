using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityScout.Migrations
{
    /// <inheritdoc />
    public partial class relacionesproductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Usuario_UsuariosId",
                table: "Producto");

            migrationBuilder.DropIndex(
                name: "IX_Producto_UsuariosId",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "UsuariosId",
                table: "Producto");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdUsuarios",
                table: "Producto",
                column: "IdUsuarios");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Usuario_IdUsuarios",
                table: "Producto",
                column: "IdUsuarios",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Usuario_IdUsuarios",
                table: "Producto");

            migrationBuilder.DropIndex(
                name: "IX_Producto_IdUsuarios",
                table: "Producto");

            migrationBuilder.AddColumn<int>(
                name: "UsuariosId",
                table: "Producto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Producto_UsuariosId",
                table: "Producto",
                column: "UsuariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Usuario_UsuariosId",
                table: "Producto",
                column: "UsuariosId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
