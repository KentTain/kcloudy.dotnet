using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KC.DataAccess.Pay.Migrations
{
    public partial class initComPayContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cTest");

            migrationBuilder.CreateTable(
                name: "pay_BankAccount",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MemberId = table.Column<string>(maxLength: 40, nullable: true),
                    BankId = table.Column<string>(maxLength: 20, nullable: true),
                    BankName = table.Column<string>(maxLength: 256, nullable: true),
                    AccountNum = table.Column<string>(maxLength: 32, nullable: true),
                    AccountName = table.Column<string>(maxLength: 128, nullable: true),
                    AccountType = table.Column<int>(nullable: false),
                    BankAccountType = table.Column<string>(maxLength: 10, nullable: true),
                    CardType = table.Column<string>(maxLength: 10, nullable: true),
                    CardNumber = table.Column<string>(maxLength: 32, nullable: true),
                    CrossMark = table.Column<string>(maxLength: 32, nullable: true),
                    OpenBankCode = table.Column<string>(maxLength: 12, nullable: true),
                    OpenBankName = table.Column<string>(maxLength: 128, nullable: true),
                    ProvinceCode = table.Column<string>(maxLength: 20, nullable: true),
                    ProvinceName = table.Column<string>(maxLength: 128, nullable: true),
                    CityCode = table.Column<string>(maxLength: 20, nullable: true),
                    CityName = table.Column<string>(maxLength: 128, nullable: true),
                    BankState = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_BankAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_CashUsageDetail",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentCustomerId = table.Column<string>(maxLength: 128, nullable: true),
                    PaymentCustomerDisplayName = table.Column<string>(maxLength: 128, nullable: true),
                    PayeeCustomerId = table.Column<string>(maxLength: 128, nullable: true),
                    PayeeCustomerDisplayName = table.Column<string>(maxLength: 128, nullable: true),
                    ReferenceId = table.Column<string>(maxLength: 128, nullable: true),
                    ConsumptionMoney = table.Column<decimal>(nullable: false),
                    ConsumptionDate = table.Column<DateTime>(nullable: false),
                    CashType = table.Column<int>(nullable: false),
                    CashStatus = table.Column<bool>(nullable: true),
                    TradingAccount = table.Column<int>(nullable: false),
                    PaymentMethod = table.Column<string>(maxLength: 128, nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    PaymentType = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_CashUsageDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_CautionMoneyLog",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderNum = table.Column<string>(maxLength: 128, nullable: true),
                    OtherParty = table.Column<string>(maxLength: 128, nullable: true),
                    Operator = table.Column<string>(maxLength: 128, nullable: true),
                    ActionName = table.Column<string>(maxLength: 128, nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    PaymentNumber = table.Column<string>(maxLength: 128, nullable: true),
                    IsReceivable = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_CautionMoneyLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_EntrustedPaymentRecord",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<string>(maxLength: 128, nullable: true),
                    PayableNumber = table.Column<string>(maxLength: 128, nullable: true),
                    PaymentAmount = table.Column<decimal>(nullable: false),
                    SellerTenantName = table.Column<string>(maxLength: 128, nullable: true),
                    Seller = table.Column<string>(maxLength: 128, nullable: true),
                    GuaranteeTenantName = table.Column<string>(maxLength: 128, nullable: true),
                    Guarantee = table.Column<string>(maxLength: 128, nullable: true),
                    EntrustedPaymentStatus = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_EntrustedPaymentRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_OfflinePayment",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<string>(maxLength: 128, nullable: true),
                    PayableNumber = table.Column<string>(maxLength: 128, nullable: true),
                    PayDateTime = table.Column<DateTime>(nullable: false),
                    AmountOfMoney = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    SupplementRemark = table.Column<string>(nullable: true),
                    BusinessNumber = table.Column<string>(maxLength: 128, nullable: true),
                    ReceivableSource = table.Column<int>(nullable: true),
                    PayableSource = table.Column<int>(nullable: true),
                    Customer = table.Column<string>(maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_OfflinePayment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_OfflineUsageBill",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<string>(maxLength: 128, nullable: true),
                    PayableNumber = table.Column<string>(maxLength: 128, nullable: true),
                    PayDateTime = table.Column<DateTime>(nullable: false),
                    AmountOfMoney = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    BillNumber = table.Column<string>(maxLength: 128, nullable: true),
                    BankBill = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    SupplementRemark = table.Column<string>(nullable: true),
                    BusinessNumber = table.Column<string>(maxLength: 128, nullable: true),
                    ReceivableSource = table.Column<int>(nullable: true),
                    PayableSource = table.Column<int>(nullable: true),
                    Customer = table.Column<string>(maxLength: 128, nullable: true),
                    CashPayment = table.Column<bool>(nullable: true),
                    CreditUsageDetailId = table.Column<string>(maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_OfflineUsageBill", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_OnlinePaymentRecord",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaymentOrderId = table.Column<string>(maxLength: 128, nullable: true),
                    OrderNo = table.Column<string>(maxLength: 128, nullable: false),
                    OrderAmount = table.Column<decimal>(nullable: false),
                    PaymentAmount = table.Column<decimal>(nullable: false),
                    OrderDatetime = table.Column<string>(maxLength: 128, nullable: true),
                    PayDatetime = table.Column<string>(maxLength: 128, nullable: true),
                    ReturnDatetime = table.Column<string>(maxLength: 128, nullable: true),
                    PayResult = table.Column<string>(maxLength: 128, nullable: true),
                    VerifyResult = table.Column<string>(maxLength: 128, nullable: true),
                    ErrorCode = table.Column<string>(maxLength: 128, nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    BankName = table.Column<string>(maxLength: 128, nullable: true),
                    BankNumber = table.Column<string>(maxLength: 128, nullable: true),
                    CurrencyType = table.Column<string>(maxLength: 128, nullable: true),
                    PaymentMethod = table.Column<string>(maxLength: 128, nullable: true),
                    ConfigId = table.Column<int>(nullable: false),
                    MemberId = table.Column<string>(nullable: true),
                    PeeMemberId = table.Column<string>(nullable: true),
                    SearchCount = table.Column<int>(nullable: false),
                    NextSearchTime = table.Column<DateTime>(nullable: false),
                    OperationType = table.Column<int>(nullable: false),
                    PaymentType = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_OnlinePaymentRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_Payable",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayableNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Source = table.Column<int>(nullable: false),
                    OrderId = table.Column<string>(maxLength: 128, nullable: true),
                    OrderAmount = table.Column<decimal>(nullable: false),
                    PayableAmount = table.Column<decimal>(nullable: false),
                    AlreadyPayAmount = table.Column<decimal>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Customer = table.Column<string>(maxLength: 128, nullable: true),
                    CustomerTenant = table.Column<string>(maxLength: 128, nullable: true),
                    Remark = table.Column<string>(maxLength: 1024, nullable: true),
                    UsePoint = table.Column<int>(nullable: false),
                    PointEqualMoney = table.Column<decimal>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_Payable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_PayableAndReceivableRecord",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayableNumber = table.Column<string>(maxLength: 128, nullable: true),
                    PayDateTime = table.Column<DateTime>(nullable: false),
                    AmountOfMoney = table.Column<decimal>(nullable: false),
                    PaymentType = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    Operator = table.Column<string>(maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_PayableAndReceivableRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_PaymentAttachment",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusinessNumber = table.Column<string>(maxLength: 128, nullable: true),
                    BlobId = table.Column<string>(maxLength: 128, nullable: true),
                    FileName = table.Column<string>(maxLength: 128, nullable: true),
                    Url = table.Column<string>(maxLength: 1024, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_PaymentAttachment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_PaymentBankAccount",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountNo = table.Column<string>(maxLength: 36, nullable: true),
                    AccountName = table.Column<string>(maxLength: 128, nullable: true),
                    BankEId = table.Column<string>(maxLength: 128, nullable: true),
                    MemberId = table.Column<string>(maxLength: 40, nullable: true),
                    State = table.Column<int>(nullable: false),
                    BindBankAccountId = table.Column<int>(nullable: false),
                    BindBankAccount = table.Column<string>(maxLength: 40, nullable: true),
                    BindBankAccountName = table.Column<string>(maxLength: 128, nullable: true),
                    BindBankId = table.Column<string>(maxLength: 20, nullable: true),
                    BindBankName = table.Column<string>(nullable: true),
                    OpenBankCode = table.Column<string>(maxLength: 20, nullable: true),
                    OpenBankName = table.Column<string>(maxLength: 128, nullable: true),
                    PaymentType = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    FreezeAmount = table.Column<decimal>(nullable: false),
                    CFWinTotalAmount = table.Column<decimal>(nullable: false),
                    CFWinFreezeAmount = table.Column<decimal>(nullable: false),
                    AmountUpdateTime = table.Column<DateTime>(nullable: false),
                    IsPlatformAccount = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_PaymentBankAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_PaymentInfo",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    TradePassword = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    PaymentType = table.Column<int>(nullable: false),
                    PaymentAccount = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_PaymentInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_PaymentOperationLog",
                schema: "cTest",
                columns: table => new
                {
                    ProcessLogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(nullable: false),
                    OperatorId = table.Column<string>(nullable: true),
                    Operator = table.Column<string>(maxLength: 50, nullable: true),
                    OperateDate = table.Column<DateTime>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    PaymentNumber = table.Column<string>(maxLength: 128, nullable: true),
                    ReferenceId = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_PaymentOperationLog", x => x.ProcessLogId);
                });

            migrationBuilder.CreateTable(
                name: "pay_PaymentRecord",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayableNumber = table.Column<string>(maxLength: 128, nullable: true),
                    PaymentNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Status = table.Column<bool>(nullable: true),
                    PayeeTenant = table.Column<string>(maxLength: 128, nullable: true),
                    Payee = table.Column<string>(maxLength: 128, nullable: true),
                    OrderNumber = table.Column<string>(nullable: true),
                    Source = table.Column<int>(nullable: true),
                    PaymentAmount = table.Column<decimal>(nullable: false),
                    Remark = table.Column<string>(nullable: true),
                    TradingAccount = table.Column<int>(nullable: false),
                    PaymentType = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_PaymentRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_PaymentTradeRecord",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MemberId = table.Column<string>(nullable: true),
                    SrlNo = table.Column<string>(maxLength: 40, nullable: true),
                    RetSrlNo = table.Column<string>(maxLength: 40, nullable: true),
                    PaymentName = table.Column<string>(maxLength: 30, nullable: true),
                    PaymentType = table.Column<int>(nullable: false),
                    InterfaceName = table.Column<string>(maxLength: 30, nullable: true),
                    OperationType = table.Column<int>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    RetCode = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ReferenceId = table.Column<int>(nullable: false),
                    PostXML = table.Column<string>(nullable: true),
                    ReturnXML = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_PaymentTradeRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_Receivable",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayableNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Source = table.Column<int>(nullable: false),
                    OrderId = table.Column<string>(maxLength: 128, nullable: true),
                    OrderAmount = table.Column<decimal>(nullable: false),
                    ReceivableAmount = table.Column<decimal>(nullable: false),
                    AlreadyPayAmount = table.Column<decimal>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Customer = table.Column<string>(maxLength: 128, nullable: true),
                    CustomerTenant = table.Column<string>(maxLength: 128, nullable: true),
                    Remark = table.Column<string>(maxLength: 1024, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_Receivable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pay_VoucherPaymentRecord",
                schema: "cTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<string>(maxLength: 128, nullable: true),
                    PayableNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Amounts = table.Column<decimal>(nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Debtor = table.Column<string>(maxLength: 128, nullable: true),
                    DebtorBankNumber = table.Column<string>(maxLength: 128, nullable: true),
                    DebtorBank = table.Column<string>(maxLength: 128, nullable: true),
                    DebtorSocialCreditCode = table.Column<string>(maxLength: 128, nullable: true),
                    Creditor = table.Column<string>(maxLength: 128, nullable: true),
                    CreditorBank = table.Column<string>(maxLength: 128, nullable: true),
                    CreditorBankNumber = table.Column<string>(maxLength: 128, nullable: true),
                    CreditorSocialCreditCode = table.Column<string>(maxLength: 128, nullable: true),
                    VoucherId = table.Column<string>(maxLength: 128, nullable: true),
                    CreditUsageId = table.Column<string>(maxLength: 128, nullable: true),
                    FinancialInstitutionTenantName = table.Column<string>(maxLength: 128, nullable: true),
                    FinancialInstitutionName = table.Column<string>(maxLength: 128, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pay_VoucherPaymentRecord", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pay_BankAccount",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_CashUsageDetail",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_CautionMoneyLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_EntrustedPaymentRecord",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_OfflinePayment",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_OfflineUsageBill",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_OnlinePaymentRecord",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_Payable",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_PayableAndReceivableRecord",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_PaymentAttachment",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_PaymentBankAccount",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_PaymentInfo",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_PaymentOperationLog",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_PaymentRecord",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_PaymentTradeRecord",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_Receivable",
                schema: "cTest");

            migrationBuilder.DropTable(
                name: "pay_VoucherPaymentRecord",
                schema: "cTest");
        }
    }
}
