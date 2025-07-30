let globalData;
if (!globalData) globalData = {};
globalData.currentAppId = '';
globalData.apps = [];

let defaultGuid = '{00000000-0000-0000-0000-000000000000}';
let AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};

//统计常用方法
let StatisticsUtil = {
    /**分组：根据对象属性列表进行分组
     * 返回分组后的对象列表：{'keyString':'[key1, key2]', 'keyObject': [key1, key2], 'total': 12, 'list': Arrary[12]}
     * 实例：
        let arr = [
            { key1: 'x1', key2: 'y1', value: 10 },
            { key1: 'x2', key2: 'y2', value: 20 },
            { key1: 'x3', key2: 'y3', value: 30 },
            { key1: 'x3', key2: 'y3', value: 20 }
        ];
        let groupData = StatisticsUtil.groupBy(arr, (item) => {
            return [item.key1, item.key2];
        });
        console.log(groupData);
        结果：
        [{'keyString': 'x1-y1', 'keyObject': [x1, y1], 'total': 1, 'list': [
                             { key1: "x1", key2: "y1", value: 10 }]
         },
         {'keyString': 'x2-y2', 'keyObject': [x2, y2], 'total': 1, 'list': [
                             { key1: "x2", key2: "y2", value: 20 }]
         },
         {'keyString': 'x3-y3', 'keyObject': [x3, y3], 'total': 2, 'list': [
                             { key1: "x3", key2: "y3", value: 30 },
                             { key1: "x3", key2: "y3", value: 20 }}]
         }]
     * @return {Array}
     */
    groupBy: function (array, fn) {
        //debugger;
        const groups = {};
        array.forEach(function (o) {
            const group = JSON.stringify(fn(o));
            groups[group] = groups[group] || [];
            groups[group].push(o);
        });
        return Object.keys(groups).map(function (group) {
            return { 'keyString': JSON.parse(group).join('-'), 'keyObject': JSON.parse(group), 'total': groups[group].length, 'list': groups[group] };
        });
    },
    /**统计个数：根据对象属性列表进行统计个数
     * 返回分组后的字典类对象：[x1-y1:1, X2-y2:1, x3-y3:2]
     * 实例：
        let arr = [
            { key1: 'x1', key2: 'y1', value: 10 },
            { key1: 'x2', key2: 'y2', value: 20 },
            { key1: 'x3', key2: 'y3', value: 30 },
            { key1: 'x3', key2: 'y3', value: 20 }
        ];
        let countData = StatisticsUtil.countGroupBy(arr, (item) => {
            return [item.key1, item.key2];
        });
        console.log(countData);
        结果：可通过下标访问，如：countData[x1-y1] = 1
        [x1-y1:1, X2-y2:1, x3-y3:2]
     * @return int
     */
    countGroupBy: function (array, fn) {
        //debugger;
        const groups = {};
        array.forEach(function (o) {
            const group = fn(o).join('-');
            groups[group] = groups[group] || [];
            groups[group].push(o);
        });

        const result = new Array();
        Object.keys(groups).forEach(function (group) {
            result[group] = groups[group].length;
        });

        return result;
    },
    /**单属性排序比较：根据对象单属性进行排序
     * 返回排序后的对象列表：
     * 实例：
        let arr  = [
            { key1: 'x1', key2: 'y1', value: 10 },
            { key1: 'x3', key2: 'y3', value: 20 },
            { key1: 'x2', key2: 'y2', value: 30 },
            { key1: 'x3', key2: 'y3', value: 20 }
        ];
        console.log(arr.sort(StatisticsUtil.compare('key1')))
        结果：
        [{ key1: 'x1', key2: 'y1', value: 10 },
        { key1: 'x2', key2: 'y2', value: 20 },
        { key1: 'x3', key2: 'y3', value: 30 },
        { key1: 'x3', key2: 'y3', value: 20 }]
     * @return {Array}
     */
    compare: function (property) {
        //当 a - b < 0  时， 则 a 元素排在 b 元素的前面。
        //当 a - b = 0 时， a , b 元素的位置不变。
        //当 a - b > 0 是， 则 b 元素排在 a 元素的前面。
        return function (a, b) {
            var value1 = a[property];
            var value2 = b[property];
            return value1 - value2;
        }
    },
    /**单属性排序比较：根据对象属性进行排序，第二个参数没有传递 默认升序排列
     * 返回排序后的对象列表：
     * 实例：
        let arr  = [
            { key1: 'x1', key2: 'y1', value: 10 },
            { key1: 'x3', key2: 'y3', value: 20 },
            { key1: 'x2', key2: 'y2', value: 30 },
            { key1: 'x3', key2: 'y3', value: 20 }
        ];
        console.log(arr.sort(StatisticsUtil.singleSortBy('key1', false)))
        结果：
        [{ key1: 'x3', key2: 'y3', value: 20 },
        { key1: 'x3', key2: 'y3', value: 30 },
        { key1: 'x2', key2: 'y2', value: 20 },
        { key1: 'x1', key2: 'y1', value: 10 },]
     * @return {Array}
     */
    singleSortBy: function (attr, desc) {
        //第二个参数没有传递 默认升序排列
        let rev = 1;
        if (desc == undefined) {
            rev = 1;
        } else {
            rev = (desc) ? 1 : -1;
        }
        //当 a - b < 0  时， 则 a 元素排在 b 元素的前面。
        //当 a - b = 0 时， a , b 元素的位置不变。
        //当 a - b > 0 是， 则 b 元素排在 a 元素的前面。
        return function (a, b) {
            a = a[attr];
            b = b[attr];
            if (a < b) {
                return rev * -1;
            }
            if (a > b) {
                return rev * 1;
            }
            return 0;
        }
    },
    /**多属性排序比较：根据对象属性进行排序，第二个参数没有传递 默认升序排列
     * 返回排序后的对象列表：
     * 实例：
        let arr  = [
            { key1: 'x1', key2: 'y1', value: 10 },
            { key1: 'x3', key2: 'y3', value: 20 },
            { key1: 'x2', key2: 'y2', value: 30 },
            { key1: 'x3', key2: 'y3', value: 20 }
        ];
        console.log(arr.sort(StatisticsUtil.multiSortBy(['key1', 'key2'], false)))
        结果：
        [{ key1: 'x3', key2: 'y3', value: 20 },
        { key1: 'x3', key2: 'y3', value: 30 },
        { key1: 'x2', key2: 'y2', value: 20 },
        { key1: 'x1', key2: 'y1', value: 10 },]
     * @return {Array}
     */
    multiSortBy: function (propertyArray, desc) {
        //第二个参数没有传递 默认升序排列
        let rev = 1;
        if (desc == undefined) {
            rev = 1;
        } else {
            rev = (desc) ? 1 : -1;
        }
        //当 a - b < 0  时， 则 a 元素排在 b 元素的前面。
        //当 a - b = 0 时， a , b 元素的位置不变。
        //当 a - b > 0 是， 则 b 元素排在 a 元素的前面。
        return function (item1, item2) {
            var level = 0;
            var sorting = function () {
                var propertyName = propertyArray[level];
                level++;

                var itemCell1 = item1[propertyName],
                    itemCell2 = item2[propertyName];

                //to check type
                if (!checkLetter.test(itemCell1)) {
                    itemCell1 = parseInt(itemCell1, 10);
                    itemCell2 = parseInt(itemCell2, 10);
                }

                if (itemCell1 < itemCell2) {
                    return rev * -1; //从小到大排序
                } else if (itemCell1 > itemCell2) {
                    return rev * 1;
                } else if (itemCell1 === itemCell2) {
                    if (level === levelCount) {
                        return 0;
                    } else {
                        return sorting();
                    }
                }

            };
            return sorting();
        }
    }
};

