var statusDetails = [],paymentDialog=null;
function goPayment(s,source, order, id, index) {
    if(index && $.util.isNumber(index))
        $dataGrid.datagrid('unselectRow', index);
    checkOpenPayAndBindBank(function () {
        if (s === 1 || s === 2 || s === 9)
            payMarketOrder(source + order + '还款',id);
        else
            paymentDialog=showMaxDialog({ title: source + (s===4||s===8?'':order) + '付款', href: paymentUrl + '/' + id, border: false });
    });
}

function closePaymentDialog() {
    if (paymentDialog != null) {
        paymentDialog.dialog('close');
        paymentDialog = null;
    }
}

function payMarketOrder(title,id) {
    $dialog = $.easyui.showDialog({
        title:title,
        width: '100%',
        height: '100%',
        href: marketPayUrl+'?id='+id+'&isShop=true',
        iniframe: true,
        enableApplyButton: false,
        enableCloseButton: false,
        enableSaveButton: false,
        closable: true
    });
}

function remindingPayment(id,customerTenant,source, index) {
    if(index && $.util.isNumber(index))
        $dataGrid.datagrid('unselectRow', index);
    $.easyui.loading({ msg: '正在通知付款方，请稍等...' });
    $.post(remindBuyerPaymentUrl,
    { id: id,customerTenant: customerTenant,isLocal:(source ===0 ||source ===2|| source ===3)},
    function(data) {
        $.easyui.loaded();
        if (data && data.result)
            $.messager.showInfoTopCenter('提醒成功！');
        else
            $.messager.showErrorTopCenter(data.message ==null?'通知失败.':data.message);
    });
}

function showPayableOrderInfo(id,source, index,so) {
    if(index && $.util.isNumber(index))
        $dataGrid.datagrid('unselectRow', index);
    switch (source) {
        case 0:
        case 5:
        case 6:
            poInfo(id);
            break;
        case 1:
            bcoInfo(id);
            break;
        case 2:
            foInfo(id);
            break;
        case 3:
        case 7:
            if (so === true)
                soInfo(id);
            else
                poInfo(id);
            break;
        case 12:
            asInfo(id);
            break;
    }
}

function showReceivableOrderInfo(id, source, index) {
    if(index && $.util.isNumber(index))
        $dataGrid.datagrid('unselectRow', index);
    switch (source) {
        case 0:
        case 2:
        case 3:
            soInfo(id);
            break;
        case 1:
            scoInfo(id);
            break;
        case 9:
            asInfo(id);
    }
}

function poInfo(id) {
    showMaxDialog({title:'采购订单：' + id,href:'/Admin/Purchase/Details/' + id});
}

function bcoInfo(id) {
    showMaxDialog({title:'赊购订单：' + id,href:'/Admin/Purchase/Details/' + id});
}

function foInfo(id) {
     
}

function soInfo(id) {
    showMaxDialog({title:'销售订单：' + id,href:'/Admin/Sales/Details/' + id});
}

function scoInfo(id) {
    showMaxDialog({title:'赊销订单：' + id,href:'/Admin/Sales/Details/' + id});
}

function asInfo(id) {
    showMaxDialog({ title: '对账单：' + id, href: '/Admin/AccountStatement/Details_V3/' + id });
}

function showOrderStatusDetails() {
    statusDetails = [];
    $('.showStatusDetails').tooltipster({
        contentAsHTML: true,
        interactive: true,
        animation: 'grow',
        theme: 'tooltipster-shadow',
        functionBefore: function (instance, helper) {

            var $content = $('.statusDetailsList').eq(0).clone().show();
            var statusData = null;
            var orderNum = $(helper.origin).attr('data-orderNum');
            for (var i = 0; i < statusDetails.length; i++) {
                if (statusDetails[i].OrderNum === orderNum) {
                    statusData = statusDetails[i];
                }
            }
            if (statusData) {
                assembleOrderStatusDetails($content, statusData);
            } else {
                getOrderStatusDetails(orderNum, $content);
            }
            instance.content($content);
        }
    });
}

function getOrderStatusDetails(orderNum, obj) {
    setTimeout(function () { ShowMask($('.tooltipster-content')); }, 10);
    var data = $dataGrid.datagrid('getData');
    var statusData = {};
    for (var i = 0; i < data.rows.length; i++) {
        if (data.rows[i].OrderId === orderNum) {
            var item = data.rows[i];
            statusData = {
                'OrderNum': orderNum,
                'Amount': item.OrderAmount
            };
            break;
        }
    }
    $.get(getOrderStatusUrl,
        { orderNum: orderNum },
        function (data) {
            HideMask();
            var paid = 0;
            var shipped = 0;
            var receivedGoods = 0;
            if (data && data.result) {
                paid = data.result.Item3;
                shipped = data.result.Item1;
                receivedGoods = data.result.Item2;
            }
            statusData['paid'] = paid;
            statusData['shipped'] = shipped;
            statusData['receivedGoods'] = receivedGoods;
            statusDetails.push(statusData);
            assembleOrderStatusDetails(obj, statusData);
        });
}

