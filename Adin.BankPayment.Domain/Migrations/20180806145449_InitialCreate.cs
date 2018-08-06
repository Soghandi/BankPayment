using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Adin.BankPayment.Domain.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Title = table.Column<string>(maxLength: 32, nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    PublicKey = table.Column<string>(maxLength: 128, nullable: true),
                    PrivateKey = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bank",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Title = table.Column<string>(maxLength: 16, nullable: true),
                    PostUrl = table.Column<string>(maxLength: 64, nullable: true),
                    Code = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationBank",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    BankId = table.Column<Guid>(nullable: false)
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
                    Id = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    ApplicationId = table.Column<Guid>(nullable: false),
                    BankId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Mobile = table.Column<long>(nullable: true),
                    UserTrackCode = table.Column<string>(maxLength: 64, nullable: true),
                    BankTrackCode = table.Column<string>(maxLength: 64, nullable: true),
                    ReferenceNumber = table.Column<string>(maxLength: 64, nullable: true),
                    BankErrorCode = table.Column<int>(nullable: false),
                    BankRedirectUrl = table.Column<string>(maxLength: 128, nullable: true),
                    CallbackUrl = table.Column<string>(maxLength: 128, nullable: true),
                    ExpirationTime = table.Column<DateTime>(nullable: true),
                    BankErrorMessage = table.Column<string>(maxLength: 64, nullable: true),
                    ErrorCode = table.Column<byte>(nullable: false)
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
                    Id = table.Column<Guid>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    ApplicationBankId = table.Column<Guid>(nullable: false),
                    ParamKey = table.Column<string>(maxLength: 32, nullable: true),
                    ParamValue = table.Column<string>(maxLength: 128, nullable: true)
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
                columns: new[] { "Id", "Code", "CreatedBy", "CreationDate", "IsDeleted", "ModifiedBy", "ModifiedOn", "PostUrl", "RowVersion", "Status", "Title" },
                values: new object[] { new Guid("482a591e-7536-4f47-a544-e9d4342586bd"), (byte)1, 1, new DateTime(2018, 8, 6, 19, 24, 49, 523, DateTimeKind.Local), false, null, null, "https://sep.shaparak.ir/MobilePG/MobilePayment", null, (byte)0, "سامان" });

            migrationBuilder.InsertData(
                table: "Bank",
                columns: new[] { "Id", "Code", "CreatedBy", "CreationDate", "IsDeleted", "ModifiedBy", "ModifiedOn", "PostUrl", "RowVersion", "Status", "Title" },
                values: new object[] { new Guid("ab3f226a-be56-4092-bbd0-2ae8ffbce131"), (byte)2, 1, new DateTime(2018, 8, 6, 19, 24, 49, 525, DateTimeKind.Local), false, null, null, "https://pec.shaparak.ir/pecpaymentgateway/default.aspx?au={0}", null, (byte)0, "پارسیان" });

            migrationBuilder.InsertData(
                table: "Bank",
                columns: new[] { "Id", "Code", "CreatedBy", "CreationDate", "IsDeleted", "ModifiedBy", "ModifiedOn", "PostUrl", "RowVersion", "Status", "Title" },
                values: new object[] { new Guid("98504148-3d89-4abb-9fb5-281bed8714e3"), (byte)3, 1, new DateTime(2018, 8, 6, 19, 24, 49, 525, DateTimeKind.Local), false, null, null, "https://bpm.shaparak.ir/pgwchannel/startpay.mellat", null, (byte)0, "ملت" });

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