let CommonUtil = {
    // 加载json文件
    loadJsonFromUrl: function (url, method, callback, noCache) {
        if (noCache === undefined) noCache = true;
        var xmlhttp;
        if (!method) {
            method = 'GET';
        }
        try {
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        } catch (e) {
            try {
                xmlhttp = new XMLHttpRequest();
            } catch (e) {
                return null;
            }
        }
        xmlhttp.open(method, url);
        if (noCache) {
            xmlhttp.setRequestHeader('Cache-Control', 'max-age=0');
        }
        xmlhttp.onreadystatechange = function () {
            if (xmlhttp.readyState === 4) {
                if (xmlhttp.status === 200) {
                    callback(null, xmlhttp.responseText);
                } else {
                    callback(xmlhttp.status, xmlhttp.responseText);
                }
            }
        };
        try {
            xmlhttp.send(null);
        } catch (e) {
            callback(e, '');
        }
    },
    // 获取Guid
    getGuid: function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            let r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        })
    },
    // 获取随机数
    getRandomNumber: function () {
        return Math.floor(Math.random() * 999999 + 1);
    },
    /**
     * 根据form表单的id获取表单下所有可提交的表单数据，封装成数组对象
     */
    getFormData: function (formId) {
        let data = {};
        let results = $(formId).serializeArray();
        $.each(results, function (index, item) {
            //文本表单的值不为空才处理
            if (item.value && $.trim(item.value) !== "") {
                if (!data[item.name]) {
                    data[item.name] = item.value;
                } else {
                    //name属性相同的表单，值以英文,拼接
                    data[item.name] = data[item.name] + ',' + item.value;
                }
            }
        });
        //console.log(data);
        return data;
    },
    /**
     * 根据form表单的id获取表单下所有可提交的表单数据，封装成数组对象
     * 示例：将下列表单的数据转换成Array<Object>对象
       <form id="saveForm">
         <input type="hidden" name="workflowFields[0][id]" value="1">
         <input type="hidden" name="workflowFields[0][text]" value="aaa">
         <input type="hidden" name="workflowFields[1][id]" value="2">
         <input type="hidden" name="workflowFields[1][text]" value="bbb">
       </form>
       调用：CommonUtil.getArrayFormDataByPropertyName('saveForm', 'workflowFields');
       结果：[{'id': 1, 'text': 'aaa'}, {'id': 2, 'text': 'bbb'}]
     */
    getArrayFormDataByPropertyName: function (formId, arrPropertyName) {
        var formData = new FormData(document.querySelector(formId));
        var results = [];
        for (var key of formData.keys()) {
            var obj = this.getDictObjectWithIndexAndPropertyName(arrPropertyName, key);
            if (obj) {
                if (results[obj.index])
                    results[obj.index][obj.key] = formData.get(key);
                else
                    results[obj.index] = { [obj.key]: formData.get(key) };
            }
        }
        return results;
    },
    /**
     * 根据表单列表对象的属性名称，获取index、propertyName
     * 示例：workflowFields[0][text]  ---->    {'index': 0, 'key': 'text'}
             workflowFields[2][displayName]  ---->    {'index': 2, 'key': 'displayName'}
       调用：CommonUtil.getDictObjectWithIndexAndPropertyName('workflowFields', 'workflowFields[0][text]')
       结果：{'index': 0, 'key': 'text'}
    */
    getDictObjectWithIndexAndPropertyName: function (name, key) {
        if (key.includes(name)) {
            var splitStr = key.split(/\[|\]/g);
            return {
                index: splitStr[1],
                key: splitStr[3],
            }
        }
        return null;
    },
    /**
     * url表示请求路径,进入后台处理,后台返回一个文件流
    * 例如:url为htpp://127.0.0.1/test
    */
    downloadFile: function (url, filename) {
        fetch(url).then(res => res.blob().then(blob => {
            let a = document.createElement('a');
            let url = window.URL.createObjectURL(blob);
            a.href = url;
            a.download = filename;
            a.click();
            window.URL.revokeObjectURL(url);
        }));
    },
    //打开预览文件
    reviewFile: function (fileUrl) {
        fetch(fileUrl).then(res => res.blob().then(blob => {
            if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                let ieBlob = new Blob([blob], { type: 'application/pdf' });
                window.navigator.msSaveOrOpenBlob(ieBlob);
            } else {
                if (/constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === '[object SafariRemoteNotification]'; })
                    (!window['safari'] || (typeof safari !== 'undefined' && safari.pushNotification))) {
                    let safariBlob = new Blob([blob], { type: 'application/pdf' });
                    window.saveAs(safariBlob, stmtCde);
                } else {
                    let link = window.URL.createObjectURL(blob);
                    window.open(link, '', 'width = 700,height = 400');
                }
            }
        }));
    },
    /**
     * 根据扩展名获取文件的iCon
     * @return {string}
     */
    getIconClassByExt: function (ext) {
        let iconClass = 'fa-file-text-o';
        switch (ext) {
            case 'txt':
                iconClass = 'fa-file-text-o';
                break;
            case 'pdf':
                iconClass = 'fa-file-pdf-o';
                break;
            case 'doc':
            case 'docx':
            case 'ppt':
            case 'pptx':
                iconClass = 'fa-file-word-o';
                break;
            case 'xls':
            case 'xlsx':
                iconClass = 'fa-file-excel-o';
                break;
            case 'jpg':
            case 'jpeg':
            case 'png':
            case 'gif':
            case 'bmp':
            case 'tiff':
            case 'wmf':
                iconClass = 'fa-file-image-o';
                break;
            case 'au':
            case 'ram':
            case 'rmi':
            case 'aif':
            case 'mp3':
                iconClass = 'fa-file-audio-o';
                break;
            case 'wav':
            case 'mp4':
            case 'flv':
            case 'avi':
            case 'mov':
                iconClass = 'fa-file-video-o';
                break;
        }

        return iconClass;
    },
    /**
     * 将Html中的特殊字符进行转义为后台能接受的字符串
     * @return {string}
     */
    html2Escape: function (sHtml) {
        return sHtml.replace(/[<>&"]/g, function (c) { return { '<': '&lt;', '>': '&gt;', '&': '&amp;', '"': '&quot;' }[c]; });
    },
    /**
     * 将后台Json字符串转义后的特殊字符还原回Html的字符
     * @return {string}
     */
    escape2Html: function (str) {
        let arrEntities = { 'lt': '<', 'gt': '>', 'nbsp': ' ', 'amp': '&', 'quot': '"' };
        return str.replace(/&(lt|gt|nbsp|amp|quot);/ig, function (all, t) { return arrEntities[t]; });
    },
    /**
     * 设置按钮样式
     * @return {string}
     */
    buttonStyle: function () {
        $(".btnSearch").linkbutton({ iconCls: 'fa fa-search' });    //搜索
        $(".btnRefresh").linkbutton({ iconCls: 'fa fa-refresh' });  //刷新
        $(".btnInfo").linkbutton({ iconCls: 'fa fa-info' });        //详情
        $(".btnAdd").linkbutton({ iconCls: 'fa fa-plus' });         //新增
        $(".btnEdit").linkbutton({ iconCls: 'fa fa-pencil' });      //编辑
        $(".btnDelete").linkbutton({ iconCls: 'fa fa-trash' });     //删除
        $(".btnFilter").linkbutton({ iconCls: 'fa fa-filter' });    //筛选

        $(".btnSave").linkbutton({ iconCls: 'fa fa-floppy-o' });    //保存
        $(".btnSubmit").linkbutton({ iconCls: 'fa fa-check' });     //提交
        $(".btnCancel").linkbutton({ iconCls: 'fa fa-ban' });       //取消
        $(".btnBack").linkbutton({ iconCls: 'fa fa-undo' });        //回退
        $(".btnClose").linkbutton({ iconCls: 'fa fa-close' });      //关闭

        $(".btnPrint").linkbutton({ iconCls: 'fa fa-print' });          //打印
        $(".btnUpload").linkbutton({ iconCls: 'fa fa-upload' });        //上传
        $(".btnDownload").linkbutton({ iconCls: 'fa fa-download' });    //下载
        $(".btnExport").linkbutton({ iconCls: 'fa fa-cloud-download' });//导出
        $(".btnImport").linkbutton({ iconCls: 'fa fa-cloud-upload' });  //导入
        $(".btnShare").linkbutton({ iconCls: 'fa fa-share-alt' });      //分享
        $(".btnStar").linkbutton({ iconCls: 'fa fa-star' });            //关注


        $(".btnLog").linkbutton({ iconCls: 'fa fa-file-text-o' });    //日志
        $(".btnEmail").linkbutton({ iconCls: 'fa fa-envelope' });     //邮件
        $(".btnMobile").linkbutton({ iconCls: 'fa fa-mobile' });       //手机
        $(".btnPhone").linkbutton({ iconCls: 'fa fa-phone' });        //座机
        $(".btnMessage").linkbutton({ iconCls: 'fa fa-comments' });   //消息
        $(".btnRemind").linkbutton({ iconCls: 'fa fa-bell' });        //提醒

        $(".btnTxt").linkbutton({ iconCls: 'fa fa-file-text-o' });    //Text
        $(".btnPdf").linkbutton({ iconCls: 'fa fa-file-pdf-o' });     //Pdf
        $(".btnWord").linkbutton({ iconCls: 'fa fa-file-word-o' });   //Word
        $(".btnExcel").linkbutton({ iconCls: 'fa fa-file-excel-o' }); //Excel
        $(".btnImage").linkbutton({ iconCls: 'fa fa-file-image-o' }); //Image
        $(".btnAudio").linkbutton({ iconCls: 'fa fa-file-audio-o' }); //Audio
        $(".btnVideo").linkbutton({ iconCls: 'fa fa-file-video-o' }); //Video

        $(".btnList").linkbutton({ iconCls: 'fa fa-list-alt' });        //列表
        $(".btnTable").linkbutton({ iconCls: 'fa fa-table' });          //列表
        $(".btnCard").linkbutton({ iconCls: 'fa fa-th-large' });        //卡片

        $(".btnBarcode").linkbutton({ iconCls: 'fa fa-barcode' });     //条形码
        $(".btnQrcode").linkbutton({ iconCls: 'fa fa-qrcode' });       //二维码

        $(".btnAreaChart").linkbutton({ iconCls: 'fa fa-area-chart' });   //Area
        $(".btnBarChart").linkbutton({ iconCls: 'fa fa-bar-chart' });     //Bar
        $(".btnLineChart").linkbutton({ iconCls: 'fa fa-line-chart' });   //Line
        $(".btnPieChart").linkbutton({ iconCls: 'fa fa-pie-chart' });     //Pie

        $(".btnUp").linkbutton({ iconCls: 'fa fa-chevron-up' });      //Up
        $(".btnDown").linkbutton({ iconCls: 'fa fa-chevron-down' });  //Down
        $(".btnLeft").linkbutton({ iconCls: 'fa fa-chevron-left' });  //Left
        $(".btnRight").linkbutton({ iconCls: 'fa fa-chevron-right' });//Right

    },
    /**
     * 滚动到指定控件（控件Id）位置
     * @return {string}
     */
    scrollToControlById: function (id) {
        var elem = document.getElementById(id);
        var curleft = 0, curtop = 0;
        if (elem.offsetParent) {
            curleft = elem.offsetLeft;
            curtop = elem.offsetTop;
            while (elem = elem.offsetParent) {
                curleft += elem.offsetLeft;
                curtop += elem.offsetTop;
            }
        }
        var scrollPos = curtop;
        scrollPos = scrollPos - document.documentElement.scrollTop;
        var remainder = scrollPos % 50;
        var repeatTimes = (scrollPos - remainder) / 50;
        ScrollSmoothly(scrollPos, repeatTimes);
        window.scrollBy(0, remainder);
    },
    /**
     * 根据RadionButton组的name，获取选中的值
     * @return {string}
     */
    getRadioSelectValue: function (name) {
        let result = $('.radiobutton.radiobutton-checked input[name="' + name + '"]').val();
        if (result) return result;
        return '';
    },
    /**
     * 在div中显示文件名，单击后可下载文件，相关html及JavaScript如下：
    //前端html
    <input id="hiddenBlobDataId" type="hidden" data-options="required:true,readonly:true"
            value="@(null != Model ? Model.BlobString : string.Empty)" />
    <div id="filList" style="height: 30px; width: 100%;" class="webUploader-file-single"></div>
    
    //调用图片显示方法
    let baseUrl = '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/';//通过Api站点显示图片的路径
    showBlobFile('filList', 'hiddenBlobDataId', baseUrl);
    
    //文件显示的相关方法
    let showBlobFile = function (appendId, blobDivId, baseUrl) {
        let selectFileJsonString = $('#' + blobDivId).val();
        if (selectFileJsonString !== '') {
            let selectBlobs = JSON.parse(selectFileJsonString);
            $('#' + appendId).append(CommonUtil.getBlobToDownloadHtml(selectBlobs, false, baseUrl));
        }
    }
     * @param {any} selectBlob 文件的blob对象列表或单个文件Blob对象
     * @param {any} isWrap 多文件是否换行
     * @param {any} baseUrl 文件下载的路径，可设置为通过Api站点下载文件的路径
     */
    getBlobToDownloadHtml: function (selectBlob, isWrap, baseUrl) {
        if (selectBlob === undefined || selectBlob === null) return '';

        const defaultShowImageUrl = baseUrl ? baseUrl + 'ShowImage?id=' : '/Home/ShowImage?id=';
        const defaultDownloadFileUrl = baseUrl ? 'DownloadFile?id=' : '/Home/ShowImage?id=';
        let isArray = $.array.isArray(selectBlob);
        if (isArray) {
            let aList = [];
            for (var i = 0; i < selectBlob.length; i++) {
                let blob = selectBlob[i] ? selectBlob[i] : null;
                let downloadUrl = blob.downloadFileUrl ? blob.downloadFileUrl : defaultDownloadFileUrl + blob.blobId;
                if (blob != null)
                    aList.push('<a target="_blank" href="' + downloadUrl + '" download="' + blob.blobName + '">' + blob.blobName + '</a>');
            }

            if (isWrap === undefined || isWrap === null) isWrap = true;

            return isWrap ? aList.join('<br/>') : aList.join('&nbsp;&nbsp;');
        }

        let downloadUrl = selectBlob.downloadFileUrl ? selectBlob.downloadFileUrl : defaultDownloadFileUrl + selectBlob.blobId;
        return '<a class="singleFile" target="_blank" href="' + downloadUrl + '" download="' + selectBlob.blobName + '">' + selectBlob.blobName + '</a>';
    },
    /**
     * 在div中显示图片，进入图片后放大图片，相关html及JavaScript如下：
    //前端html
    <input id="hiddenBlobDataId" type="hidden" data-options="required:true,readonly:true"
            value="@(null != Model ? Model.BlobString : string.Empty)" />
    <div class="imageUploaderEditor">
        <div id="imageList" style="height: 30px; width: 100%;" class="webUploader-image-single"></div>
        <p id="imagePreviewContainer-single" class="imagePreview"></p>
    </div>

    //调用图片显示方法
    let baseUrl = '@(KC.Framework.Base.GlobalConfig.ApiServerUrl)Resources/';//通过Api站点显示图片的路径
    showBlobImage('imageList', 'hiddenBlobDataId', baseUrl);

    //图片显示的相关方法
    let showBlobImage = function (appendId, blobDivId, baseUrl) {
        let selectFileJsonString = $('#' + blobDivId).val();
        if (selectFileJsonString !== '') {
            let selectBlobs = JSON.parse(selectFileJsonString);
            $('#' + appendId).append(CommonUtil.getBlobToImageHtml(selectBlobs, baseUrl));
        }
    }
     * @param {any} selectBlob 图片的blob对象列表或单个文件Blob对象
     * @param {any} baseUrl  图片显示的路径，可设置为通过Api站点显示图片的路径
     */
    getBlobToImageHtml: function (selectBlob, baseUrl) {
        if (selectBlob === undefined || selectBlob === null) return '';

        const defaultShowImageUrl = baseUrl ? baseUrl + 'ShowImage?id=' : '/Home/ShowImage?id=';
        const defaultDownloadFileUrl = baseUrl ? 'DownloadFile?id=' : '/Home/DownloadFile?id=';
        let isArray = $.array.isArray(selectBlob);
        if (isArray) {
            let aList = [];
            for (var i = 0; i < selectBlob.length; i++) {
                let blob = selectBlob[i] ? selectBlob[i] : null;
                if (blob != null) {
                    let downloadUrl = blob.showImageUrl ? blob.showImageUrl : defaultShowImageUrl + blob.blobId;
                    let divShowFile =
                        '<div>' +
                        '   <div id="file-' + blob.blobId + '" class="imageFile">' +
                        '       <img id="img-' + blob.blobId + '" class="fileImage" src="' + downloadUrl + '" alt="' + blob.blobName + '">' +
                        '   </div>' +
                        '</div>';

                    aList.push(divShowFile);
                }
            }

            return aList.join('&nbsp;&nbsp;');
        }

        let downloadUrl = selectBlob.showImageUrl ? selectBlob.showImageUrl : defaultShowImageUrl + selectBlob.blobId;
        return '<div>' +
            '   <div id="file-' + selectBlob.blobId + '" class="imageFile">' +
            '       <img id="img-' + selectBlob.blobId + '" class="fileImage" src="' + downloadUrl + '" alt="' + selectBlob.blobName + '">' +
            '   </div>' +
            '</div>';
    },
    showBlogImageDetail: function (selectBlob) {
        if (selectBlob === undefined || selectBlob === null) return;
        let isArray = $.array.isArray(selectBlob);
        if (isArray) {
            for (var i = 0; i < selectBlob.length; i++) {
                let blob = selectBlob[i] ? selectBlob[i] : null;
                if (blob != null) {
                    const imgId = '#img-' + blob.blobId;
                    $(imgId).hover(function (event) {
                        let e = event || window.event;
                        let scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
                        let scrollY = document.documentElement.scrollTop || document.body.scrollTop;
                        let x = e.pageX || e.clientX + scrollX;
                        let y = e.pageY || e.clientY + scrollY;

                        // if (settings.isEditor !== undefined && settings.isEditor != null && settings.isEditor){
                        //     let actualLeft = getOffsetLeftByBody(this);
                        //     let actualTop = getOffsetTopByBody(this);
                        //
                        //     x = actualLeft - 200; //需要减去左侧菜单的宽度：200
                        //     y = actualTop - 126; //需要减去上方功能菜单高度：43，MainTab页高度：39+3(height+padding)，SubTab页的高度：38+3(height+padding)
                        // }
                        //console.info("x: " + x + ", y: " + y);
                        //console.info("actualLeft: " + actualLeft + ", actualTop: " + actualTop);

                        let imagePreview = $(".imagePreview");
                        imagePreview.css("top", (y + 2) + "px")
                            .css("left", (x + 2) + "px")
                            .html("<img style='width:260px;' src=" + $(this).attr("src") + " alt='" + $(this).attr("alt") + "' /><br/>" + $(this).attr("alt"))
                            .fadeIn("slow");
                    }, function () {
                        let imagePreview = $(".imagePreview");
                        imagePreview.fadeOut("fast");
                    });
                }
            }

            return;
        }

        const imgId = '#img-' + selectBlob.blobId;
        $(imgId).hover(function (event) {
            let e = event || window.event;
            let scrollX = document.documentElement.scrollLeft || document.body.scrollLeft;
            let scrollY = document.documentElement.scrollTop || document.body.scrollTop;
            let x = e.pageX || e.clientX + scrollX;
            let y = e.pageY || e.clientY + scrollY;

            // if (settings.isEditor !== undefined && settings.isEditor != null && settings.isEditor){
            //     let actualLeft = getOffsetLeftByBody(this);
            //     let actualTop = getOffsetTopByBody(this);
            //
            //     x = actualLeft - 200; //需要减去左侧菜单的宽度：200
            //     y = actualTop - 126; //需要减去上方功能菜单高度：43，MainTab页高度：39+3(height+padding)，SubTab页的高度：38+3(height+padding)
            // }
            //console.info("x: " + x + ", y: " + y);
            //console.info("actualLeft: " + actualLeft + ", actualTop: " + actualTop);

            let imagePreview = $(".imagePreview");
            imagePreview.css("top", (y + 2) + "px")
                .css("left", (x + 2) + "px")
                .html("<img style='width:260px;' src=" + $(this).attr("src") + " alt='" + $(this).attr("alt") + "' /><br/>" + $(this).attr("alt"))
                .fadeIn("slow");
        }, function () {
            let imagePreview = $(".imagePreview");
            imagePreview.fadeOut("fast");
        });
    }
};

var repeatCount = 0;
var cTimeout;
var timeoutIntervals = new Array();
var timeoutIntervalSpeed;
let ScrollSmoothly = function (scrollPos, repeatTimes) {
    if (repeatCount < repeatTimes) {
        window.scrollBy(0, 50);
    }
    else {
        repeatCount = 0;
        clearTimeout(cTimeout);
        return;
    }
    repeatCount++;
    cTimeout = setTimeout("ScrollSmoothly('" + scrollPos + "','" + repeatTimes + "')", 10);
};

/**
 * 初始化ajax返回调用逻辑
 */
$.ajaxSetup({
    complete: function (request, status) {
        if (typeof (request) != 'undefined') {
            let responseText = request.getResponseHeader("X-Responded-JSON");
            if (responseText != null) {
                if ($.isFunction(window.tipError) && window !== undefined) {
                    window.tipError('系统提示', '登录超时，请重新登录!',
                        function () {
                            window.location.href = window.location.href;
                        }
                    );
                } else {
                    $.messager.alert('系统提示', '登录超时，请重新登录!', function (r) {
                        window.location.href = window.location.href;
                    });
                }
            }
        }
    },
    error: function (jqXHR, textStatus, errorThrown) {
        let status = 0;
        let msg = '未知错误';
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
        if (status > 0) {
            if ($.isFunction(window.tipError) && window !== undefined) {
                window.tipError('系统提示', '请联系网站管理员，错误代码：' + status + '，错误消息：' + msg, function (r) {
                    if (status === 401) {
                        window.location.href = '/Account/SigIn';
                    } else {
                        window.location.href = window.location.href;
                    }
                });
            } else {
                $.messager.alert('系统提示', '请联系网站管理员，错误代码：' + status + '，错误消息：' + msg, function (r) {
                    if (status === 401) {
                        window.location.href = '/Account/SigIn';
                    } else {
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
    let token = $("meta[name='_csrf']").attr("content");
    let header = $("meta[name='_csrf_header']").attr("content");
    if (token !== undefined && token != null && token !== ""
        && header !== undefined && header != null && header !== "") {
        $(document).ajaxSend(function (e, xhr, options) {
            xhr.setRequestHeader(header, token);
        });
    }

    /**
     * 将form里面的内容序列化成json
     * 相同的checkbox用分号拼接起来
     * @method serializeJson
     * @param otherString
     * */
    $.fn.serializeJson = function (otherString) {
        let serializeObj = {},
            array = this.serializeArray();
        $(array).each(function () {
            if (serializeObj[this.name]) {
                serializeObj[this.name] += ';' + this.value;
            } else {
                serializeObj[this.name] = this.value;
            }
        });
        if (otherString !== undefined) {
            let otherArray = otherString.split(';');
            $(otherArray).each(function () {
                let otherSplitArray = this.split(':');
                serializeObj[otherSplitArray[0]] = otherSplitArray[1];
            });
        }
        return serializeObj;
    };
    /**
     * 将json对象赋值给form
     * @method serializeJson
     * @param jsonValue
     * */
    $.fn.setForm = function (jsonValue) {
        let obj = this;
        $.each(jsonValue, function (name, ival) {
            let $oinput = obj.find("input[name=" + name + "]");
            if ($oinput.attr("type") === "checkbox") {
                if (ival !== null) {
                    let checkboxObj = $("[name=" + name + "]");
                    let checkArray = ival.split(";");
                    for (let i = 0; i < checkboxObj.length; i++) {
                        for (let j = 0; j < checkArray.length; j++) {
                            if (checkboxObj[i].value === checkArray[j]) {
                                checkboxObj[i].click();
                            }
                        }
                    }
                }
            }
            else if ($oinput.attr("type") === "radio") {
                $oinput.each(function () {
                    let radioObj = $("[name=" + name + "]");
                    for (let i = 0; i < radioObj.length; i++) {
                        if (radioObj[i].value === ival) {
                            radioObj[i].click();
                        }
                    }
                });
            }
            else if ($oinput.attr("type") === "textarea") {
                obj.find("[name=" + name + "]").html(ival);
            }
            else {
                obj.find("[name=" + name + "]").val(ival);
            }

            let $ainput = obj.find("a[name=" + name + "]");
            if ($ainput !== undefined && $ainput.length > 0) {
                $ainput.attr("href", ival);
                $ainput.attr("download", "附件-" + CommonUtil.getGuid());
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
        let len = total < 10 ? total : 10;
        let temp = pageIndex - 5;
        temp = temp < 0 ? 0 : temp;
        let ul = '<ul class="pagination pagination-sm">';
        ul += '<li class="list">共有&nbsp;' + total + '&nbsp;页</li>';
        ul += '<li class="list">每页&nbsp;' + pageSize + '&nbsp;条</li>';
        ul += '<li><a href="javascript:' + pageChanged + '(' + 1 + ')">«</a></li>';
        for (let i = 1; i <= len; i++) {
            let current = temp > 0 ? temp + i : i;
            if (current > total) {
                break;
            }
            if (current === pageIndex) {
                ul += '<li class="active"><a href="javascript:' + pageChanged + '(' + current + ') ">' + current + '</a></li>';
            }
            else {
                if (i === 1 && temp > 0) {
                    ul += '<li><a href="javascript:' + pageChanged + '(' + temp + ')">...</a></li>';
                }
                ul += '<li><a href="javascript:' + pageChanged + '(' + current + ')">' + current + '</a></li>';
                if (i === len && total > current) {
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
    };

    /**
     *  初始化Validator组件
     **/
    if ($.isFunction($.validator) && $.validator !== undefined) {
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
                if ($.isFunction($(element).tooltip)) {
                    $(element).tooltip('destroy'); /*必需*/
                    $(element).attr('title', $(error).text()).tooltip('show');
                }
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
                if ($.isFunction($(element).tooltip))
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