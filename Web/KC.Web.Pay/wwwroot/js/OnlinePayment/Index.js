var payment;
if (!payment) payment = {};
payment.dialog = null;
payment.queryPaymentMethodUrl = '';
payment.createPaymentUrl = '';
payment.getOnlinePaymentRecordsUrl = '';
payment.getCBSAccounts = '';
payment.cbsPaymentUrl = '';
payment.orderAmount = 0;
payment.cashAmount = 0;//现金支付成功总额
payment.billAmount = 0;//票据支付成功总额
payment.alreadyCheckAmount = 0;//已输入额度+现金支付成功总额
payment.creditAmount = 0;
payment.orderId = '';
payment.isLoad = 0;
payment.cbsPayWaiting = 0;
payment.type = '支付';//充值，支付
payment.cashType = 0;
payment.chargeOrderId = '充值记录';
payment.platformPortalDomain = '';
payment.getRecordInterval = null;
payment.orderType = '';
payment.needPayAmount = 0;
payment.goodsName = '';
payment.asNumber = '';
payment.paymentConfirm = function (orderAmount) {
    var isNumber = /^(?!(0[0-9]{0,}$))[0-9]{1,}[.]{0,}[0-9]{0,}$/;
    var $orderAmount = $('#' + orderAmount);
    var inputAmount = new Number($orderAmount.val());
    if (!isNumber.test($.trim($orderAmount.val())) || inputAmount <= 0) {
        layer.alert('请输入合法的金额', {
            icon: 2,
            title: '系统提示',
            skin: 'layer-ext-moon',
        });
        return;
    }
    var $payType = $("input[name='payType'][checked]");
    if ($payType.val() == null) {
        layer.alert('请选择在线' + payment.type + '方式', {
            icon: 2,
            title: '系统提示',
            skin: 'layer-ext-moon'
        });

        return;
    }

    if (inputAmount > payment.needPayAmount) {
        layer.alert('本次' + payment.type + '金额大于所需金额', {
            icon: 2,
            title: '系统提示',
            skin: 'layer-ext-moon'
        });
        return;
    }
    var amount = new Number(inputAmount * 100).toFixed(0);

    if ($('.paymentName', $payType.parents('.row-container')).val().indexOf('CBS') > -1) {
        if (payment.orderType === 'bill') {
            confirmCashPayment(amount / 100, function(items){payment.cbsPayment($payType, $orderAmount.val(), amount ,items)});
        } else {
            payment.cbsPayment($payType, $orderAmount.val(), amount);
        }
        
    } else {
        if (payment.orderType === 'bill') {
            confirmCashPayment(amount / 100, function(items){payment.payment($payType, amount,items)});
        } else {
            payment.payment($payType, amount);
        }
    }
    $orderAmount.spinner('setValue','');
}

payment.closeDialog = function () {
    if (payment.cashType === 0 || payment.cashType ===6)
        payment.getOnlinePaymentRecords();
}

payment.needHelp = function () {
   
    //TODO:
}

payment.paySuccess = function () {
    
    //TODO:
    if (payment.cashType === 0 || payment.cashType ===6)
        payment.getOnlinePaymentRecords();
    else{
        if (jumpChargeFlow && $.isFunction(jumpChargeFlow))
            jumpChargeFlow.apply(this, arguments);
    }
}

payment.rePayment = function () {
    
    if (payment.cashType ===0 || payment.cashType ===6)
        payment.getOnlinePaymentRecords();

    $('#payOrderId').val('');
    $('#orderRequestDatetime').val('');
}

