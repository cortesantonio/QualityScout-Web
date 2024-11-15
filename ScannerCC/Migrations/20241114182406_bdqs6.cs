using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualityScout.Migrations
{
    /// <inheritdoc />
    public partial class bdqs6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InformacionQuimica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cepa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAzucar = table.Column<float>(type: "real", nullable: false),
                    MaxAzucar = table.Column<float>(type: "real", nullable: false),
                    MinSulfuroso = table.Column<float>(type: "real", nullable: false),
                    MaxSulfuroso = table.Column<float>(type: "real", nullable: false),
                    MinDensidad = table.Column<float>(type: "real", nullable: false),
                    MaxDensidad = table.Column<float>(type: "real", nullable: false),
                    MinGradoAlcohol = table.Column<float>(type: "real", nullable: false),
                    MaxGradoAlcohol = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformacionQuimica", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    idRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.idRol);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rut = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "idRol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Informe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarios = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enfoque = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Informe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Informe_Usuario_IdUsuarios",
                        column: x => x.IdUsuarios,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdInformacionQuimica = table.Column<int>(type: "int", nullable: false),
                    CodigoBarra = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoVE = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    URLImagen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaisDestino = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUsuarios = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Idioma = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescripcionCapsula = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producto_InformacionQuimica_IdInformacionQuimica",
                        column: x => x.IdInformacionQuimica,
                        principalTable: "InformacionQuimica",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Producto_Usuario_IdUsuarios",
                        column: x => x.IdUsuarios,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BotellaDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreBotella = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlturaBotella = table.Column<int>(type: "int", nullable: false),
                    AnchoBotella = table.Column<int>(type: "int", nullable: false),
                    ProductosId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotellaDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BotellaDetalle_Producto_ProductosId",
                        column: x => x.ProductosId,
                        principalTable: "Producto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Controles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProductos = table.Column<int>(type: "int", nullable: false),
                    Linea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaisDestino = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipodecontrol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaHoraPrimerControl = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoFinal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUsuarios = table.Column<int>(type: "int", nullable: false),
                    FechaHoraControlFinal = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Controles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Controles_Producto_IdProductos",
                        column: x => x.IdProductos,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Controles_Usuario_IdUsuarios",
                        column: x => x.IdUsuarios,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Escaneo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProductos = table.Column<int>(type: "int", nullable: false),
                    IdUsuarios = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escaneo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Escaneo_Producto_IdProductos",
                        column: x => x.IdProductos,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Escaneo_Usuario_IdUsuarios",
                        column: x => x.IdUsuarios,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductoHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProductos = table.Column<int>(type: "int", nullable: false),
                    FechaCosecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaProduccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEnvasado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoHistorial_Producto_IdProductos",
                        column: x => x.IdProductos,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProductos = table.Column<int>(type: "int", nullable: false),
                    IdBotellaDetalles = table.Column<int>(type: "int", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    TipoCapsula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoEtiqueta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColorBotella = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Medalla = table.Column<bool>(type: "bit", nullable: false),
                    ColorCapsula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoCorcho = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedidaEtiquetaABoquete = table.Column<int>(type: "int", nullable: false),
                    MedidaEtiquetaABase = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductoDetalle_BotellaDetalle_IdBotellaDetalles",
                        column: x => x.IdBotellaDetalles,
                        principalTable: "BotellaDetalle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoDetalle_Producto_IdProductos",
                        column: x => x.IdProductos,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotellaDetalle_ProductosId",
                table: "BotellaDetalle",
                column: "ProductosId");

            migrationBuilder.CreateIndex(
                name: "IX_Controles_IdProductos",
                table: "Controles",
                column: "IdProductos");

            migrationBuilder.CreateIndex(
                name: "IX_Controles_IdUsuarios",
                table: "Controles",
                column: "IdUsuarios");

            migrationBuilder.CreateIndex(
                name: "IX_Escaneo_IdProductos",
                table: "Escaneo",
                column: "IdProductos");

            migrationBuilder.CreateIndex(
                name: "IX_Escaneo_IdUsuarios",
                table: "Escaneo",
                column: "IdUsuarios");

            migrationBuilder.CreateIndex(
                name: "IX_Informe_IdUsuarios",
                table: "Informe",
                column: "IdUsuarios");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdInformacionQuimica",
                table: "Producto",
                column: "IdInformacionQuimica");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdUsuarios",
                table: "Producto",
                column: "IdUsuarios");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoDetalle_IdBotellaDetalles",
                table: "ProductoDetalle",
                column: "IdBotellaDetalles");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoDetalle_IdProductos",
                table: "ProductoDetalle",
                column: "IdProductos");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoHistorial_IdProductos",
                table: "ProductoHistorial",
                column: "IdProductos");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_RolId",
                table: "Usuario",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Controles");

            migrationBuilder.DropTable(
                name: "Escaneo");

            migrationBuilder.DropTable(
                name: "Informe");

            migrationBuilder.DropTable(
                name: "ProductoDetalle");

            migrationBuilder.DropTable(
                name: "ProductoHistorial");

            migrationBuilder.DropTable(
                name: "BotellaDetalle");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "InformacionQuimica");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
