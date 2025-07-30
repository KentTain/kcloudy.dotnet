
var WorkflowDesigner = function () {
    const canvasId = '#canvasId';
    const canvansContainer = "#Container";
    const showGridId = '#showGridId';
    const mouseToolsBtn = '#mouseToolsBtn';
    const connectionToolsBtn = '#connectionToolsBtn';
    const nodeSelecter = '.moveLight';
    //节点颜色设置
    const startNodeColor = '#5cb85c';
    const conditionNodeColor = '#5a9cf9';
    const taskNodeColor = '#5a9cf9';
    const endNodeColor = '#dc6b6b';
    const subflowNodeColor = '#edef31';
    const laneColor = '#edef31';

    const multipleSelectedRectangleId = 'multipleSelectedRectangle';
    const multipleSelectedRectangle = '#' + multipleSelectedRectangleId;

    let me = this;
    let options = {
        flowId: '',                         // 流程ID
        flowCode: '',                       // 流程编码
        flowVerson: '',                     // 流程版本
        flowName: '',                       // 流程编码
        px: '',                             // 鼠标在画布中的横向坐标
        py: '',                             // 鼠标在画布中的纵向坐标
        allTimer: {},                       // 所有的定时器对象
        isClear: true,                      // 是否清空画布
        nodeIdPool: {},                     // 节点id池
        plumb: {},                          // jsPlumb实例对象
        graph: new graphlib.Graph(),        // 图形对象
        selectedNodeList: [],               // 被选中的节点列表
        selectedMultipleFlag: false,        // 全选标识
        allowMultipleSelectedFlag: false,   // 允许多选标识
        isSmallMove: false,                 // 是否产生微移标识
        undoStack: [],                      // 撤销栈
        redoStack: [],                      // 重做栈
        myclipboard: [],                    // 剪贴板
        tempFlow: {},                       // 微移临时的流程图对象
        tempFlag: true,                     // 微移临时的标识
        laneObjs: [],                       // 泳道数组
        nodeClickFn: null,                  // 节点单击事件
        linkClickFn: null,                  // 连线单击事件
        createNewNodeFn: null,              // 创建新节点事件
        deleteNodeFn: null,                 // 删除节点事件
        deleteLinkFn: null,                 // 删除连线事件
        saveFn: null,                       // 保存流程事件
    };
    let _data = {};
    me._base = options;
    me.init = function (opt) {
        if (opt != null)
            $.extend(true, options, opt);

        // 1、渲染流程设计器
        render();

        // 2、初始化节点id池
        _initIdPool();

    };
    // 加载json数据到流程设计器
    me.loadJson = function (data) {
        let _base = me._base;
        let nodeArr = [];
        let linkArr = [];
        if (data.nodeDataArray !== undefined && data.nodeDataArray !== null && data.nodeDataArray.length > 0) {
            nodeArr = data.nodeDataArray;
        }
        if (data.linkDataArray !== undefined && data.linkDataArray !== null && data.linkDataArray.length > 0) {
            linkArr = data.linkDataArray;
        }

        //根据后台数据，转换为画布识别数据
        if (nodeArr.length === 0 && linkArr.length === 0)
            _getWorkflowDataByNodeData(data, nodeArr, linkArr);

        // 1、渲染节点到画布
        for (let i = 0, len = nodeArr.length; i < len; i++) {
            // 根据节点类型获取要渲染的节点对象
            _createNewNode(nodeArr[i]);
        }

        // 2、渲染连线到画布
        for (let i = 0, len = linkArr.length; i < len; i++) {
            // 2.1、连线
            plumbUtil.connectNode(linkArr[i].from, linkArr[i].to, linkArr[i].routerId, linkArr[i].sourceAnchors, linkArr[i].targetAnchors);

            // 2.2、添加连线文本
            plumbUtil.setRouterLabel(linkArr[i].from, linkArr[i].to, linkArr[i].label);
        }
    };
    // 创建新节点
    me.createNewNode = function (nodeType, pos) {
        let _base = me._base;

        // 1、创建前的检查
        let msg = graphUtil.checkGraphBeforeCreate(nodeType);
        if (msg != '0') {
            $.messager.showInfoTopCenter('系统提示', msg, 2000);
            return;
        }

        // 2、修改保存状态为未保存
        //$("#saveStatus").css('display', '');

        // 3、将当前流程图push到撤销栈
        _base.undoStack.push(me.getCurrentFlow());

        // 4、根据节点类型获取要渲染的节点对象
        let renderNode = _getRenderNodeFromType(nodeType);
        renderNode.locTop = pos.top;
        renderNode.locLeft = pos.left;

        // 5、创建节点
        _createNewNode(renderNode);

        // 6、切换为鼠标工具
        _mouseTool();

        // 7、调用外部服务
        if (me._base.createNewNodeFn !== null)
            me._base.createNewNodeFn(renderNode);

        return renderNode.id;
    };
    let _createNewNode = function (renderNode) {
        let _base = me._base;

        // 1、节点类型为泳道时特殊处理
        if (renderNode.nodeType == 'broadwiseLane' || renderNode.nodeType == 'directionLane') {
            _createLane(renderNode);
            return renderNode.key;
        }

        // 2、添加节点到画布
        let nodeDiv = '<div class="jnode-panel moveLight" id="' + renderNode.key + '" nodeid="' + renderNode.id + '" nodecode="' + renderNode.code + '" nodetext="' + renderNode.text + '" nodetype="' + renderNode.type + '"  nodestatus="' + renderNode.taskStatus + '" nodetooltip="' + renderNode.tooltip + '" >' +
                '<div class="' + renderNode.cla + '">' +
                    '<span class="nodeTextCla">' + renderNode.text + '</span>' + renderNode.icon +
                '</div>' +
             '</div>';
        $(canvansContainer).append(nodeDiv);

        let nodeId = $.string.getJQSel(renderNode.key);
        // 2、设置单击事件
        $(nodeId).click(function (event) {
            event = document.all ? window.event : arguments[0] ? arguments[0] : event;
            event.stopPropagation();
            if (me._base.nodeClickFn != null)
                me._base.nodeClickFn(renderNode.nodeType, renderNode.key);
        })

        // 3、设置节点位置
        $(nodeId).offset({ top: renderNode.locTop, left: renderNode.locLeft });

        // 4、设置节点的样式
        $(nodeId).css({
            'height': renderNode.nodeHeight,
            'width': renderNode.nodeWidth,
        });

        // 5、设置节点的属性
        $(nodeId).attr('bgColor-gradient', renderNode.bgColor);

        // 6、设置节点的右键菜单
        //window.ContextMenu.bind(nodeId, nodeMenuJson);

        // 7、设置节点可以被移动
        plumbUtil.setNodeDraggable(renderNode.key);

        // 8、设置节点是否可以被缩放
        if (CONFIG.defaultConfig.resizableFlag) plumbUtil.nodeResizable(renderNode.key);

        // 9、监听节点
        _registerNodeEvent(renderNode.key);

        // 10、将节点添加到图对象
        let graphNode = {
            id: renderNode.id,
            text: renderNode.text,
            key: renderNode.key,
            type: renderNode.type,
            nodeType: renderNode.nodeType,
            locTop: renderNode.locTop,
            locLeft: renderNode.locLeft,
            nodeHeight: $(nodeId).css('height'),
            nodeWidth: $(nodeId).css('width'),
            bgColor: renderNode.bgColor,
            isSelected: false,
            //以下数据为流程实例的任务执行数据
            isProcessTask: renderNode.isProcessTask,
            taskStatus: renderNode.taskStatus,
            taskStatusString: renderNode.taskStatusString,
            executeUserName: renderNode.executeUserName,
            executeDateTime: renderNode.executeDateTime,
            executeStatus: renderNode.executeStatus,
            executeStatusString: renderNode.executeStatusString,
            executeRemark: renderNode.executeRemark
        };
        _base.graph.setNode(renderNode.key, graphNode);

        return renderNode;
    };
    let _createLane = function (renderNode) {
        let _base = me._base;

        // 添加到画板中
        let textArr = renderNode.text.split(''), i, tempText = '', a, b, c;
        if (renderNode.nodeType == 'broadwiseLane') {
            for (i = 0; i < textArr.length; i++) {
                tempText += '<span style="display: block;">' + textArr[i] + '</span>';
            }
            a = '50px';
            b = '247px';
        } else {
            tempText = '<span>' + renderNode.text + '</span>';
        }
        c = 'lane-' + CommonUtil.getGuid();
        $(canvansContainer).append('<div id="' + renderNode.key + '" class="' + renderNode.cla + '">' +
            '<div id="' + c + '" class="laneLabelDivClass" style="width: ' + a + '; height: ' + b + ';">' +
            tempText +
            '</div>' +
            '</div>'
        );

        // canvasId的相对位置
        let canvasX = $(canvasId).offset().left;
        let canvasY = $(canvasId).offset().top;

        let nodeId = $.string.getJQSel(renderNode.key);

        // 设置节点位置
        let t = renderNode.nodeType == 'broadwiseLane' ? renderNode.locTop : canvasY;
        let l = renderNode.nodeType == 'broadwiseLane' ? canvasX : renderNode.locLeft;
        $(nodeId).offset({ top: t, left: l });

        // 设置节点的属性
        $(nodeId).attr('bgColor-gradient', renderNode.bgColor);

        // 设置节点的样式
        if (renderNode.nodeType == 'broadwiseLane') {
            $(nodeId).css({
                'line-height': plumbUtil.getLaneLineHeight(renderNode.text, $(nodeId).css('height'))
            });
        }

        // 设置右键菜单
        //window.ContextMenu.bind($.string.getJQSel(c), laneMenuJson);

        // 设置节点可拖拽
        $(nodeId).draggable({
            containment: canvansContainer,
            handle: '.laneLabelDivClass',
            axis: renderNode.nodeType == 'broadwiseLane' ? 'y' : 'x',
            // 拖拽结束后更新图对象中存储的泳道位置
            stop: function (event) {
                // 更新泳道对象
                graphUtil.updateLaneObjs(event.target.id);
            }
        });

        // 设置泳道可被缩放
        plumbUtil.laneResizable(renderNode.key);

        /**
         * 阻止事件的传播行为，防止点击节点时触发父节点绑定的click事件，以及在拖动泳道时会出现多选框
         */
        let laneId = $.string.getJQSel(c);
        $(laneId).click(function (event) {
            event = document.all ? window.event : arguments[0] ? arguments[0] : event;
            event.stopPropagation();
            if (me._base.nodeClickFn != null)
                me._base.nodeClickFn(renderNode.nodeType, renderNode.key);
        }).mousemove(function (event) {
            _base.px = '';
            _base.py = '';
        });

        $(nodeId).click(function (event) {
            event = document.all ? window.event : arguments[0] ? arguments[0] : event;
            event.stopPropagation();
        }).mousemove(function (event) {
            _base.px = '';
            _base.py = '';
        });

        // 将泳道节点添加到泳道对象中
        let laneObj = {
            text: renderNode.text,
            key: renderNode.key,
            nodeType: renderNode.nodeType,
            locTop: renderNode.locTop,
            locLeft: renderNode.locLeft,
            nodeHeight: $(nodeId).css('height'),
            nodeWidth: $(nodeId).css('width'),
            bgColor: renderNode.bgColor
        };
        _base.laneObjs[renderNode.key] = laneObj;
    };
    // 根据节点类型获取要渲染的节点对象
    let _getRenderNodeFromType = function (type) {
        let renderNode;
        switch (type) {
            case 'start':
                renderNode = new graphUtil.Node(
                    me.getNextNodeId('E'),
                    0,
                    'start',
                    '开始',
                    'startNode',
                    startNodeColor,
                    '',
                    '60px',
                    '60px'
                );
                break;
            case 'task':
                renderNode = new graphUtil.Node(
                    me.getNextNodeId('T'),
                    1,
                    'task',
                    '审批',
                    'taskNode',
                    taskNodeColor,
                    //'<i class="fa fa-user" style="color:#666666;position:absolute;right: 100px;top:5px;"></i>'
                    '',
                    '60px',
                    '120px'
                );
                break;
            case 'condition':
                renderNode = new graphUtil.Node(
                    me.getNextNodeId('G'),
                    2,
                    'condition',
                    '条件',
                    'conditionNode',
                    conditionNodeColor,
                    '',
                    '94px',
                    '94px'
                );
                break;
            case 'subflow':
                renderNode = new graphUtil.Node(
                    me.getNextNodeId('S'),
                    3,
                    'subflow',
                    '子流程',
                    'subflowNode',
                    subflowNodeColor,
                    '',
                    '60px',
                    '100px'
                );
                break;
            case 'end':
                renderNode = new graphUtil.Node(
                    me.getNextNodeId('E'),
                    4,
                    'end',
                    '结束',
                    'endNode',
                    endNodeColor,
                    '',
                    '60px',
                    '60px'
                );
                break;
            case 'broadwiseLane':
                renderNode = new graphUtil.Node(
                    me.getNextNodeId('L'),
                    9,
                    'broadwiseLaneNodeOnContainer laneNode',
                    'broadwiseLane',
                    '横向泳道',
                    laneColor,
                    '',
                    '60px',
                    '100px'
                );
                break;
            case 'directionLane':
                renderNode = new graphUtil.Node(
                    me.getNextNodeId('L'),
                    9,
                    'directionLaneNodeOnContainer laneNode',
                    'directionLane',
                    '纵向泳道',
                    laneColor,
                    '',
                    '60px',
                    '100px'
                );
                break;
            default:
                $.messager.showInfoTopCenter('系统提示', CONFIG.msg.chooseNodeObjErr, 2000);
                return;
        }
        return renderNode;
    };
    // 监听节点
    let _registerNodeEvent = function (tempId) {
        let selector = $.string.isNullOrWhiteSpace(tempId) ? nodeSelecter : $.string.getJQSel(tempId);
        let _base = me._base;

        /**
         * 当鼠标移动到节点上时将发光属性保存到临时的属性temp-box-shadow中
         * 然后改变节点的发光样式，显示可拖拽区域
         *
         * 当鼠标移出节点时将节点的发光样式还原为临时保存的属性temp-box-shadow，隐藏可拖拽区域
         */
        $(selector).mouseover(function () {
            // 当节点选中标识为false，也就是未被选中时
            if (!_base.graph.node($(this).attr('id')).isSelected) {
                $(this).find('div').css('box-shadow', CONFIG.defaultStyle.selectedBoxShadow);
            }

            //debugger;
            let tipId = $(this).attr('id');
            let tipText = $(this).attr('nodetext');
            let tooltip = $(this).attr('nodetooltip');
            if (tooltip && tooltip !== '') {
                $(tipId).tooltip({ content: tooltip });
            } else {
                $(tipId).tooltip({ content: tipText });
            }
        }).mouseout(function () {
            // 当节点选中标识为false，也就是未被选中时
            if (!_base.graph.node($(this).attr('id')).isSelected) {
                $(this).find('div').css('box-shadow', CONFIG.defaultStyle.noSelectedBoxShadow);
            }
            //layer.close(layer.tips());
        });

        /**
         * 单击选中事件
         */
        $(selector).mousedown(function (event) {
            // 兼容浏览器写法
            event = document.all ? window.event : arguments[0] ? arguments[0] : event;

            // 当鼠标按钮不为左键时终止函数的执行，0是左键，1是滚轮键，2是右键
            // if (event.button != 0) return;

            $(this).find('div').css('box-shadow', '0 0 35px green');

            // 当没有多选时将其他被选中的节点改为未选中
            if (!_base.selectedMultipleFlag) {
                me.changeToNoSelected($(this).attr('id'));
            }
        }).mouseup(function (event) {
            // 显示节点全名
            if (!_base.selectedMultipleFlag && !_base.allowMultipleSelectedFlag) {
                let tipId = $.string.getJQSel($(this).attr('id'));

                $(tipId).tooltip({ content: _base.graph.node($(this).attr('id')).text });
            }

            // 兼容浏览器写法
            var event = document.all ? window.event : arguments[0] ? arguments[0] : event;

            // 当鼠标按钮不为左键时终止函数的执行，0是左键，1是滚轮键，2是右键
            // if (event.button != 0) return;

            $(this).find('div').css('box-shadow', CONFIG.defaultStyle.selectedBoxShadow);
            me.clearAllTimer();

            // 当允许多选时
            if (_base.allowMultipleSelectedFlag) {
                me.selectedNode($(this).attr('id'));
            }

            // 当没有多选时
            if (!_base.selectedMultipleFlag) {
                _base.selectedNodeList[0] = $(this).attr('id');
                _base.plumb.addToDragSelection($(this).attr('id'));
                _base.graph.node($(this).attr('id')).isSelected = true;
            }
        });

        /**
         * 阻止事件的传播行为，防止点击节点时触发父节点绑定的click事件
         */
        $(selector).click(function (event) {
            event = document.all ? window.event : arguments[0] ? arguments[0] : event;
            event.stopPropagation();
        });
    };
    // 保存流程图
    me.save = function () {
        // 1、检查流程图合法性
        let checkMsg = graphUtil.checkGraph();
        if (checkMsg != '0') {
            $.messager.showInfoTopCenter('系统提示', checkMsg, 1000);
            return;
        }

        //debugger;
        // 2、获取当前流程图对象
        let obj = me.getCurrentFlow();

        // 将流程图对象json数据持久化到数据库中
        if (me._base.saveFn != null)
            me._base.saveFn(obj);
        // var res = saveObj(obj);

        // 3、保存状态为已保存
        //$("#saveStatus").css('display', 'none');

        // 4、提示保存成功
        $.messager.showInfoTopCenter('系统提示', CONFIG.msg.saveSuccess, 2000);
    };
    // 保存为图片
    me.save2Photo = function () {
        // 1、检查当前流程图是否可以保存为图片
        let checkMsg = graphUtil.checkGraphBySave2Photo();
        if (checkMsg != '0') {
            $.messager.showInfoTopCenter('系统提示', checkMsg, 1000);
            return;
        }

        // 2、计算生成图片的尺寸
        let positionObj = graphUtil.getCanvasSizeByNode();

        // 3、处理svg标签，这里只做对连接线的转换，端点暂不考虑
        let removeArr = [];
        let svgElem = $(canvasId).find('svg[id]');
        let i;
        svgElem.each(function (index, node) {
            // 3.1、创建canvas标签，设置标签id并将id放入移除数组中，便于生成图片后移除canvas
            let canvas = document.createElement('canvas');
            let canvasId = 'canvas-' + CommonUtil.getGuid();
            canvas.id = canvasId;
            removeArr.push(canvasId);

            // 3.2、svg标签内容
            let svg = node.outerHTML.trim();

            // 3.3、转换为canvas
            canvg(canvas, svg);

            // 3.4、设置位置
            if (node.style.position) {
                canvas.style.position += node.style.position;
                canvas.style.left += node.style.left;
                canvas.style.top += node.style.top;
            }
            $(canvansContainer).append(canvas);
        });

        // 4、将流程图转换为canvas，然后再转成base64编码
        html2canvas(document.getElementById('Container'), {
            width: positionObj.canvasLeft,
            height: positionObj.canvasTop,
            // 关闭日志
            logging: false
        }).then(function (canvas) {
            // 将canvas转成base64编码，然后再转成图片url
            let imgUri = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream");
            // 下载图片
            let alink = document.createElement('a');
            let alinkId = 'alink-' + CommonUtil.getGuid();
            removeArr.push(alinkId);
            alink.id = alinkId;
            alink.href = imgUri;
            alink.download = '流程设计图_' + CommonUtil.getGuid() + '.jpg';
            alink.click();
        });

        // 5、移除生成的canvas、alink，这里采用异步的方式是因为生成图片需要时间，若在生成图片时执行了清除代码则svg内容不会被转成图片
        setTimeout(function () {
            for (i = 0; i < removeArr.length; i++) {
                $($.string.getJQSel(removeArr[i])).remove();
            }
        }, 1000);
    };
    // 获取当前的流程图对象
    me.getCurrentFlow = function () {
        let _base = me._base;
        let flowDoc = {};
        let nodeDataArray = [];
        // 当前滚动条位置
        let scrollX = $(canvasId).scrollLeft();
        let scrollY = $(canvasId).scrollTop();

        // 节点
        $.each($(canvansContainer).children(nodeSelecter), function (index) {
            let node = _base.graph.node($(this).attr('id'));
            //debugger;
            let tempObj = node ? node : {};
            tempObj.id = $(this).attr('nodeid');
            tempObj.key = $(this).attr('id');
            tempObj.type = $(this).attr('nodetype');
            tempObj.text = $(this).attr('nodetext');
            tempObj.nodeType = node.nodeType;
            tempObj.locTop = $(this).offset().top + scrollY;
            tempObj.locLeft = $(this).offset().left + scrollX;
            tempObj.nodeHeight = $(this).css('height');
            tempObj.nodeWidth = $(this).css('width');
            tempObj.bgColor = $(this).attr('bgColor-gradient');
            nodeDataArray.push(tempObj);
        });

        // 泳道
        $.each($(canvansContainer).children('.laneNode'), function (index) {
            let tempObj = {};
            tempObj.text = _base.laneObjs[$(this).attr('id')].text;
            tempObj.key = $(this).attr('id');
            tempObj.nodeType = _base.laneObjs[$(this).attr('id')].nodeType;
            tempObj.locTop = $(this).offset().top + scrollY;
            tempObj.locLeft = $(this).offset().left + scrollX;
            tempObj.nodeHeight = $(this).css('height');
            tempObj.nodeWidth = $(this).css('width');
            tempObj.bgColor = $(this).attr('bgColor-gradient');
            nodeDataArray.push(tempObj);
        });
        flowDoc.nodeDataArray = nodeDataArray;

        //连线
        let linkDataArray = [];
        $.each(_base.plumb.getAllConnections(), function () {
            let tempObj = {};
            tempObj.from = $(this)[0].sourceId;
            tempObj.to = $(this)[0].targetId;
            tempObj.routerId = _base.graph.edge($(this)[0].sourceId, $(this)[0].targetId).id;
            tempObj.label = plumbUtil.getRouterLabel($(this)[0].sourceId, $(this)[0].targetId);
            tempObj.sourceAnchors = _base.graph.edge($(this)[0].sourceId, $(this)[0].targetId).sourceAnchors;
            tempObj.targetAnchors = _base.graph.edge($(this)[0].sourceId, $(this)[0].targetId).targetAnchors;
            linkDataArray.push(tempObj);
        });
        flowDoc.linkDataArray = linkDataArray;

        //console.log(JSON.stringify(flowDoc));
        return flowDoc;
    };
    // 清除所有的定时器
    me.clearAllTimer = function () {
        let _base = me._base;
        let allTimer = _base.allTimer;

        // 1、点击画布时清除所有的定时器
        for (let timerName in allTimer) {
            clearTimeout(allTimer[timerName]);
        }

        _base.allTimer = {};

        // 2、移除所有连接的 connectionAnimateClass 样式
        $.each(_base.plumb.getAllConnections('*'), function () {
            $(this)[0].removeClass('connectionAnimateClass');
        });
    };
    // 清除画布，重新绘制
    me.clearCanvas = function () {
        let _base = me._base;
        $.messager.confirm("系统提示", "是否清除画布，重新绘制？",
            function (r) {
                if (r) {
                    // 1、保存状态为未保存
                    //$("#saveStatus").css('display', '');
                    // 2、将当前流程对象放入可撤销数组中
                    _base.undoStack.push(me.getCurrentFlow());

                    me.removeAll();
                }
            });
    };
    // 选中节点
    me.selectedNode = function (id) {
        let _base = me._base;

        // 1、清除所有的定时器
        me.clearAllTimer();

        if (_base.selectedNodeList.indexOf(id) == -1) {
            // 将节点的样式改为选中的样式
            $($.string.getJQSel(id)).find('div').css('box-shadow', CONFIG.defaultStyle.selectedBoxShadow);
            // 将节点选中状态改为选中
            _base.graph.node(id).isSelected = true;
            // 添加到实例对象被选中列表中
            _base.plumb.addToDragSelection(id);
            // 添加到被选中节点列表中
            _base.selectedNodeList.push(id);
        }

        // 3、多选标识改为true
        _base.selectedMultipleFlag = true;
    };
    // 取消选中节点
    me.noSelectedNode = function (id) {
        let _base = me._base;

        if (_base.selectedNodeList.indexOf(id) != -1) {
            // 将节点的样式改为非选中的样式
            $($.string.getJQSel(id)).find('div').css('box-shadow', CONFIG.defaultStyle.noSelectedBoxShadow);
            // 将节点选中状态改为选中
            _base.graph.node(id).isSelected = false;
            // 从INSTANCE_JSPLUMB被选中列表中移除
            _base.plumb.removeFromDragSelection(id);
            // 从被选中节点列表中移除
            //deleteDataFromArr(SELECTED_NODE_LIST, id);
            _base.selectedNodeList.deleteOne(id);
        }
    };
    // 除了 id 之外的节点，将节点变为非选中状态，若不传id则表示将所有的节点变为非选中状态
    me.changeToNoSelected = function (id) {
        let selector;
        let _base = me._base;

        // 清除所有jsplumb中的拖拽列表
        _base.plumb.clearDragSelection();

        // 清空被选中节点列表
        _base.selectedNodeList = [];

        if (id == undefined) {
            selector = nodeSelecter;
        } else {
            selector = nodeSelecter + ':not(#' + id + ')';
        }

        // 将节点变为非选中状态
        $.each($(selector), function () {
            $(this).find('div').css('box-shadow', CONFIG.defaultStyle.noSelectedBoxShadow);
            _base.graph.node($(this).attr('id')).isSelected = false;
        });
    };
    // 移除实例中所有节点、端点、连线，清空画布，重置节点id对象、JsPlumb实例对象、图对象
    me.removeAll = function () {
        let _base = me._base;

        // 1、移除实例中所有节点、端点、连线等，用JsPlumb提供的remove方法即可
        let nodeIds = _base.graph.nodes();
        for (let i = 0, len = nodeIds.length; i < len; i++) {
            _base.plumb.remove(nodeIds[i]);

            //if (me._base.deleteNodeFn != null)
            //    me._base.deleteNodeFn(nodeIds[i]);
        }

        // 2、清空画布
        $(canvansContainer).empty();

        // 3、重置节点id对象
        _initIdPool();

        // 4、重置图对象
        _base.graph = new graphlib.Graph();
    };
    // 显示/隐藏网格
    me.changeGrid = function () {
        if ($(canvasId).css('background-image') == 'none') {
            $(canvasId).css('background-image', 'url(../../../../../images/grid.png)');
            $(showGridId).children(':first-child').attr('class', 'fa fa-eye fa-2x iconClass showItemTxt');
            $(showGridId).children(':last-child').text('隐藏网格');
        } else {
            $(canvasId).css('background', 'none');
            $(canvasId).css('background-color', '#f8f8f8');
            $(showGridId).children(':first-child').attr('class', 'fa fa-eye-slash fa-2x iconClass showItemTxt');
            $(showGridId).children(':last-child').text('显示网格');
        }

        $(showGridId).tooltip({ content: $(showGridId).children(':last-child').text() });
    };
    // 撤销
    me.undo = function () {
        let _base = me._base;

        if (_base.undoStack.length > 0) {
            // 1、修改保存状态为未保存
            //$("#saveStatus").css('display', '');

            // 2、将当前流程图push到重做栈
            _base.redoStack.push(me.getCurrentFlow());

            // 3、加载撤销栈中的流程图前需要清除和初始化各元素
            me.removeAll();

            // 4、弹出撤销栈中的流程图并加载
            me.loadJson(_base.undoStack.pop());
        }
    };
    // 重做
    me.redo = function () {
        let _base = me._base;

        if (_base.redoStack.length > 0) {
            // 1、保存状态为未保存
            //$("#saveStatus").css('display', '');

            // 2、将当前流程图push到撤销栈
            _base.undoStack.push(me.getCurrentFlow());

            // 3、加载撤销栈中的流程图前需要清除和初始化各元素
            me.removeAll();

            // 4、弹出重做栈中的流程图并加载
            me.loadJson(_base.redoStack.pop());
        }
    };
    // 退出流程设计器
    me.exit = function () {
        let saveStatus = $('#saveStatus').css('display');
        if (saveStatus == 'block') {
            $.messager.confirm("系统提示", "是否退出流程设计器？",
                function (r) {
                    if (r) {
                        parent.window.close();
                    }
                });

        } else {
            parent.window.close();
        }
    };
    // 全选
    me.selectedAll = function () {
        let _base = me._base;

        $.each($(nodeSelecter), function (index) {
            // 1、将所有节点的样式改为选中的样式
            $(this).find('div').css('box-shadow', CONFIG.defaultStyle.selectedBoxShadow);
            // 2、清除所有的定时器
            me.clearAllTimer();
            // 3、将所有的节点选中状态改为选中
            _base.graph.node($(this).attr('id')).isSelected = true;
            // 4、添加到实例对象被选中列表中
            _base.plumb.addToDragSelection($(this).attr('id'));
            // 5、多选标识改为true
            _base.selectedMultipleFlag = true;
        });

        // 添加到被选中节点列表中，全选采用的是图对象中的被选中的节点 id 列表
        _base.selectedNodeList = _base.selectedNodeList.concat(graphUtil.getSelectedNodeIds());
    };
    // 复制
    me.copyNode = function (tempId) {
        let _base = me._base;

        // 1、清空剪贴板
        _base.myclipboard.length = 0;

        // 2、获取选中的节点id数组
        let selectedNodeIdArr = graphUtil.getSelectedNodeIds();

        // 3、将选中的节点push到剪贴板
        for (let i = 0, len = selectedNodeIdArr.length; i < len; i++) {
            // 开始节点无法被复制
            if (_base.graph.node(selectedNodeIdArr[i]).nodeType != 'start') {
                _base.myclipboard.push(selectedNodeIdArr[i]);
            }
        }
    };
    // 粘贴
    me.pasteNode = function () {
        let _base = me._base;

        // 0、剪贴板无数据直接return
        if (_base.myclipboard.length <= 0) return;

        // 1、修改保存状态为未保存，将当前流程对象放入可撤销数组中
        //$("#saveStatus").css('display', '');
        _base.undoStack.push(me.getCurrentFlow());

        // 2、获取鼠标位置
        let mousePos = _getMousePos(event);
        let top = mousePos.y;
        let left = mousePos.x;
        let tempTop = top;
        let tempLeft = left;
        let copyNodeIdArr = []; // 粘贴生成的新节点的id数组

        // 3、粘贴节点
        $.each(_base.myclipboard, function (index) {
            let newNodeId = me.createNewNode(_base.graph.node(_base.myclipboard[index]).nodeType, { 'top': top, 'left': left });
            copyNodeIdArr.push(newNodeId);
            if (index < _base.myclipboard.length - 1) {
                top = tempTop - (_base.graph.node(_base.myclipboard[0]).locTop - _base.graph.node(_base.myclipboard[index + 1]).locTop);
                left = tempLeft - (_base.graph.node(_base.myclipboard[0]).locLeft - _base.graph.node(_base.myclipboard[index + 1]).locLeft);
            }
        });

        // 4、粘贴连线
        let i, j;
        for (i = 0; i < _base.myclipboard.length; i++) {
            for (j = i + 1; j < _base.myclipboard.length; j++) {
                if (_base.graph.hasEdge(_base.myclipboard[i], _base.myclipboard[j])) {
                    // 4.1、获取粘贴的新路由的id
                    let connId = me.getNextNodeId('R');

                    // 4.2、连线
                    plumbUtil.connectNode(copyNodeIdArr[i], copyNodeIdArr[j], connId, CONFIG.anchors.defaultAnchors, CONFIG.anchors.defaultAnchors);
                }
            }
        }
    };
    // 删除节点
    me.deleteNode = function (tempId) {
        let _base = me._base;

        // 1、获取被选中的节点id数组
        let selectedNodeIdArr = graphUtil.getSelectedNodeIds();
        if (selectedNodeIdArr.length == 0) return;

        $.messager.confirm("系统提示", "确定要删除吗？",
            function (r) {
                if (r) {
                    // 2、修改保存状态为未保存，将当前流程对象放入可撤销数组中
                    //$("#saveStatus").css('display', '');
                    _base.undoStack.push(me.getCurrentFlow());

                    // 3、清除剪贴板
                    _base.myclipboard.length = 0;

                    // 4、删除
                    for (let i = 0, len = selectedNodeIdArr.length; i < len; i++) {
                        // 4.1、删除节点的端点、连线
                        graphUtil.removeNodeAndEdgesById(selectedNodeIdArr[i]);

                        // 4.2、删除id池中的id
                        _removeNodeId(selectedNodeIdArr[i]);

                        // 4.3、删除节点
                        $($.string.getJQSel(selectedNodeIdArr[i])).remove();

                        if (me._base.deleteNodeFn != null)
                            me._base.deleteNodeFn(selectedNodeIdArr[i]);
                    }
                }
            });
    };
    // 删除泳道
    me.deleteLane = function (tempId) {
        let _base = me._base;
        let id = $($.string.getJQSel(tempId)).parent().attr('id');

        // 删除泳道对象中的数据
        delete _base.laneObjs[id];

        $($.string.getJQSel(id)).remove();

        $.messager.showInfoTopCenter('系统提示', CONFIG.msg.deleteLane, 1000);
    };
    // 删除连线
    me.deleteConnection = function (connId) {
        let _base = me._base;

        $.messager.confirm("系统提示", CONFIG.msg.deleteConn,
            function (r) {
                if (r) {
                    // 1、修改保存状态为未保存
                    //$("#saveStatus").css('display', '');

                    // 2、将当前流程对象放入可撤销数组中
                    _base.undoStack.push(me.getCurrentFlow());

                    // 3、清除定时器，这里清除定时器的目的是防止显示后继路径动画时删除动画的连接会报错
                    me.clearAllTimer();

                    // 4、移除端点以及线段
                    let sourceId = $(connId).attr('sourceId');
                    let targetId = $(connId).attr('targetId');
                    let e = _base.graph.edge(sourceId, targetId);
                    if (e.sourceEndPointId != undefined) {
                        // 4.1、移除端点
                        _base.plumb.deleteEndpoint(e.sourceEndPointId);
                        _base.plumb.deleteEndpoint(e.targetEndPointId);
                        // 4.2、移除线段
                        let connDiv = _base.plumb.getConnections({
                            source: sourceId,
                            target: targetId
                        })[0];
                        _base.plumb.deleteConnection(connDiv);

                        if (me._base.deleteLinkFn != null)
                            me._base.deleteLinkFn(e.id);
                    }

                    // 5、移除图对象中的线段
                    _base.graph.removeEdge(sourceId, targetId);

                    // 6、从id池中移除该连线id
                    _removeNodeId($.string.removeJQSel(connId));
                }
            });
    };
    // 闪烁显示连线路径
    me.showConnectionRoute = function (id, type) {
        id = $.string.removeJQSel(id);
        let _base = me._base;
        let noRouteFlag = true;
        let conns, message;

        switch (type) {
            case 'front':
                conns = _base.plumb.getConnections({ target: id });
                message = CONFIG.msg.noFrontRoute;
                break;
            case 'behind':
                conns = _base.plumb.getConnections({ source: id });
                message = CONFIG.msg.noBehindRoute;
                break;
        }
        $.each(conns, function () {
            noRouteFlag = false;
            let o = $(this)[0];
            let timerName = CommonUtil.getGuid();
            let changeFlag = true;
            _base.allTimer[timerName] = setInterval(function () {
                if (changeFlag) {
                    o.addClass('connectionAnimateClass');
                    changeFlag = false;
                } else {
                    o.removeClass('connectionAnimateClass');
                    changeFlag = true;
                }
            }, 400);
        });
        // 当无后继路径时用layer的tips层进行提示
        if (noRouteFlag) {
            $($.string.getJQSel(id)).tooltip({ content: message });
        }
    };

    // 初始化节点id池
    let _initIdPool = function () {
        me._base.nodeIdPool = {
            'T': [], //人工节点、自动节点
            'E': [], //开始、结束、事件节点
            'G': [], //网关节点
            'S': [], //子流程节点
            'R': [], //路由线
            'L': []  //泳道
        }
    };
    // 记录节点id到池中
    me.recordNodeId = function (nodeId) {
        let _base = me._base;
        let prefix = nodeId.substring(0, 1);
        let v = parseInt(nodeId.substring(1));

        // 判断池中是否已经存在该id，不存在时再记录
        if (_base.nodeIdPool[prefix].indexOf(v) == -1) {
            _base.nodeIdPool[prefix].push(v);
            _base.nodeIdPool[prefix].sort(function (a, b) {
                return a - b;
            });
        }
    };
    // 获取下一个指定类型的节点id
    me.getNextNodeId = function (prefix) {
        let pool = me._base.nodeIdPool;
        let arr = pool[prefix], i;
        for (i = 1; i <= arr.length; i++) {
            if (i != arr[i - 1]) {
                break;
            }
        }
        let nextId = prefix + _addLeftZero(i);
        arr.push(i);
        arr.sort(function (a, b) {
            return a - b;
        });
        return nextId;
    };
    // 从池中移除节点id
    let _removeNodeId = function (nodeId) {
        let _base = me._base;
        let prefix = nodeId.substring(0, 1);
        let index = parseInt(nodeId.substring(1));
        let arr = _base.nodeIdPool[prefix], i;
        if (arr) {
            for (i = index - 1; i < arr.length - 1; i++) {
                arr[i] = arr[i + 1];
            }
        }
        _base.nodeIdPool[prefix].pop();
    };
    // 左补零
    let _addLeftZero = function (i) {
        let numStr = i.toString();
        let c = numStr.length - 5;
        let r = '';
        while (c < 0) {
            r += '0';
            c++;
        }
        r += numStr;
        return r;
    };

    // 根据后台返回的StartNode，生成画布所需的节点及连线数据
    let _getWorkflowDataByNodeData = function (nodeData, nodeDataArray, linkDataArray) {
        //设置图形大小
        let nodeKey = nodeData.code;
        let type = nodeData.nodeType;
        let nodeType = 'task';
        switch (nodeData.nodeType) {
            case 0://Start
                nodeType = 'start';
                //next link line
                let wfNextLink0 = {
                    "from": nodeKey,
                    "to": nodeData.nextNodeCode,
                    "routerId": CommonUtil.getGuid(),
                    "label": "",
                    "sourceAnchors": ["Bottom", "Right", "Top", "Left"],
                    "targetAnchors": ["Bottom", "Right", "Top", "Left"]
                };
                linkDataArray.push(wfNextLink0);
                break;
            case 1://Task
                nodeType = 'task';
                //next link line
                let wfNextLink1 = {
                    "from": nodeKey,
                    "to": nodeData.nextNodeCode,
                    "routerId": CommonUtil.getGuid(),
                    "label": "",
                    "sourceAnchors": ["Bottom", "Right", "Top", "Left"],
                    "targetAnchors": ["Bottom", "Right", "Top", "Left"]
                };
                linkDataArray.push(wfNextLink1);
                break;
            case 2://Condition
                nodeType = 'condition';
                //next link line
                let wfNextLink2 = {
                    "from": nodeKey,
                    "to": nodeData.nextNodeCode,
                    "routerId": CommonUtil.getGuid(),
                    "label": "真",
                    "sourceAnchors": ["Bottom", "Right", "Top", "Left"],
                    "targetAnchors": ["Bottom", "Right", "Top", "Left"]
                };
                linkDataArray.push(wfNextLink2);

                //return link line
                if (!$.string.isNullOrWhiteSpace(nodeData.returnNodeCode)) {
                    let wfReturnLink = {
                        "from": nodeData.code,
                        "to": nodeData.returnNodeCode,
                        "routerId": CommonUtil.getGuid(),
                        "label": "假",
                        "sourceAnchors": ["Top", "Left", "Bottom", "Right"],
                        "targetAnchors": ["Top", "Left", "Bottom", "Right"]
                    };
                    linkDataArray.push(wfReturnLink);
                }

                break;
            case 3://Subflow
                break;
            case 4://End
                nodeType = 'end';
                break;
        }

        let wfNode = _getRenderNodeFromType(nodeType);
        wfNode.id = nodeData.id;
        wfNode.code = nodeKey;
        wfNode.key = nodeKey;
        wfNode.text = nodeData.name;
        wfNode.type = type;
        wfNode.nodeType = nodeType;
        wfNode.locTop = nodeData.locTop;
        wfNode.locLeft = nodeData.locLeft;
        //当传入对象为：WorkflowProTask时，需要设置任务的相应属性
        let isProcessTask = nodeData.taskStatus ? true : false;
        if (isProcessTask) {
            wfNode.isProcessTask = true;
            wfNode.taskName = nodeData.name;
            wfNode.taskStatus = nodeData.taskStatus;
            wfNode.taskStatusString = nodeData.taskStatusString;
            //根据任务状态，设置任务的背景颜色
            switch (wfNode.taskStatus) {
                //未处理
                case 0:
                case '0':
                    wfNode.cla += ' UnProcess';
                    wfNode.bgColor = laneColor;
                    break;
                //处理中   
                case 1:
                case '1':
                    wfNode.cla += ' Process';
                    wfNode.bgColor = taskNodeColor;
                    break;
                //已完成
                case 2:
                case '2':
                    wfNode.cla += ' Finished';
                    wfNode.bgColor = startNodeColor;
                    break;
                //退回
                case 3:
                case '3':
                    wfNode.cla += ' Return';
                    wfNode.bgColor = endNodeColor;
                    break;
            }
            //以下数据为流程实例的任务执行数据
            if (nodeData.taskExecutes && nodeData.taskExecutes.length > 0) {
                //debugger;
                let lastExecute = $.array.last(nodeData.taskExecutes);
                wfNode.executeUserName = lastExecute.executeUserName;
                wfNode.executeDateTime = lastExecute.executeDateTime;
                wfNode.executeStatus = lastExecute.executeStatus;
                wfNode.executeStatusString = lastExecute.executeStatusString;
                wfNode.executeRemark = lastExecute.executeRemark;
                wfNode.tooltip = _getTaskNodeDetailHtml(lastExecute);
            } 
        } else {
            wfNode.isProcessTask = false;
            wfNode.taskStatus = '';
            wfNode.taskStatusString = '';
            wfNode.lastExecute = null;
            wfNode.executeUserName = '';
            wfNode.executeDateTime = '';
            wfNode.executeStatus = '';
            wfNode.executeStatusString = '';
            wfNode.executeRemark = '';
            wfNode.tooltip = '';
        }
        nodeDataArray.push(wfNode);

        //迭代
        if (nodeData.nextNode != null) {
            _getWorkflowDataByNodeData(nodeData.nextNode, nodeDataArray, linkDataArray);
        }
    };

    //渲染流程图
    let render = function () {
        // 1、加载语言环境
        if (!window.location.href.substring(0, 4) == 'file') {
            CommonUtil.loadJsonFromUrl('/FlowDesigner/json/lang/' + CONFIG.defaultConfig.language + '.json', 'GET', function (err, text) {
                if (!err) {
                    let data = JSON.parse(text);
                    CONFIG.msg = data;
                }
            });
        }

        // 2、JsPlumb初始化
        jsPlumb.ready(function () {
            // 2.1、监听流程设计器
            _registerBaseEvent();

            // 2.2、实例化jsPlumb
            plumbUtil.getInstance();

            // 2.3、选择初始工具：鼠标工具
            _mouseTool();

            // 2.4、随机生成流程id
            //me._base.flowId = CommonUtil.getGuid();
            //attrCfgUtil.setCanvasAttr();
        });
    };
    //渲染流程时绑定事件
    let _registerBaseEvent = function () {

        let _base = me._base;

        // 监听画布右键
        //window.ContextMenu.bind(canvasId, canvasMenuJson);

        // 监听节点的拖拽：jquery-easyui-draggable
        $(".controler").draggable({
            //拖拽模式为克隆
            proxy: 'clone',
            cursor: 'auto',
            //设置拖拽时鼠标位于节点中心
            deltaY: -10,
            deltaX: -30,
            //拖拽未放置到指定区域时动画还原到原位置
            revert: 'false',
            onStartDrag: function (event, ui) {
                event.target.style.fontWeight = 'bolder';
            },
            onStopDrag: function (event, ui) {
                event.target.style.fontWeight = 'normal';
            }
        });

        // 监听左侧节点颜色变化
        $(".controler").mouseover(function () {
            $(this).css('background-color', '#eee');
        }).mouseout(function () {
            $(this).css('background-color', 'transparent');
        });

        // 监听节点的放置：jquery-easyui-draggable
        $(canvansContainer).droppable({
            //标识
            scope: 'node',
            //放置触发函数
            onDrop: function (event, ui) {
                let source = $(ui);
                let draggableId = ui.firstElementChild.id;
                let proxy = source.draggable('proxy');
                if (proxy) {
                    let offset = source.draggable('proxy').offset();
                    me.createNewNode(draggableId, offset);
                }
            }
        });

        // 监听画布的点击、画多选框
        $(canvansContainer).click(function (event) {
            me.clearAllTimer();
            if (_base.isClear) {
                me.changeToNoSelected();
                // 全选标识改为 false
                _base.selectedMultipleFlag = false;
            }
            //attrCfgUtil.setCanvasAttr();
            let containId = $.string.removeJQSel(canvansContainer);
            if (me._base.nodeClickFn != null)
                me._base.nodeClickFn('canvans', _base.flowId);
        }).mousemove(function (event) {
            // 未按下鼠标时结束方法
            if (_base.px == '' || _base.py == '') {
                return;
            }

            // 移动一次获取一次矩形宽高
            let pxx = event.pageX;
            let pyy = event.pageY;
            let h = pyy - _base.py;
            let w = pxx - _base.px;
            // canvasId的相对位置
            let canvasX = $(canvasId).offset().left;
            let canvasY = $(canvasId).offset().top;

            // 滚动条的位置
            let scrollX = $(canvasId).scrollLeft();
            let scrollY = $(canvasId).scrollTop();

            // 创建矩形div，只创建一次
            if ($(multipleSelectedRectangle).attr('id') == undefined) {
                $(canvansContainer).append('<div id="' + multipleSelectedRectangleId + '" style="background-color:#31676f;"></div>');
            }

            // 画出矩形
            if (h < 0 && w >= 0) {
                $(multipleSelectedRectangle).css({ "height": (-h) + "px", "width": w + "px", "position": "absolute", "left": _base.px - canvasX + scrollX + "px", "top": pyy - canvasY + scrollY + "px", "opacity": "0.2", "border": "1px dashed #000" });
            }
            else if (h >= 0 && w < 0) {
                $(multipleSelectedRectangle).css({ "height": h + "px", "width": (-w) + "px", "position": "absolute", "left": pxx - canvasX + scrollX + "px", "top": _base.py - canvasY + scrollY + "px", "opacity": "0.2", "border": "1px dashed #000" });
            }
            else if (h < 0 && w < 0) {
                $(multipleSelectedRectangle).css({ "height": (-h) + "px", "width": (-w) + "px", "position": "absolute", "left": pxx - canvasX + scrollX + "px", "top": pyy - canvasY + scrollY + "px", "opacity": "0.2", "border": "1px dashed #000" });
            }
            else {
                $(multipleSelectedRectangle).css({ "height": h + "px", "width": w + "px", "position": "absolute", "left": _base.px - canvasX + scrollX + "px", "top": _base.py - canvasY + scrollY + "px", "opacity": "0.2", "border": "1px dashed #000" });
            }
            if (w < 0) {
                w = 0 - w;
            }
            if (h < 0) {
                h = 0 - h;
            }

            //获取矩形四个点的坐标
            let x1 = $(multipleSelectedRectangle).offset().left;
            let y1 = $(multipleSelectedRectangle).offset().top;
            let x2 = x1 + w;
            let y2 = y1;
            let x3 = x1 + w;
            let y3 = y1 + h;
            let x4 = x1;
            let y4 = y1 + h;

            //取出所有的节点，判断每一个节点是否在多选框中，若在多选框中将其状态改为选中
            let nodeArr = _base.graph.nodes(), i;
            for (i = 0; i < nodeArr.length; i++) {
                let coordinate = plumbUtil.getNodeCoordinate(nodeArr[i]);
                let flag = false;

                if ((coordinate.x11 > x1 && coordinate.y11 > y1) && (coordinate.x11 < x2 && coordinate.y11 > y2) && (coordinate.x11 < x3 && coordinate.y11 < y3) && (coordinate.x11 > x4 && coordinate.y11 < y4)) {
                    flag = true;
                }
                else if ((coordinate.x22 > x1 && coordinate.y22 > y1) && (coordinate.x22 < x2 && coordinate.y22 > y2) && (coordinate.x22 < x3 && coordinate.y22 < y3) && (coordinate.x22 > x4 && coordinate.y22 < y4)) {
                    flag = true;
                }
                else if ((coordinate.x33 > x1 && coordinate.y33 > y1) && (coordinate.x33 < x2 && coordinate.y33 > y2) && (coordinate.x33 < x3 && coordinate.y33 < y3) && (coordinate.x33 > x4 && coordinate.y33 < y4)) {
                    flag = true;
                }
                else if ((coordinate.x44 > x1 && coordinate.y44 > y1) && (coordinate.x44 < x2 && coordinate.y44 > y2) && (coordinate.x44 < x3 && coordinate.y44 < y3) && (coordinate.x44 > x4 && coordinate.y44 < y4)) {
                    flag = true;
                }
                //反向
                else if ((x1 > coordinate.x11 && y1 > coordinate.y11) && (x1 < coordinate.x22 && y1 > coordinate.y22) && (x1 < coordinate.x33 && y1 < coordinate.y33) && (x1 > coordinate.x44 && y1 < coordinate.y44)) {
                    flag = true;
                }
                else if ((x2 > coordinate.x11 && y2 > coordinate.y11) && (x2 < coordinate.x22 && y2 > coordinate.y22) && (x2 < coordinate.x33 && y2 < coordinate.y33) && (x2 > coordinate.x44 && y2 < coordinate.y44)) {
                    flag = true;
                }
                else if ((x3 > coordinate.x11 && y3 > coordinate.y11) && (x3 < coordinate.x22 && y3 > coordinate.y22) && (x3 < coordinate.x33 && y3 < coordinate.y33) && (x3 > coordinate.x44 && y3 < coordinate.y44)) {
                    flag = true;
                }
                else if ((x4 > coordinate.x11 && y4 > coordinate.y11) && (x4 < coordinate.x22 && y4 > coordinate.y22) && (x4 < coordinate.x33 && y4 < coordinate.y33) && (x4 > coordinate.x44 && y4 < coordinate.y44)) {
                    flag = true;
                }
                //中间横
                else if ((x1 > coordinate.x11 && y1 < coordinate.y11) && (x2 < coordinate.x22 && y2 < coordinate.y22) && (x3 < coordinate.x33 && y3 > coordinate.y33) && (x4 > coordinate.x44 && y4 > coordinate.y44)) {
                    flag = true;
                }
                //中间竖
                else if ((coordinate.x11 > x1 && coordinate.y11 < y1) && (coordinate.x22 < x2 && coordinate.y22 < y2) && (coordinate.x33 < x3 && coordinate.y33 > y3) && (coordinate.x44 > x4 && coordinate.y44 > y4)) {
                    flag = true;
                }

                if (flag) {
                    _base.isClear = false;
                    me.selectedNode(nodeArr[i]);
                } else {
                    me.noSelectedNode(nodeArr[i]);
                }
            }

            if (_base.selectedNodeList.length > 0) {
                _base.selectedNodeList.length = 0;
                _base.selectedNodeList = _base.selectedNodeList.concat(graphUtil.getSelectedNodeIds());
            }

        }).mouseup(function () {
            //松开鼠标初始化，移除多选框
            _base.px = '';
            _base.py = '';
            $(multipleSelectedRectangle).remove();
        });

        // 监听导航条
        let tempIndex;
        $('.showItemTxt').mouseover(function (event) {
            //debugger;
            $(this).parent().tooltip({ content: $(this).next().text() });
        }).mouseout(function (event) {
            //layer.close(tempIndex);
        });

        // 监听键盘
        $(document).keydown(function (event) {
            // 单键快捷键
            switch (event.which) {
                case CONFIG.keyboardParam.multipleSelectedKey: // ctrl 键
                    if (!_base.allowMultipleSelectedFlag) {
                        _base.allowMultipleSelectedFlag = true;
                    }
                    break;
                case CONFIG.keyboardParam.deleteKey: // delete 键
                    me.deleteNode();
                    break;
                case CONFIG.keyboardParam.upKey: // 上
                    event.preventDefault();
                    _smallMove('up');
                    break;
                case CONFIG.keyboardParam.downKey: // 下
                    event.preventDefault();
                    _smallMove('down');
                    break;
                case CONFIG.keyboardParam.leftKey: // 左
                    event.preventDefault();
                    _smallMove('left');
                    break;
                case CONFIG.keyboardParam.rightKey: // 右
                    event.preventDefault();
                    _smallMove('right');
                    break;
            }

            // Ctrl + ... 快捷键
            if (event.ctrlKey == true) {
                if (event.which == CONFIG.keyboardParam.undoKey) {
                    // 撤销ctrl + Z
                    me.undo();
                } else if (event.which == CONFIG.keyboardParam.redoKey) {
                    //重做ctrl + Y
                    me.redo();
                } else if (event.which == CONFIG.keyboardParam.selectedAllKey) {
                    // 全选ctrl + A
                    event.preventDefault(); // 禁用浏览器的全选
                    me.selectedAll();
                } else if (event.which == CONFIG.keyboardParam.saveKey) {
                    // 保存ctrl + S
                    event.preventDefault();
                    me.save();
                } else if (event.which == CONFIG.keyboardParam.save2PhotoKey) {
                    // 保存为图片ctrl + P
                    event.preventDefault();
                    me.save2Photo();
                } else if (event.which == CONFIG.keyboardParam.clearKey) {
                    // 重新绘制ctrl + D
                    event.preventDefault();
                    me.clearCanvas();
                } else if (event.which == CONFIG.keyboardParam.showGridKey) {
                    // 显示、隐藏网格ctrl + Q
                    event.preventDefault();
                    me.changeGrid();
                } else if (event.which == 82) {
                    // 禁用浏览器ctrl + R刷新功能
                    event.preventDefault();
                } else if (event.which == CONFIG.keyboardParam.settingKey) {
                    // 打开设置窗口ctrl + F
                    event.preventDefault();
                    layuiUtil.setting();
                } else if (event.which == 67) {
                    // 复制ctrl + C
                    event.preventDefault();
                } else if (event.which == 86) {
                    // 粘贴ctrl + V
                    event.preventDefault();
                } else if (event.which == 76) {
                    // 直接加载json数据为流程图(测试)ctrl + L
                    layuiUtil.test();
                }
            }

            // Alt + ... 快捷键
            if (event.altKey == true) {
                if (event.which == CONFIG.keyboardParam.mouseToolKey) {
                    // 鼠标工具Alt + Q
                    event.preventDefault();
                    _mouseTool();
                } else if (event.which == CONFIG.keyboardParam.connectionToolKey) {
                    // 连线工具Alt + R
                    event.preventDefault();
                    _connectionTool();
                }
            }
        }).keyup(function (event) {
            switch (event.which) {
                case CONFIG.keyboardParam.multipleSelectedKey: //ctrl 键
                    _base.allowMultipleSelectedFlag = false;
                    break;
                case CONFIG.keyboardParam.upKey: // 上
                    _smallMoveHandler();
                    break;
                case CONFIG.keyboardParam.downKey: // 下
                    _smallMoveHandler();
                    break;
                case CONFIG.keyboardParam.leftKey: // 左
                    _smallMoveHandler();
                    break;
                case CONFIG.keyboardParam.rightKey: // 右
                    _smallMoveHandler();
                    break;
            }
        });

        // 监听连线工具
        jsPlumb.on($('#enableDraggableDiv'), 'click', function (e) {
            _connectionTool();
        });

        // 监听鼠标工具
        jsPlumb.on($('#unableDraggableDiv'), 'click', function (e) {
            _mouseTool();
        });
    };
    // 切换为鼠标工具
    let _mouseTool = function () {

        let _base = me._base;

        // 切换显示
        $(mouseToolsBtn).css('color', '#0000FF');
        $(mouseToolsBtn).parent().css('background-color', '#eee');
        $(connectionToolsBtn).css('color', '#444444');
        $(connectionToolsBtn).parent().css('background-color', 'transparent');

        // 修改鼠标样式
        $(canvansContainer).css('cursor', 'default');

        // 鼠标工具可以使用多选框
        let $events = $._data($(canvansContainer)[0], 'events');
        if (!$events || !$events['mousedown']) {
            $(canvansContainer).bind('mousedown', function (event) {
                //在画布中按下鼠标获取鼠标位置
                _base.px = event.pageX;
                _base.py = event.pageY;
                _base.isClear = true;
            });
        }

        // 鼠标工具可以移动画布中的节点
        let nodeArr = _base.graph.nodes();
        $.each(nodeArr, function (index) {
            plumbUtil.ableDraggable(nodeArr[index]);
            // 修改鼠标样式
            $('#' + nodeArr[index]).css('cursor', 'move');
            _base.plumb.unmakeSource(nodeArr[index]);
            _base.plumb.unmakeTarget(nodeArr[index]);
        });
    };
    // 切换为连线工具
    let _connectionTool = function () {
        // 切换显示
        $(mouseToolsBtn).css('color', '#444444');
        $(mouseToolsBtn).parent().css('background-color', 'transparent');
        $(connectionToolsBtn).css('color', '#0000FF');
        $(connectionToolsBtn).parent().css('background-color', '#eee');

        // 修改鼠标样式
        $(canvansContainer).css('cursor', 'crosshair');

        // 连线工具无法使用多选框
        let $events = $._data($(canvansContainer)[0], 'events');
        if ($events && $events['mousedown']) {
            $(canvansContainer).unbind('mousedown');
        }

        // 连线工具可以连接画布中的节点
        let _base = me._base;
        let nodeArr = _base.graph.nodes();
        $.each(nodeArr, function (index) {
            plumbUtil.unableDraggable(nodeArr[index]);
            //修改鼠标样式
            $('#' + nodeArr[index]).css('cursor', 'crosshair');
            _base.plumb.makeSource(nodeArr[index], {
                filter: "a",
                filterExclude: true,
                maxConnections: -1,
                endpoint: ["Dot", { radius: 7 }],
                anchor: CONFIG.anchors.defaultAnchors
            });
            _base.plumb.makeTarget(nodeArr[index], {
                filter: "a",
                filterExclude: true,
                maxConnections: -1,
                endpoint: ["Dot", { radius: 7 }],
                anchor: CONFIG.anchors.defaultAnchors
            });
        });
    };
    // 微移
    let _smallMove = function (moveType) {
        let _base = me._base;
        let t, l, movePX = CONFIG.defaultConfig.smallMovePX;

        // 记录在移动前的流程图
        if (_base.tempFlag) {
            _base.tempFlow = me.getCurrentFlow();
            _base.tempFlag = false;
        }

        switch (moveType) {
            case 'up':
                t = -movePX;
                l = 0;
                break;
            case 'down':
                t = movePX;
                l = 0;
                break;
            case 'left':
                t = 0;
                l = -movePX;
                break;
            case 'right':
                t = 0;
                l = movePX;
                break;
        }
         
        // 获取被选中的节点id列表
        let selectedArr = graphUtil.getSelectedNodeIds();
        // 移动每一个被选中的元素
        for (let i = 0, len = selectedArr.length; i < len; i++) {
            let newTop = $($.string.getJQSel(selectedArr[i])).offset().top + t;
            let newLeft = $($.string.getJQSel(selectedArr[i])).offset().left + l;
            $($.string.getJQSel(selectedArr[i])).offset({ top: newTop, left: newLeft });

            // 更新图对象
            _base.graph.node(selectedArr[i]).locTop = newTop;
            _base.graph.node(selectedArr[i]).locLeft = newLeft;

            // 产生了微移，将微移标识改为true
            _base.isSmallMove = true;
        }

        // 重绘
        _base.plumb.repaintEverything();
    };
    // 流程图产生微移后的处理
    let _smallMoveHandler = function () {
        let _base = me._base;

        _base.tempFlag = true;

        if (_base.isSmallMove) {
            // 修改保存状态为未保存，将当前流程对象放入可撤销数组中
            //$("#saveStatus").css('display', '');
            _base.undoStack.push(_base.tempFlow);
            _base.isSmallMove = false;
        }
    };
    // 获取鼠标位置(相对于浏览器窗口)
    let _getMousePos = function (event) {
        event = document.all ? window.event : arguments[0] ? arguments[0] : event;
        return {
            'x': event.pageX,
            'y': event.pageY
        };
    };
    // 根据十六进制颜色获取节点渐变背景样式
    let _getNodeLinearBgFromHex = function (hex) {
        let nodeBg = 'linear-gradient(to right, ' + hex + 'e6, ' + hex + 'cc, ' + hex + 'e6)';
        return nodeBg;
    };
    // 获取任务节点执行的详细情况html
    let _getTaskNodeDetailHtml = function (lastExecute) {
        if (!lastExecute) return '';

        let resultHtml = '';
        resultHtml += '标    题：' + lastExecute.taskName + '</br>';
        resultHtml += '执 行 者：' + lastExecute.executeUserName + '</br>';
        resultHtml += '执行时间：' + lastExecute.executeDateTime + '</br>';
        resultHtml += '状    态：' + lastExecute.executeStatusString + '</br>';
        resultHtml += '处理意见：' + lastExecute.executeRemark + '</br>';
        return resultHtml;
    };
};