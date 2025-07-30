var globalData;
if (!globalData) globalData = {};
globalData.currentAppId = '';
globalData.apps = [];

var defaultGuid = '{00000000-0000-0000-0000-000000000000}';
var AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};

Array.prototype.toQueryString = function(){
    var out = new Array();
    for(key in this){
        out.push(key + '=' + encodeURIComponent(this[key]));
    }
    return out.join('&');
};
Array.prototype.contains = function (item) {
    return RegExp("\\b" + item + "\\b").test(this);
};
Array.prototype.remove = function (index) {
    if (isNaN(index) || index > this.length) {
        return false;
    }
    this.splice(index, 1);
};
Array.prototype.indexOf = function(object) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == object) {
            return i;
        }
    }
    return -1;
};
Array.prototype.removeChild = function(object) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == object) {
            this.remove(i);
            break;
        }
    }
};

String.prototype.trim = function (s) {
    s = (s ? s : "\\s");
    s = ("(" + s + ")");
    var reg_trim = new RegExp("(^" + s + "*)|(" + s + "*$)", "g");
    return this.replace(reg_trim, "");
};
String.prototype.format = function(args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof(args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        } else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                } else {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, '');
                }
            }
        }
    }
    return result;
};

function guid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    })
}

function GetRandomNumber() {
    return Math.floor(Math.random() * 999999 + 1);
}

//fmoney("12345.675910", 3)，返回12,345.676
//s:传入的float数字 ，n:希望返回小数点几位
function fmoney(s, n) {
    n = n > 0 && n <= 20 ? n : 2;
    s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
    var l = s.split(".")[0].split("").reverse(),
    r = s.split(".")[1];
    var t = "";
    for (var i = 0; i < l.length; i++) {
        t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
    }
    return t.split("").reverse().join("") + "." + r;
}
//rmoney(12,345.676) //返回结果为：12345.676
function rmoney(s) {
    return parseFloat(s.replace(/[^\d\.-]/g, ""));
}

function JsonDateFormat(jsonDate) {
    try {
        var date = new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
        var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
        var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
        return date.getFullYear() + "-" + month + "-" + day + " " + hours + ":" + minutes + ":" + seconds;
    } catch (ex) {
        return "";
    }
}

function formatShortDate(d) {
    return d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();
}

/**
 * 根据form表单的id获取表单下所有可提交的表单数据，封装成数组对象
 */
function getFormData(formId){
    var data = {};
    var results = $(formId).serializeArray();
    $.each(results,function(index,item){
        //文本表单的值不为空才处理
        if(item.value && $.trim(item.value)!=""){
            if(!data[item.name]){
                data[item.name]=item.value;
            }else{
                //name属性相同的表单，值以英文,拼接
                data[item.name]=data[item.name]+','+item.value;
            }
        }
    });
    //console.log(data);
    return data;
};
/**
 * 初始化ajax返回调用逻辑
 */
$.ajaxSetup({
    complete: function (request, status) {
        if (typeof (request) != 'undefined') {
            var responseText = request.getResponseHeader("X-Responded-JSON");
            if (responseText != null) {
                if($.isFunction(window) && window != undefined){
                    window.tipError({ title: '系统提示', msg: '登录超时，请重新登录!', closeCallback: function () { window.location.href = window.location.href; } });
                }else{
                    $.messager.alert('系统提示', '登录超时，请重新登录!',function(r){
                        window.location.href = window.location.href;
                    });
                }
            }
        }
    },
    error: function (jqXHR, textStatus, errorThrown) {
        var status = 0;
        var msg = '未知错误';
        switch (jqXHR.status) {
            case (500):
                msg = '服务器系统内部错误';
                status = 500;
                break;
            case (400):
                msg = '语义有误，当前请求无法被服务器理解；请求参数有误';
                status = 400;
                break;
            case (401):
                msg = '未登录';
                status = 401;
                break;
            case (403):
                msg = '无权限执行此操作';
                status = 403;
                break;
            case (404):
                msg = '网页已被删除被移动或从未存在';
                status = 404;
                break;
            case (408):
                msg = '请求超时';
                status = 408;
                break;
            case (0):
                msg = '请求取消';
                break;
            default:
                status = jqXHR.status;
        }
        if (status > 0){
            if($.isFunction(window) && window != undefined){
                window.tipError({ title: '系统提示', msg: '请联系网站管理员，错误代码：' + status + '，错误消息：' + msg});
            }else{
                $.messager.alert('系统提示', '请联系网站管理员，错误代码：' + status+ '，错误消息：' + msg,function(r){
                    if (status === 401) {
                        window.location.href = '/Account/Sigin';
                    }else{
                        window.location.href = window.location.href;
                    }
                });
            }
        }
    }
});

