using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RoomRentalManagerServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contract",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roomRentalId = table.Column<long>(type: "bigint", nullable: false),
                    tenantId = table.Column<long>(type: "bigint", nullable: false),
                    startDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    depositAmout = table.Column<decimal>(type: "numeric", nullable: false),
                    monthlyRent = table.Column<decimal>(type: "numeric", nullable: false),
                    statusContract = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creatorUser = table.Column<string>(type: "text", nullable: false),
                    updaterUser = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contract", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastUpdateUser = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creatorUser = table.Column<string>(type: "text", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "image_descriptions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imageFileName = table.Column<string>(type: "text", nullable: false),
                    image = table.Column<byte[]>(type: "bytea", nullable: false),
                    imageType = table.Column<string>(type: "text", nullable: false),
                    createdDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creatorUser = table.Column<string>(type: "text", nullable: false),
                    lastUpdateUser = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_descriptions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "invoice",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contractId = table.Column<long>(type: "bigint", nullable: false),
                    invoiceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    totalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    invoiceStatus = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creatorUser = table.Column<string>(type: "text", nullable: false),
                    lastUpdateUser = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    invoiceId = table.Column<long>(type: "bigint", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    paymentDate = table.Column<decimal>(type: "numeric", nullable: false),
                    paymentMethod = table.Column<int>(type: "integer", nullable: false),
                    statusPayment = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lastUpdateUser = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roomEquipment",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roomId = table.Column<long>(type: "bigint", nullable: false),
                    equipmentId = table.Column<long>(type: "bigint", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roomEquipment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roomrental",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roomNumber = table.Column<int>(type: "integer", nullable: false),
                    roomType = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: false),
                    statusRoom = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    area = table.Column<double>(type: "double precision", nullable: false),
                    images = table.Column<List<int>>(type: "integer[]", nullable: true),
                    createdDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creatorUser = table.Column<string>(type: "text", nullable: false),
                    lsatUpdateUser = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roomrental", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleGroupId = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    provinceId = table.Column<long>(type: "bigint", nullable: false),
                    districtId = table.Column<long>(type: "bigint", nullable: false),
                    wardId = table.Column<long>(type: "bigint", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    idCard = table.Column<long>(type: "bigint", nullable: false),
                    job = table.Column<string>(type: "text", nullable: false),
                    dateofbirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: false),
                    bikeId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contract");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "image_descriptions");

            migrationBuilder.DropTable(
                name: "invoice");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "roomEquipment");

            migrationBuilder.DropTable(
                name: "roomrental");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
