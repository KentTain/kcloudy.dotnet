/*
 * 比较两个日期的大小
 * 传入的参数推荐是"yyyy-mm-dd"的格式，其他的日期格式也可以，但要保证一致
 */
var dateCompare = function (date1, date2) {
    if (date1 && date2) {
        var a = new Date(date1);
        var b = new Date(date2);
        return a < b;
    }
};
/* 
 * @author : Kent
 * @desc: 比较两个时间的大小（传入的参数是"HH:mm"的格式，）
 * @param: time1:目标时间;time2:被比较时间
 */
var timeCompare = function (time1, time2) {
    //console.info(time1+"-"+time2);
    try {
        if (time1 && time2) {
            var t1 = parseInt(time1.split(":")[0] * 60) + parseInt(time1.split(":")[1]);
            var t2 = parseInt(time2.split(":")[0] * 60) + parseInt(time2.split(":")[1]);
            return t1 < t2;
        }
        return false;
    } catch (e) {
        return false;
    }
};
/* 
 * @author : Kent
 * @desc: 比较两个时间的大小，支持的格式可在formatArr扩展
 * @param: datetime1:目标时间;datetime2:被比较时间
 */
var dateTimeCompare = function (datetime1, datetime2) {

    var formatArr = new Array('YYYY-MM-DD',
        'YYYY-MM-DD HH:mm',
        'YYYY-MM-DD HH:mm:ss'
    );//支持的格式
    try {
        if (datetime1 && datetime2) {
            var dt1 = moment(datetime1, formatArr);
            var dt2 = moment(datetime2, formatArr);
//            console.info(dt1+","+dt2);
            return dt1 < dt2;
        }
        return false;
    } catch (e) {
        return false;
    }
};
//校验身份证合法性
var checkIdcard = function (idcard) {
    var Errors = new Array(
        "验证通过!",
        "身份证号码位数不对!",
        "身份证号码出生日期超出范围或含有非法字符!",
        "身份证号码校验错误!",
        "身份证地区非法!"
    );
    var area = {
        11: "北京",
        12: "天津",
        13: "河北",
        14: "山西",
        15: "内蒙古",
        21: "辽宁",
        22: "吉林",
        23: "黑龙江",
        31: "上海",
        32: "江苏",
        33: "浙江",
        34: "安徽",
        35: "福建",
        36: "江西",
        37: "山东",
        41: "河南",
        42: "湖北",
        43: "湖南",
        44: "广东",
        45: "广西",
        46: "海南",
        50: "重庆",
        51: "四川",
        52: "贵州",
        53: "云南",
        54: "西藏",
        61: "陕西",
        62: "甘肃",
        63: "青海",
        64: "宁夏",
        65: "新疆",
        71: "台湾",
        81: "香港",
        82: "澳门",
        91: "国外"
    };

    //var idcard=idcard;
    var Y, JYM;
    var S, M;
    var idcard_array = new Array();
    idcard_array = idcard.split("");
    //地区检验
    if (area[parseInt(idcard.substr(0, 2))] == null) {
        //alert(Errors[4]);
        //setItemFocus(0, 0, "CertID");
        return Errors[4];
    }

    //身份号码位数及格式检验
    switch (idcard.length) {
        case 15:
            if ((parseInt(idcard.substr(6, 2)) + 1900) % 4 == 0 || ((parseInt(idcard.substr(6, 2)) + 1900) % 100 == 0 && (parseInt(idcard.substr(6, 2)) + 1900) % 4 == 0)) {
                ereg = /^[1-9][0-9]{5}[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}$/;//测试出生日期的合法性
            } else {
                ereg = /^[1-9][0-9]{5}[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}$/;//测试出生日期的合法性
            }

            if (ereg.test(idcard)) {
                //alert(Errors[0]);
                //setItemFocus(0, 0, "CertID");
                return true;

            } else {
                //alert(Errors[2]);
                //setItemFocus(0, 0, "CertID");
                return Errors[2];
            }
            break;
        case 18:
            //18位身份号码检测
            //出生日期的合法性检查
            //闰年月日:((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))
            //平年月日:((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))
            if (parseInt(idcard.substr(6, 4)) % 4 == 0 || (parseInt(idcard.substr(6, 4)) % 100 == 0 && parseInt(idcard.substr(6, 4)) % 4 == 0)) {
                ereg = /^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}[0-9Xx]$/;//闰年出生日期的合法性正则表达式
            } else {
                ereg = /^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}[0-9Xx]$/;//平年出生日期的合法性正则表达式
            }
            if (ereg.test(idcard)) {//测试出生日期的合法性
                //计算校验位
                S = (parseInt(idcard_array[0]) + parseInt(idcard_array[10])) * 7
                    + (parseInt(idcard_array[1]) + parseInt(idcard_array[11])) * 9
                    + (parseInt(idcard_array[2]) + parseInt(idcard_array[12])) * 10
                    + (parseInt(idcard_array[3]) + parseInt(idcard_array[13])) * 5
                    + (parseInt(idcard_array[4]) + parseInt(idcard_array[14])) * 8
                    + (parseInt(idcard_array[5]) + parseInt(idcard_array[15])) * 4
                    + (parseInt(idcard_array[6]) + parseInt(idcard_array[16])) * 2
                    + parseInt(idcard_array[7]) * 1
                    + parseInt(idcard_array[8]) * 6
                    + parseInt(idcard_array[9]) * 3;
                Y = S % 11;
                M = "F";
                JYM = "10X98765432";
                M = JYM.substr(Y, 1);//判断校验位
                if (M == idcard_array[17]) {
                    return true;
                    //Errors[0];        //检测ID的校验位
                } else {
                    //alert(Errors[3]);
                    //setItemFocus(0, 0, "CertID");
                    return Errors[3];
                }
            } else {
                //alert(Errors[2]);
                //setItemFocus(0, 0, "CertID");
                return Errors[2];
            }
            break;
        default:
            //alert(Errors[1]);
            //setItemFocus(0, 0, "CertID");
            return Errors[1];
            break;
    }
}

