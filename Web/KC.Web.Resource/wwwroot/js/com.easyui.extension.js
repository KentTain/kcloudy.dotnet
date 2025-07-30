//日期对比
let CompareUtil = {
    /*
     * 比较两个日期的大小
     * 传入的参数推荐是"yyyy-mm-dd"的格式，其他的日期格式也可以，但要保证一致
     */
    dateCompare: function (date1, date2) {
        if (date1 && date2) {
            let a = new Date(date1);
            let b = new Date(date2);
            return a <= b;
        }
    },
    /*
     * @author : Kent
     * @desc: 比较两个时间的大小（传入的参数是"HH:mm"的格式，）
     * @param: time1:目标时间;time2:被比较时间
     */
    timeCompare: function (time1, time2) {
        //console.info(time1+"-"+time2);
        try {
            if (time1 && time2) {
                let t1 = parseInt(time1.split(":")[0] * 60) + parseInt(time1.split(":")[1]);
                let t2 = parseInt(time2.split(":")[0] * 60) + parseInt(time2.split(":")[1]);
                return t1 <= t2;
            }
            return false;
        } catch (e) {
            return false;
        }
    },
    /*
     * @author : Kent
     * @desc: 比较两个时间的大小，支持的格式可在formatArr扩展
     * @param: datetime1:目标时间;datetime2:被比较时间
     */
    dateTimeCompare: function (datetime1, datetime2) {

        let formatArr = ['YYYY-MM-DD',
            'YYYY-MM-DD HH:mm',
            'YYYY-MM-DD HH:mm:ss'];//支持的格式
        try {
            if (datetime1 && datetime2) {
                let dt1 = moment(datetime1, formatArr);
                let dt2 = moment(datetime2, formatArr);
                //            console.info(dt1+","+dt2);
                return dt1 <= dt2;
            }
            return false;
        } catch (e) {
            return false;
        }
    },
}

//EasyUI用DataGrid格式化
let FormatterUtil = {
    /**布尔型格式化为：是/否
     * @return {string}
     */
    BoolFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') {
            return "";
        }
        /*json格式时间转js时间格式*/
        if (value) {
            return "是";
        } else {
            return "否";
        }
    },
    /**性别格式化为：男/女
     * @return {string}
     */
    SexFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') {
            return "";
        }
        /*json格式时间转js时间格式*/
        if (value) {
            return "男";
        } else {
            return "女";
        }
    },
    /**json格式日期格式化为：yyyy-MM-dd
     * @return {string}
     */

    DateFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') {
            return "";
        }

        let dateValue;
        if (!$.string.isDate(value)) {
            /*json格式时间转js时间格式*/
            value = value.substr(1, value.length - 2);
            let obj = eval('(' + "{Date: new " + value + "}" + ')');
            dateValue = obj["Date"];
        } else {
            dateValue = $.string.toDate(value);
        }

        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd");
    },
    /**json格式时间格式化为：HH:mm
     * @return {string}
     */
    TimeFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') {
            return "";
        }

        let dateValue;
        if (!$.string.isDate(value)) {
            /*json格式时间转js时间格式*/
            value = value.substr(1, value.length - 2);
            let obj = eval('(' + "{Date: new " + value + "}" + ')');
            dateValue = obj["Date"];
        } else {
            dateValue = $.string.toDate(value);
        }

        if (dateValue.getFullYear() < 1900) {
            return "";
        }
        let val = dateValue.format("yyyy-MM-dd HH:mm");
        return val.substr(11, 5);
    },
    /**json格式时间日期格式化为：yyyy-MM-dd HH:mm
     * @return {string}
     */
    DateTimeFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') {
            return "";
        }

        let dateValue;
        if (!$.string.isDate(value)) {
            /*json格式时间转js时间格式*/
            value = value.substr(1, value.length - 2);
            let obj = eval('(' + "{Date: new " + value + "}" + ')');
            dateValue = obj["Date"];
        } else {
            dateValue = $.string.toDate(value);
        }

        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd HH:mm");
    },
    /**json格式时间日期格式化为本地时间（东八区）：yyyy-MM-dd HH:mm
     * @return {string}
     */
    LocalDateTimeFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') {
            return "";
        }

        let dateValue;
        if (!$.string.isDate(value)) {
            /*json格式时间转js时间格式*/
            value = value.substr(1, value.length - 2);
            let obj = eval('(' + "{Date: new " + value + "}" + ')');
            dateValue = obj["Date"];
        } else {
            dateValue = $.string.toDate(value);
        }

        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd HH:mm");
    },
    /**json格式时间日期格式化为：yyyy-MM-dd HH:mm:ss
     * @return {string}
     */
    DateTimeSecondFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') {
            return "";
        }

        let dateValue;
        if (!$.string.isDate(value)) {
            /*json格式时间转js时间格式*/
            value = value.substr(1, value.length - 2);
            let obj = eval('(' + "{Date: new " + value + "}" + ')');
            dateValue = obj["Date"];
        } else {
            dateValue = $.string.toDate(value);
        }

        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd HH:mm:ss");
    },
    /**json格式时间日期格式化为本地时间（东八区）：yyyy-MM-dd HH:mm:ss
     * @return {string}
     */
    LocalDateTimeSecondFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') {
            return "";
        }

        let dateValue;
        if (!$.string.isDate(value)) {
            /*json格式时间转js时间格式*/
            value = value.substr(1, value.length - 2);
            let obj = eval('(' + "{Date: new " + value + "}" + ')');
            dateValue = obj["Date"];
        } else {
            dateValue = $.string.toDate(value);
        }

        if (dateValue.getFullYear() < 1900) {
            return "";
        }

        return dateValue.format("yyyy-MM-dd HH:mm:ss");
    },

    /**标题格式化为：8位字符串长度的缩写格式，xxxxxxxx...
     * @return {string}
     */
    TitleFormatter: function (value, rec, index) {
        if (value.length > 10) value = value.substr(0, 8) + "...";
        return value;
    },
    /**长标题格式化为：12位字符串长度的缩写格式，xxxxxxxxxxxx...
     * @return {string}
     */
    LongTitleFormatter: function (value, rec, index) {
        if (value.length > 15) value = value.substr(0, 12) + "...";
        return value;
    },

    /**Blob对象格式化为：下载链接
     * @return {string}
     */
    DownloadBlobFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') return '';
        let isArray = $.array.isArray(value);
        if (isArray) {
            let aList = [];
            for (var i = 0; i < value.length; i++) {
                let blob = value[i] ? value[i] : null;
                if (blob != null)
                    aList.push('<a style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis; width:60px;" title="' + blob.blobName + '" target="_blank" href="' + blob.downloadFileUrl + '" download="' + blob.blobName + '">' + blob.blobName + '</a>');
            }
            return aList.join('<br/>');
        }
        if (value.blobName == undefined) {
            let data = JSON.parse(value);
            return '<a style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis; width:60px;" title="' + data.blobName + '" target="_blank" href="' + data.downloadFileUrl + '" download="' + data.blobName + '">' + data.blobName + '</a>';
        }
        return '<a style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis; width:60px;" title="' + value.blobName + '" target="_blank" href="' + value.downloadFileUrl + '" download="' + value.blobName + '">' + value.blobName + '</a>';
    },
    /**Blob对象格式化为：显示图片
     * @return {string}
     */
    ShowImageBlobFormatter: function (value, rec, index) {
        if (value === undefined || value === null || value === '') return '';
        let isArray = $.array.isArray(value);
        if (isArray) {
            let aList = [];
            for (var i = 0; i < value.length; i++) {
                let blob = value[i] ? value[i] : null;
                if (blob != null)
                    aList.push('<img src="' + blob.showImageUrl + '" alt="' + blob.blobName + '" height="42" />');
            }

            return aList.join('&nbsp;&nbsp;');
        }
        return '<img src="' + value.showImageUrl + '" alt="' + value.blobName + '" height="42" />';
    },
};

/*
 * 对easyui-validatebox的验证类型的扩展
 */