payment.getPaymentMethod = function () {
    if (payment.isLoad == 1)
        return false;
    payment.isLoad = 1;
    ShowMask($paymentList);
    $.get(payment.queryPaymentMethodUrl, function (data) {
        HideMask($paymentList);
        if (data.success && data.result) {
            $('#paymentList').empty();
            for (var i = 0; i < data.result.length; i++) {
                $paymentList.append('<li class="row-container"><div class="row-basic">' +
                    '<input type="hidden" class="paymentSign" value="' + data.result[i].ConfigSign + '"/>' +
                    '<input type="hidden" class="paymentState" value="' + data.result[i].State + '"/>' +
                    '<input type="hidden" class="paymentName" value="' + data.result[i].ConfigName + '"/>' +
                    '<input name="payType" type="radio" value="' + data.result[i].ConfigId + '" >' +
                    '<span class="payText"><img src="' + payment.platformPortalDomain + data.result[i].ConfigImgUrl + '" style="width:160px;height:48px;"></span>' + data.result[i].ConfigDescription + '</span>' +
                    '<span class="realTimeAmount">' + payment.type + '<label></label>元</span></div></li>');
            }
            $('.row-container').on('click', function () {
                $('#paymentList input:radio[name=payType]').removeAttr('checked');
                $('input:radio[name=payType]', this).attr('checked', 'checked')[0].checked = true;
                $('#paymentList li').removeClass('row-container-focus');
                $('.realTimeAmount label').empty();
                $('.realTimeAmount').hide();
                $(this).addClass('row-container-focus');
                var $cashInput = $('#cashInput');
                if ($cashInput.val().length && $cashInput.val() > 0) {
                    $('.realTimeAmount label', this).text(fmoney($cashInput.val(), 2));
                    $('.realTimeAmount', this).show();
                } else {
                    $('.realTimeAmount label').empty();
                    $('.realTimeAmount').hide();
                }
            });
        }
    });
}

payment.getOnlinePaymentRecords = function () {
    ShowMask($('.payList'));
    var $btnRefreshPayRecord = $('#btnRefreshPayRecord').button('loading');
    $.post(payment.getOnlinePaymentRecordsUrl, { orderId: payment.orderId,type:payment.cashType }, function (data) {
        HideMask($('.payList'));
        $btnRefreshPayRecord.button('reset');
        if (data.success && data.result  && data.result.length) {
            var $payList = $('.records').empty();
            var amount = 0;
            for (var i = 0; i < data.result.length; i++) {
                var payClass = '';
                if (data.result[i].PaymentStatus === '1') {
                    payClass = 'succssPay';
                    amount += data.result[i].OrderAmount;
                }
                var tr = '<tr class="' + payClass + '">';
                 tr += '<td>' + (1 + i) + '</td>';
                 tr += '<td>' + data.result[i].PaymentMethod + '</td>';
                 //tr += '<td>' + data.result[i].PaymentOrderId + '</td>';
                 tr += '<td>' + fmoney(data.result[i].OrderAmount, 2) + '</td>';
                 tr += '<td>' + data.result[i].OrderDatetime + '</td>';
                 tr += '<td>' + data.result[i].PaymentStatusName + '</td>';
                tr += '</tr>';
                $payList.append(tr);
            }
            payment.cashAmount =Number(amount.toFixed(2));

        }
        $('.paymentCount').text(fmoney(payment.cashAmount, 2));

        var $paidCashAmount = $('#paidCashAmount');
        if($paidCashAmount.length)
            $paidCashAmount.numberbox('setValue', payment.cashAmount);
        var $needPay = $('#needAmount');
        if ($needPay.length) {
            $needPay.numberbox('setValue', payment.orderAmount-payment.cashAmount-$('#paidBillAmount').val());
        }
        var $paidMoney = $('#paidMoney'),$waitPayMoney=$('.waitPay');
        if($paidMoney.length)
            $paidMoney.text(fmoney(payment.cashAmount,2) );
         if($waitPayMoney.length)
            $waitPayMoney.text(fmoney(payment.orderAmount-payment.cashAmount,2));
        //订单支付页面
         if (typeof (showPayTotal) == 'function') {
             showPayTotal(payment.cashAmount);
         }
    });
}