//校验统一社会信用编码
function CheckSocialCreditCodeOrg(Code) {
    var patrn = /^[0-9A-Z]+$/;
    //18位校验及大写校验
    if ((Code.length != 18) || (patrn.test(Code) == false)) {
        return false;
    } else {
        var Ancode;//信用代码/税号的每一个值
        var Ancodevalue;//信用代码/税号每一个值的权重
        var total = 0;
        var weightedfactors = [1, 3, 9, 27, 19, 26, 16, 17, 20, 29, 25, 13, 8, 24, 10, 30, 28];//加权因子
        var str = '0123456789ABCDEFGHJKLMNPQRTUWXY';
        //不用I、O、S、V、Z
        for (var i = 0; i < Code.length - 1; i++) {
            Ancode = Code.substring(i, i + 1);
            Ancodevalue = str.indexOf(Ancode);
            total = total + Ancodevalue * weightedfactors[i];
            //权重与加权因子相乘之和
        }
        var logiccheckcode = 31 - total % 31;
        if (logiccheckcode == 31) {
            logiccheckcode = 0;
        }
        var Str = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,J,K,L,M,N,P,Q,R,T,U,W,X,Y";
        var Array_Str = Str.split(',');
        logiccheckcode = Array_Str[logiccheckcode];


        var checkcode = Code.substring(17, 18);
        if (logiccheckcode != checkcode) {
            return false;
            // alert("不是有效的统一社会信用编码！");
        } else {
            return true;
        }
    }
}

