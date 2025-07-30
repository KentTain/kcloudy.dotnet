using System;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using KC.Service.Pay;
using KC.Service.DTO.Pay;
using KC.Common.ToolsHelper;
using KC.Service.DTO.Pay.BoHai;
using KC.Service.DTO.Pay.CMB;
using KC.Common;
using KC.Framework.Base;
using System.Web;
using KC.Common.HttpHelper;

namespace KC.UnitTest.Pay
{
    public class PaymentServiceTest : PayTestBase
    {
        private ILogger _logger;
        public PaymentServiceTest(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(PaymentServiceTest));
        }

        #region 中金

        private const string payApiUrl = "http://localpay.starlu.com/";
        [Xunit.Fact]
        public void CheckOnlinePaymentJob()
        {
            InjectTenant(TestTenant);

            var PaymentService = ServiceProvider.GetService<IPaymentService>();
            var payments = PaymentService.QueryCPCNChargeLastHourNotComplete();
            //循环去跑
            foreach (var item in payments)
            {
                InTransactionSearchDTO paramDTO = new InTransactionSearchDTO();
                paramDTO.MemberId = item.MemberId;
                paramDTO.UserName = "Job";
                paramDTO.PaymentOrderId = item.PaymentOrderId;
                var postUrl = payApiUrl + "CPCNPay" + "/" + "InTransactionSearch";
                var postData = GetPostData(paramDTO);
                var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
            }
        }


        [Xunit.Fact]
        public void Test_FreezeAmount()
        {
            FreezeAmtDTO paramDTO = new FreezeAmtDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.Usage = "333";
            paramDTO.FreezeAmt = 100;
            paramDTO.BusiType = 2;
            paramDTO.PaymentOrderId = OtherUtilHelper.GetSerialNumber("Fz");
            var postUrl = payApiUrl + "CPCNPay" + "/" + "InTransactionSearch";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }


