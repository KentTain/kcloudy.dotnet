using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using KC.Framework.Extension;
using Xunit;
using KC.Framework.Base;
using Microsoft.Extensions.Logging;
using System.Collections;
using KC.Framework.Tenant;

namespace KC.Common.UnitTest
{
    /// <summary>
    /// EncryptTest 的摘要说明
    /// </summary>

    public class SerializeHelper_Test : KC.UnitTest.Core.CommomTestBase
    {
        private ILogger _logger;
        public SerializeHelper_Test(KC.UnitTest.CommonFixture data)
            : base(data)
        {
            _logger = LoggerFactory.CreateLogger(nameof(SerializeHelper_Test));
        }

        [Xunit.Fact]
        public void Test_SerializeHelper()
        {
            #region data
            var jsonData = @"[
    {
        '$id': '1',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016688',
        'CustomerName': '广东海辰科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': null,
        'ContactQQ': null,
        'ContactEmail': 'zshaichen@163.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0760-88389543',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016688',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '2',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016689',
        'CustomerName': '深圳壹创国际设计股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '董旭梅',
        'ContactQQ': null,
        'ContactEmail': 'donglin@yh-design.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-82795800',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016689',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '3',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016690',
        'CustomerName': '广东南帆科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '陈旭东',
        'ContactQQ': null,
        'ContactEmail': '396046754@qq.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0754-88801830',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016690',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '4',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016691',
        'CustomerName': '广东三盟科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '王国彬',
        'ContactQQ': null,
        'ContactEmail': 'sanmengkeji@sunmnet.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-37629898',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016691',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '5',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016692',
        'CustomerName': '广州市聚赛龙工程塑料股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '任萍',
        'ContactQQ': null,
        'ContactEmail': 'selon@gzselon.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-87886018',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016692',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '6',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016693',
        'CustomerName': '广州金穗隆信息科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '蔡妙玲',
        'ContactQQ': null,
        'ContactEmail': 'caimiaoling@kinslot.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-66608000',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016693',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '7',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016694',
        'CustomerName': '深圳市华移科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '胡蓉',
        'ContactQQ': null,
        'ContactEmail': 'cm@cnmobi.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-36631187',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016694',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '8',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016695',
        'CustomerName': '佛山市盈博莱科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '林永春',
        'ContactQQ': null,
        'ContactEmail': null,
        'ContactPhoneNumber': null,
        'ContactTelephone': '0757-81776161',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016695',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '9',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016696',
        'CustomerName': '深圳日盛科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '苏奕芬',
        'ContactQQ': null,
        'ContactEmail': 'sure@szgns.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-27384542',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016696',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '10',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016697',
        'CustomerName': '广东微云科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '何家亮',
        'ContactQQ': null,
        'ContactEmail': 'galen.he@microcloudsys.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0769-22860068',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016697',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '11',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016698',
        'CustomerName': '广州捷宝电子科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '邓韵图',
        'ContactQQ': null,
        'ContactEmail': 'mkt_dengyuntu@jiebaodz.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-85564970',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016698',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '12',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016699',
        'CustomerName': '广东一通科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '廖丽华',
        'ContactQQ': null,
        'ContactEmail': '4006666233@b.qq.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0760-23685033',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016699',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '13',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016700',
        'CustomerName': '深圳市君信达环境科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '李小晏',
        'ContactQQ': null,
        'ContactEmail': null,
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-26409509',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016700',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '14',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016701',
        'CustomerName': '超亮显示系统(深圳)股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '邓晓蓉',
        'ContactQQ': null,
        'ContactEmail': 'dxr@maxbright.KC.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-29810022',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016701',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '15',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016702',
        'CustomerName': '深圳市悦诚达信息技术股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '钟晖',
        'ContactQQ': null,
        'ContactEmail': 'zhonghui@yocod.KC.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-25322878',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016702',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '16',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016703',
        'CustomerName': '深圳瑞福来智能科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '董丽萍',
        'ContactQQ': null,
        'ContactEmail': 'DLP@reflyings.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-26718815',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016703',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '17',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016704',
        'CustomerName': '广东思柏科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '李勉其',
        'ContactQQ': null,
        'ContactEmail': 'info@simpact.KC.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-37700888',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016704',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '18',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016705',
        'CustomerName': '广州市森锐科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '朱岸青',
        'ContactQQ': null,
        'ContactEmail': null,
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-38488825',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016705',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '19',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016706',
        'CustomerName': '广州大石馆文化创意股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '赵巍',
        'ContactQQ': null,
        'ContactEmail': 'emg@emgstone.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-38663122',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016706',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '20',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016707',
        'CustomerName': '深圳市联合信息科技发展股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '宁维',
        'ContactQQ': null,
        'ContactEmail': 'ningw@szunion-info.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-83672396',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016707',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '21',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016708',
        'CustomerName': '东莞市启天自动化设备股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '殷娟',
        'ContactQQ': null,
        'ContactEmail': 'qt@cnqtkj.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0769-38834566',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016708',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '22',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016709',
        'CustomerName': '广东雅励新材料股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '金辉',
        'ContactQQ': null,
        'ContactEmail': 'kin_ki@163.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0769-82239088',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016709',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '23',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016710',
        'CustomerName': '深圳市京格建设股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '徐海红',
        'ContactQQ': null,
        'ContactEmail': 'jingge20090821@sina.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-89648866',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016710',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '24',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016711',
        'CustomerName': '珠海天威新材料股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '朱崇友',
        'ContactQQ': null,
        'ContactEmail': 'npjgmo@apollojet.KC.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0756-8687768',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016711',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '25',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016712',
        'CustomerName': '珠海市长陆工业自动控制系统股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '项进解',
        'ContactQQ': null,
        'ContactEmail': 'dongmi@longtec.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0756-7239507',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016712',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '26',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016713',
        'CustomerName': '广东天讯达资讯科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '叶涛',
        'ContactQQ': null,
        'ContactEmail': 'txdservice@sina.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0760-88665699',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016713',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '27',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016714',
        'CustomerName': '佛山市欣源电子股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '薛占青',
        'ContactQQ': null,
        'ContactEmail': 'xydz@nh-xinyuan.KC.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0757-86816568',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016714',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '28',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016715',
        'CustomerName': '广州晟烨信息科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '郑静如',
        'ContactQQ': null,
        'ContactEmail': 'hr@shengyeit.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-38847645',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016715',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '29',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016716',
        'CustomerName': '珠海美光原科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '李立本',
        'ContactQQ': null,
        'ContactEmail': 'xp@mphoton.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0756-7630211',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016716',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '30',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016717',
        'CustomerName': '广东汇兴精工智造股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '黄艳',
        'ContactQQ': null,
        'ContactEmail': '2853658227@qq.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0769-38988888',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016717',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '31',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016718',
        'CustomerName': '深圳市泊林商业经营管理股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '王静',
        'ContactQQ': null,
        'ContactEmail': 'bolinzhubao@foxmail.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-83708193',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016718',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '32',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016719',
        'CustomerName': '深圳联合金融服务集团股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '霍元',
        'ContactQQ': null,
        'ContactEmail': 'info@cufs.KC.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-88350198',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016719',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '33',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016720',
        'CustomerName': '深圳兴融联科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '范文玲',
        'ContactQQ': null,
        'ContactEmail': 'sales@china-xrl.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-86728685',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016720',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '34',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016721',
        'CustomerName': '广州逸臣文化传媒股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '郭萍',
        'ContactQQ': null,
        'ContactEmail': 'yichencm@126.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-36021529',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016721',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '35',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016722',
        'CustomerName': '深圳臻云技术股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '何海霞',
        'ContactQQ': null,
        'ContactEmail': null,
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-86716755',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016722',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '36',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016723',
        'CustomerName': '深圳中琛源科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '何莹',
        'ContactQQ': null,
        'ContactEmail': 'hy@ibumobile.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-86950607',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016723',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '37',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016724',
        'CustomerName': '深圳市长城网信息科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '郭涛',
        'ContactQQ': null,
        'ContactEmail': null,
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-83248455',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016724',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '38',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016725',
        'CustomerName': '宝德科技集团股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '董卫屏',
        'ContactQQ': null,
        'ContactEmail': 'marketing@powerleader.KC.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-29528988',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016725',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '39',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016726',
        'CustomerName': '国银金融租赁股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '黄敏黄秀萍',
        'ContactQQ': null,
        'ContactEmail': null,
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-23980999',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016726',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '40',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016727',
        'CustomerName': '中国广核电力股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '魏其岩莫明慧',
        'ContactQQ': null,
        'ContactEmail': 'IR@cgnpc.KC.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-83671649',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016727',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '41',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016728',
        'CustomerName': '深圳市明华澳汉科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '谈兆普',
        'ContactQQ': null,
        'ContactEmail': 'szmw@mwcard.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-83345003',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016728',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '42',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016729',
        'CustomerName': '研祥智能科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '徐振权',
        'ContactQQ': null,
        'ContactEmail': null,
        'ContactPhoneNumber': null,
        'ContactTelephone': null,
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016729',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '43',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016730',
        'CustomerName': '绿色动力环保集团股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '朱曙光沈施加美',
        'ContactQQ': null,
        'ContactEmail': 'ir@dynagreen.KC.cn',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-33631280',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016730',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '44',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016731',
        'CustomerName': '广东粤运交通股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '刘志全',
        'ContactQQ': null,
        'ContactEmail': 'zqb@gdyueyun.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-37637013',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016731',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '45',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016732',
        'CustomerName': '广东中盈盛达融资担保投资股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '黄日东郑正强',
        'ContactQQ': null,
        'ContactEmail': 'zysd@join-share.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0757-83303188',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016732',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '46',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016733',
        'CustomerName': '深圳市海王英特龙生物技术股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '慕凌霞李美仪',
        'ContactQQ': null,
        'ContactEmail': 'hjb@interlong.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': null,
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016733',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '47',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016734',
        'CustomerName': '创美药业股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '林志雄吴咏珊',
        'ContactQQ': null,
        'ContactEmail': null,
        'ContactPhoneNumber': null,
        'ContactTelephone': '0754-88103483',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016734',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '48',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016735',
        'CustomerName': '中航国际控股股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '钟思均',
        'ContactQQ': null,
        'ContactEmail': null,
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-83793891',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016735',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '49',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016736',
        'CustomerName': '广州富力地产股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '李启明张诗贤',
        'ContactQQ': null,
        'ContactEmail': 'rf@rfchina.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '020-38882777',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016736',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    },
    {
        '$id': '50',
        'CustomerId': 0,
        'CustomerCode': 'CR2016102016737',
        'CustomerName': '深圳市元征科技股份有限公司',
        'CustomerType': 2,
        'ClientType': 0,
        'CustomerSource': 2,
        'CustomerLevel': 1,
        'ContactName': '张江波',
        'ContactQQ': null,
        'ContactEmail': 'investor@cnlaunch.com',
        'ContactPhoneNumber': null,
        'ContactTelephone': '0755-84528888',
        'PositionName': '董事会秘书',
        'RecommandedUserId': 'UR2014112600002',
        'RecommandedUserName': '财富共赢融资租赁（深圳）有限公司',
        'CustomerManangeId': '27ef24fa-904c-41e1-ae79-a4f7d054fd92',
        'CustomerManangeName': '刘妮妮',
        'ReferenceId': 'CR2016102016737',
        'Introduction': null,
        'Contents': null,
        'Subject': null,
        'IsDeleted': false,
        'CreatedBy': 'liunini@cfwin.com',
        'CreatedDate': '2016-10-20T09:32:01.617',
        'ModifiedBy': 'liunini@cfwin.com',
        'ModifiedDate': '2016-10-20T09:32:06.82'
    }
]";
            #endregion