/**
 *验证营业执照是否合法：营业执照长度须为15位数字，前14位为顺序码，
 *最后一位为根据GB/T 17710 1999(ISO 7064:1993)的混合系统校验位生成算法
 *计算得出。此方法即是根据此算法来验证最后一位校验位是否政正确。如果
 *最后一位校验位不正确，则认为此营业执照号不正确(不符合编码规则)。
 *以下说明来自于网络:
 *我国现行的营业执照上的注册号都是15位的，不存在13位的，从07年开始国
 *家进行了全面的注册号升级就全部都是15位的了，如果你看见的是13位的注
 *册号那肯定是假的。
 *15位数字的含义，代码结构工商注册号由14位数字本体码和1位数字校验码
 *组成，其中本体码从左至右依次为：6位首次登记机关码、8位顺序码。
 * 一、前六位代表的是工商行政管理机关的代码，国家工商行政管理总局用
 * “100000”表示，省级、地市级、区县级登记机关代码分别使用6位行
 * 政区划代码表示。设立在经济技术开发区、高新技术开发区和保税区
 * 的工商行政管理机关（县级或县级以上）或者各类专业分局应由批准
 * 设立的上级机关统一赋予工商行政管理机关代码，并报国家工商行政
 * 管理总局信息化管理部门备案。
 * 二、顺序码是7-14位，顺序码指工商行政管理机关在其管辖范围内按照先
 * 后次序为申请登记注册的市场主体所分配的顺序号。为了便于管理和
 * 赋码，8位顺序码中的第1位（自左至右）采用以下分配规则：
 *　　 1）内资各类企业使用“0”、“1”、“2”、“3”；
 *　　 2）外资企业使用“4”、“5”；
 *　　 3）个体工商户使用“6”、“7”、“8”、“9”。
 * 顺序码是系统根据企业性质情况自动生成的。
 *三、校验码是最后一位，校验码用于检验本体码的正确性
 */
function isValidBusCode(busCode) {
    var ret = false;
    if (busCode.length === 15) {//15位
        var sum = 0;
        var s = [];
        var p = [];
        var a = [];
        var m = 10;
        p[0] = m;
        for (var i = 0; i < busCode.length; i++) {
            a[i] = parseInt(busCode.substring(i, i + 1), m);
            s[i] = (p[i] % (m + 1)) + a[i];
            if (0 == s[i] % m) {
                p[i + 1] = 10 * 2;
            } else {
                p[i + 1] = (s[i] % m) * 2;
            }
        }
        if (1 == (s[14] % m)) {//营业执照编号正确!
            ret = true;
        } else {//营业执照编号错误!
            ret = false;
        }
    } else {//营业执照格式不对，必须是15位数的！
        ret = false;
    }
    return ret;
}

/*
* 纳税人识别号验证
* 15位包括地区编码6位+组织机构代码9位
*/
function checkTaxpayerId15(taxpayerId) {
    if (taxpayerId != "" && taxpayerId.length === 15) {
        var addressCode = taxpayerId.substring(0, 6);
        // 校验地址码
        var check = checkAddressCode(addressCode);
        if (!check) {
            return false;
        }
        // 校验组织机构代码
        var orgCode = taxpayerId.substring(6, 9);
        check = isValidOrgCode(orgCode);
        if (!check) {
            return false;
        }
        return true;
    } else {
        return false;
    }
}

/*
* 纳税人识别号验证
* 18位包括地区编码6位+组织机构代码9位
*/
function checkTaxpayerId18(taxpayerId) {
    if (taxpayerId != "" && taxpayerId.length === 15) {
        var reg = /^([0-9ABCDEFGHJKLMNPQRTUWXY]{2})([0-9]{6})([0-9ABCDEFGHJKLMNPQRTUWXY]{9})([0-9Y])$/;
        if (!reg.test(code)) {
            alert("社会信用代码校验错误！");
            return false;
        }
        var str = '0123456789ABCDEFGHJKLMNPQRTUWXY';
        var ws = [1, 3, 9, 27, 19, 26, 16, 17, 20, 29, 25, 13, 8, 24, 10, 30, 28];
        var codes = new Array();
        codes[0] = code.substr(0, code.length - 1);
        codes[1] = code.substr(code.length - 1, code.length);
        var sum = 0;
        for (var i = 0; i < 17; i++) {
            sum += str.indexOf(codes[0].charAt(i)) * ws[i];
        }
        var c18 = 31 - (sum % 31);
        if (c18 == 31) {
            alert("第18位 == 31");
            c18 = 'Y';
        } else if (c18 == 30) {
            alert("第18位 == 30");
            c18 = '0';
        }
        if (c18 != codes[1]) {
            alert("社会信用代码有误！" + c18);
            return false;
        }
    } else {
        return false;
    }
}

/**
 *验证组织机构代码是否合法：组织机构代码为8位数字或者拉丁字母+1位校验码。
 *验证最后那位校验码是否与根据公式计算的结果相符。
 */