payment.initOnlinePaymentForm = function () {
    if ($('#paymentList').length == 0)
        return;
    payment.getPaymentMethod();
    try {
        if (undefined != orderInfoId) {
            payment.orderId = orderInfoId;
        }
    } catch (e) {

    } 
    
    if (payment.cashType ===0 || payment.cashType===6)
        payment.getOnlinePaymentRecords();
    var $cashInput = $('#cashInput');
    $cashInput.on('blur', function () {
        var $realTimeAmount = $('.row-container-focus .realTimeAmount');
        if ($cashInput.val().length && $cashInput.val() > 0 && $realTimeAmount.length) {
            $('label', $realTimeAmount).text(fmoney($cashInput.val(), 2));
            $realTimeAmount.show();
        } else {
            $('.realTimeAmount label').empty();
            $('.realTimeAmount').hide();
        }
    });

}

payment.cbsPayComplete = function (transactionData) {
    if (payment.cbsPayWaiting == 1) {
        setTimeout(function() {
            payment.cbsPayCompleteShow(transactionData);
        },300);
        
    } else {
       var timer = setInterval(function() {
           if (payment.cbsPayWaiting == 1) {
               clearInterval(timer);
               setTimeout(function () {
                   payment.cbsPayCompleteShow(transactionData);
               }, 300);
           }
       }, 1e3);
    }
}

payment.cbsPayCompleteShow = function (transactionData) {
    layer.closeAll();
    if (transactionData) {
        if (payment.cashType ===0 || payment.cashType ===6)
            payment.getOnlinePaymentRecords();
        if (transactionData.success && transactionData.Result.Item1) {
            layer.open({
                closeBtn: 1,
                type: 1,
                shade: 0.6,
                area: ['440px', '260px'],
                content: $('.cbsPaySuccess'),
                move: false,
                title: false,
                shift: 0,
                success: function () {
                    document.getElementById("countdownPaySuccess").className = "countdown";
                    document.getElementById("timerPaySuccess").innerHTML = 7;
                    var myCounter1 = new Countdown({ seconds: 7, onUpdateStatus: function (e) { document.getElementById("timerPaySuccess").innerHTML = e }, onCounterEnd: function () { document.getElementById("countdownPaySuccess").className = "countdown end"; layer.closeAll(); } });
                    myCounter1.start();
                }
            });
        } else {
            layer.msg(transactionData.message + '。<span id="errorTimer">5</span>S', { shade: 0.6, time: 5000, icon: 2 });
            var $errorTimer = $('#errorTimer');
            var t = new Countdown({
                seconds: 4, onUpdateStatus: function (e) {
                 if($errorTimer.length)$errorTimer.html(e)
            } });
            t.start();
        }
    } else {
        layer.msg('系统错误，请稍后重试！<span id="errorTimer">5</span>', { shade: 0.6, time: 5000, icon: 2 });
        var $errorTimer = $('#errorTimer');
        var t = new Countdown({
            seconds: 4, onUpdateStatus: function (e) {
                if ($errorTimer.length) $errorTimer.html(e)
            }
        });
        t.start();
    }
}