            var obj = SerializeHelper.FromJson<List<CustomerInfoDTO>>(jsonData);
            var jsonObj = SerializeHelper.ToJson(obj);
            Assert.True(obj != null);
            Assert.True(!string.IsNullOrWhiteSpace(jsonObj));

            var binaryBytes = SerializeHelper.ToBinary(obj);
            var binaryObj = SerializeHelper.FromBinary<List<CustomerInfoDTO>>(binaryBytes);
            Assert.True(binaryObj != null);
            Assert.True(binaryBytes.Any());

            var protoBytes = SerializeHelper.ToProtobufBinary(obj);
            var protoObj = SerializeHelper.FromProtobufBinary<List<CustomerInfoDTO>>(protoBytes);
            Assert.True(protoBytes.Any());
            Assert.True(protoObj != null);
        }

        [Xunit.Fact]
        public void Test_GetDictListByArrayJson()
        {
            var listObject =
                "[{'field-1':1,'field-2':'test-1','field-3':'2021-01-31'}," +
                " {'field-1':2,'field-2':'test-2','field-3':'2021-02-31'}," +
                " {'field-1':3,'field-2':'test-3','field-3':'2021-03-31'}]";

            var listDict = SerializeHelper.GetDictListByArrayJson(listObject);
            Assert.NotNull(listDict);
            foreach (var dict in listDict)
            {
                foreach (var field in dict.Keys)
                {
                    Console.WriteLine(field + "---" + dict[field]);
                }
            }
        }