(function ($) {
    /**
     *  csrf认证
     **/
    var token = $("meta[name='_csrf']").attr("content");
    var header = $("meta[name='_csrf_header']").attr("content");
    if(token != undefined &&  token != null && token != ""
        && header != undefined &&  header != null && header != ""){
        $(document).ajaxSend(function (e, xhr, options) {
            xhr.setRequestHeader(header, token);
        });
    }

    /**
     * 将form里面的内容序列化成json
     * 相同的checkbox用分号拼接起来
     * @param {dom} 指定的选择器
     * @param {obj} 需要拼接在后面的json对象
     * @method serializeJson
     * */
    $.fn.serializeJson=function(otherString){
        var serializeObj={},
            array=this.serializeArray();
        $(array).each(function(){
            if(serializeObj[this.name]){
                serializeObj[this.name]+=';'+this.value;
            }else{
                serializeObj[this.name]=this.value;
            }
        });
        if(otherString!=undefined){
            var otherArray = otherString.split(';');
            $(otherArray).each(function(){
                var otherSplitArray = this.split(':');
                serializeObj[otherSplitArray[0]]=otherSplitArray[1];
            });
        }
        return serializeObj;
    };
    /**
     * 将josn对象赋值给form
     * @param {dom} 指定的选择器
     * @param {obj} 需要给form赋值的json对象
     * @method serializeJson
     * */
    $.fn.setForm = function(jsonValue){
        var obj = this;
        $.each(jsonValue,function(name,ival){
            var $oinput = obj.find("input[name="+name+"]");
            if($oinput.attr("type")=="checkbox"){
                if(ival !== null){
                    var checkboxObj = $("[name="+name+"]");
                    var checkArray = ival.split(";");
                    for(var i=0;i<checkboxObj.length;i++){
                        for(var j=0;j<checkArray.length;j++){
                            if(checkboxObj[i].value == checkArray[j]){
                                checkboxObj[i].click();
                            }
                        }
                    }
                }
            }
            else if($oinput.attr("type")=="radio"){
                $oinput.each(function(){
                    var radioObj = $("[name="+name+"]");
                    for(var i=0;i<radioObj.length;i++){
                        if(radioObj[i].value == ival){
                            radioObj[i].click();
                        }
                    }
                });
            }
            else if($oinput.attr("type")=="textarea"){
                obj.find("[name="+name+"]").html(ival);
            }
            else{
                obj.find("[name="+name+"]").val(ival);
            }

            var $ainput = obj.find("a[name="+name+"]");
            if($ainput != undefined && $ainput.length > 0){
                $ainput.attr("href", ival);
                $ainput.attr("download", "附件-" + guid());
                $ainput.html("下载附件");
                $ainput.val("下载附件");
            }
        })
    };

    /**
     *  分页组件
     **/
    $.fn.cfwinPager = function (records, total, pageSize, pageIndex, pageChanged, $pagination) {

        if (records <= pageSize)
            return;
        var len = total < 10 ? total : 10;
        var temp = pageIndex - 5;
        temp = temp < 0 ? 0 : temp;
        var ul = '<ul class="pagination pagination-sm">';
        ul += '<li class="list">共有&nbsp;' + total + '&nbsp;页</li>';
        ul += '<li class="list">每页&nbsp;' + pageSize + '&nbsp;条</li>';
        ul += '<li><a href="javascript:' + pageChanged + '(' + 1 + ')">«</a></li>';
        for (var i = 1; i <= len; i++) {
            var current = temp > 0 ? temp + i : i;
            if (current > total) {
                break;
            }
            if (current == pageIndex) {
                ul += '<li class="active"><a href="javascript:' + pageChanged + '(' + current + ') ">' + current + '</a></li>';
            }
            else {
                if (i == 1 && temp > 0) {
                    ul += '<li><a href="javascript:' + pageChanged + '(' + temp + ')">...</a></li>';
                }
                ul += '<li><a href="javascript:' + pageChanged + '(' + current + ')">' + current + '</a></li>';
                if (i == len && total > current) {
                    ul += '<li><a href="javascript:' + pageChanged + '(' + (current + 1) + ')">...</a></li>';
                }
            }
        }
        ul += '<li><a href="javascript:' + pageChanged + '(' + total + ')">»</a></li>';
        ul += '<li class="list">总共&nbsp;' + records + '&nbsp;条</li>';
        ul += '</ ul >';
        if ($pagination)
            $pagination.html(ul);
        else
            $('.paginationlocal', this).html(ul);
    }

    /**
     *  初始化Validator组件
     **/
    if($.isFunction($.validator) && $.validator != undefined){
        //console.log("------初始化Validator组件");
        $.validator.setDefaults({
            // 仅做校验，不提交表单
            //debug: true,
            // 提交表单时做校验
            onsubmit: true,
            // 焦点自动定位到第一个无效元素
            focusInvalid: true,
            // 元素获取焦点时清除错误信息
            focusCleanup: true,
            //忽略 class="ignore" 的项不做校验
            ignore: ".ignore",
            // 忽略title属性的错误提示信息
            ignoreTitle: true,
            // 为错误信息提醒元素的 class 属性增加 invalid
            errorClass: "invalid",
            // 为通过校验的元素的 class 属性增加 valid
            validClass: "valid",
            // 使用 <div> 元素进行错误提醒
            errorElement: "div",
            // 使用 <li> 元素包装错误提醒元素
            //wrapper: "li",
            // 将错误提醒元素统一添加到指定元素
            //errorLabelContainer: "#error_messages ul",
            // 自定义错误容器
            //errorContainer: "#error_messages, #error_container",
            // 自定义错误提示如何展示
            showErrors: function (errorMap, errorList) {
                $("#error_tips").html("Your form contains " + this.numberOfInvalids() + " errors, see details below.");
                this.defaultShowErrors();
            },
            // 自定义错误提示位置
            errorPlacement: function (error, element) {
                //console.info("---errorPlacement--" + error.text());
                //error.insertAfter(element);

                $(element).tooltip('destroy'); /*必需*/
                $(element).attr('title', $(error).text()).tooltip('show');
            },
            // 单个元素校验通过后处理
            success: function (label, element) {
                //console.log(label);
                //console.log(element);
                //label.addClass("valid").text("Ok!")
            },
            highlight: function (element, errorClass, validClass) {
                $(element).addClass(errorClass).removeClass(validClass);
                $(element.form).find("label[for=" + element.id + "]").addClass(errorClass);
            },
            unhighlight: function (element, errorClass, validClass) {
                //console.info("---unhighlight--" + errorClass);

                $(element).removeClass(errorClass).addClass(validClass);
                $(element.form).find("label[for=" + element.id + "]").removeClass(errorClass);
                $(element).tooltip('destroy').removeClass(errorClass);
            },
            //校验通过后的回调，可用来提交表单
            // submitHandler: function (form, event) {
            //     console.log($(form).attr("id"));
            //     //$(form).ajaxSubmit();
            //     //form.submit();
            // },
            //校验未通过后的回调
            // invalidHandler: function (event, validator) {
            //     // 'this' refers to the form
            //     var errors = validator.numberOfInvalids();
            //     if (errors) {
            //         var message = errors == 1 ? 'You missed 1 field. It has been highlighted' : 'You missed ' + errors + ' fields. They have been highlighted';
            //         console.log(message);
            //     }
            // }
        });
    }

})(jQuery);