function isValidOrgCode(value) {
    if (value != "") {
        var part1 = value.substring(0, 8);
        var part2 = value.substring(value.length - 1, 1);
        var ws = [3, 7, 9, 10, 5, 8, 4, 2];
        var str = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
        var reg = /^([0-9A-Z]){8}$/;
        if (!reg.test(part1)) {
            return true
        }
        var sum = 0;
        for (var i = 0; i < 8; i++) {
            sum += str.indexOf(part1.charAt(i)) * ws[i];
        }
        var C9 = 11 - (sum % 11);
        var YC9 = part2 + '';
        if (C9 == 11) {
            C9 = '0';
        } else if (C9 == 10) {
            C9 = 'X';
        } else {
            C9 = C9 + '';
        }
        return YC9 != C9;
    }
}

/*
*校验地址码
*/
function checkAddressCode(addressCode) {
    var provinceAndCitys = {
        11: "北京", 12: "天津", 13: "河北", 14: "山西", 15: "内蒙古", 21: "辽宁", 22: "吉林", 23: "黑龙江",
        31: "上海", 32: "江苏", 33: "浙江", 34: "安徽", 35: "福建", 36: "江西", 37: "山东", 41: "河南", 42: "湖北", 43: "湖南", 44: "广东",
        45: "广西", 46: "海南", 50: "重庆", 51: "四川", 52: "贵州", 53: "云南", 54: "西藏", 61: "陕西", 62: "甘肃", 63: "青海", 64: "宁夏",
        65: "新疆", 71: "台湾", 81: "香港", 82: "澳门", 91: "国外"
    };
    var check = /^[1-9]\d{5}$/.test(addressCode);
    if (!check) return false;
    if (provinceAndCitys[parseInt(addressCode.substring(0, 2))]) {
        return true;
    } else {
        return false;
    }
}

//EasyUI用DataGrid用日期格式化
var Common = {
    //EasyUI用DataGrid用日期格式化
    BoolFormatter: function (value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        if (value) {
            return "是";
        } else {
            return "否";
        }
    },
    SexFormatter: function (value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        if (value) {
            return "男";
        } else {
            return "女";
        }
    },
    TimeFormatter: function (value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }
        var val = dateValue.format("yyyy-MM-dd HH:mm");
        return val.substr(11, 5);
    },
    DateTimeFormatter: function (value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd HH:mm");
    },
    DateTimeSecondFormatter: function (value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd HH:mm:ss");
    },

    //EasyUI用DataGrid用日期格式化
    DateFormatter: function (value, rec, index) {
        if (value == undefined) {
            return "";
        }
        /*json格式时间转js时间格式*/
        value = value.substr(1, value.length - 2);
        var obj = eval('(' + "{Date: new " + value + "}" + ')');
        var dateValue = obj["Date"];
        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd");
    },
    TitleFormatter: function (value, rec, index) {
        if (value.length > 10) value = value.substr(0, 8) + "...";
        return value;
    },
    LongTitleFormatter: function (value, rec, index) {
        if (value.length > 15) value = value.substr(0, 12) + "...";
        return value;
    },
    isEmail: function (str) {
        return /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i.test(str);
    },
    isMobile: function (str) {
        return /^(13|14|15|17|18)\d{9}$/i.test(str);
    }
};

/*
 * 对easyui-validatebox的验证类型的扩展
 */