payment.payment = function ($payType, amount, items) {
    var loadingIndex = layer.msg('正在准备' + payment.type + '，请稍等...', {
        icon: 16,
        shade: 0.3,
        time: 0
    });
    var configState = $payType.prev().prev().val();
    var configSign = $payType.prev().prev().prev().val();
    var postData = {
        configSign: configSign,
        configState: configState,
        payType: $payType.val(),
        orderAmount: amount,
        orderId: payment.orderId,
        type: payment.cashType,
        asNumber: payment.asNumber
    };
    if (items && items.length) {
        for (var i = 0; i < items.length; i++) {
            postData['items[' + i + '].OrderNumber'] = items[i].OrderNumber;
            postData['items[' + i + '].PONumber'] = items[i].PONumber;
            postData['items[' + i + '].SONumber'] = items[i].SONumber;
            postData['items[' + i + '].Amount'] = items[i].Amount;
        }
    }
    setTimeout(function () {
        $.ajax({
            async: true,
            type: 'POST',
            url: payment.createPaymentUrl,
            data: postData,
            success: function (data) {
                if (payment.cashType === 0 || payment.cashType === 6)
                    payment.getOnlinePaymentRecords();
                if (data.success && data.result) {
                    if (data.result.Item1) {
                        var billNo = data.result.Item2.split(',')[0];
                        var orderTime = data.result.Item2.split(',')[1];

                        $('#payType').val($payType.val());
                        $('#orderAmount').val(amount);
                        $('#billNo').val(billNo);
                        $('#orderTime').val(orderTime);
                        $('#isAdminPortal').val(configState == 0);
                        $('#payOrderId').val(billNo);
                        $('#orderRequestDatetime').val(orderTime);
                        $('#goodsName').val(payment.goodsName);
                        layer.confirm('<table style="height:120px" class="table table-striped table-bordered table-hover payTipsTable"><tr><td colspan="2">请确认以下信息</td></tr><tr><td> ' + payment.type + '方式：</td><td><lable class="payTips">' + ($payType.prev().val()) + ' </lable></td></tr><tr><td>' + payment.type + '金额（元）：</td><td> <lable class="payTips">' + fmoney(amount / 100, 2) + '</lable></td></tr><tr></table>', {
                            title: '系统提示',
                            btn: ['前往' + payment.type + '', '重新选择'],
                            btnAlign: 'c',
                            area: ['450px', '300px']
                        }, function () {
                            layer.close(layer.index);
                            if (payment.cashType === 0 || payment.cashType === 6) {
                                if (payment.getRecordInterval != null)
                                    clearInterval(payment.getRecordInterval);
                                payment.getRecordInterval = setInterval(function () { payment.getOnlinePaymentRecords(); }, 5000);
                            }

                            $('#btnPay').click();
                            layer.msg('支付完成前，请不要关闭此页面！', {
                                time: 0,
                                icon: 0
                                 , closeBtn: 1
                                 , shade: 0.3
                                 , btn: [payment.type + '成功', payment.type + '失败', '遇到问题']
                                 , yes: function (index, layero) {
                                     payment.paySuccess();
                                     layer.close(index);
                                 }
                                 , cancel: function (index, layero) {
                                     payment.closeDialog();
                                 }
                                 , btn2: function (index, layero) {
                                     payment.rePayment();
                                 }
                                 , btn3: function (index, layero) {
                                     payment.needHelp();
                                 }
                            });
                        });
                    } else {
                        layer.alert(data.result.Item2, {
                            icon: 2,
                            title: '系统提示',
                            skin: 'layer-ext-moon'
                        });
                    }
                } else {
                    layer.alert(data.message, {
                        icon: 2,
                        title: '系统提示',
                        skin: 'layer-ext-moon'
                    });
                }
            },
            complete: function () {
                layer.close(loadingIndex);
            }
        });
    }, 10);

}