        [Xunit.Fact]
        public void Test_GetMapByJson()
        {
            var jsonString = "{\"$id\":\"1\",\"httpStatusCode\":200,\"resultType\":0,\"message\":\"操作成功！\",\"logMessage\":null,\"result\":{\"$id\":\"2\",\"canEdit\":false,\"referenceId\":null,\"databasePoolId\":1,\"ownDatabase\":{\"$id\":\"3\",\"databasePoolId\":1,\"server\":\"localhost\",\"database\":\"MSSqlKCContext\",\"userName\":\"cDba\",\"userPasswordHash\":\"v/yK4vAwB5oH84zisbz4XA==\",\"passwordExpiredTime\":null,\"tenantCount\":2,\"canEdit\":false,\"cloudType\":1,\"databaseType\":0,\"isDeleted\":false,\"createdBy\":\"admin\",\"createdDate\":\"2020-05-09T08:16:38.9268583\",\"modifiedBy\":\"admin\",\"modifiedDate\":\"2020-05-09T08:16:39.6354778\"},\"storagePoolId\":1,\"ownStorage\":null,\"queuePoolId\":1,\"ownQueue\":null,\"noSqlPoolId\":1,\"ownNoSql\":null,\"serviceBusPoolId\":1,\"serviceBusPool\":null,\"tenantSettings\":[],\"databaseType\":0,\"server\":\"localhost\",\"database\":\"MSSqlKCContext\",\"databasePasswordHash\":\"0QVw0yFoX2GuwkMSQyz1tg==\",\"storageType\":0,\"storageEndpoint\":\"https://cfwinstorage.blob.core.chinacloudapi.cn/\",\"storageAccessName\":\"cfwinstorage\",\"storageAccessKeyPasswordHash\":\"aeY1k3PdTMoaDM72+mG7/LHYVxAzyj5h7/d4DGf8zioXuYf/Z7FBh5rmthiipdjpTpl6Bo4bUuFAEhFWblFoBk2EUVZteEN9m4z+HsY/LBKgvWbJf1mUItVwASVvv52r\",\"queueType\":0,\"queueEndpoint\":\"https://cfwinstorage.queue.core.chinacloudapi.cn/\",\"queueAccessName\":\"cfwinstorage\",\"queueAccessKeyPasswordHash\":\"aeY1k3PdTMoaDM72+mG7/LHYVxAzyj5h7/d4DGf8zioXuYf/Z7FBh5rmthiipdjpTpl6Bo4bUuFAEhFWblFoBk2EUVZteEN9m4z+HsY/LBKgvWbJf1mUItVwASVvv52r\",\"noSqlType\":0,\"noSqlEndpoint\":\"https://cfwinstorage.table.core.chinacloudapi.cn/\",\"noSqlAccessName\":\"cfwinstorage\",\"noSqlAccessKeyPasswordHash\":\"aeY1k3PdTMoaDM72+mG7/LHYVxAzyj5h7/d4DGf8zioXuYf/Z7FBh5rmthiipdjpTpl6Bo4bUuFAEhFWblFoBk2EUVZteEN9m4z+HsY/LBKgvWbJf1mUItVwASVvv52r\",\"serviceBusType\":0,\"serviceBusEndpoint\":\"sb://devcfwin.servicebus.chinacloudapi.cn/\",\"serviceBusAccessName\":\"RootManageSharedAccessKey\",\"serviceBusAccessKeyPasswordHash\":\"fU4SNIOcZd2e6S0A8kPh91pskkd4VH+Fgp2RepngzvaDuq/rmwK0IckOORzh6DXe\",\"contactName\":\"田长军\",\"contactEmail\":\"tianchangjun@outlook.com\",\"contactPhone\":\"13682381319\",\"nickName\":null,\"nickNameLastModifyDate\":null,\"passwordExpiredTime\":null,\"tenantId\":1,\"tenantName\":\"cDba\",\"tenantDisplayName\":\"租户管理\",\"tenantSignature\":\"W8mKe7ozff0E1OIHtodfC4+E7yEmIIpCG0ist4RJhCaGDgAZoGcZWgn/c56g8tFLI0tpJsQM18SNUhYSma1WODXzPxPHwpRowAsCd2ATiak=\",\"tenantType\":63,\"version\":7,\"cloudType\":1,\"orgUSCC\":null,\"orgCode\":null,\"privateEncryptKey\":\"dev-cfwin-EncryptKey\",\"hostnames\":[\"http://localhost:1001\",\"http://localhost:1003\",\"http://localhost:1005\",\"http://cdba.localhost:1101\",\"http://cdba.localhost:1103\",\"http://cdba.localhost:1105\",\"http://cdba.localhost:1107\",\"http://cdba.localhost:2001\",\"http://cdba.localhost:2003\",\"http://cdba.localhost:1107\",\"http://cdba.localhost:2007\",\"http://cdba.localhost:3001\",\"http://cdba.localhost:3003\",\"http://cdba.localhost:3005\",\"http://cdba.localhost:3006\",\"http://cdba.localhost:4001\",\"http://cdba.localhost:4003\",\"http://cdba.localhost:4005\",\"http://cdba.localhost:4007\",\"http://cdba.localhost:5001\",\"http://cdba.localhost:5001\",\"http://cdba.localhost:7001\",\"http://cdba.localhost:8001\",\"http://cdba.localhost:9001\"],\"scopes\":{\"ssoapi\":\"统一认证系统\",\"adminapi\":\"租户管理\",\"blogapi\":\"博客管理\",\"cfgapi\":\"配置管理\",\"dicapi\":\"字典管理\",\"appapi\":\"应用管理\",\"msgapi\":\"消息管理\",\"accapi\":\"账户管理\",\"econapi\":\"合同管理\",\"docapi\":\"文档管理\",\"hrapi\":\"人事管理\",\"crmapi\":\"客户管理\",\"srmapi\":\"供应商管理\",\"prdapi\":\"商品管理\",\"pmcapi\":\"物料管理\",\"portalapi\":\"门户网站\",\"somapi\":\"销售管理\",\"pomapi\":\"采购管理\",\"wmsapi\":\"仓储管理\",\"jrapi\":\"融资管理\",\"trainapi\":\"培训管理\",\"flowapi\":\"工作流管理\",\"payapi\":\"支付管理\",\"wxapi\":\"微信管理\"},\"isDeleted\":false,\"createdBy\":\"admin\",\"createdDate\":\"2020-05-09T08:16:40.1108661\",\"modifiedBy\":\"admin\",\"modifiedDate\":\"2020-05-09T08:16:40.358372\"},\"success\":true}";
            var result = SerializeHelper.GetMapByJson(jsonString);
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
            Assert.Equal(6, result.Count);
            foreach (DictionaryEntry dict in result)
            {
                Console.WriteLine(dict.Key + "---" + dict.Value);
            }
        }

