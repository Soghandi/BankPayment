using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Adin.BankPayment.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PublicKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PrivateKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bank",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PostUrl = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Code = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationBank",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationBank_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationBank_Bank_BankId",
                        column: x => x.BankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Mobile = table.Column<long>(type: "bigint", nullable: true),
                    UserTrackCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BankTrackCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BankErrorCode = table.Column<int>(type: "int", nullable: false),
                    BankRedirectUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CallbackUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankErrorMessage = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ErrorCode = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaction_Bank_BankId",
                        column: x => x.BankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationBankParam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationBankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParamKey = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    ParamValue = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationBankParam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationBankParam_ApplicationBank_ApplicationBankId",
                        column: x => x.ApplicationBankId,
                        principalTable: "ApplicationBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Bank",
                columns: new[] { "Id", "Code", "CreatedBy", "CreationDate", "ModifiedBy", "ModifiedOn", "PostUrl", "Status", "Title" },
                values: new object[,]
                {
                    { new Guid("482a591e-7536-4f47-a544-e9d4342586bd"), (byte)1, 1, new DateTime(2020, 12, 26, 21, 21, 39, 519, DateTimeKind.Local).AddTicks(7727), null, null, "https://sep.shaparak.ir/MobilePG/MobilePayment", (byte)0, "سامان" },
                    { new Guid("ab3f226a-be56-4092-bbd0-2ae8ffbce131"), (byte)2, 1, new DateTime(2020, 12, 26, 21, 21, 39, 523, DateTimeKind.Local).AddTicks(1324), null, null, "https://pec.shaparak.ir/NewIPG?Token={0}", (byte)0, "پارسیان" },
                    { new Guid("98504148-3d89-4abb-9fb5-281bed8714e3"), (byte)3, 1, new DateTime(2020, 12, 26, 21, 21, 39, 523, DateTimeKind.Local).AddTicks(1429), null, null, "https://bpm.shaparak.ir/pgwchannel/startpay.mellat", (byte)0, "ملت" },
                    { new Guid("8c4d3794-983f-4610-bf88-abe8db1ad07d"), (byte)4, 1, new DateTime(2020, 12, 26, 21, 21, 39, 523, DateTimeKind.Local).AddTicks(1439), null, null, "https://pf.efarda.ir/pf/api/ipg/purchase", (byte)0, "تجارت الکترونیکی ارتباط فردا" },
                    { new Guid("e147830f-696d-4c4a-a7b4-b20722dd3ff6"), (byte)5, 1, new DateTime(2020, 12, 26, 21, 21, 39, 523, DateTimeKind.Local).AddTicks(1445), null, null, "https://pep.shaparak.ir/payment.aspx", (byte)0, "پاسارگاد" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationBank_ApplicationId",
                table: "ApplicationBank",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationBank_BankId",
                table: "ApplicationBank",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationBankParam_ApplicationBankId",
                table: "ApplicationBankParam",
                column: "ApplicationBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ApplicationId",
                table: "Transaction",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_BankId",
                table: "Transaction",
                column: "BankId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationBankParam");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "ApplicationBank");

            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "Bank");
        }
    }
}