$.extend($.fn.validatebox.defaults.rules, {
    //select空值验证
    selectNotNull: {
        validator: function (value, param) {
            //console.info(value);
            return $(param[0]).find("option:contains('" + value + "')").val() != '';
            //return value!='';
        },
        message: "请选择"
    },
    //正整数
    pnum: {
        validator: function (value, param) {
            return /^[0-9]*[1-9][0-9]*$/.test(value);
        },
        message: "请输入正整数"
    },
    //非0开头正整数
    pznum: {
        validator: function (value, param) {
            return /^[1-9]*[1-9][0-9]*$/.test(value);
        },
        message: "请输入非0开头的正整数"
    },
    //正实数，包含小数
    num: {
        validator: function (value, param) {
            return /^\d+(\.\d+)?$/.test(value);
        },
        message: "请输入正整数或者小数"
    },
    //2位正整数，或精确两位小数
    numTwoOrPointTwo: {
        validator: function (value, param) {
            return /^([1-9]\d?(\.\d{1,2})?|0\.\d{1,2}|0)$/.test(value);
        },
        message: "请输入1到2位的正整数或者精确到2位的小数"
    },
    //6位正整数，或精确两位小数
    numSixOrPointTwo: {
        validator: function (value, param) {
            return /^(([0-9]|([1-9][0-9]{0,5}))((\.[0-9]{1,2})?))$/.test(value);
        },
        message: "请输入1到6位的正整数或者精确到2位的小数"
    },
    //过滤特殊字符
    filterSpecial: {
        validator: function (value, param) {

            //过滤空格
            var flag = /\s/.test(value);
            //过滤特殊字符串
            var pattern = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）——|{}【】’‘《》；：”“'。，、？]");
            var specialFlag = pattern.test(value);
            return !flag && !specialFlag;
        },
        message: "非法字符，请重新输入"
    },

    //身份证
    IDCard: {
        validator: function (value, param) {
            //return /^\d{15}(\d{2}[A-Za-z0-9])?$/i.test(value);
            var flag = checkIdcard(value);
            return flag == true ? true : false;
        },
        message: "请输入正确的身份证号码"
    },
    mdStartDate: {
        validator: function (value, param) {
            var startTime2 = $(param[0]).datetimebox('getValue');
            var d1 = $.fn.datebox.defaults.parser(startTime2);
            var d2 = $.fn.datebox.defaults.parser(value);
            varify = d2 <= d1;
            return varify;
        },
        message: '开始时间必须小于或等于结束时间！'
    },
    //比较日期选择器
    compareDate: {
        validator: function (value, param) {
            return dateCompare($(param[0]).datebox("getValue"), value);
        },
        message: "结束日期不能小于或等于开始日期"
    },
    //比较时间选择器（时分秒）
    compareTime: {
        validator: function (value, param) {
            return timeCompare($(param[0]).timespinner("getValue"), value);
        },
        message: "结束时间不能小于或等于开始时间"
    },
    //比较时间选择器（时分秒）
    compareDateTime: {
        validator: function (value, param) {
            return dateTimeCompare($(param[0]).timespinner("getValue"), value);
        },
        message: "结束时间不能小于或等于开始时间"
    },
    // 验证是否包含空格和非法字符
    unnormal: {
        validator: function (value) {
            return /^[a-zA-Z0-9]/i.test(value);

        },
        message: '输入值不能为空和包含其他非法字符'
    },
    remote: {
        validator: function (value, param) {
            var flag = true;
            var postData = {};
            postData[param[1]] = value;
            $.ajax({
                async: false,
                type: 'POST',
                dataType: 'json',
                timeout: 2000,
                url: param[0],
                data: postData,
                success: function (result) {
                    debugger;
                    if (result.toString() == "true") {
                        flag = false;
                    }
                }
            });
            return flag;
        },
        message: "{2}"
    },
    dynamicRemote: {
        validator: function (value, param) {
            var flag = true;
            var postData = {};
            postData[param[1]] = value;
            $.ajax({
                async: false,
                type: 'POST',
                dataType: 'json',
                timeout: 2000,
                url: param[0],
                data: postData,
                success: function (result) {
                    if (!result.success) {
                        flag = false;
                        $.fn.validatebox.defaults.rules.dynamicRemote.message = result.message;
                    }
                }
            });
            return flag;
        },
        message: ""
    }
});

$.extend($.fn.treegrid.methods, {
    //iscontains是否包含父节点（即子节点被选中时是否也取父节点）
    getAllChecked: function (jq, iscontains) {
        var keyValues = new Array();
        /*
          tree-checkbox2 有子节点被选中的css
          tree-checkbox1 节点被选中的css
          tree-checkbox0 节点未选中的css
        */
        var checkNodes = jq.treegrid("getPanel").find(".tree-checkbox1");
        for (var i = 0; i < checkNodes.length; i++) {
            var keyValue1 = $($(checkNodes[i]).closest('tr')[0]).attr("node-id");
            keyValues.push(keyValue1);
        }

        if (iscontains) {
            var childCheckNodes = jq.treegrid("getPanel").find(".tree-checkbox2");
            for (var i = 0; i < childCheckNodes.length; i++) {
                var keyValue2 = $($(childCheckNodes[i]).closest('tr')[0]).attr("node-id");
                keyValues.push(keyValue2);
            }
        }

        return keyValues;
    }
});