        [Xunit.Fact]
        public void Test_GetMapValueByKey()
        {
            var jsonString = "{\"$id\":\"1\",\"httpStatusCode\":200,\"resultType\":0,\"message\":\"操作成功！\",\"logMessage\":null,\"result\":{\"$id\":\"2\",\"canEdit\":false,\"referenceId\":null,\"databasePoolId\":1,\"ownDatabase\":{\"$id\":\"3\",\"databasePoolId\":1,\"server\":\"localhost\",\"database\":\"MSSqlKCContext\",\"userName\":\"cDba\",\"userPasswordHash\":\"v/yK4vAwB5oH84zisbz4XA==\",\"passwordExpiredTime\":null,\"tenantCount\":2,\"canEdit\":false,\"cloudType\":1,\"databaseType\":0,\"isDeleted\":false,\"createdBy\":\"admin\",\"createdDate\":\"2020-05-09T08:16:38.9268583\",\"modifiedBy\":\"admin\",\"modifiedDate\":\"2020-05-09T08:16:39.6354778\"},\"storagePoolId\":1,\"ownStorage\":null,\"queuePoolId\":1,\"ownQueue\":null,\"noSqlPoolId\":1,\"ownNoSql\":null,\"serviceBusPoolId\":1,\"serviceBusPool\":null,\"tenantSettings\":[],\"databaseType\":0,\"server\":\"localhost\",\"database\":\"MSSqlKCContext\",\"databasePasswordHash\":\"0QVw0yFoX2GuwkMSQyz1tg==\",\"storageType\":0,\"storageEndpoint\":\"https://cfwinstorage.blob.core.chinacloudapi.cn/\",\"storageAccessName\":\"cfwinstorage\",\"storageAccessKeyPasswordHash\":\"aeY1k3PdTMoaDM72+mG7/LHYVxAzyj5h7/d4DGf8zioXuYf/Z7FBh5rmthiipdjpTpl6Bo4bUuFAEhFWblFoBk2EUVZteEN9m4z+HsY/LBKgvWbJf1mUItVwASVvv52r\",\"queueType\":0,\"queueEndpoint\":\"https://cfwinstorage.queue.core.chinacloudapi.cn/\",\"queueAccessName\":\"cfwinstorage\",\"queueAccessKeyPasswordHash\":\"aeY1k3PdTMoaDM72+mG7/LHYVxAzyj5h7/d4DGf8zioXuYf/Z7FBh5rmthiipdjpTpl6Bo4bUuFAEhFWblFoBk2EUVZteEN9m4z+HsY/LBKgvWbJf1mUItVwASVvv52r\",\"noSqlType\":0,\"noSqlEndpoint\":\"https://cfwinstorage.table.core.chinacloudapi.cn/\",\"noSqlAccessName\":\"cfwinstorage\",\"noSqlAccessKeyPasswordHash\":\"aeY1k3PdTMoaDM72+mG7/LHYVxAzyj5h7/d4DGf8zioXuYf/Z7FBh5rmthiipdjpTpl6Bo4bUuFAEhFWblFoBk2EUVZteEN9m4z+HsY/LBKgvWbJf1mUItVwASVvv52r\",\"serviceBusType\":0,\"serviceBusEndpoint\":\"sb://devcfwin.servicebus.chinacloudapi.cn/\",\"serviceBusAccessName\":\"RootManageSharedAccessKey\",\"serviceBusAccessKeyPasswordHash\":\"fU4SNIOcZd2e6S0A8kPh91pskkd4VH+Fgp2RepngzvaDuq/rmwK0IckOORzh6DXe\",\"contactName\":\"田长军\",\"contactEmail\":\"tianchangjun@outlook.com\",\"contactPhone\":\"13682381319\",\"nickName\":null,\"nickNameLastModifyDate\":null,\"passwordExpiredTime\":null,\"tenantId\":1,\"tenantName\":\"cDba\",\"tenantDisplayName\":\"租户管理\",\"tenantSignature\":\"W8mKe7ozff0E1OIHtodfC4+E7yEmIIpCG0ist4RJhCaGDgAZoGcZWgn/c56g8tFLI0tpJsQM18SNUhYSma1WODXzPxPHwpRowAsCd2ATiak=\",\"tenantType\":63,\"version\":7,\"cloudType\":1,\"orgUSCC\":null,\"orgCode\":null,\"privateEncryptKey\":\"dev-cfwin-EncryptKey\",\"hostnames\":[\"http://localhost:1001\",\"http://localhost:1003\",\"http://localhost:1005\",\"http://cdba.localhost:1101\",\"http://cdba.localhost:1103\",\"http://cdba.localhost:1105\",\"http://cdba.localhost:1107\",\"http://cdba.localhost:2001\",\"http://cdba.localhost:2003\",\"http://cdba.localhost:1107\",\"http://cdba.localhost:2007\",\"http://cdba.localhost:3001\",\"http://cdba.localhost:3003\",\"http://cdba.localhost:3005\",\"http://cdba.localhost:3006\",\"http://cdba.localhost:4001\",\"http://cdba.localhost:4003\",\"http://cdba.localhost:4005\",\"http://cdba.localhost:4007\",\"http://cdba.localhost:5001\",\"http://cdba.localhost:5001\",\"http://cdba.localhost:7001\",\"http://cdba.localhost:8001\",\"http://cdba.localhost:9001\"],\"scopes\":{\"ssoapi\":\"统一认证系统\",\"adminapi\":\"租户管理\",\"blogapi\":\"博客管理\",\"cfgapi\":\"配置管理\",\"dicapi\":\"字典管理\",\"appapi\":\"应用管理\",\"msgapi\":\"消息管理\",\"accapi\":\"账户管理\",\"econapi\":\"合同管理\",\"docapi\":\"文档管理\",\"hrapi\":\"人事管理\",\"crmapi\":\"客户管理\",\"srmapi\":\"供应商管理\",\"prdapi\":\"商品管理\",\"pmcapi\":\"物料管理\",\"portalapi\":\"门户网站\",\"somapi\":\"销售管理\",\"pomapi\":\"采购管理\",\"wmsapi\":\"仓储管理\",\"jrapi\":\"融资管理\",\"trainapi\":\"培训管理\",\"flowapi\":\"工作流管理\",\"payapi\":\"支付管理\",\"wxapi\":\"微信管理\"},\"isDeleted\":false,\"createdBy\":\"admin\",\"createdDate\":\"2020-05-09T08:16:40.1108661\",\"modifiedBy\":\"admin\",\"modifiedDate\":\"2020-05-09T08:16:40.358372\"},\"success\":true}";
            var result = SerializeHelper.GetMapValueByKey(jsonString, "result");
            Assert.False(result.IsNullOrEmpty());

            var canEdit = SerializeHelper.GetMapValueByKey(result, "canEdit");
            Assert.False(canEdit.IsNullOrEmpty());
            Assert.Equal("false", canEdit);
        }