$.extend($.fn.validatebox.defaults.rules, {
    //select空值验证
    selectNotNull: {
        validator: function (value, param) {
            //console.info(value);
            return $(param[0]).find("option:contains('" + value + "')").val() !== '';
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
            let flag = /\s/.test(value);
            //过滤特殊字符串
            let pattern = new RegExp("[`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）——|{}【】’‘《》；：”“'。，、？]");
            let specialFlag = pattern.test(value);
            return !flag && !specialFlag;
        },
        message: "非法字符，请重新输入"
    },

    //身份证
    IDCard: {
        validator: function (value, param) {
            if (value === undefined || value == null || value === "")
                return false;
            //需要引用：jquery.extension.core.js（针对String对象的扩展）
            let flag = String.isIDCard(value);
            return flag === true;
        },
        message: "请输入正确的身份证号码"
    },
    //统一社会信用编码
    SocialCreditCode: {
        validator: function (value, param) {
            if (value === undefined || value == null || value === "")
                return false;
            //需要引用：jquery.extension.core.js（针对String对象的扩展）
            let flag = String.isSocialCreditCode(value);
            return flag === true;
        },
        message: "请输入正确的统一社会信用编码"
    },
    //营业执照代码
    BusinessCode: {
        validator: function (value, param) {
            if (value === undefined || value == null || value === "")
                return false;
            //需要引用：jquery.extension.core.js（针对String对象的扩展）
            let flag = String.isBusinessCode(value);
            return flag === true;
        },
        message: "请输入正确的营业执照代码"
    },
    //组织机构代码
    OrganizationCode: {
        validator: function (value, param) {
            if (value === undefined || value == null || value === "")
                return false;
            //需要引用：jquery.extension.core.js（针对String对象的扩展）
            let flag = String.isOrganizationCode(value);
            return flag === true;
        },
        message: "请输入正确的组织机构代码"
    },
    //纳税人识别号
    TaxpayerCode: {
        validator: function (value, param) {
            if (value === undefined || value == null || value === "")
                return false;
            //需要引用：jquery.extension.core.js（针对String对象的扩展）
            let flag = String.isTaxpayerCode(value);
            return flag === true;
        },
        message: "请输入正确的纳税人识别号"
    },

    mdStartDate: {
        validator: function (value, param) {
            let startTime2 = $(param[0]).datetimebox('getValue');
            let d1 = $.fn.datebox.defaults.parser(startTime2);
            let d2 = $.fn.datebox.defaults.parser(value);
            varify = d2 <= d1;
            return varify;
        },
        message: '开始时间必须小于或等于结束时间！'
    },
    //比较日期选择器
    compareDate: {
        validator: function (value, param) {
            let startTime = $(param[0]).val();
            if (startTime === undefined || startTime === null || startTime === '')
                startTime = $(param[0]).date("getValue");
            if (startTime === undefined || startTime === null || startTime === '')
                startTime = $(param[0]).datetimebox("getValue");
            return CompareUtil.dateCompare(startTime, value);
        },
        message: "结束日期不能小于或等于开始日期"
    },
    //比较时间选择器（时分秒）
    compareTime: {
        validator: function (value, param) {
            let startTime = $(param[0]).val();
            if (startTime === undefined || startTime === null || startTime === '')
                startTime = $(param[0]).timespinner("getValue");
            return CompareUtil.timeCompare(startTime, value);
        },
        message: "结束时间不能小于或等于开始时间"
    },
    //比较日期时间选择器（时分秒）
    compareDateTime: {
        validator: function (value, param) {
            let startTime = $(param[0]).val();
            if (startTime === undefined || startTime === null || startTime === '')
                startTime = $(param[0]).timespinner("getValue");
            return CompareUtil.dateTimeCompare(startTime, value);
        },
        message: "结束时间不能小于或等于开始时间"
    },
    //验证是否包含空格和非法字符
    unnormal: {
        validator: function (value) {
            return /^[a-zA-Z0-9]/i.test(value);

        },
        message: '输入值不能为空和包含其他非法字符'
    },
    //验证名称是否存在
    existName: {
        validator: function (value, param) {
            let flag = true;
            let url = param[0];
            let postData = {};
            postData['name'] = value;
            postData['id'] = param[1];
            postData['isEditMode'] = param[2];
            postData['orginalName'] = param[3];
            $.ajax({
                async: false,
                type: 'POST',
                dataType: 'json',
                url: url,
                data: postData,
                success: function (result) {
                    flag = !result;
                }
            });
            return flag;
        },
        message: '{4}'
    },
    remote: {
        validator: function (value, param) {
            let flag = true;
            let postData = {};
            postData[param[1]] = value;
            $.ajax({
                async: false,
                type: 'POST',
                dataType: 'json',
                timeout: 2000,
                url: param[0],
                data: postData,
                success: function (result) {
                    //debugger;
                    if (result.toString() === "true") {
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
            let flag = true;
            let postData = {};
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

/**
 * 获取选中的option对象
 */
$.extend($.fn.combobox.methods, {
    getSelectRow: function (jq) {
        let state = $.data(jq[0], 'combobox');
        let opts = state.options;
        let data = state.data;
        let selected = $(jq[0]).combobox('getValue');
        for (let i = 0; i < data.length; i++) {
            let dataValue = data[i][opts.valueField].toString();
            if (dataValue === selected) {
                return data[i];
            }
        }
    }
});

/*
 * 对easyui-treegrid的树表格类型的扩展
 */
$.extend($.fn.treegrid.methods, {
    //isContains是否包含父节点（即子节点被选中时是否也取父节点）
    getAllChecked: function (jq, isContains) {
        let keyValues = [];
        /*
          tree-checkbox2 有子节点被选中的css
          tree-checkbox1 节点被选中的css
          tree-checkbox0 节点未选中的css
        */
        let checkNodes = jq.treegrid("getPanel").find(".tree-checkbox1");
        for (let i = 0; i < checkNodes.length; i++) {
            let keyValue1 = $($(checkNodes[i]).closest('tr')[0]).attr("node-id");
            keyValues.push(keyValue1);
        }

        if (isContains) {
            let childCheckNodes = jq.treegrid("getPanel").find(".tree-checkbox2");
            for (let i = 0; i < childCheckNodes.length; i++) {
                let keyValue2 = $($(childCheckNodes[i]).closest('tr')[0]).attr("node-id");
                keyValues.push(keyValue2);
            }
        }

        return keyValues;
    }
});

/*
 * 对easyui-datagrid的表格类型的扩展
 */
$.extend($.fn.datagrid.methods, {
    /*
    *  datagrid 获取正在编辑的Editor组件的行索引，使用如下：
    *  $('#id').datagrid('getCurrentEditorRowIndex', this); //获取当前datagrid中正在编辑的Editor组件的行索引
    */
    getCurrentEditorRowIndex: function (jq, editorComp) {
        let dgEditTR = $(editorComp).parentsUntil(".datagrid-row-editing")[5];
        let currentEditRowIndex = $(dgEditTR).parent().attr("datagrid-row-index");
        return currentEditRowIndex;
    },
    /*
    *  datagrid 获取正在编辑状态的行索引列表，使用如下：
    *  $('#id').datagrid('getEditingRowIndexs'); //获取当前datagrid中在编辑状态的行编号列表
    */
    getEditingRowIndexs: function (jq) {
        let rows = $.data(jq[0], "datagrid").panel.find('.datagrid-row-editing');
        let indexes = [];
        rows.each(function (i, row) {
            let index = row.sectionRowIndex;
            if (indexes.indexOf(index) === -1) {
                indexes.push(index);
            }
        });
        return indexes;
    },
    /*
    *  datagrid 根据列的Filed的值，获取当前列的名称，使用如下：
    *  $('#id').datagrid('getColumnTitleByField', 'id'); //获取当前datagrid中获取当前列的名称
    */
    getColumnTitleByField: function (jq, field) {
        let columns = $(jq).datagrid('getColumnFields');
        let result = '';
        for (let i in columns) {
            //获取每一列的列名对象
            if (columns.hasOwnProperty(i)
                && typeof columns[i] != "function") {
                let col = $(jq).datagrid("getColumnOption", columns[i]);
                if (col.field === field)
                    result = col.title;
            }
        }

        return result;
    },
    /*
    *  datagrid 根据所有列的属性值列表，使用如下：
    *  $('#id').datagrid('getAllColumnProperties');
    * 结果：[{filed:'',title:''},{filed:'',title:''},...]
    */
    getAllColumnProperties: function () {
        let columns = $(dgProductId).datagrid('getColumnFields');
        let properties = [];
        for (let i in columns) {
            let property = {};
            //获取每一列的列名对象
            if (columns.hasOwnProperty(i)
                && typeof columns[i] != "function"
                && columns[i].startsWith(dynamicColumnId)) {
                let col = $(dgProductId).datagrid("getColumnOption", columns[i]);
                property.field = col.field;
                property.title = col.title;
                //追加对象
                properties.push(property);
            }
        }

        return properties;
    },
    /*
    *  datagrid 下添加编辑器：
    *  $('#id').datagrid('addEditor', column); //获取当前datagrid中在编辑状态的行编号列表
    */
    addEditor: function (jq, param) {
        if (param instanceof Array) {
            $.each(param, function (index, item) {
                let e = $(jq).datagrid('getColumnOption', item.field);
                e.editor = item.editor;
            });
        } else {
            let e = $(jq).datagrid('getColumnOption', param.field);
            e.editor = param.editor;
        }
    },
    /*
    *  datagrid 下删除编辑器：
    *  $('#id').datagrid('removeEditor', column); //获取当前datagrid中在编辑状态的行编号列表
    */
    removeEditor: function (jq, param) {
        if (param instanceof Array) {
            $.each(param, function (index, item) {
                let e = $(jq).datagrid('getColumnOption', item);
                e.editor = {};
            });
        } else {
            let e = $(jq).datagrid('getColumnOption', param);
            e.editor = {};
        }
    }
});

/*
 * 对easyui-datagrid的editor的扩展
 */
$.extend($.fn.datagrid.defaults.editors, {
    datebox: {
        index: 0,
        init: function (container, options) {
            let name = 'datebox_' + this.index++;
            let input = $('<input name="' + name + '" type="text">').appendTo(container);
            input.datebox(options);
            return input;
        },
        destroy: function (target) {
            $(target).datebox('destroy');
        },
        getValue: function (target) {
            let oldValue = $(target).datebox('getValue');//获得旧值
            return oldValue;
        },
        setValue: function (target, value) {
            if ("99999999" === value) {
                $(target).datebox('setValue', "2099-12-31");//设置新值的日期格式
            } else {
                $(target).datebox('setValue', value);//设置新值的日期格式
            }

        },
        resize: function (target, width) {
            $(target).datebox('resize', width);
        }
    },
    /*
    * RadionButton编辑器，使用示例如下：
        editor:{
            type:'radioGroup',
            options:{
                value: '0',
                labelPosition: 'after',
                labelWidth: 40,
                checked: true,
                items:[
                    {value:'0',text:'面议'},
                    {value:'1',text:'单价'}
                ]
            }
        }
    * */
    radioGroup: {
        index: 0,
        init: function (container, options) {
            let span = $('<span></span>').appendTo(container);
            let name = 'radio_' + this.index++;
            $.map(options.items || [], function (item) {
                let checked = item.value === options.value ? 'checked' : '';
                let input = null;
                if (checked)
                    input = $('<input data-options="checked:true," class="easyui-radiobutton" name="' + name + '" value="' + item.value + '" label="' + item.text + '"/>').appendTo(span);
                else
                    input = $('<input class="easyui-radiobutton" name="' + name + '" value="' + item.value + '" label="' + item.text + '"/>').appendTo(span);
                input.radiobutton(options);
            });
            return span;
        },
        destroy: function (target) {
            $(target).remove();
        },
        getValue: function (target) {
            return $(target).find('input:checked').val();
        },
        setValue: function (target, value) {
            $(target).find('input[value=' + value + ']')._propAttr('checked', true);
        },
        resize: function (target, width) {

        }
    },
    /*
    * ImageUploader编辑器，使用示例如下：
        field: 'productImageBlobs',
        title: '图片【格式：[[${uploadConfig.imageExt}]]，大小：[[${uploadConfig.imageMaxSize}]]M】',
        width: 360,
        align: 'left',
        formatter: function (value, row, index) {
            //debugger;
            let tdContext = '';
            if (value !== undefined && value != null && value.length > 0) {
                tdContext = '<div class="webUploader-image-list showImage">';
                $.each(value, function (i, blob) {
                    blob.imageUrl = '/Home/ShowTempImage?id=' + blob.id;
                    tdContext += imageUploader.getImageListHtmlView(blob, false, false);
                });
                tdContext += "</div>";
            }
                            return tdContext;
       },
       editor: {
            type: 'imageUploader',
            options: {
                //设置预览图片的标签：<p id="imagePreviewContainer" class="imagePreview"></p>
                imagePreviewContainer: $('#imagePreviewContainer'),
                fileNumLimit: imageNumLimit,
                configure: {
                    imageMaxSize: imageSize,
                    imageExt: imageExt,
                    fileMaxSize: fileMaxSize,
                    fileExt: fileExt
                },
                callback: {
                    onRemoveImage: function (file) {
                        //新上传未保存至数据库的图片，未生成propertyId
                        if (file.propertyId !== undefined && file.propertyId != null && file.propertyId !== '')
                            removeProductPropertyIds.push(file.propertyId);

                        return true;
                    }
               }
            }
        }
    * */
    imageUploader: {
        index: 0,
        dgImageUploader: {},
        init: function (container, options) {
            //debugger;
            let idx = '-' + this.index++;
            let uploaderId = "imageUploader" + idx;
            let divUploader = '<div class="imageUploaderEditor" id="' + uploaderId + '">' +
                '<div>' +
                '   <div style="display: flex;height: 130px;">' +
                '       <div id="btnAddImageFile' + idx + '">' +
                '           <a href="javascript:void(0)"><i class="fa fa-plus-square fa-5x"></i></a>' +
                '       </div>' +
                '       <div id="imageFileList' + idx + '" style="width:280px;" class="webUploader-image-list"></div>' +
                '   </div>' +
                '</div>' +
                '</div>';

            let input = $(divUploader).appendTo(container);

            let settings = {
                componentName: uploaderId,
                btnAddFile: '#btnAddImageFile' + idx,
                fileList: '#imageFileList' + idx,
                //params: {ids: options.ids},
                isRegister: true,
                isSetDefault: true,
                isEditor: true,
                callback: {
                    uploadStart: function (file) {
                    },
                    uploadProgress: function (file, percentage) {
                    },
                    uploadComplete: function (file) { //不管成功或者失败，文件上传完成时触发
                    },
                    uploadSuccess: function (file, response, blob) {
                    },
                    uploadError: function (file, reason) {
                        let fileName = file.name;
                        console.error("---file[" + fileName + "] throw error: " + reason);
                    },
                    onFileQueued: function (file) {
                    }
                }
            };
            $.extend(true, settings, options);
            this.dgImageUploader[uploaderId] = imageUploader(settings);

            return input;
        },
        destroy: function (target) {
            $(target).remove();
        },
        getValue: function (target) {
            //debugger;
            let blobs = [];
            let txtBlobs = $(target).find('input.imageBlobId');
            if (txtBlobs !== undefined && txtBlobs != null && txtBlobs.length > 0) {
                $.each(txtBlobs, function (i, txtBlob) {
                    let isSelect = false;
                    let blobId = $(txtBlob).val();
                    let blobName = $(txtBlob).attr('title');
                    let ext = $(txtBlob).attr('ext');
                    let size = $(txtBlob).attr('size');
                    let propertyId = $(txtBlob).attr('propertyId');
                    let checked = $(txtBlob).attr('checked');
                    if (checked !== undefined
                        && checked != null
                        && checked !== '') {
                        isSelect = true;
                    }
                    let blob = {};
                    blob.blobId = blobId;
                    blob.blobName = blobName;
                    blob.ext = ext;
                    blob.size = size;
                    blob.propertyId = propertyId;
                    blob.isSelect = isSelect;
                    blobs.push(blob);
                });
            }
            return blobs;
        },
        setValue: function (target, value) {
            if (value !== undefined && value != null && value.length > 0) {
                //debugger;
                let uploaderId = target[0].id;
                let uploader = this.dgImageUploader[uploaderId];
                uploader.initImageView(value);
            }
        },
        resize: function (target, width) {
            //$(target).numberspinner('resize',width);
        }
    },
    /*
     * FileUploader文件上传编辑器，使用示例如下：
    //上传的前端div容器
    //fileNumLimit设置为：1，btnAddFile设置为：上传按钮的div容器id筛选器，fileList设置为：文件展示的div容器id筛选器
    field: 'productFileBlobs',
    title: '技术文档【格式：[[${uploadConfig.fileExt}]]，大小：[[${uploadConfig.fileMaxSize}]]M】',
    width: 360,
    align: 'left',
    formatter: function (value, row, index) {
        //debugger;
        let tdContext = '';
        if (value !== undefined && value != null) {
            //单文件
            //value.imageUrl = '/Home/ShowTempImage?id=' + value.blobId;
            //tdContext += fileUploader.getFileListHtmlView(value, false, true);
            //多文件
            tdContext = '<div class="webUploader-file-list">';
            $.each(value, function (i, blob) {
                blob.imageUrl = '/Home/ShowTempImage?id=' + blob.id;
                tdContext += fileUploader.getFileListHtmlView(blob, false, false);
            });
            tdContext += "</div>";
        }
                        return tdContext;
    },
    editor: {
        type: 'fileUploader',
        options: {
            fileNumLimit: 3,
            configure: {
                imageMaxSize: imageSize,
                imageExt: imageExt,
                fileMaxSize: fileMaxSize,
                fileExt: fileExt
            },
            callback: {
                onRemoveImage: function (file) {
                    //新上传未保存至数据库的图片，未生成propertyId
                    if (file.propertyId !== undefined && file.propertyId != null && file.propertyId !== '')
                        removeProductPropertyIds.push(file.propertyId);

                    return true;
                }
            }
        }
    }
    * */
    fileUploader: {
        index: 0,
        isSingleFile: false,
        dgFileUploader: {},
        init: function (container, options) {
            //debugger;
            let idx = '-' + this.index++;
            let uploaderId = "fileUploader" + idx;
            this.isSingleFile = options.fileNumLimit !== undefined && options.fileNumLimit != null
                ? options.fileNumLimit === 1
                : false;
            let divUploader = '<div class="fileUploaderEditor" id="' + uploaderId + '">' +
                '<div>' +
                '   <div style="display: flex;height: 130px;">' +
                '       <div id="btnAddDocFile' + idx + '">' +
                '           <a href="javascript:void(0)"><i class="fa fa-plus-square fa-5x"></i></a>' +
                '       </div>' +
                '       <div id="docFileList' + idx + '" style="width:280px;" class="webUploader-file-list"></div>' +
                '   </div>' +
                '</div>' +
                '</div>';
            if (this.isSingleFile) {
                divUploader = '<div class="fileUploaderEditor" style="display: flex;" id="' + uploaderId + '">' +
                    '<div id="docFileList' + idx + '" class="webUploader-file-single"></div>' +
                    '<a id="btnAddDocFile' + idx + '" href="javascript:void(0)" class="easyui-linkbutton btnSelect" iconcls="fa fa-pencil">选择</a>' +
                    '</div>';
            }

            let input = $(divUploader).appendTo(container);

            $(".btnSelect").linkbutton({ iconCls: 'fa fa-pencil' });    //日志
            let settings = {
                componentName: uploaderId,
                btnAddFile: '#btnAddDocFile' + idx,
                fileList: '#docFileList' + idx,
                //params: {ids: options.ids},
                isRegister: true,
                isEditor: true,
                callback: {
                    uploadStart: function (file) {
                    },
                    uploadProgress: function (file, percentage) {
                    },
                    uploadComplete: function (file) { //不管成功或者失败，文件上传完成时触发
                    },
                    uploadSuccess: function (file, response, blob) {
                    },
                    uploadError: function (file, reason) {
                        let fileName = file.name;
                        console.error("---file[" + fileName + "] throw error: " + reason);
                    },
                    onFileQueued: function (file) {
                    }
                }
            };
            $.extend(true, settings, options);
            this.dgFileUploader[uploaderId] = fileUploader(settings);

            return input;
        },
        destroy: function (target) {
            $(target).remove();
        },
        getValue: function (target) {
            //debugger;
            let blobs = [];
            let txtBlobs = $(target).find('input.fileBlobId');
            if (txtBlobs !== undefined && txtBlobs != null && txtBlobs.length > 0) {
                $.each(txtBlobs, function (i, txtBlob) {
                    let isSelect = false;
                    let blobId = $(txtBlob).val();
                    let blobName = $(txtBlob).attr('title');
                    let ext = $(txtBlob).attr('ext');
                    let size = $(txtBlob).attr('size');
                    let propertyId = $(txtBlob).attr('propertyId');
                    let checked = $(txtBlob).attr('checked');
                    if (checked !== undefined
                        && checked != null
                        && checked !== '') {
                        isSelect = true;
                    }
                    let blob = {};
                    blob.blobId = blobId;
                    blob.blobName = blobName;
                    blob.ext = ext;
                    blob.size = size;
                    blob.propertyId = propertyId;
                    blob.isSelect = isSelect;
                    blobs.push(blob);
                });
            }

            if (this.isSingleFile)
                return blobs.length === 0 ? null : blobs[0];

            return blobs;
        },
        setValue: function (target, value) {
            if (value !== undefined && value != null) {
                //debugger;
                let uploaderId = target[0].id;
                let uploader = this.dgFileUploader[uploaderId];
                if (uploader !== undefined && uploader != null)
                    uploader.initFileView(value);
            }
        },
        resize: function (target, width) {
            //$(target).numberspinner('resize',width);
        }
    },
    /*
     * 产品规格编辑器，使用示例如下：
       let properties = item.serviceProviderAttrs;
       formatter: function (value, row, index) {
            let tdContext = '';
            if (value === undefined || value == null || value === '') {
                tdContext += '面议';
            } else {
                tdContext += '单价：' + value;
            }
            return tdContext;
        },
       editor: {
            type: 'specCombobox',
            options: {
                data: properties,
                width: 138,
                valueField: "id",
                textField: "name",
                panelHeight: "auto",
                required: true
            }
        }
     */
    specCombobox: {
        index: 0,
        init: function (container, options) {
            let idx = '-' + this.index++;
            let divSpec = $('<div id="spec' + idx + '"></div>').appendTo(container);
            let divHiddenSpec = '<input type="hidden" class="propertyId" id="txtSpec' + idx + '" />';
            options.onChange = function (newValue, oldValue) {
                if (newValue !== undefined
                    && newValue != null
                    && newValue !== ''
                    && newValue !== oldValue) {
                    //debugger;
                    let serviceProviderAttr = $(this).combobox('getSelectRow');
                    let propertyAttrId = $(this).combobox('getValue');
                    let propertyAttrName = $(this).combobox('getText');

                    let hiddenProperty = $(this).find('input.propertyId');
                    hiddenProperty.attr('providerId', serviceProviderAttr.serviceProviderId);
                    hiddenProperty.attr('providerName', serviceProviderAttr.serviceProviderName);
                    hiddenProperty.attr('propertyAttrId', serviceProviderAttr.id);
                    hiddenProperty.attr('propertyName', serviceProviderAttr.name);
                }
            };

            $(divSpec).combobox(options);
            $(divHiddenSpec).appendTo(divSpec);

            return divSpec;
        },
        destroy: function (target) {
            $(target).remove();
        },
        getValue: function (target) {
            //debugger;
            let hiddenProperty = $(target).find('input.propertyId');
            if (hiddenProperty === undefined || hiddenProperty == null)
                return null;

            let propertyAttrId = $(target).combobox('getValue');
            let propertyAttrName = $(target).combobox('getText');

            let property = {};
            property.id = hiddenProperty.val();
            property.name = hiddenProperty.attr('propertyName');
            property.refProviderId = hiddenProperty.attr('providerId');
            property.refProviderAttrId = propertyAttrId;
            property.value = propertyAttrName;
            return property;
        },
        setValue: function (target, productProperty) {
            //debugger;
            let specCombobox = $(target);
            let hiddenProperty = $(target).find('input.propertyId');
            //由于是动态列，后台返回对象未包括动态列的字段，前端不可能获取到所绑定的对象，故以下代码无用
            if (productProperty !== undefined && productProperty != null) {
                let propertyId = productProperty.id;
                let propertyName = productProperty.name;
                let propertyValue = productProperty.value;
                let refProviderId = productProperty.refProviderId;
                let refProviderAttrId = productProperty.refProviderAttrId;
                specCombobox.combobox('select', refProviderAttrId);
                hiddenProperty.val(propertyId);
                hiddenProperty.attr('providerId', refProviderId);
                hiddenProperty.attr('propertyName', propertyName);
            }
        },
        resize: function (target, width) {

        }
    },
    /*
     * 产品价格编辑器，使用示例如下：
       formatter: function (value, row, index) {
            let tdContext = '';
            if (value === undefined || value == null || value === '') {
                tdContext += '面议';
            } else {
                tdContext += '单价：' + value;
            }
            return tdContext;
        },
       editor: {
            type: 'priceGroup',
            options: {
                value: '',
                labelPosition: 'after',
            }
        }
     */
    priceGroup: {
        index: 0,
        init: function (container, options) {
            //debugger;
            let idx = '-' + this.index++;
            let divPrice = $('<div id="priceGroup' + idx + '"></div>').appendTo(container);
            let divPrice1 = '<input class="rdoProductPrice"  id="rdoProductPrice' + idx + '-0" name="productPrice' + idx + '" data-options="labelPosition:\'after\',labelWidth:140,checked:true," class="easyui-radiobutton" value="0" label="面议">';
            let input = $(divPrice1).appendTo(divPrice);
            input.radiobutton();
            let divPrice2 = '<input class="rdoProductPrice" id="rdoProductPrice' + idx + '-1" name="productPrice' + idx + '" data-options="labelPosition:\'after\',labelWidth:40," class="easyui-radiobutton" value="1" label="单价">';
            input = $(divPrice2).appendTo(divPrice);
            input.radiobutton();
            let divPrice3 = '<input class="txtProductPrice" id="txtProductPrice' + idx + '" data-options="width:100,min:0,precision:2"  class="easyui-numberspinner"/>';
            input = $(divPrice3).appendTo(divPrice);
            input.numberspinner();

            return divPrice;
        },
        destroy: function (target) {
            $(target).remove();
        },
        getValue: function (target) {
            //debugger;
            let priceSelect = $(target).find('span.radiobutton.radiobutton-checked input.radiobutton-value').val();
            //let priceSelect = $(target).find('input:checked').val();
            if (priceSelect === "0") {
                return null;
            } else {
                let price = $(target).find('input.txtProductPrice').val();
                if (price === undefined || price == null || price === '')
                    return 0;
                else
                    return price;
            }
        },
        setValue: function (target, value) {
            let radio1 = null;
            let radio2 = null;
            let radios = $(target).find('.rdoProductPrice');
            let price = $(target).find('.txtProductPrice');
            $.each(radios, function (i, radio) {
                let rdoValue = $(radio).val();
                if (rdoValue === '0') {
                    radio1 = radio;
                } else {
                    radio2 = radio;
                }
            });
            if (value === undefined || value == null || value === '') {
                $(radio1).radiobutton('check');
            } else {
                $(radio2).radiobutton('check');
                price.numberspinner('setValue', value);
            }
        },
        resize: function (target, width) {

        }
    },
    /*
     * 颜色选择器编辑器，使用示例如下：
       editor: {
            type: 'colorPicker',
            options: {
                value: '',
            }
        }
     */
    colorPicker: {//colorpicker就是你要自定义editor的名称
        init: function (container, options) {
            // var input = $('<input class="easyui-color">').appendTo(container);
            var input = $('<input>').appendTo(container);

            input.colorpicker({
                color: '#0000ff',
                onShow: function (colpkr) {
                    $(colpkr).fadeIn(500);
                    return false;
                },
                onHide: function (colpkr) {
                    $(colpkr).fadeOut(500);
                    return false;
                },
                onChange: function (hsb, hex, rgb) {
                    //$('#colorSelector div').css('backgroundColor', '#' + hex);
                    input.css('backgroundColor', '#' + hex);
                    input.val('#' + hex);
                }
            });
            return input;
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).val(value);
            $(target).css('backgroundColor', value);
            $(target).ColorPickerSetColor(value);
        },
        resize: function (target, width) {
            var input = $(target);
            if ($.boxModel == true) {
                input.width(width - (input.outerWidth() - input.width()));
            } else {
                input.width(width);
            }
        }
    },
    /*
     * 颜色选择器编辑器，使用示例如下：
       editor: {
            type: 'iconPicker',
            options: {
                value: '',
            }
        }
     */
    iconPicker: {
        init: function (container, options) {
            // var input = $('<input class="easyui-iconpicker">').appendTo(container);
            var input = $('<input>').appendTo(container);

            input.iconpicker({
                icon: 'fa-address-book',
                onShow: function (colpkr) {
                    $(colpkr).fadeIn(500);
                    return false;
                },
                onHide: function (colpkr) {
                    $(colpkr).fadeOut(500);
                    return false;
                },
                onChange: function (hsb, hex, rgb) {
                    // $('#colorSelector div').css('backgroundColor', '#' + hex);
                    input.val(hex);
                }
            });
            return input;
        },
        getValue: function (target) {
            return $(target).val();
        },
        setValue: function (target, value) {
            $(target).iconpicker('setValue', value);
        },
        resize: function (target, width) {
            var input = $(target);
            if ($.boxModel == true) {
                input.width(width - (input.outerWidth() - input.width()));
            } else {
                input.width(width);
            }
        }
    },
});

/*
 * 对easyui-datagrid的view的扩展
 */
$.extend($.fn.datagrid.defaults.view, {
    /*
    * CommentView编辑器，使用示例如下：
        rowData:{
            publishUserId: 'userId',
            publishUserName: 'userDisplayName',
            publishDate: '2019-09-09',
            commentId: 'rowId',
            commentContent: 'comment',
            commentFileBlobInfo: {
                blobId: 'blobId'
                blobName: 'blobName'
                downloadFileUrl: 'download url'
            },
            commentImageBlobInfos: {
                blobId: 'blobId'
                blobName: 'blobName'
                showImageUrl: 'image url'
            },
            projectReplys: [{
                replyId: 1,
                replyUserId: 'replyUserId',
                replyUserName: 'replyUserName',
                replyDate: '2019-09-09',
                replyContent: 'reply content'
            },{
                replyId: 2,
                replyUserId: 'replyUserId',
                replyUserName: 'replyUserName',
                replyDate: '2019-09-09',
                replyContent: 'reply content'
            }]
        }
    * */
    CommentView: {
        renderRow: function (target, fields, frozen, rowIndex, rowData) {
            //debugger;
            let cc = [];
            cc.push('<td colspan=' + fields.length + ' style="padding:10px 5px;border:0;">');
            if (!frozen) {
                //用户信息
                cc.push('<div class="c-user">');
                cc.push('<div><img src="/images/user/user.jpg" height="70" width="70"></div>');
                cc.push('<div><span class="c-label">用户名:</span><span class="c-name">' + rowData.publishUserName + '</span></div>');
                cc.push('<div> 发布时间: ' + rowData.publishDate + '</div>');
                //系统管理员及发布评论者可以删除该条评论
                if (currentUserId == rowData.publishUserId || currentUserId == adminUserId) {
                    cc.push('<div> <a href="javascript:void(0)" id="Inquire" class="easyui-linkbutton l-btn l-btn-small" onclick="removeComment(' + rowData.commentId + ')" data-options="iconCls:"fa fa-trash"" ><span class="l-btn-left l-btn-icon-left"><span class="l-btn-text">删除</span><span class="l-btn-icon fa fa-trash">&nbsp;</span></span></a></div>');
                }
                cc.push('</div>');
                cc.push('<div>');

                //评论信息
                if (rowData.commentContent != null) {
                    cc.push('<div class="c-content" id="content_' + rowData.commentId + '"> ' + (rowIndex + 1) + '楼：' + rowData.commentContent + '</div>');
                } else {
                    cc.push('<div class="c-content" id="content_' + rowData.commentId + '"> ' + '</div>');
                }
                //附件
                let fileBlob = rowData.commentFileBlobInfo;
                if (fileBlob != undefined && fileBlob != null) {
                    cc.push('<div>附件：');
                    cc.push('<a target="_blank" href="' + fileBlob.downloadFileUrl + '" download="' + fileBlob.blobName + '">' + fileBlob.blobName + '</a>');
                    cc.push('</div>');
                }
                //图片
                let ss = '<div>';
                let imageBlobs = rowData.commentImageBlobInfos;
                if (imageBlobs != undefined && imageBlobs != null) {
                    for (let i = 0, len = imageBlobs.length; i < len; i++) {
                        ss += '<img style="margin-top:10px;margin-left:10px;" src="' + imageBlobs[i].blobId + '" height="100" width="100" >';
                    }
                }
                ss += '</div> ';
                cc.push(ss);

                //回复
                let reples = rowData.projectReplys;
                if (reples != undefined && reples != null && reples.length > 0) {
                    let rr = '<div class="c-reply">';
                    for (let i = 0, len = reples.length; i < len; i++) {
                        rr += '<div class="c-reply-content">';
                        rr += '<div class="content" id="replyid-' + reples[i].replyId + '">回复' + (i + 1) + '：' + reples[i].replyContent + '</div>';
                        rr += '<div class="user" id="userid-' + reples[i].replyUserId + '"><span  class="c-name">' + reples[i].replyUserName + '</span>&nbsp;&nbsp;回复于：' + reples[i].replyDate + '</div>';
                        //系统管理员及发布评论者可以删除该条回复
                        if (currentUserId == rowData.replyUserId || currentUserId == adminUserId) {
                            rr += '<div class="removebtn"><a href="javascript:void(0)" id="Inquire" class="easyui-linkbutton l-btn l-btn-small" onclick="removeReplay(' + rowData.replyId + ')" data-options="iconCls:\'fa fa-trash\'" ><span class="l-btn-left l-btn-icon-left"><span class="l-btn-text">删除</span><span class="l-btn-icon fa fa-trash">&nbsp;</span></span></a></div>';
                        }
                        rr += '</div>';
                    }
                    rr += '</div> ';
                    cc.push(rr);
                }
                cc.push('<div class="c-button"><input class="easyui-textbox" type="text" id="txtUserReplay-' + rowIndex + '" data-options="width:\'80%\'" />');
                cc.push('<a href="javascript:void(0)" id="Inquire" class="easyui-linkbutton l-btn l-btn-small" onclick="addReplay(\'txtUserReplay-' + rowIndex + '\',' + rowData.commentId + ')" data-options="iconCls:\'fa fa-trash\'" ><span class="l-btn-left l-btn-icon-left"><span class="l-btn-text">回复</span><span class="l-btn-icon fa fa-search">&nbsp;</span></span></a></div>')

                cc.push('</div>');
            }
            cc.push('</td>');
            return cc.join('');
        },
        onAfterRender: function (target) {
            var rows = $(target).datagrid('getRows');
            $.each(rows, function (index, row) {
                $('#txtUserReplay-' + index).textbox();
            });

        }
    },
    /*
    * CardView显示，使用示例如下：
        rowData:{
            id: 'rowId',
            name: 'title',
            createdDate: '2019-09-09',
            imageBlob: {
                showImageUrl: 'image url'
            }
        }
    * */
    cardview: {
        renderRow: function (target, fields, frozen, rowIndex, rowData) {
            let cc = [];
            cc.push('<td colspan=' + fields.length + ' style="padding:10px 5px;border:0;">');
            if (!frozen && rowData.id) {
                let showImageUrl = "images/image_no.jpg"
                if (rowData.imageBlob)
                    showImageUrl = rowData.imageBlob.showImageUrl;
                cc.push('<div class="c-user">');
                cc.push('<div><img src="' + showImageUrl + '" width="150"></div>');
                cc.push('<div><span class="c-label">推荐名称:</span><span class="c-name">' + rowData.name + '</span></div>');
                cc.push('<div> 发布时间: ' + rowData.createdDate + '</div>');
                cc.push('</div>');
            }
            cc.push('</td>');
            return cc.join('');
        }
    }
});

/*
 * 对EasyUI插件：colorpicker
 * 使用实例：<input class="easyui-colorpicker" data-options="cellWidth:20,cellHeight:20">
 */
(function ($) {
    $(function () {
        if (!$('#easyui-colorpicker-style').length) {
            $('head').append(
                '<style id="easyui-colorpicker-style">' +
                '.colorpicker-cell{display:inline-block;float:left;cursor:pointer;border:1px solid #fff}' +
                '.colorpicker-cell:hover{border:1px solid #000}' +
                '</style>'
            );
        }
    });

    function create(target) {
        var opts = $.data(target, 'colorpicker').options;
        $(target).combo($.extend({}, opts, {
            panelWidth: opts.cellWidth * 8 + 2,
            panelHeight: opts.cellHeight * 7 + 2,
            onShowPanel: function () {
                var p = $(this).combo('panel');
                if (p.is(':empty')) {
                    var colors = [
                        "0,0,0", "68,68,68", "102,102,102", "153,153,153", "204,204,204", "238,238,238", "243,243,243", "255,255,255",
                        "244,204,204", "252,229,205", "255,242,204", "217,234,211", "208,224,227", "207,226,243", "217,210,233", "234,209,220",
                        "234,153,153", "249,203,156", "255,229,153", "182,215,168", "162,196,201", "159,197,232", "180,167,214", "213,166,189",
                        "224,102,102", "246,178,107", "255,217,102", "147,196,125", "118,165,175", "111,168,220", "142,124,195", "194,123,160",
                        "204,0,0", "230,145,56", "241,194,50", "106,168,79", "69,129,142", "61,133,198", "103,78,167", "166,77,121",
                        "153,0,0", "180,95,6", "191,144,0", "56,118,29", "19,79,92", "11,83,148", "53,28,117", "116,27,71",
                        "102,0,0", "120,63,4", "127,96,0", "39,78,19", "12,52,61", "7,55,99", "32,18,77", "76,17,48"
                    ];
                    for (var i = 0; i < colors.length; i++) {
                        var a = $('<a class="colorpicker-cell"></a>').appendTo(p);
                        a.css('backgroundColor', 'rgb(' + colors[i] + ')');
                    }
                    var cells = p.find('.colorpicker-cell');
                    cells._outerWidth(opts.cellWidth)._outerHeight(opts.cellHeight);
                    cells.bind('click.colorpicker', function (e) {
                        var color = $(this).css('backgroundColor');
                        $(target).colorpicker('setValue', color);
                        $(target).combo('hidePanel');
                    });
                }
            }
        }));
        if (opts.value) {
            $(target).colorpicker('setValue', opts.value);
        }
    }

    $.fn.colorpicker = function (options, param) {
        if (typeof options == 'string') {
            var method = $.fn.colorpicker.methods[options];
            if (method) {
                return method(this, param);
            } else {
                return this.combo(options, param);
            }
        }
        options = options || {};
        return this.each(function () {
            var state = $.data(this, 'colorpicker');
            if (state) {
                $.extend(state.options, options);
            } else {
                state = $.data(this, 'colorpicker', {
                    options: $.extend({}, $.fn.colorpicker.defaults, $.fn.colorpicker.parseOptions(this), options)
                });
            }
            create(this);
        });
    };

    $.fn.colorpicker.methods = {
        options: function (jq) {
            return jq.data('colorpicker').options;
        },
        setValue: function (jq, value) {
            return jq.each(function () {
                var tb = $(this).combo('textbox').css('backgroundColor', value);
                value = tb.css('backgroundColor');
                if (value.indexOf('rgb') >= 0) {
                    var bg = value.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
                    value = '#' + hex(bg[1]) + hex(bg[2]) + hex(bg[3]);
                }
                $(this).combo('setValue', value).combo('setText', value);

                function hex(x) {
                    return ('0' + parseInt(x).toString(16)).slice(-2);
                }
            })
        },
        clear: function (jq) {
            return jq.each(function () {
                $(this).combo('clear');
                $(this).combo('textbox').css('backgroundColor', '');
            });
        }
    };

    $.fn.colorpicker.parseOptions = function (target) {
        return $.extend({}, $.fn.combo.parseOptions(target), {

        });
    };

    $.fn.colorpicker.defaults = $.extend({}, $.fn.combo.defaults, {
        editable: false,
        cellWidth: 20,
        cellHeight: 20
    });

    $.parser.plugins.push('colorpicker');
})(jQuery);

/*
 * 对EasyUI插件：iconpicker
 * 使用实例：<input class="easyui-iconpicker" data-options="lgTimes:3,cellWidth:40,cellHeight:40,">
 */
(function ($) {
    $(function () {
        if (!$('#easyui-iconpicker-style').length) {
            $('head').append(
                '<style id="easyui-iconpicker-style">' +
                '.iconpicker-cell{display:inline-block;float:left;cursor:pointer;border:1px solid #fff;text-align:center;vertical-align:middle;padding:6px 0;}' +
                '.iconpicker-cell:hover{border:1px solid #000;text-align:center}' +
                '</style>'
            );
        }
    });

    function create(target) {
        var opts = $.data(target, 'iconpicker').options;
        $(target).combobox($.extend({}, opts, {
            panelWidth: opts.cellWidth * 14 + 2,
            panelHeight: opts.cellHeight * 14 + 2,
            onShowPanel: function () {
                var p = $(this).combobox('panel');
                if (p.is(':empty')) {
                    var icons = [{ "value": "fa-address-book" }, { "value": "fa-address-book-o" }, { "value": "fa-address-card" }, { "value": "fa-address-card-o" }, { "value": "fa-bandcamp" }, { "value": "fa-bath" }, { "value": "fa-bathtub" }, { "value": "fa-drivers-license" }, { "value": "fa-drivers-license-o" }, { "value": "fa-eercast" }, { "value": "fa-envelope-open" }, { "value": "fa-envelope-open-o" }, { "value": "fa-etsy" }, { "value": "fa-free-code-camp" }, { "value": "fa-grav" }, { "value": "fa-handshake-o" }, { "value": "fa-id-badge" }, { "value": "fa-id-card" }, { "value": "fa-id-card-o" }, { "value": "fa-imdb" }, { "value": "fa-linode" }, { "value": "fa-meetup" }, { "value": "fa-microchip" }, { "value": "fa-podcast" }, { "value": "fa-quora" }, { "value": "fa-ravelry" }, { "value": "fa-s15" }, { "value": "fa-shower" }, { "value": "fa-snowflake-o" }, { "value": "fa-superpowers" }, { "value": "fa-telegram" }, { "value": "fa-thermometer" }, { "value": "fa-thermometer-0" }, { "value": "fa-thermometer-1" }, { "value": "fa-thermometer-2" }, { "value": "fa-thermometer-3" }, { "value": "fa-thermometer-4" }, { "value": "fa-thermometer-empty" }, { "value": "fa-thermometer-full" }, { "value": "fa-thermometer-half" }, { "value": "fa-thermometer-quarter" }, { "value": "fa-thermometer-three-quarters" }, { "value": "fa-times-rectangle" }, { "value": "fa-times-rectangle-o" }, { "value": "fa-user-circle" }, { "value": "fa-user-circle-o" }, { "value": "fa-user-o" }, { "value": "fa-vcard" }, { "value": "fa-vcard-o" }, { "value": "fa-window-close" }, { "value": "fa-window-close-o" }, { "value": "fa-window-maximize" }, { "value": "fa-window-minimize" }, { "value": "fa-window-restore" }, { "value": "fa-wpexplorer" }, { "value": "fa-address-book" }, { "value": "fa-address-book-o" }, { "value": "fa-address-card" }, { "value": "fa-address-card-o" }, { "value": "fa-adjust" }, { "value": "fa-american-sign-language-interpreting" }, { "value": "fa-anchor" }, { "value": "fa-archive" }, { "value": "fa-area-chart" }, { "value": "fa-arrows" }, { "value": "fa-arrows-h" }, { "value": "fa-arrows-v" }, { "value": "fa-asl-interpreting" }, { "value": "fa-assistive-listening-systems" }, { "value": "fa-asterisk" }, { "value": "fa-at" }, { "value": "fa-audio-description" }, { "value": "fa-automobile" }, { "value": "fa-balance-scale" }, { "value": "fa-ban" }, { "value": "fa-bank" }, { "value": "fa-bar-chart" }, { "value": "fa-bar-chart-o" }, { "value": "fa-barcode" }, { "value": "fa-bars" }, { "value": "fa-bath" }, { "value": "fa-bathtub" }, { "value": "fa-battery" }, { "value": "fa-battery-0" }, { "value": "fa-battery-1" }, { "value": "fa-battery-2" }, { "value": "fa-battery-3" }, { "value": "fa-battery-4" }, { "value": "fa-battery-empty" }, { "value": "fa-battery-full" }, { "value": "fa-battery-half" }, { "value": "fa-battery-quarter" }, { "value": "fa-battery-three-quarters" }, { "value": "fa-bed" }, { "value": "fa-beer" }, { "value": "fa-bell" }, { "value": "fa-bell-o" }, { "value": "fa-bell-slash" }, { "value": "fa-bell-slash-o" }, { "value": "fa-bicycle" }, { "value": "fa-binoculars" }, { "value": "fa-birthday-cake" }, { "value": "fa-blind" }, { "value": "fa-bluetooth" }, { "value": "fa-bluetooth-b" }, { "value": "fa-bolt" }, { "value": "fa-bomb" }, { "value": "fa-book" }, { "value": "fa-bookmark" }, { "value": "fa-bookmark-o" }, { "value": "fa-braille" }, { "value": "fa-briefcase" }, { "value": "fa-bug" }, { "value": "fa-building" }, { "value": "fa-building-o" }, { "value": "fa-bullhorn" }, { "value": "fa-bullseye" }, { "value": "fa-bus" }, { "value": "fa-cab" }, { "value": "fa-calculator" }, { "value": "fa-calendar" }, { "value": "fa-calendar-check-o" }, { "value": "fa-calendar-minus-o" }, { "value": "fa-calendar-o" }, { "value": "fa-calendar-plus-o" }, { "value": "fa-calendar-times-o" }, { "value": "fa-camera" }, { "value": "fa-camera-retro" }, { "value": "fa-car" }, { "value": "fa-caret-square-o-down" }, { "value": "fa-caret-square-o-left" }, { "value": "fa-caret-square-o-right" }, { "value": "fa-caret-square-o-up" }, { "value": "fa-cart-arrow-down" }, { "value": "fa-cart-plus" }, { "value": "fa-cc" }, { "value": "fa-certificate" }, { "value": "fa-check" }, { "value": "fa-check-circle" }, { "value": "fa-check-circle-o" }, { "value": "fa-check-square" }, { "value": "fa-check-square-o" }, { "value": "fa-child" }, { "value": "fa-circle" }, { "value": "fa-circle-o" }, { "value": "fa-circle-o-notch" }, { "value": "fa-circle-thin" }, { "value": "fa-clock-o" }, { "value": "fa-clone" }, { "value": "fa-close" }, { "value": "fa-cloud" }, { "value": "fa-cloud-download" }, { "value": "fa-cloud-upload" }, { "value": "fa-code" }, { "value": "fa-code-fork" }, { "value": "fa-coffee" }, { "value": "fa-cog" }, { "value": "fa-cogs" }, { "value": "fa-comment" }, { "value": "fa-comment-o" }, { "value": "fa-commenting" }, { "value": "fa-commenting-o" }, { "value": "fa-comments" }, { "value": "fa-comments-o" }, { "value": "fa-compass" }, { "value": "fa-copyright" }, { "value": "fa-creative-commons" }, { "value": "fa-credit-card" }, { "value": "fa-credit-card-alt" }, { "value": "fa-crop" }, { "value": "fa-crosshairs" }, { "value": "fa-cube" }, { "value": "fa-cubes" }, { "value": "fa-cutlery" }, { "value": "fa-dashboard" }, { "value": "fa-database" }, { "value": "fa-deaf" }, { "value": "fa-deafness" }, { "value": "fa-desktop" }, { "value": "fa-diamond" }, { "value": "fa-dot-circle-o" }, { "value": "fa-download" }, { "value": "fa-drivers-license" }, { "value": "fa-drivers-license-o" }, { "value": "fa-edit" }, { "value": "fa-ellipsis-h" }, { "value": "fa-ellipsis-v" }, { "value": "fa-envelope" }, { "value": "fa-envelope-o" }, { "value": "fa-envelope-open" }, { "value": "fa-envelope-open-o" }, { "value": "fa-envelope-square" }, { "value": "fa-eraser" }, { "value": "fa-exchange" }, { "value": "fa-exclamation" }, { "value": "fa-exclamation-circle" }, { "value": "fa-exclamation-triangle" }, { "value": "fa-external-link" }, { "value": "fa-external-link-square" }, { "value": "fa-eye" }, { "value": "fa-eye-slash" }, { "value": "fa-eyedropper" }, { "value": "fa-fax" }, { "value": "fa-feed" }, { "value": "fa-female" }, { "value": "fa-fighter-jet" }, { "value": "fa-file-archive-o" }, { "value": "fa-file-audio-o" }, { "value": "fa-file-code-o" }, { "value": "fa-file-excel-o" }, { "value": "fa-file-image-o" }, { "value": "fa-file-movie-o" }, { "value": "fa-file-pdf-o" }, { "value": "fa-file-photo-o" }, { "value": "fa-file-picture-o" }, { "value": "fa-file-powerpoint-o" }, { "value": "fa-file-sound-o" }, { "value": "fa-file-video-o" }, { "value": "fa-file-word-o" }, { "value": "fa-file-zip-o" }, { "value": "fa-film" }, { "value": "fa-filter" }, { "value": "fa-fire" }, { "value": "fa-fire-extinguisher" }, { "value": "fa-flag" }, { "value": "fa-flag-checkered" }, { "value": "fa-flag-o" }, { "value": "fa-flash" }, { "value": "fa-flask" }, { "value": "fa-folder" }, { "value": "fa-folder-o" }, { "value": "fa-folder-open" }, { "value": "fa-folder-open-o" }, { "value": "fa-frown-o" }, { "value": "fa-futbol-o" }, { "value": "fa-gamepad" }, { "value": "fa-gavel" }, { "value": "fa-gear" }, { "value": "fa-gears" }, { "value": "fa-gift" }, { "value": "fa-glass" }, { "value": "fa-graduation-cap" }, { "value": "fa-group" }, { "value": "fa-hand-grab-o" }, { "value": "fa-hand-lizard-o" }, { "value": "fa-hand-paper-o" }, { "value": "fa-hand-peace-o" }, { "value": "fa-hand-pointer-o" }, { "value": "fa-hand-rock-o" }, { "value": "fa-hand-scissors-o" }, { "value": "fa-hand-spock-o" }, { "value": "fa-hand-stop-o" }, { "value": "fa-handshake-o" }, { "value": "fa-hard-of-hearing" }, { "value": "fa-hashtag" }, { "value": "fa-hdd-o" }, { "value": "fa-headphones" }, { "value": "fa-heart" }, { "value": "fa-heart-o" }, { "value": "fa-heartbeat" }, { "value": "fa-history" }, { "value": "fa-home" }, { "value": "fa-hotel" }, { "value": "fa-hourglass" }, { "value": "fa-hourglass-1" }, { "value": "fa-hourglass-2" }, { "value": "fa-hourglass-3" }, { "value": "fa-hourglass-end" }, { "value": "fa-hourglass-half" }, { "value": "fa-hourglass-o" }, { "value": "fa-hourglass-start" }, { "value": "fa-i-cursor" }, { "value": "fa-id-badge" }, { "value": "fa-id-card" }, { "value": "fa-id-card-o" }, { "value": "fa-image" }, { "value": "fa-inbox" }, { "value": "fa-industry" }, { "value": "fa-info" }, { "value": "fa-info-circle" }, { "value": "fa-institution" }, { "value": "fa-keyboard-o" }, { "value": "fa-language" }, { "value": "fa-laptop" }, { "value": "fa-leaf" }, { "value": "fa-legal" }, { "value": "fa-lemon-o" }, { "value": "fa-level-down" }, { "value": "fa-level-up" }, { "value": "fa-life-bouy" }, { "value": "fa-life-buoy" }, { "value": "fa-life-ring" }, { "value": "fa-life-saver" }, { "value": "fa-lightbulb-o" }, { "value": "fa-line-chart" }, { "value": "fa-location-arrow" }, { "value": "fa-lock" }, { "value": "fa-low-vision" }, { "value": "fa-magic" }, { "value": "fa-magnet" }, { "value": "fa-mail-forward" }, { "value": "fa-mail-reply" }, { "value": "fa-mail-reply-all" }, { "value": "fa-male" }, { "value": "fa-map-marker" }, { "value": "fa-map-o" }, { "value": "fa-map-pin" }, { "value": "fa-map-signs" }, { "value": "fa-meh-o" }, { "value": "fa-microchip" }, { "value": "fa-microphone" }, { "value": "fa-microphone-slash" }, { "value": "fa-minus" }, { "value": "fa-minus-circle" }, { "value": "fa-minus-square" }, { "value": "fa-minus-square-o" }, { "value": "fa-mobile" }, { "value": "fa-mobile-phone" }, { "value": "fa-money" }, { "value": "fa-moon-o" }, { "value": "fa-mortar-board" }, { "value": "fa-motorcycle" }, { "value": "fa-mouse-pointer" }, { "value": "fa-music" }, { "value": "fa-navicon" }, { "value": "fa-newspaper-o" }, { "value": "fa-object-group" }, { "value": "fa-object-ungroup" }, { "value": "fa-paint-brush" }, { "value": "fa-paper-plane" }, { "value": "fa-paper-plane-o" }, { "value": "fa-paw" }, { "value": "fa-pencil" }, { "value": "fa-pencil-square" }, { "value": "fa-pencil-square-o" }, { "value": "fa-percent" }, { "value": "fa-phone" }, { "value": "fa-phone-square" }, { "value": "fa-photo" }, { "value": "fa-picture-o" }, { "value": "fa-pie-chart" }, { "value": "fa-plane" }, { "value": "fa-plug" }, { "value": "fa-plus" }, { "value": "fa-plus-circle" }, { "value": "fa-plus-square" }, { "value": "fa-plus-square-o" }, { "value": "fa-podcast" }, { "value": "fa-power-off" }, { "value": "fa-print" }, { "value": "fa-puzzle-piece" }, { "value": "fa-qrcode" }, { "value": "fa-question" }, { "value": "fa-question-circle" }, { "value": "fa-question-circle-o" }, { "value": "fa-quote-left" }, { "value": "fa-quote-right" }, { "value": "fa-random" }, { "value": "fa-recycle" }, { "value": "fa-refresh" }, { "value": "fa-registered" }, { "value": "fa-remove" }, { "value": "fa-reorder" }, { "value": "fa-reply" }, { "value": "fa-reply-all" }, { "value": "fa-retweet" }, { "value": "fa-road" }, { "value": "fa-rocket" }, { "value": "fa-rss" }, { "value": "fa-rss-square" }, { "value": "fa-s15" }, { "value": "fa-search" }, { "value": "fa-search-minus" }, { "value": "fa-search-plus" }, { "value": "fa-send" }, { "value": "fa-send-o" }, { "value": "fa-server" }, { "value": "fa-share" }, { "value": "fa-share-alt" }, { "value": "fa-share-alt-square" }, { "value": "fa-share-square" }, { "value": "fa-share-square-o" }, { "value": "fa-shield" }, { "value": "fa-ship" }, { "value": "fa-shopping-bag" }, { "value": "fa-shopping-basket" }, { "value": "fa-shopping-cart" }, { "value": "fa-shower" }, { "value": "fa-sign-in" }, { "value": "fa-sign-language" }, { "value": "fa-sign-out" }, { "value": "fa-signal" }, { "value": "fa-signing" }, { "value": "fa-sitemap" }, { "value": "fa-sliders" }, { "value": "fa-smile-o" }, { "value": "fa-snowflake-o" }, { "value": "fa-soccer-ball-o" }, { "value": "fa-sort" }, { "value": "fa-sort-alpha-asc" }, { "value": "fa-sort-alpha-desc" }, { "value": "fa-sort-amount-asc" }, { "value": "fa-sort-amount-desc" }, { "value": "fa-sort-asc" }, { "value": "fa-sort-desc" }, { "value": "fa-sort-down" }, { "value": "fa-sort-numeric-asc" }, { "value": "fa-sort-numeric-desc" }, { "value": "fa-sort-up" }, { "value": "fa-space-shuttle" }, { "value": "fa-spinner" }, { "value": "fa-spoon" }, { "value": "fa-square" }, { "value": "fa-square-o" }, { "value": "fa-star" }, { "value": "fa-star-half" }, { "value": "fa-star-half-empty" }, { "value": "fa-star-half-full" }, { "value": "fa-star-half-o" }, { "value": "fa-star-o" }, { "value": "fa-sticky-note" }, { "value": "fa-sticky-note-o" }, { "value": "fa-street-view" }, { "value": "fa-suitcase" }, { "value": "fa-sun-o" }, { "value": "fa-support" }, { "value": "fa-tablet" }, { "value": "fa-tachometer" }, { "value": "fa-tag" }, { "value": "fa-tags" }, { "value": "fa-tasks" }, { "value": "fa-taxi" }, { "value": "fa-television" }, { "value": "fa-terminal" }, { "value": "fa-thumb-tack" }, { "value": "fa-thumbs-down" }, { "value": "fa-thumbs-o-down" }, { "value": "fa-thumbs-o-up" }, { "value": "fa-thumbs-up" }, { "value": "fa-ticket" }, { "value": "fa-times" }, { "value": "fa-times-circle" }, { "value": "fa-times-circle-o" }, { "value": "fa-times-rectangle" }, { "value": "fa-times-rectangle-o" }, { "value": "fa-tint" }, { "value": "fa-toggle-down" }, { "value": "fa-toggle-left" }, { "value": "fa-toggle-off" }, { "value": "fa-toggle-on" }, { "value": "fa-toggle-right" }, { "value": "fa-toggle-up" }, { "value": "fa-trademark" }, { "value": "fa-trash" }, { "value": "fa-trash-o" }, { "value": "fa-tree" }, { "value": "fa-trophy" }, { "value": "fa-truck" }, { "value": "fa-tty" }, { "value": "fa-tv" }, { "value": "fa-umbrella" }, { "value": "fa-universal-access" }, { "value": "fa-university" }, { "value": "fa-unlock" }, { "value": "fa-unlock-alt" }, { "value": "fa-unsorted" }, { "value": "fa-upload" }, { "value": "fa-user" }, { "value": "fa-user-plus" }, { "value": "fa-user-secret" }, { "value": "fa-user-times" }, { "value": "fa-users" }, { "value": "fa-vcard" }, { "value": "fa-vcard-o" }, { "value": "fa-video-camera" }, { "value": "fa-volume-control-phone" }, { "value": "fa-volume-down" }, { "value": "fa-volume-off" }, { "value": "fa-volume-up" }, { "value": "fa-warning" }, { "value": "fa-wheelchair" }, { "value": "fa-wheelchair-alt" }, { "value": "fa-wifi" }, { "value": "fa-window-close" }, { "value": "fa-window-close-o" }, { "value": "fa-window-maximize" }, { "value": "fa-window-minimize" }, { "value": "fa-window-restore" }, { "value": "fa-wrench" }, { "value": "fa-american-sign-language-interpreting" }, { "value": "fa-asl-interpreting" }, { "value": "fa-assistive-listening-systems" }, { "value": "fa-audio-description" }, { "value": "fa-blind" }, { "value": "fa-braille" }, { "value": "fa-cc" }, { "value": "fa-deaf" }, { "value": "fa-deafness" }, { "value": "fa-hard-of-hearing" }, { "value": "fa-low-vision" }, { "value": "fa-question-circle-o" }, { "value": "fa-sign-language" }, { "value": "fa-signing" }, { "value": "fa-tty" }, { "value": "fa-universal-access" }, { "value": "fa-volume-control-phone" }, { "value": "fa-wheelchair" }, { "value": "fa-wheelchair-alt" }, { "value": "fa-hand-grab-o" }, { "value": "fa-hand-lizard-o" }, { "value": "fa-hand-o-down" }, { "value": "fa-hand-o-left" }, { "value": "fa-hand-o-right" }, { "value": "fa-hand-o-up" }, { "value": "fa-hand-paper-o" }, { "value": "fa-hand-peace-o" }, { "value": "fa-hand-pointer-o" }, { "value": "fa-hand-rock-o" }, { "value": "fa-hand-scissors-o" }, { "value": "fa-hand-spock-o" }, { "value": "fa-hand-stop-o" }, { "value": "fa-thumbs-down" }, { "value": "fa-thumbs-o-down" }, { "value": "fa-thumbs-o-up" }, { "value": "fa-thumbs-up" }, { "value": "fa-ambulance" }, { "value": "fa-automobile" }, { "value": "fa-bicycle" }, { "value": "fa-bus" }, { "value": "fa-cab" }, { "value": "fa-car" }, { "value": "fa-fighter-jet" }, { "value": "fa-motorcycle" }, { "value": "fa-plane" }, { "value": "fa-rocket" }, { "value": "fa-ship" }, { "value": "fa-space-shuttle" }, { "value": "fa-subway" }, { "value": "fa-taxi" }, { "value": "fa-train" }, { "value": "fa-truck" }, { "value": "fa-wheelchair" }, { "value": "fa-wheelchair-alt" }, { "value": "fa-genderless" }, { "value": "fa-intersex" }, { "value": "fa-mars" }, { "value": "fa-mars-double" }, { "value": "fa-mars-stroke" }, { "value": "fa-mars-stroke-h" }, { "value": "fa-mars-stroke-v" }, { "value": "fa-mercury" }, { "value": "fa-neuter" }, { "value": "fa-transgender" }, { "value": "fa-transgender-alt" }, { "value": "fa-venus" }, { "value": "fa-venus-double" }, { "value": "fa-venus-mars" }, { "value": "fa-file" }, { "value": "fa-file-archive-o" }, { "value": "fa-file-audio-o" }, { "value": "fa-file-code-o" }, { "value": "fa-file-excel-o" }, { "value": "fa-file-image-o" }, { "value": "fa-file-movie-o" }, { "value": "fa-file-o" }, { "value": "fa-file-pdf-o" }, { "value": "fa-file-photo-o" }, { "value": "fa-file-picture-o" }, { "value": "fa-file-powerpoint-o" }, { "value": "fa-file-sound-o" }, { "value": "fa-file-text" }, { "value": "fa-file-text-o" }, { "value": "fa-file-video-o" }, { "value": "fa-file-word-o" }, { "value": "fa-file-zip-o" }, { "value": "fa-circle-o-notch" }, { "value": "fa-cog" }, { "value": "fa-gear" }, { "value": "fa-refresh" }, { "value": "fa-spinner" }, { "value": "fa-check-square" }, { "value": "fa-check-square-o" }, { "value": "fa-circle" }, { "value": "fa-circle-o" }, { "value": "fa-dot-circle-o" }, { "value": "fa-minus-square" }, { "value": "fa-minus-square-o" }, { "value": "fa-plus-square" }, { "value": "fa-plus-square-o" }, { "value": "fa-square" }, { "value": "fa-square-o" }, { "value": "fa-cc-amex" }, { "value": "fa-cc-diners-club" }, { "value": "fa-cc-discover" }, { "value": "fa-cc-jcb" }, { "value": "fa-cc-mastercard" }, { "value": "fa-cc-paypal" }, { "value": "fa-cc-stripe" }, { "value": "fa-cc-visa" }, { "value": "fa-credit-card" }, { "value": "fa-credit-card-alt" }, { "value": "fa-google-wallet" }, { "value": "fa-paypal" }, { "value": "fa-area-chart" }, { "value": "fa-bar-chart" }, { "value": "fa-bar-chart-o" }, { "value": "fa-line-chart" }, { "value": "fa-pie-chart" }, { "value": "fa-bitcoin" }, { "value": "fa-btc" }, { "value": "fa-cny" }, { "value": "fa-dollar" }, { "value": "fa-eur" }, { "value": "fa-euro" }, { "value": "fa-gbp" }, { "value": "fa-gg" }, { "value": "fa-gg-circle" }, { "value": "fa-ils" }, { "value": "fa-inr" }, { "value": "fa-jpy" }, { "value": "fa-krw" }, { "value": "fa-money" }, { "value": "fa-rmb" }, { "value": "fa-rouble" }, { "value": "fa-rub" }, { "value": "fa-ruble" }, { "value": "fa-rupee" }, { "value": "fa-shekel" }, { "value": "fa-sheqel" }, { "value": "fa-try" }, { "value": "fa-turkish-lira" }, { "value": "fa-usd" }, { "value": "fa-won" }, { "value": "fa-yen" }, { "value": "fa-align-center" }, { "value": "fa-align-justify" }, { "value": "fa-align-left" }, { "value": "fa-align-right" }, { "value": "fa-bold" }, { "value": "fa-chain" }, { "value": "fa-chain-broken" }, { "value": "fa-clipboard" }, { "value": "fa-columns" }, { "value": "fa-copy" }, { "value": "fa-cut" }, { "value": "fa-dedent" }, { "value": "fa-eraser" }, { "value": "fa-file" }, { "value": "fa-file-o" }, { "value": "fa-file-text" }, { "value": "fa-file-text-o" }, { "value": "fa-files-o" }, { "value": "fa-floppy-o" }, { "value": "fa-font" }, { "value": "fa-header" }, { "value": "fa-indent" }, { "value": "fa-italic" }, { "value": "fa-link" }, { "value": "fa-list" }, { "value": "fa-list-alt" }, { "value": "fa-list-ol" }, { "value": "fa-list-ul" }, { "value": "fa-outdent" }, { "value": "fa-paperclip" }, { "value": "fa-paragraph" }, { "value": "fa-paste" }, { "value": "fa-repeat" }, { "value": "fa-rotate-left" }, { "value": "fa-rotate-right" }, { "value": "fa-save" }, { "value": "fa-scissors" }, { "value": "fa-strikethrough" }, { "value": "fa-subscript" }, { "value": "fa-superscript" }, { "value": "fa-table" }, { "value": "fa-text-height" }, { "value": "fa-text-width" }, { "value": "fa-th" }, { "value": "fa-th-large" }, { "value": "fa-th-list" }, { "value": "fa-underline" }, { "value": "fa-undo" }, { "value": "fa-unlink" }, { "value": "fa-angle-double-down" }, { "value": "fa-angle-double-left" }, { "value": "fa-angle-double-right" }, { "value": "fa-angle-double-up" }, { "value": "fa-angle-down" }, { "value": "fa-angle-left" }, { "value": "fa-angle-right" }, { "value": "fa-angle-up" }, { "value": "fa-arrow-circle-down" }, { "value": "fa-arrow-circle-left" }, { "value": "fa-arrow-circle-o-down" }, { "value": "fa-arrow-circle-o-left" }, { "value": "fa-arrow-circle-o-right" }, { "value": "fa-arrow-circle-o-up" }, { "value": "fa-arrow-circle-right" }, { "value": "fa-arrow-circle-up" }, { "value": "fa-arrow-down" }, { "value": "fa-arrow-left" }, { "value": "fa-arrow-right" }, { "value": "fa-arrow-up" }, { "value": "fa-arrows" }, { "value": "fa-arrows-alt" }, { "value": "fa-arrows-h" }, { "value": "fa-arrows-v" }, { "value": "fa-caret-down" }, { "value": "fa-caret-left" }, { "value": "fa-caret-right" }, { "value": "fa-caret-square-o-down" }, { "value": "fa-caret-square-o-left" }, { "value": "fa-caret-square-o-right" }, { "value": "fa-caret-square-o-up" }, { "value": "fa-caret-up" }, { "value": "fa-chevron-circle-down" }, { "value": "fa-chevron-circle-left" }, { "value": "fa-chevron-circle-right" }, { "value": "fa-chevron-circle-up" }, { "value": "fa-chevron-down" }, { "value": "fa-chevron-left" }, { "value": "fa-chevron-right" }, { "value": "fa-chevron-up" }, { "value": "fa-exchange" }, { "value": "fa-hand-o-down" }, { "value": "fa-hand-o-left" }, { "value": "fa-hand-o-right" }, { "value": "fa-hand-o-up" }, { "value": "fa-long-arrow-down" }, { "value": "fa-long-arrow-left" }, { "value": "fa-long-arrow-right" }, { "value": "fa-long-arrow-up" }, { "value": "fa-toggle-down" }, { "value": "fa-toggle-left" }, { "value": "fa-toggle-right" }, { "value": "fa-toggle-up" }, { "value": "fa-arrows-alt" }, { "value": "fa-backward" }, { "value": "fa-compress" }, { "value": "fa-eject" }, { "value": "fa-expand" }, { "value": "fa-fast-backward" }, { "value": "fa-fast-forward" }, { "value": "fa-forward" }, { "value": "fa-pause" }, { "value": "fa-pause-circle" }, { "value": "fa-pause-circle-o" }, { "value": "fa-play" }, { "value": "fa-play-circle" }, { "value": "fa-play-circle-o" }, { "value": "fa-random" }, { "value": "fa-step-backward" }, { "value": "fa-step-forward" }, { "value": "fa-stop" }, { "value": "fa-stop-circle" }, { "value": "fa-stop-circle-o" }, { "value": "fa-youtube-play" }, { "value": "fa-ambulance" }, { "value": "fa-h-square" }, { "value": "fa-heart" }, { "value": "fa-heart-o" }, { "value": "fa-heartbeat" }, { "value": "fa-hospital-o" }, { "value": "fa-medkit" }, { "value": "fa-plus-square" }, { "value": "fa-stethoscope" }, { "value": "fa-user-md" }, { "value": "fa-wheelchair" }, { "value": "fa-wheelchair-alt" }];
                    var lgtimes = 1;
                    if (opts.lgTimes)
                        lgtimes = opts.lgTimes;
                    var faXtimes = 'fa-' + lgtimes + 'x';
                    for (var i = 0; i < icons.length; i++) {
                        $('<a class="iconpicker-cell"><i class="fa ' + icons[i].value + ' ' + faXtimes + '" aria-hidden="true"></i></a>').appendTo(p);
                    }
                    var cells = p.find('.iconpicker-cell');
                    cells._outerWidth(opts.cellWidth)._outerHeight(opts.cellHeight);
                    cells.bind('click.iconpicker', function (e) {
                        var iconpicker = $(this).find('i').attr("class").replace(" " + faXtimes, "");
                        $(target).iconpicker('setValue', iconpicker);
                        if (p.parent()) {
                            var scrollTop = $(document).scrollTop()
                            var top = $(target).offset().top;
                            var left = $(target).offset().left;
                            p.parent().css('top', scrollTop + top);
                            p.parent().css('left', left);
                        }
                        $(target).combobox('hidePanel');
                    });
                }
            }
        }));
        if (opts.value) {
            $(target).iconpicker('setValue', opts.value);
        }
    }

    $.fn.iconpicker = function (options, param) {
        if (typeof options == 'string') {
            var method = $.fn.iconpicker.methods[options];
            if (method) {
                return method(this, param);
            } else {
                return this.combobox(options, param);
            }
        }
        options = options || {};
        return this.each(function () {
            var state = $.data(this, 'iconpicker');
            if (state) {
                $.extend(state.options, options);
            } else {
                state = $.data(this, 'iconpicker', {
                    options: $.extend({}, $.fn.iconpicker.defaults, $.fn.iconpicker.parseOptions(this), options)
                });
            }
            create(this);
        });
    };

    $.fn.iconpicker.methods = {
        options: function (jq) {
            return jq.data('iconpicker').options;
        },
        setValue: function (jq, value) {
            return jq.each(function () {
                //debugger;
                var target = $(this).iconpicker('options').target;
                var lgtimes = $(this).iconpicker('options').lgTimes;
                if (target) {
                    var faXtimes = 'fa-1x';
                    if (lgtimes)
                        faXtimes = 'fa-' + lgtimes + 'x';
                    var tClassList = ($('#' + target)[0]).classList;
                    if (tClassList.length === 3) {
                        faXtimes = tClassList[2];
                    }
                    
                    $('#' + target).removeClass();
                    $('#' + target).addClass(value + ' ' + faXtimes);
                } else {
                    $(this).next('.fa').remove();
                    $(this).next().before($('<i class="fa ' + value + '" aria-hidden="true" style="margin:10px 4px 0 0;font-size:24px;"></i>'));
                }
                $(this).combobox('setValue', value).combobox('setText', value);
            })
        },
        clear: function (jq) {
            return jq.each(function () {
                $(this).combobox('clear');
            });
        }
    };

    $.fn.iconpicker.parseOptions = function (target) {
        return $.extend({}, $.fn.combobox.parseOptions(target), {

        });
    };

    $.fn.iconpicker.defaults = $.extend({}, $.fn.combobox.defaults, {
        editable: false,
        cellWidth: 56,
        cellHeight: 56
    });

    $.parser.plugins.push('iconpicker');
})(jQuery);