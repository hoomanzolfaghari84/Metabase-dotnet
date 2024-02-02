using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Metabase.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Databases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Databases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatabaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relations_Databases_DatabaseId",
                        column: x => x.DatabaseId,
                        principalTable: "Databases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RelationId = table.Column<int>(type: "int", nullable: false),
                    NotNull = table.Column<bool>(type: "bit", nullable: true),
                    Unique = table.Column<bool>(type: "bit", nullable: true),
                    PrimaryKey = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attributes_Relations_RelationId",
                        column: x => x.RelationId,
                        principalTable: "Relations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForeignKeyConstraints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferencedRelationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignKeyConstraints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForeignKeyConstraints_Relations_ReferencedRelationId",
                        column: x => x.ReferencedRelationId,
                        principalTable: "Relations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForeignKeyConstraints_Relations_RelationId",
                        column: x => x.RelationId,
                        principalTable: "Relations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FKReference",
                columns: table => new
                {
                    ForeignKeyConstraintId = table.Column<int>(type: "int", nullable: false),
                    ReferencingAttributeId = table.Column<int>(type: "int", nullable: false),
                    ReferencedAttributeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FKReference", x => new { x.ForeignKeyConstraintId, x.ReferencingAttributeId, x.ReferencedAttributeId });
                    table.ForeignKey(
                        name: "FK_FKReference_Attributes_ReferencedAttributeId",
                        column: x => x.ReferencedAttributeId,
                        principalTable: "Attributes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FKReference_Attributes_ReferencingAttributeId",
                        column: x => x.ReferencingAttributeId,
                        principalTable: "Attributes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FKReference_ForeignKeyConstraints_ForeignKeyConstraintId",
                        column: x => x.ForeignKeyConstraintId,
                        principalTable: "ForeignKeyConstraints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_RelationId",
                table: "Attributes",
                column: "RelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Databases_Name",
                table: "Databases",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FKReference_ReferencedAttributeId",
                table: "FKReference",
                column: "ReferencedAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_FKReference_ReferencingAttributeId",
                table: "FKReference",
                column: "ReferencingAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignKeyConstraints_ReferencedRelationId",
                table: "ForeignKeyConstraints",
                column: "ReferencedRelationId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignKeyConstraints_RelationId",
                table: "ForeignKeyConstraints",
                column: "RelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Relations_DatabaseId",
                table: "Relations",
                column: "DatabaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FKReference");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropTable(
                name: "ForeignKeyConstraints");

            migrationBuilder.DropTable(
                name: "Relations");

            migrationBuilder.DropTable(
                name: "Databases");
        }
    }
}