        [Xunit.Fact]
        public void Test_GetMapObjectByKey()
        {
            var jsonString = "{\"$id\":\"1\",\"httpStatusCode\":200,\"resultType\":0,\"message\":\"操作成功！\",\"logMessage\":null,\"result\":{\"$id\":\"2\",\"canEdit\":false,\"referenceId\":null,\"databasePoolId\":1,\"ownDatabase\":{\"$id\":\"3\",\"databasePoolId\":1,\"server\":\"localhost\",\"database\":\"MSSqlKCContext\",\"userName\":\"cDba\",\"userPasswordHash\":\"v/yK4vAwB5oH84zisbz4XA==\",\"passwordExpiredTime\":null,\"tenantCount\":2,\"canEdit\":false,\"cloudType\":1,\"databaseType\":0,\"isDeleted\":false,\"createdBy\":\"admin\",\"createdDate\":\"2020-05-09T08:16:38.9268583\",\"modifiedBy\":\"admin\",\"modifiedDate\":\"2020-05-09T08:16:39.6354778\"},\"storagePoolId\":1,\"ownStorage\":null,\"queuePoolId\":1,\"ownQueue\":null,\"noSqlPoolId\":1,\"ownNoSql\":null,\"serviceBusPoolId\":1,\"serviceBusPool\":null,\"tenantSettings\":[],\"databaseType\":0,\"server\":\"localhost\",\"database\":\"MSSqlKCContext\",\"databasePasswordHash\":\"0QVw0yFoX2GuwkMSQyz1tg==\",\"storageType\":0,\"storageEndpoint\":\"https://cfwinstorage.blob.core.chinacloudapi.cn/\",\"storageAccessName\":\"cfwinstorage\",\"storageAccessKeyPasswordHash\":\"aeY1k3PdTMoaDM72+mG7/LHYVxAzyj5h7/d4DGf8zioXuYf/Z7FBh5rmthiipdjpTpl6Bo4bUuFAEhFWblFoBk2EUVZteEN9m4z+HsY/LBKgvWbJf1mUItVwASVvv52r\",\"queueType\":0,\"queueEndpoint\":\"https://cfwinstorage.queue.core.chinacloudapi.cn/\",\"queueAccessName\":\"cfwinstorage\",\"queueAccessKeyPasswordHash\":\"aeY1k3PdTMoaDM72+mG7/LHYVxAzyj5h7/d4DGf8zioXuYf/Z7FBh5rmthiipdjpTpl6Bo4bUuFAEhFWblFoBk2EUVZteEN9m4z+HsY/LBKgvWbJf1mUItVwASVvv52r\",\"noSqlType\":0,\"noSqlEndpoint\":\"https://cfwinstorage.table.core.chinacloudapi.cn/\",\"noSqlAccessName\":\"cfwinstorage\",\"noSqlAccessKeyPasswordHash\":\"aeY1k3PdTMoaDM72+mG7/LHYVxAzyj5h7/d4DGf8zioXuYf/Z7FBh5rmthiipdjpTpl6Bo4bUuFAEhFWblFoBk2EUVZteEN9m4z+HsY/LBKgvWbJf1mUItVwASVvv52r\",\"serviceBusType\":0,\"serviceBusEndpoint\":\"sb://devcfwin.servicebus.chinacloudapi.cn/\",\"serviceBusAccessName\":\"RootManageSharedAccessKey\",\"serviceBusAccessKeyPasswordHash\":\"fU4SNIOcZd2e6S0A8kPh91pskkd4VH+Fgp2RepngzvaDuq/rmwK0IckOORzh6DXe\",\"contactName\":\"田长军\",\"contactEmail\":\"tianchangjun@outlook.com\",\"contactPhone\":\"13682381319\",\"nickName\":null,\"nickNameLastModifyDate\":null,\"passwordExpiredTime\":null,\"tenantId\":1,\"tenantName\":\"cDba\",\"tenantDisplayName\":\"租户管理\",\"tenantSignature\":\"W8mKe7ozff0E1OIHtodfC4+E7yEmIIpCG0ist4RJhCaGDgAZoGcZWgn/c56g8tFLI0tpJsQM18SNUhYSma1WODXzPxPHwpRowAsCd2ATiak=\",\"tenantType\":63,\"version\":7,\"cloudType\":1,\"orgUSCC\":null,\"orgCode\":null,\"privateEncryptKey\":\"dev-cfwin-EncryptKey\",\"hostnames\":[\"http://localhost:1001\",\"http://localhost:1003\",\"http://localhost:1005\",\"http://cdba.localhost:1101\",\"http://cdba.localhost:1103\",\"http://cdba.localhost:1105\",\"http://cdba.localhost:1107\",\"http://cdba.localhost:2001\",\"http://cdba.localhost:2003\",\"http://cdba.localhost:1107\",\"http://cdba.localhost:2007\",\"http://cdba.localhost:3001\",\"http://cdba.localhost:3003\",\"http://cdba.localhost:3005\",\"http://cdba.localhost:3006\",\"http://cdba.localhost:4001\",\"http://cdba.localhost:4003\",\"http://cdba.localhost:4005\",\"http://cdba.localhost:4007\",\"http://cdba.localhost:5001\",\"http://cdba.localhost:5001\",\"http://cdba.localhost:7001\",\"http://cdba.localhost:8001\",\"http://cdba.localhost:9001\"],\"scopes\":{\"ssoapi\":\"统一认证系统\",\"adminapi\":\"租户管理\",\"blogapi\":\"博客管理\",\"cfgapi\":\"配置管理\",\"dicapi\":\"字典管理\",\"appapi\":\"应用管理\",\"msgapi\":\"消息管理\",\"accapi\":\"账户管理\",\"econapi\":\"合同管理\",\"docapi\":\"文档管理\",\"hrapi\":\"人事管理\",\"crmapi\":\"客户管理\",\"srmapi\":\"供应商管理\",\"prdapi\":\"商品管理\",\"pmcapi\":\"物料管理\",\"portalapi\":\"门户网站\",\"somapi\":\"销售管理\",\"pomapi\":\"采购管理\",\"wmsapi\":\"仓储管理\",\"jrapi\":\"融资管理\",\"trainapi\":\"培训管理\",\"flowapi\":\"工作流管理\",\"payapi\":\"支付管理\",\"wxapi\":\"微信管理\"},\"isDeleted\":false,\"createdBy\":\"admin\",\"createdDate\":\"2020-05-09T08:16:40.1108661\",\"modifiedBy\":\"admin\",\"modifiedDate\":\"2020-05-09T08:16:40.358372\"},\"success\":true}";
            Tenant result = SerializeHelper.GetMapObjectByKey<Tenant>(jsonString, "result");
            Assert.NotNull(result);
            Assert.Equal("cDba", result.TenantName);
            Assert.Equal("cfwinstorage", result.StorageAccessName);
        }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    public class CustomerInfoDTO : Entity
    {
        public CustomerInfoDTO()
        {
            CustomerType = 1;
        }

        [DataMember]
        public int CustomerId { get; set; }

        [DataMember]
        [DisplayName("客户代码")]
        public string CustomerCode { get; set; }

        [DataMember]
        [DisplayName("客户名称")]

        public string CustomerName { get; set; }

        [DataMember]
        [DisplayName("客户类型")]
        public int CustomerType { get; set; }

        [DataMember]
        [DisplayName("客户分类")]
        public int ClientType { get; set; }

        [DataMember]
        [DisplayName("客户来源")]
        public int CustomerSource { get; set; }

        [DataMember]
        [DisplayName("客户等级")]
        public int CustomerLevel { get; set; }

        [DataMember]
        [DisplayName("联系人")]

        public string ContactName { get; set; }

        [DataMember]
        [DisplayName("联系人QQ")]

        public string ContactQQ { get; set; }

        [DataMember]
        [DisplayName("联系人微信")]

        public string ContactWeixin { get; set; }

        [DataMember]
        [DisplayName("联系人邮箱")]

        public string ContactEmail { get; set; }

        [DataMember]
        [DisplayName("联系人手机")]

        public string ContactPhoneNumber { get; set; }

        [DataMember]
        [DisplayName("联系人座机")]

        public string ContactTelephone { get; set; }

        [DataMember]
        [DisplayName("联系人职务")]

        public string PositionName { get; set; }

        [DataMember]
        [DisplayName("推送人Id")]

        public string RecommandedUserId { get; set; }

        /// <summary>
        /// 显示需要DisplayName改成创建人
        /// </summary>
        [DataMember]
        [DisplayName("创建人")]
        public string RecommandedUserName { get; set; }

        [DataMember]

        public string ReferenceId { get; set; }

        [DataMember]
        [DisplayName("性别")]

        public string Sex { get; set; }

        [DataMember]
        [DisplayName("身份证")]
        public string IdCard { get; set; }

        [DataMember]
        [DisplayName("银行帐号")]

        public string BankAccount { get; set; }

        [DataMember]
        [DisplayName("所属单位")]

        public string AffiliatedCompany { get; set; }

        [DataMember]
        [DisplayName("所属部门")]

        public string AffiliatedDepartment { get; set; }

        [DataMember]
        [DisplayName("客户简介")]

        public string Introduction { get; set; }

        [DataMember]
        [DisplayName("法人代表")]

        public string LegalPerson { get; set; }

        [DisplayName("注册资金")]

        public int? RegisteredAssets { get; set; }

        [DataMember]
        [DisplayName("注册地址")]
        public string RegisteredAddress
        {
            get { return ProviceName + CityName; }
        }

        [DataMember]
        [DisplayName("注册号")]
        public string RegistrationNumber { get; set; }

        [DataMember]
        [DisplayName("组织机构代码")]
        public string OrganizationCodeNumber { get; set; }

        [DataMember]
        [DisplayName("统一社会信用代码")]

        public string UnifiedSocialCreditCode { get; set; }

        [DataMember]
        [DisplayName("登记机关")]

        public string RegistrationAuthority { get; set; }

        [DataMember]
        [DisplayName("公司电话")]

        public string CompanyPhone { get; set; }

        [DataMember]
        [DisplayName("公司网站")]

        public string CompanyWebsite { get; set; }

        [DataMember]
        [DisplayName("公司邮箱")]

        public string CompanyEmail { get; set; }

        [DataMember]
        [DisplayName("员工人数")]
        public int CompanyPersons { get; set; }

        [DataMember]
        [DisplayName("所属行业")]

        public string Industry { get; set; }

        [DataMember]
        [DisplayName("营业期限")]

        public string BusinessTerm { get; set; }

        [DataMember]
        [DisplayName("经营规模")]

        public string BusinessScale { get; set; }

        [DataMember]
        [DisplayName("经营范围")]

        public string BusinessScope { get; set; }

        [DataMember]
        [DisplayName("经营状态")]

        public int BusinessState { get; set; }

        [DataMember]
        [DisplayName("省市地址")]
        public string ProviceAndCity
        {
            get { return ProviceName + CityName; }
        }

        [DataMember]
        [DisplayName("省ID")]
        public int ProviceID { get; set; }

        [DataMember]
        [DisplayName("省名称")]

        public string ProviceName { get; set; }

        [DataMember]
        [DisplayName("市ID")]
        public int CityID { get; set; }

        [DataMember]
        [DisplayName("市名称")]

        public string CityName { get; set; }

        [DataMember]
        [DisplayName("详细地址")]

        public string Address { get; set; }

        /// <summary>
        /// 分配情况(1/2,1/3)
        /// </summary>
        [DataMember]
        [DisplayName("客户分配情况")]
        public string AssignedDisplay { get; set; }

        public string Contents { get; set; }

        [DataMember]
        public string Subject { get; set; }
    }
}