function assembleOrderStatusDetails($content, statusData) {
    var width = '100%';
    $('.list_amount .progress_text', $content).text(fmoney(statusData.Amount));
    $('.list_amount .progress-bar').css({ width: width });
    if (statusData.paid / statusData.Amount < 1)
        width = (statusData.paid / statusData.Amount * 100) + '%';
    $('.list_paid .progress_text', $content).text(fmoney(statusData.paid));
    $('.list_paid .progress-bar').css({ width: width });
    if (statusData.shipped / statusData.Amount < 1)
        width = (statusData.shipped / statusData.Amount * 100) + '%';
    else
        width = '100%';
    $('.list_shipped .progress_text', $content).text(fmoney(statusData.shipped));
    $('.list_shipped .progress-bar').css({ width: width });
    if (statusData.receivedGoods / statusData.Amount < 1)
        width = (statusData.receivedGoods / statusData.Amount * 100) + '%';
    else
        width = '100%';
    $('.list_received_goods .progress_text', $content).text(fmoney(statusData.receivedGoods));
    $('.list_received_goods .progress-bar').css({ width: width });
}

function cancelPayable(id, souce, index) {
    var rows = $dataGrid.datagrid('getRows');
    var row =null;
    for (var i = 0; i < rows.length; i++)
    {
        if(rows[i].Id == id)
        {
            row = rows[i];
            break;
        }
    }
    if (row.PayableAmount != row.UnPaidTotal) {
        $.messager.showErrorTopCenter('该订单已付部分金额，不可取消!');
        return;
    }
    if(index && $.util.isNumber(index))
        $dataGrid.datagrid('unselectRow', index);
    $.messager.confirm('确认取消该笔付款吗？', function (flag) {
        if (!flag)
            return;
        $.easyui.loading({msg:'正在提交中...'});
        $.post(cancelPayableUrl, { id: id ,souce:souce}, function (data) {
            $.easyui.loaded();
            if (data) {
                if (data.result) {
                    $.messager.showInfoTopCenter('取消成功!');
                    if (pageName === 'AssetSummary')
                        getPayables();
                    else
                        search();
                } else
                    $.messager.showErrorTopCenter(data.message);
               
            } else {
                $.messager.showErrorTopCenter('取消失败!');
            }
        });

    });
}

function marketPayComplete() {
    if (parent.pageName !== 'AdminHome') {
        refreshAvailableBalance();
        if (parent.pageName === 'AssetSummary') {
            parent.refreshSummary();
            parent.getPayables();
        } else if (parent.parent.pageName === 'AssetSummary') {
            parent.parent.refreshSummary();
            parent.parent.getPayables();
            parent.search();
        } else if (parent.parent.pageName === 'AdminHome') {
            parent.search();
        }
    } else if(parent.pageName === 'AdminHome'){
        parent.getTODO();
    }
    if ($dialog != null) {
        $dialog.dialog('close');
        $dialog = null;
    }
}

function marketPayError() {
    if ($dialog != null) {
        $dialog.dialog('close');
        $dialog = null;
    }
}

function loadLogs(num, payable, index, source) {
    $dataGrid.datagrid('unselectRow', index);
    var $logs = $('.logs').clone().show();
    var dialog = $.easyui.showDialog({
        iconCls:'',
        title: '日志',
        width: 800,
        height: 330,
        modal: true,
        content:$logs,
        enableHeaderContextMenu: false,
        enableApplyButton: false,
        enableSaveButton: false,
        enableCloseButton: false
    });
    
    if ((payable && (source === 1 || source === 2 || source === 9)) || (!payable && (source === 1 || source === 4))) {
        var row = rows[index];
        bindLogs(row.Logs,$logs);
    } else {
        $.easyui.loading({ msg: '获取数据中...', locale: dialog });
        $.get(loadLogsUrl, { num: num,payable:payable }, function (data) {
            $.easyui.loaded({ locale: dialog });
            if (data && data.result) {
                bindLogs(data.result,$logs);
            }
        });
    }
}

function bindLogs(logs,$logs) {
    var $logsTbody = $('#logs', $logs);
    for (var i = 0; i < logs.length; i++) {
        var log = logs[i];
        if (log.Remark)
            $logsTbody.append('<tr><td>' + log.CreatedDateStr + '</td><td>' + log.Remark.replace('$','￥') + '</td><td>' + (log.Operator?log.Operator:'') + '</td></tr>');
    }
}

function getReceivedAndPaidCautionMoney() {
    $.get(getReceivedAndPaidCautionMoneyUrl, function (data) {
        if (data && data.result) {
            $('.receivedCautionMoney').text(fmoney(data.result.Item1));
            $('.paidCautionMoney').text(fmoney(data.result.Item2));
        }
    });
}