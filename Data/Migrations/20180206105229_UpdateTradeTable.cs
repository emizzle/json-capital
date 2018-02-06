using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JSONCapital.Data.Migrations
{
    public partial class UpdateTradeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TradeCurrencyBasePair",
                table: "Trades",
                newName: "SellCurrency");

            migrationBuilder.RenameColumn(
                name: "TradeCurrency",
                table: "Trades",
                newName: "ImportedTradeID");

            migrationBuilder.AddColumn<float>(
                name: "BuyAmount",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuyCurrency",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CoinTrackingTradeID",
                table: "Trades",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Exchange",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "FeeAmount",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeeCurrency",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportedFrom",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImportedTime",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "SellAmount",
                table: "Trades",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "Trades",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyAmount",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "BuyCurrency",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "CoinTrackingTradeID",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Exchange",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "FeeAmount",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "FeeCurrency",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ImportedFrom",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "ImportedTime",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "SellAmount",
                table: "Trades");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "Trades");

            migrationBuilder.RenameColumn(
                name: "SellCurrency",
                table: "Trades",
                newName: "TradeCurrencyBasePair");

            migrationBuilder.RenameColumn(
                name: "ImportedTradeID",
                table: "Trades",
                newName: "TradeCurrency");
        }
    }
}