        [Xunit.Fact]
        public void Test_SearchWithdrawAmt()
        {
            PayBaseParamDTO paramDTO = new PayBaseParamDTO();
            paramDTO.MemberId = "DevDB";
            paramDTO.UserName = "test";

            var postUrl = payApiUrl + "CPCNPay" + "/" + "SearchWithdrawAmt";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        [Xunit.Fact]
        public void Test_QRInTransaction()
        {
            InTransactionDTO paramDTO = new InTransactionDTO();
            paramDTO.Amount = 900M;
            paramDTO.PaymentOrderId = OtherUtilHelper.GetSerialNumber("");
            paramDTO.OrderNo = OtherUtilHelper.GetSerialNumber("OD");
            paramDTO.PeeMemberId = "Devdb123";
            //paramDTO.PaymentOrderId = "20190807095954682291";
            paramDTO.Usage = "333";
            paramDTO.MemberId = "Devdb123";
            paramDTO.UserName = "123";
            paramDTO.SecPayType = 3;
            var postUrl = payApiUrl + "CPCNPay" + "/" + "QRInTransaction";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        #endregion

        #region CMB


        [Xunit.Fact]
        public void CheckCBSOnlinePaymentJob()
        {
            InjectTenant(TestTenant);

            var PaymentService = ServiceProvider.GetService<IPaymentService>();
            var payments = PaymentService.QueryCBSPayLastDayNotComplete();
            //循环去跑
            foreach (var item in payments)
            {

                CMBPaySearchDTO paramDTO = new CMBPaySearchDTO();
                paramDTO.MemberId = item.MemberId;
                paramDTO.UserName = "Job";
                paramDTO.PaymentOrderId = item.PaymentOrderId;
                paramDTO.CBSReturnId = item.BankNumber;
                var postUrl = "http://localpay.cfwin.com/" + "CMBPay" + "/" + "PaySearch";
                var postData = GetPostData(paramDTO);
                var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
            }

        }

        [Xunit.Fact]
        public void Test_CMBPay()
        {
            CMBPayDTO paramDTO = new CMBPayDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.OrderNo = "12345678999990";
            paramDTO.Amount = 3.98M;
            paramDTO.Usage = "1234";
            paramDTO.PayeeMemberId = "Devdb";
            paramDTO.PaymentOrderId = OtherUtilHelper.GetSerialNumber("pay");
            paramDTO.AccountNumber = "78901234567888";
            paramDTO.CLTNBR = "sz06";
            //paramDTO.PaymentOrderId = "Pay20180921140737276400";
            var postUrl = "http://localpay.cfwin.com/" + "CMBPay" + "/" + "Pay";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        [Xunit.Fact]
        public void Test_CMBPaySearch()
        {
            CMBPaySearchDTO paramDTO = new CMBPaySearchDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            //paramDTO.PaymentOrderId = "pay20180925201319785911";
            //paramDTO.CBSReturnId = "AP2000001571";
            paramDTO.PaymentOrderId = "BSH1800009008";
            paramDTO.CBSReturnId = "AP2000003028";
            var postUrl = "http://localpay.cfwin.com/" + "CMBPay" + "/" + "PaySearch";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);

            if (result.Item1)
            {
                var returnModel = SerializeHelper.FromJson<PaymentModel>(result.Item2);
                if (returnModel.Success)
                {
                    var payResult = returnModel.ReturnData.ToString();
                    if (payResult == "1")
                    {
                        var noticeUrl = GlobalConfig.PayWebDomain.Replace("subdomain", "cTest") + "/Home/CBSCallBack?paymentOrderId=" + paramDTO.PaymentOrderId;
                        HttpWebRequestHelper.WebClientDownload(noticeUrl);
                    }
                }
            }

        }

        [Xunit.Fact]
        public void Test_GetBankList()
        {
            CMBBankListDTO paramDTO = new CMBBankListDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.CompanyName = "财富共赢集团（深圳）有限公司";
            var postUrl = "http://localpay.cfwin.com/" + "CMBPay" + "/" + "GetBankList";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        #endregion

        #region 渤海云账本单元测试

        [Xunit.Fact]
        public void Test_BH_OpenAcct()
        {
            OpenBankParamDTO paramDTO = new OpenBankParamDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.Password = "123456";
            var postUrl = payApiUrl + "BoHaiPay" + "/" + "AcctOpen";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        [Xunit.Fact]
        public void Test_BHOpenAcct1()
        {
            OpenBankParamDTO paramDTO = new OpenBankParamDTO();
            paramDTO.MemberId = "UR20120418000013";
            paramDTO.UserName = "123";
            paramDTO.Password = "123456";
            //paramDTO.BankId = 1005;
            var postUrl = payApiUrl + "BoHaiPay" + "/" + "AcctOpen";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        [Xunit.Fact]
        public void Test_BH_AcctQry()
        {
            PayBaseParamDTO paramDTO = new PayBaseParamDTO();
            //paramDTO.MemberId = "UR20120418000013";
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            var postUrl = payApiUrl + "BoHaiPay" + "/" + "AcctQry";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }



        [Xunit.Fact]
        public void Test_BH_AcctBinding()
        {
            BindBankAccountDTO paramDTO = new BindBankAccountDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.BankId = 1;
            paramDTO.Password = "123456";

            var postUrl = payApiUrl + "BoHaiPay" + "/" + "AcctBinding";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        [Xunit.Fact]
        public void Test_BH_AcctBalance()
        {
            PayBaseParamDTO paramDTO = new PayBaseParamDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";

            var postUrl = payApiUrl + "BoHaiPay" + "/" + "AcctBalance";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }


        [Xunit.Fact]
        public void Test_BH_Transfer()
        {
            OrderPayDTO paramDTO = new OrderPayDTO();

            paramDTO.Amount = 600M;
            paramDTO.PaymentOrderId = OtherUtilHelper.GetSerialNumber("");
            paramDTO.Usage = "333";
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.Password = "654321";
            paramDTO.PayType = 2;
            paramDTO.PayeeAccountNumber = "200307159011";
            paramDTO.OrderNo = OtherUtilHelper.GetSerialNumber("Pay");
            var postUrl = payApiUrl + "BoHaiPay" + "/" + "Transfer";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        [Xunit.Fact]

        public void Test_BH_TransferIn()
        {
            InTransactionDTO paramDTO = new InTransactionDTO();
            paramDTO.Amount = 100M;
            paramDTO.PaymentOrderId = OtherUtilHelper.GetSerialNumber("");
            paramDTO.Usage = "333";
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.Password = "123456";

            var postUrl = payApiUrl + "BoHaiPay" + "/" + "OrderPayment";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        [Xunit.Fact]
        public void Test_BH_TransferOut()
        {
            OutTransactionDTO paramDTO = new OutTransactionDTO();
            paramDTO.Amount = 200M;
            paramDTO.PaymentOrderId = OtherUtilHelper.GetSerialNumber("");
            paramDTO.Usage = "333";
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.Password = "123456";

            var postUrl = payApiUrl + "BoHaiPay" + "/" + "TransferOut";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        [Xunit.Fact]
        public void Test_BH_Freeze()
        {
            FreezeAmtDTO paramDTO = new FreezeAmtDTO();
            paramDTO.FreezeAmt = 80M;
            paramDTO.PaymentOrderId = OtherUtilHelper.GetSerialNumber("");
            paramDTO.Usage = "333";
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.BusiType = 1;
            paramDTO.OrderNo = "20190424175807986854";
            var postUrl = payApiUrl + "BoHaiPay" + "/" + "AccFreeze";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }


        [Xunit.Fact]
        public void Test_BH_UnFreeze()
        {
            FreezeAmtDTO paramDTO = new FreezeAmtDTO();
            paramDTO.FreezeAmt = 60M;
            paramDTO.PaymentOrderId = OtherUtilHelper.GetSerialNumber("");
            paramDTO.Usage = "333";
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.BusiType = 2;
            paramDTO.OrderNo = "20190424175807986854";

            var postUrl = payApiUrl + "BoHaiPay" + "/" + "AccFreeze";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        /// <summary>
        /// 查询卡片是否支持
        /// </summary>
        [Xunit.Fact]
        public void Test_BH_CardSupportInfoQry()
        {
            CardSupportParamDTO paramDTO = new CardSupportParamDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.AcctNo = "6225883655693266";

            //model.AcctNo = "79170155300001387";

            var postUrl = payApiUrl + "BoHaiPay" + "/" + "CardSupportInfoQry";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        [Xunit.Fact]
        public void Test_BH_UpdatePassword()
        {
            UpdatePasswordParamDTO paramDTO = new UpdatePasswordParamDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.OldPassword = "123456";
            paramDTO.Password = "654321";
            var postUrl = payApiUrl + "BoHaiPay" + "/" + "UpdatePassword";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        [Xunit.Fact]
        public void Test_BH_ResetPassword()
        {
            PayBaseParamDTO paramDTO = new PayBaseParamDTO();
            paramDTO.MemberId = "Devdb";
            paramDTO.UserName = "123";
            paramDTO.Password = "666666";
            var postUrl = payApiUrl + "BoHaiPay" + "/" + "ResetPassword";
            var postData = GetPostData(paramDTO);
            var result = HttpWebRequestHelper.WebClientDownload(postUrl, postData);

        }

        #endregion

        public string GetPostData<T>(T paramDTO)
        {
            string retStr = string.Empty;
            foreach (System.Reflection.PropertyInfo p in paramDTO.GetType().GetProperties())
            {
                var value = p.GetValue(paramDTO, null);
                if (value != null)
                {
                    if (!string.IsNullOrEmpty(retStr))
                    {
                        retStr = retStr + "&" + p.Name + "=" + HttpUtility.UrlEncode(value.ToString());
                    }
                    else
                    {
                        retStr = p.Name + "=" + HttpUtility.UrlEncode(value.ToString());
                    }
                }
            }
            return retStr;
        }
    }

    public class PaymentModel
    {
        public bool Success { get; set; }

        public object ReturnData { get; set; }
    }
}
