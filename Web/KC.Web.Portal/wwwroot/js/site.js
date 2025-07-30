// Write your JavaScript code


function HeaderContact() {
    let telPhoneTips;
    $('#contact-telPhone').on({
        mouseenter: function () {
            var that = this;
            telPhoneTips = layer.tips(
                "<div class=\"layui-card\">" +
                "<div class=\"layui-card-header\">工作时间</div>" +
                "<div class=\"layui-card-body\">" +
                "<span style='color:#000;'>周一到周五&nbsp;&nbsp;&nbsp;&nbsp;9:00-17:30</span>" +
                "</div>" +
                "</div>", that, { tips: [2, '#eee'], time: 0, area: 'auto', maxWidth: 260 });
        },
        mouseleave: function () {
            layer.close(telPhoneTips);
        }
    });

    let mobilePhoneTips;
    $('#contact-mobilePhone').on({
        mouseenter: function () {
            var that = this;
            mobilePhoneTips = layer.tips(
                "<div class=\"layui-card\">" +
                "<div class=\"layui-card-header\">联系电话：</div>" +
                "<div class=\"layui-card-body\">" +
                "<span style='color:#000;'>13760255594</span>" +
                "</div>" +
                "</div>", that, { tips: [2, '#eee'], time: 0, area: 'auto', maxWidth: 260 });
        },
        mouseleave: function () {
            layer.close(mobilePhoneTips);
        }
    });

    let emailTips;
    $('#contact-email').on({
        mouseenter: function () {
            var that = this;
            emailTips = layer.tips(
                "<div class=\"layui-card\">" +
                "<div class=\"layui-card-header\">电子邮件：</div>" +
                "<div class=\"layui-card-body\">" +
                "<span style='color:#000;'>zhongzhuepc@outlook.com</span>" +
                "</div>" +
                "</div>", that, { tips: [2, '#eee'], time: 0, area: 'auto', maxWidth: 260 });
        },
        mouseleave: function () {
            layer.close(emailTips);
        }
    });

    let webchatTips;
    $('#contact-webchat').on({
        mouseenter: function () {
            var that = this;
            webchatTips = layer.tips(
                "<div class=\"layui-card\">" +
                "<div class=\"layui-card-header\">公司地址：</div>" +
                "<div class=\"layui-card-body\">" +
                "<span style='color:#000;'>湖北省&nbsp;武汉市&nbsp;东湖新技术开发区&nbsp;关山二路特一号&nbsp;武汉国际企业中心&nbsp;5幢502号-A258</span>" +
                "</div>" +
                "</div>", that, { tips: [2, '#eee'], time: 0, area: 'auto', maxWidth: 260 });
        },
        mouseleave: function () {
            layer.close(webchatTips);
        }
    });

    //$('#contact-reply').on({
    //    click: function () {
    //        let reply = this.offsetParent;
    //        //边缘弹出
    //        layer.open({
    //            type: 1
    //            , area: ["240px", "200px"]
    //            , offset: [reply.offsetTop + 200, reply.offsetLeft]
    //            //, offset: 'lb' //具体配置参考：offset参数项
    //            , content: '<textarea style="width:100%;height:100%;" id="txtClientReply" placeholder="请输入反馈内容" class="layui-textarea"></textarea>'
    //            , btn: ['提交', '关闭']
    //            , btnAlign: 'c' //按钮居中
    //            , shade: 0 //不显示遮罩
    //            , yes: function () {
    //                debugger;
    //                let replyContent = $('#txtClientReply').val();
    //                $.ajax({
    //                    //async: true,
    //                    url: "/Home/SaveReply?content=" + replyContent,
    //                    type: "get",
    //                    dataType: "json",
    //                    //contentType: "application/json;charset=UTF-8",
    //                    //data: jsonData,
    //                    contentType: "application/x-www-form-urlencoded;charset=UTF-8",
    //                    success: function (data) {
    //                        if (data.success) {
    //                            layer.closeAll();
    //                        }
    //                    },
    //                    complete: function () {
    //                    }
    //                });
    //            }, btn2: function (index, layero) {
    //                layer.closeAll();
    //            }
    //        });

    //    }
    //});
}