payment.cbsPayment=function($payType,orderAmount,amount,items) {
    var index = layer.load();
       
        $.ajax({
            type: 'POST',
            url: payment.getCBSAccounts,
            data: {
                isAdminPortal: ($payType.prev().prev().val() == 0),
                payType: $payType.val()
            },
            success: function(data) {
                if (data && data.result) {
                    if (data.result.Item1 && data.result.Item2 && data.result.Item2.length) {
                        layer.open({
                            skin: 'layui-layer-lan',
                            type: 1,
                            shade: 0.6,
                            area: ['1040px', '320px'],
                            content: $('.cbsAccounts'), 
                            move: false,
                            title: '需要' + payment.type + ' <label class="cbsPayAmout">0</label>元，请选择一个账户作为付款账户！',
                            shift: 1,
                            maxmin: true,
                            btn: ['确定', '取消'],
                            yes: function() {
                                var $cbsAccount = $("input[name='cbsAccount'][checked]");
                                if ($cbsAccount.val() == null) {
                                    layer.msg('请选择付款账户！', { time: 2000, icon: 2 });
                                    return;
                                }
                                payment.cbsPayWaiting = 0;
                                layer.closeAll();
                                layer.open({
                                    closeBtn: 0,
                                    type: 1,
                                    shade: 0.6,
                                    area: ['440px', '220px'],
                                    content: $('.cbsPaymenting'), 
                                    move: false,
                                    title: false,
                                    shift: 0,
                                    success: function() {
                                        document.getElementById("countdown").className = "countdown";
                                        document.getElementById("timer").innerHTML = 4;
                                        var myCounter = new Countdown({
                                            seconds: 4, onUpdateStatus: function(e) { document.getElementById("timer").innerHTML = e },
                                            onCounterEnd: function() {
                                                document.getElementById("countdown").className = "countdown end";
                                                payment.cbsPayWaiting = 1;
                                            }
                                        });
                                        myCounter.start();
                                    var postData={
                                        isAdminPortal: ($payType.prev().prev().val() == 0), cbsAccount: $cbsAccount.val(),
                                        configSign: $payType.prev().prev().prev().val(),
                                        configState: $payType.prev().prev().val(),
                                        payType: $payType.val(), orderAmount: amount, orderId: payment.orderId,
                                        type: payment.cashType, asNumber: payment.asNumber
                                            }
                                        if (items && items.length) {
                                            for (var i = 0; i < items.length; i++) {
                                                postData['items[' + i + '].OrderNumber'] = items[i].OrderNumber;
                                                postData['items[' + i + '].PONumber'] = items[i].PONumber;
                                                postData['items[' + i + '].SONumber'] = items[i].SONumber;
                                                postData['items[' + i + '].Amount'] = items[i].Amount;
                                            }
                                        }
                                        $.post(payment.cbsPaymentUrl, postData, function (transactionData) {
                                            payment.cbsPayComplete(transactionData);
                                        });
                                    }

                                });
                            },
                            success: function () {
                                $('.cbsPayAmout').text(fmoney(orderAmount, 2));
                                var $cbsAccounts = $('#cbsAccounts').empty();
                                for (var i = 0; i < data.result.Item2.length; i++) {
                                    $cbsAccounts.append('<tr><td><input type="radio" name="cbsAccount" value="' + data.result.Item2[i].BankNumber + '" /></td><td>' + data.result.Item2[i].OpenBankName + '</td><td>' + data.result.Item2[i].BankType + '</td><td>' + data.result.Item2[i].CompanyName + '</td><td>' + data.result.Item2[i].BankNumber + '</td><td>' +data.result.Item2[i].BankAddress + '</td></tr>');
                                }
                                $('tr', $cbsAccounts).on('click', function(event) {
                                    $('input:radio[name=cbsAccount]').removeAttr('checked');
                                    $('input:radio[name=cbsAccount]', this).attr('checked', 'checked')[0].checked = true;
                                });
                            }
                        });
                    } else {
                        layer.alert(data.result.Item3 + ', 未找到招行CBS账户，请使用其他' + payment.type + '方式！', {
                            icon: 2,
                            title: '系统提示',
                            skin: 'layer-ext-moon'
                            //,time: 3000
                        });
                    }
                } else {
                    layer.alert('未找到招行CBS账户，请使用其他' + payment.type + '方式！', {
                        icon: 2,
                        title: '系统提示',
                        skin: 'layer-ext-moon'
                        //,time: 3000
                    });
                }
            },
            complete: function() {
                layer.close(index);
            },
            error: function() {
                layer.alert('未找到招行CBS账户，请使用其他' + payment.type + '方式！', {
                    icon: 2,
                    title: '系统提示',
                    skin: 'layer-ext-moon'
                   //, time: 3000
                });
            }
        });
}

