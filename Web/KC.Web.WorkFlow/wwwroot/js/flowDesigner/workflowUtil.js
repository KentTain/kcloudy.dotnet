
var plumbUtil = {
    _tempConnObj: {}, // 临时的连线对象
    // 实例化jsPlumb
    getInstance: function () {
        let _base = myWORKFLOW._base;

        // 1、获取实例，设置相关属性
        _base.plumb = jsPlumb.getInstance({
            /**
             * 设置连线
             * Bezier(贝塞尔曲线，默认)、Straight(直线)、Flowchart(流程图线)、StateMachine(状态线)
             */
            Connector: [
                CONFIG.conn.connectionType, // 连线类型
                {
                    gap: CONFIG.conn.connectionGap, // 连线到端点的距离
                    cornerRadius: CONFIG.conn.connectionCornerRadius, // 连线圆角程度
                    alwaysRespectStubs: CONFIG.conn.connectionAlwaysRespectStubs
                }
            ],
            /**
             * 给连线添加箭头，写在配置中的名字为 connectorOverlays，写在连接中的名字为 overlays
             * location 0.5 表示箭头位于中间，location 1 表示箭头设置在连线末端。 一根连线是可以添加多个箭头的。
             */
            ConnectionOverlays: [
                [
                    'Arrow',
                    {
                        width: CONFIG.arrow.arrowWidth,
                        length: CONFIG.arrow.arrowLength,
                        location: CONFIG.arrow.arrowLocation
                    }
                ]
            ],
            /**
             * 设置连线样式
             */
            PaintStyle: {
                stroke: CONFIG.conn.stroke, // 连线颜色
                strokeWidth: CONFIG.conn.strokeWidth // 连线粗细
            },
            /**
             * 鼠标悬浮在连接线上的样式
             */
            HoverPaintStyle: {
                stroke: CONFIG.conn.hoverConnStroke // 悬浮颜色
            },
            /**
             * 设置端点样式
             */
            EndpointStyle: {
                fill: CONFIG.endPonit.fill, // 填充色
                stroke: CONFIG.endPonit.stroke, // 外框色
                strokeWidth: CONFIG.endPonit.strokeWidth, // 外框厚度
                radius: CONFIG.endPonit.radius // 半径
            },
            /**
             * 鼠标悬浮在端点上的样式
             */
            EndpointHoverStyle: {
                fill: CONFIG.endPonit.hoverEndPointStroke // 悬浮颜色
            }
        });

        // 2、建立连接前触发
        _base.plumb.bind('beforeDrop', function (info) {
            let _base = myWORKFLOW._base;
            let sourceId = info.sourceId;
            let targetId = info.targetId;

            // 2.1、出现自连接直接断开
            if (sourceId == targetId) return false;

            // 2.2、相同两节点不能出现有方向的重复连接
            if (_base.graph.hasEdge(sourceId, targetId)) {
                let showTipId = $.string.getJQSel(sourceId);
                $(showTipId).tooltip({ content: CONFIG.msg.repeatRouter });
                return false;
            }

            // 2.2、当有Condition节点时，只能设置连线的label为：一真、一假
            let source = $(info.connection.source);
            let target = $(info.connection.target);
            if (source.attr('nodetype') === '2') {
                let flowData = myWORKFLOW.getCurrentFlow();
                let linkData = flowData.linkDataArray;
                let linkRows = linkData.filter(function (link, index, array) {
                    return link.from === info.sourceId;
                });
                if (linkRows.length >= 2) {
                    let showTipId = $.string.getJQSel(sourceId);
                    $(showTipId).tooltip({ content: CONFIG.msg.repeatRouter });
                    return false;
                }
            }
            // 2.3、修改保存状态为未保存
            //$("#saveStatus").css('display', '');

            // 2.4、将当前流程图push到撤销栈
            _base.undoStack.push(myWORKFLOW.getCurrentFlow());

            // 2.5、从id池中拿连线id
            let connId = myWORKFLOW.getNextNodeId('R');

            // 2.6、连接之前赋值临时的连线对象，用于连线后触发plumb绑定的connection事件
            plumbUtil._tempConnObj.sourceId = sourceId;
            plumbUtil._tempConnObj.targetId = targetId;
            plumbUtil._tempConnObj.routerId = connId;

            // 2.7、将连线记录到图对象中
            let sourceEndPointId = sourceId + '-' + CommonUtil.getGuid();
            let targetEndPointId = targetId + '-' + CommonUtil.getGuid();
            let sourceAnchors = CONFIG.anchors.defaultAnchors;
            let targetAnchors = CONFIG.anchors.defaultAnchors;
            let edge = new graphUtil.Edge(connId, sourceEndPointId, targetEndPointId, sourceAnchors, targetAnchors);
            graphUtil.addEdge(sourceId, targetId, edge);

            return true;
        });

        // 3、建立连接后触发
        _base.plumb.bind('connection', function (connObj, b) {
            let o = plumbUtil._tempConnObj;
            let selector = $.string.getJQSel(o.routerId);

            // 3.1、连线id
            connObj.connection.canvas.id = o.routerId;

            // 3.2、存储源节点和目标节点的id
            $(selector).attr('sourceId', o.sourceId);
            $(selector).attr('targetId', o.targetId);

            // 3.3、监听连线
            //window.ContextMenu.bind(selector, connectionMenuJson);
            $(selector).click(function (event) {
                if (_base.linkClickFn !== null) {
                    _base.linkClickFn('link', $(this).attr('id'));
                }
                event = document.all ? window.event : arguments[0] ? arguments[0] : event;
                event.stopPropagation();
                //attrCfgUtil.setConnAttr($(selector).attr('sourceId'), $(selector).attr('targetId'));
            });

            // 3.4、记录连线id
            myWORKFLOW.recordNodeId(o.routerId);

            // 3.5、当有Condition节点时，需要设置连线的label为：真或假
            let source = $(connObj.source);
            let target = $(connObj.target);
            if (source.attr('nodetype') === '2') {
                let flowData = myWORKFLOW.getCurrentFlow();
                let linkData = flowData.linkDataArray;
                let linkRows = linkData.filter(function (link, index, array) {
                    return link.from === o.sourceId && link.label === "真";
                });
                if (linkRows != undefined
                    && linkRows != null
                    && linkRows.length > 0) {
                    plumbUtil.setRouterLabel(o.sourceId, o.targetId, '假');
                } else {
                    plumbUtil.setRouterLabel(o.sourceId, o.targetId, '真');
                }
            } 
        });

        // 4、设置连接是否可以被拉断
        _base.plumb.importDefaults({
            ConnectionsDetachable: CONFIG.conn.isDetachable
        });
    },
    // 给节点添加端点，返回端点id
    addEndPoint: function (nodeId, anchors) {
        let _base = myWORKFLOW._base;
        let endPointId = nodeId + '-' + CommonUtil.getGuid();

        plumbUtil.lazyExecute(function () {
            _base.plumb.addEndpoint(nodeId, {
                uuid: endPointId,
                anchors: anchors
            });
        });

        return endPointId;
    },
    // 连接两个节点
    connectNode: function (sourceId, targetId, routerId, sourceAnchors, targetAnchors) {
        let _base = myWORKFLOW._base;

        // 1、新增端点，一条连接线两个端点
        let sourceEndPointId = plumbUtil.addEndPoint(sourceId, sourceAnchors);
        let targetEndPointId = plumbUtil.addEndPoint(targetId, targetAnchors);

        // 2、连接
        plumbUtil.lazyExecute(function () {
            // 2.1、连接之前赋值临时的连线对象，用于连线后触发plumb绑定的connection事件
            plumbUtil._tempConnObj.sourceId = sourceId;
            plumbUtil._tempConnObj.targetId = targetId;
            plumbUtil._tempConnObj.routerId = routerId;

            // 2.2、连线
            let conn = _base.plumb.connect({
                // 通过编码连接endPoint，连线不会混乱
                uuids: [sourceEndPointId, targetEndPointId]
                // 源节点
                //source: id1,
                // 目标节点
                //target: id2,
                // 端点
                //endpoint: 'Rectangle'
            });
            conn.unbind("mouseover");
            conn.unbind("mouseout");
        });

        // 3、将连线添加到图对象中
        let edge = new graphUtil.Edge(routerId, sourceEndPointId, targetEndPointId, sourceAnchors, targetAnchors);
        graphUtil.addEdge(sourceId, targetId, edge);
    },
    // 设置节点可以被移动
    setNodeDraggable: function (nodeId) {
        let scrollX;
        let scrollY;
        let _base = myWORKFLOW._base;

        plumbUtil.lazyExecute(function () {
            _base.plumb.draggable(nodeId, {
                filter: ".enableDraggable",
                containment: 'parent',
                // grid: [10, 10],
                // 拖拽前记录当前的流程文档对象
                start: function () {
                    _base.undoStack.push(myWORKFLOW.getCurrentFlow());
                },
                // 拖拽过程中实时更新节点位置
                drag: function (event) {
                    // canvasId的相对位置
                    let canvasX = $('#canvasId').offset().left;
                    let canvasY = $('#canvasId').offset().top;
                    // 当前滚动条位置
                    scrollX = $('#canvasId').scrollLeft();
                    scrollY = $('#canvasId').scrollTop();
                    if (!_base.selectedMultipleFlag) {
                        let msg = 'X: ' + parseInt($('#' + event.el.id).offset().left - canvasX + scrollX) + '  Y: ' + parseInt($('#' + event.el.id).offset().top - canvasY + scrollY);
                        let showTipId = $.string.getJQSel(event.el.id);
                        $(showTipId).tooltip({ content: msg });
                    }
                },
                // 拖拽结束后更新图对象中存储的节点位置
                stop: function (event) {
                    // 更新图对象
                    let id = event.el.id;
                    graphUtil.updateNode(id);
                    let x = $('#' + id).offset().left + scrollX;
                    let y = $('#' + id).offset().top + scrollY;
                    let node = _base.graph.node(id);
                    node.locLeft = x;
                    node.locTop = y;
                }
            });
        });
    },
    // 允许节点被移动
    ableDraggable: function (nodeId) {
        let _base = myWORKFLOW._base;
        let flag = _base.plumb.toggleDraggable(nodeId);
        if (!flag) {
            _base.plumb.toggleDraggable(nodeId);
        }
    },
    // 禁止节点被移动
    unableDraggable: function (nodeId) {
        let _base = myWORKFLOW._base;
        let flag = _base.plumb.toggleDraggable(nodeId);
        if (flag) {
            _base.plumb.toggleDraggable(nodeId);
        }
    },
    // 获取连线文本
    getRouterLabel: function (sourceId, targetId) {
        let _base = myWORKFLOW._base;

        let routerLabel = _base.plumb.getConnections({
            source: $.string.removeJQSel(sourceId),
            target: $.string.removeJQSel(targetId)
        })[0].getLabel();
        if ($.string.isNullOrWhiteSpace(routerLabel)) return '';
        return routerLabel;
    },
    // 设置连线文本
    setRouterLabel: function (sourceId, targetId, label) {
        let _base = myWORKFLOW._base;

        plumbUtil.lazyExecute(function () {
            if ($.string.isNullOrWhiteSpace(label)) {
                // 1、文本为空，首先获取连接
                let c = _base.plumb.getConnections({
                    source: $.string.removeJQSel(sourceId),
                    target: $.string.removeJQSel(targetId)
                })[0];

                // 2、获取文本覆盖物，判断是否存在，存在时移除文本覆盖物
                let labelOverlay = c.getLabelOverlay();
                if (labelOverlay != undefined) {
                    c.removeOverlay(labelOverlay.id);
                }
            } else {
                // 文本不为空
                _base.plumb.getConnections({
                    source: sourceId,
                    target: targetId
                })[0].setLabel({
                    label: label,
                    cssClass: 'labelClass'
                });
            }
        });
    },
    // 对齐方式检查
    alignWayCheck: function () {
        let _base = myWORKFLOW._base;

        // 1、检查选中的节点个数是否大于等于2
        if (_base.selectedNodeList.length < 2) {
            $.messager.showInfoTopCenter('系统提示', CONFIG.msg.alignWayCheck, 2000);
            return null;
        }
        return _base.selectedNodeList;
    },
    // 垂直居中
    verticalCenter: function (selectedNodeIdArr) {
        let _base = myWORKFLOW._base;

        // 1、第一个选中的节点的初始值，其余节点以此为基准
        let topCount = parseInt($('#' + selectedNodeIdArr[0]).css('top'));
        let leftCount = parseInt($('#' + selectedNodeIdArr[0]).css('left'));
        let leftTemp = leftCount;

        // 2、对齐被选中的节点
        for (let i = 1; i < selectedNodeIdArr.length; i++) {
            // 下一个节点的 top 是上一个节点的 top 加上上一个节点的 height 加上垂直间距
            topCount = topCount + parseInt($('#' + selectedNodeIdArr[i - 1]).css('height')) + CONFIG.alignParam.verticalDistance;
            // 下一个节点的 left 是第一个节点的 width 减去下一个节点的 width 的一半加上第一个节点的left
            leftCount = leftTemp + (parseInt($('#' + selectedNodeIdArr[0]).css('width')) - parseInt($('#' + selectedNodeIdArr[i]).css('width'))) / 2;
            // 动画效果移动节点到 topCount、leftCount 的位置，动画持续时间为 500ms
            _base.plumb.animate(selectedNodeIdArr[i], {
                top: topCount,
                left: leftCount
            }, { duration: CONFIG.alignParam.alignDuration });
        }
    },
    // 左对齐
    leftAlign: function (selectedNodeIdArr) {
        let _base = myWORKFLOW._base;

        // 1、第一个选中的节点的初始值，其余节点以此为基准
        let topCount = parseInt($('#' + selectedNodeIdArr[0]).css('top'));
        let leftCount = parseInt($('#' + selectedNodeIdArr[0]).css('left'));

        // 2、对齐被选中的节点
        for (let i = 1; i < selectedNodeIdArr.length; i++) {
            // 下一个节点的 top 是上一个节点的 top 加上上一个节点的 height 加上垂直间距
            topCount = topCount + parseInt($('#' + selectedNodeIdArr[i - 1]).css('height')) + CONFIG.alignParam.verticalDistance;
            // 下一个节点的 left 是第一个节点的 left
            // leftCount = leftCount;
            // 动画效果移动节点到 topCount、leftCount 的位置，动画持续时间为 500ms
            _base.plumb.animate(selectedNodeIdArr[i], {
                top: topCount,
                left: leftCount
            }, { duration: CONFIG.alignParam.alignDuration });
        }
    },
    // 右对齐
    rightAlign: function (selectedNodeIdArr) {
        let _base = myWORKFLOW._base;

        // 1、第一个选中的节点的初始值，其余节点以此为基准
        let topCount = parseInt($('#' + selectedNodeIdArr[0]).css('top'));
        let leftCount = parseInt($('#' + selectedNodeIdArr[0]).css('left'));
        let leftCountTemp = leftCount;

        // 2、对齐被选中的节点
        for (let i = 1; i < selectedNodeIdArr.length; i++) {
            // 下一个节点的 top 是上一个节点的 top 加上上一个节点的 height 加上垂直间距
            topCount = topCount + parseInt($('#' + selectedNodeIdArr[i - 1]).css('height')) + CONFIG.alignParam.verticalDistance;
            // 下一个节点的 left 是第一个节点的 left 加上第一个节点的 width 减去下一个节点的 width
            leftCount = leftCountTemp + (parseInt($('#' + selectedNodeIdArr[0]).css('width')) - parseInt($('#' + selectedNodeIdArr[i]).css('width')));
            // 动画效果移动节点到 topCount、leftCount 的位置，动画持续时间为 500ms
            _base.plumb.animate(selectedNodeIdArr[i], {
                top: topCount,
                left: leftCount
            }, { duration: CONFIG.alignParam.alignDuration });
        }
    },
    // 水平居中
    levelAlign: function (selectedNodeIdArr) {
        let _base = myWORKFLOW._base;

        // 1、第一个选中的节点的初始值，其余节点以此为基准
        let topCount = parseInt($('#' + selectedNodeIdArr[0]).css('top'));
        let topCountTemp = topCount;
        let leftCount = parseInt($('#' + selectedNodeIdArr[0]).css('left'));

        // 2、对齐被选中的节点
        for (let i = 1; i < selectedNodeIdArr.length; i++) {
            // 下一个节点的 top 是第一个节点的 height 减去下一个节点的 height 的一半加上第一个节点的top
            topCount = topCountTemp + (parseInt($('#' + selectedNodeIdArr[0]).css('height')) - parseInt($('#' + selectedNodeIdArr[i]).css('height'))) / 2;
            // 下一个节点的 left 是上一个节点的 left 加上上一个节点的 width 加上水平间距
            leftCount = leftCount + parseInt($('#' + selectedNodeIdArr[i - 1]).css('width')) + CONFIG.alignParam.levelDistance;
            // 动画效果移动节点到 topCount、leftCount 的位置，动画持续时间为 500ms
            _base.plumb.animate(selectedNodeIdArr[i], {
                top: topCount,
                left: leftCount
            }, { duration: CONFIG.alignParam.alignDuration });
        }
    },
    // 上对齐
    upAlign: function (selectedNodeIdArr) {
        let _base = myWORKFLOW._base;

        // 1、第一个选中的节点的初始值，其余节点以此为基准
        let topCount = parseInt($('#' + selectedNodeIdArr[0]).css('top'));
        let leftCount = parseInt($('#' + selectedNodeIdArr[0]).css('left'));

        // 2、对齐被选中的节点
        for (let i = 1; i < selectedNodeIdArr.length; i++) {
            // 下一个节点的 top 是第一个节点的 top
            // topCount = topCount;
            // 下一个节点的 left 是上一个节点的 left 加上一个节点的 width 加上水平间距
            leftCount = leftCount + parseInt($('#' + selectedNodeIdArr[i - 1]).css('width')) + CONFIG.alignParam.levelDistance;
            // 动画效果移动节点到 topCount、leftCount 的位置，动画持续时间为 500ms
            _base.plumb.animate(selectedNodeIdArr[i], {
                top: topCount,
                left: leftCount
            }, { duration: CONFIG.alignParam.alignDuration });
        }
    },
    // 下对齐
    downAlign: function (selectedNodeIdArr) {
        let _base = myWORKFLOW._base;

        // 1、第一个选中的节点的初始值，其余节点以此为基准
        let topCount = parseInt($('#' + selectedNodeIdArr[0]).css('top'));
        let topCountTemp = topCount;
        let leftCount = parseInt($('#' + selectedNodeIdArr[0]).css('left'));

        // 2、对齐被选中的节点
        for (let i = 1; i < selectedNodeIdArr.length; i++) {
            // 下一个节点的 top 是第一个节点的 top 加上第一个节点的 height 减去下一个节点的 height
            topCount = topCountTemp + (parseInt($('#' + selectedNodeIdArr[0]).css('height')) - parseInt($('#' + selectedNodeIdArr[i]).css('height')));
            // 下一个节点的 left 是上一个节点的 left 加上一个节点的 width 加上水平间距
            leftCount = leftCount + parseInt($('#' + selectedNodeIdArr[i - 1]).css('width')) + CONFIG.alignParam.levelDistance;
            // 动画效果移动节点到 topCount、leftCount 的位置，动画持续时间为 500ms
            _base.plumb.animate(selectedNodeIdArr[i], {
                top: topCount,
                left: leftCount
            }, { duration: CONFIG.alignParam.alignDuration });
        }
    },
    // 设置节点可以被缩放
    nodeResizable: function (id) {
        let _base = myWORKFLOW._base;
        id = $.string.getJQSel(id);

        $(id).resizable({
            // 设置允许元素调整的最小高度
            minHeight: 50,
            // 设置允许元素调整的最小宽度
            minWidth: 100,
            // 设置允许元素调整的最大高度
            //maxHeight: 300,
            // 设置允许元素调整的最大宽度
            //maxWidth: 600,
            // 缩放时保持纵横比
            //aspectRatio: 1/1,
            // 缩放时的动画
            animate: true,
            //动画效果种类
            animateEasing: 'easeOutElastic',
            //动画效果持续时间
            animateDuration: 500,
            //缩放时的视觉反馈
            ghost: true,
            //默认隐藏掉可调整大小的手柄，除非鼠标移至元素上
            autoHide: true,
            //缩放结束后需要重新设置节点文字样式、重绘流程图，这个地方需要用到计时器，等动画结束之后重绘。更新图对象
            stop: function (event, ui) {
                var $this = $(this);
                setTimeout(function () {
                    $this.css('line-height', $this.css('height'));
                    repaintAll();
                    // 更新图对象
                    graphUtil.updateNode($this.attr('id'));
                }, 510);
            }
        });

        //设置节点可缩放后样式被改成了 relative，这里需要再次设置为 absolute
        $(id).css('position', 'absolute');
    },
    // 设置泳道可被缩放
    laneResizable: function (id) {
        let _base = myWORKFLOW._base;
        id = $.string.getJQSel(id);

        $(id).resizable({
            // 设置允许元素调整的最小高度
            minHeight: 150,
            // 设置允许元素调整的最小宽度
            minWidth: 200,
            // 设置允许元素调整的最大高度
            // maxHeight: 300,
            // 设置允许元素调整的最大宽度
            // maxWidth: 600,
            // 缩放时保持纵横比
            // aspectRatio: 1/1,
            // 缩放时的动画
            animate: true,
            // 动画效果种类
            animateEasing: 'easeOutElastic',
            // 动画效果持续时间
            animateDuration: 300,
            // 缩放时的视觉反馈
            ghost: true,
            // 默认隐藏掉可调整大小的手柄，除非鼠标移至元素上
            autoHide: true,
            // 缩放开始时设置两个值防止缩放过程中出现多选框
            start: function (event, ui) {
                px = '';
                py = '';
            },
            // 缩放结束后需要重新设置节点文字样式、重绘流程图，这个地方需要用到计时器，等动画结束之后重绘。更新泳道对象
            stop: function (event, ui) {
                let $this = $(this);
                let thisChildId = $this.children(':first-child')[0].id;
                let thisGraphNode = _base.laneObjs[$this.attr('id')];
                setTimeout(function () {
                    if (thisGraphNode.nodeType == 'broadwiseLane') {
                        $($.string.getJQSel(thisChildId)).css({
                            'height': parseInt($this.css('height')) - 3,
                            'line-height': plumbUtil.getLaneLineHeight(thisGraphNode.text, $('#' + thisChildId).css('height'))
                        });
                    }
                    // 更新泳道对象
                    graphUtil.updateLaneObjs($this.attr('id'));
                }, 310);
            }
        });

        // 设置节点可缩放后样式被改成了 relative，这里需要再次设置为 absolute
        $(id).css('position', 'absolute');
    },
    // 延时30ms执行
    lazyExecute: function (f) {
        setTimeout(function () {
            f();
        }, 50);
    },
    // 根据div的高度算出字行距实现竖排字自动排版
    getLaneLineHeight: function (text, height) {
        let textArr = text.split(''), i = textArr.length, h = parseInt(height);
        return h / (i * 15);
    },
    // 获取节点四个角坐标
    getNodeCoordinate: function (nodeId) {
        nodeId = $.string.getJQSel(nodeId);
        var x11 = $(nodeId).offset().left;
        var y11 = $(nodeId).offset().top;
        var x22 = x11 + parseInt($(nodeId).css('width'));
        var y22 = y11;
        var x33 = x11 + parseInt($(nodeId).css('width'));
        var y33 = y11 + parseInt($(nodeId).css('height'));
        var x44 = x11;
        var y44 = y11 + parseInt($(nodeId).css('height'));

        return {
            x11: x11,
            y11: y11,
            x22: x22,
            y22: y22,
            x33: x33,
            y33: y33,
            x44: x44,
            y44: y44
        };
    }
}

var graphUtil = {
    // 节点的构造函数
    Node: function (id, type, nodeType, text, cla, bgColor, icon, height, width) {
        this.key = id;
        this.type = type;
        this.nodeType = nodeType;
        this.text = text;
        this.cla = cla;
        this.bgColor = bgColor;
        this.icon = icon;
        this.nodeHeight = height;
        this.nodeWidth = width;

        //以下数据为流程实例的任务执行数据
        this.isProcessTask = false;
        this.taskStatus = 0;
        this.taskStatusString = '未处理';
        this.executeUserName = '';
        this.executeUserName = '';
        this.executeDateTime = '';
        this.executeStatus = '';
        this.executeStatusString = '';
        this.executeRemark = '';
    },
    // 连线的构造函数
    Edge: function (routerId, sourceEndPointId, targetEndPointId, sourceAnchors, targetAnchors) {
        this.routerId = routerId;
        this.sourceEndPointId = sourceEndPointId;
        this.targetEndPointId = targetEndPointId;
        this.sourceAnchors = sourceAnchors;
        this.targetAnchors = targetAnchors;
    },
    // 添加连线到图对象
    addEdge: function (sourceId, targetId, edge) {
        let _base = myWORKFLOW._base;

        _base.graph.setEdge(sourceId, targetId, { // 源节点和目标节点的id
            id: edge.routerId, // 连线id
            sourceEndPointId: edge.sourceEndPointId, // 源节点端点的id
            targetEndPointId: edge.targetEndPointId, // 目标节点端点的id
            sourceAnchors: edge.sourceAnchors, // 源节点的锚点
            targetAnchors: edge.targetAnchors // 目标节点的锚点
        });
    },
    // 获取图对象中被标记为选中的节点的id数组
    getSelectedNodeIds: function () {
        let _base = myWORKFLOW._base;
        let nodeIds = _base.graph.nodes();
        let selectedNodeIds = [];

        for (let i = 0, len = nodeIds.length; i < len; i++) {
            if (_base.graph.node(nodeIds[i]).isSelected) {
                selectedNodeIds.push(nodeIds[i]);
            }
        }
        return selectedNodeIds;
    },
    // 更新图对象中的节点
    updateNode: function (id) {
        let _base = myWORKFLOW._base;
        if (id.indexOf('#') != 0) {
            id = '#' + id;
        }
        let $this = $(id);
        if (id.indexOf('#') == 0) {
            id = id.substring(1);
        }
        let graphNode = _base.graph.node(id);

        //由于超过五个字节点上不再显示，所有这里不能用节点的text去更新图对象
        //graphNode.text = $this.children(':first-child').text();

        graphNode.locTop = $this.offset().top;
        graphNode.locLeft = $this.offset().left;
        graphNode.nodeHeight = $this.css('height');
        graphNode.nodeWidth = $this.css('width');
        graphNode.bgColor = $this.attr('bgColor-gradient');
    },
    // 更新图对象中的所有节点
    updateAllNode: function () {
        let _base = myWORKFLOW._base;
        let nodeArr = _base.graph.nodes();

        for (let i = 0, len = nodeArr.length; i < len; i++) {
            graphUtil.updateNode(nodeArr[i]);
        }
    },
    // 检查流程图合法性
    checkGraph: function () {
        let _base = myWORKFLOW._base;

        // 克隆graph对象
        let copyGraph = $.extend(true, {}, _base.graph);
        let msg = '0';
        let componentLen = graphlib.alg.components(copyGraph).length;

        if (componentLen == 0) {
            msg = CONFIG.msg.noNode; // 无节点
        } else if (componentLen > 1) {
            msg = CONFIG.msg.noConn; // 存在节点没有连接
        } /*else if (!graphlib.alg.isAcyclic(copyGraph)) {
			msg = CONFIG.msg.hasAcyclic; // 存在循环
		}*/
        return msg;
    },
    // 保存为图片之前检查是否合法
    checkGraphBySave2Photo: function () {
        let _base = myWORKFLOW._base;
        let msg = '0';
        let nodeArr = _base.graph.nodes();

        if (nodeArr.length <= 0) {
            msg = CONFIG.msg.noNodeBySave2Photo;
        }
        return msg;
    },
    // 放置、粘贴新节点时检查图对象
    checkGraphBeforeCreate: function (nodeType) {
        let _base = myWORKFLOW._base;
        let msg = '0';

        // 1、只允许有一个开始节点
        if (nodeType == 'start') {
            let nodeIds = _base.graph.nodes();
            for (let i = 0, len = nodeIds.length; i < len; i++) {
                if (_base.graph.node(nodeIds[i]).nodeType == 'start') {
                    msg = CONFIG.msg.repeatStartNode;
                    return msg;
                }
            }
        }
        // 2、只允许有一个结束节点
        if (nodeType == 'end') {
            let nodeIds = _base.graph.nodes();
            for (let i = 0, len = nodeIds.length; i < len; i++) {
                if (_base.graph.node(nodeIds[i]).nodeType == 'end') {
                    msg = CONFIG.msg.repeatEndNode;
                    return msg;
                }
            }
        }
        return msg;
    },
    // 根据画布中的节点获取canvas的尺寸
    getCanvasSizeByNode: function () {
        let _base = myWORKFLOW._base;
        let nodeArr = _base.graph.nodes();
        let firstNodeTop = _base.graph.node(nodeArr[0]).locTop;
        let firstNodeLeft = _base.graph.node(nodeArr[0]).locLeft;
        let maxTop = firstNodeTop;
        let minTop = firstNodeTop;
        let maxLeft = firstNodeLeft;
        let minLeft = firstNodeLeft;

        for (let i = 0, len = nodeArr.length; i < len; i++) {
            let t = _base.graph.node(nodeArr[i]).locTop;
            let l = _base.graph.node(nodeArr[i]).locLeft;

            if (t > maxTop) {
                maxTop = t;
            }
            if (t < minTop) {
                minTop = t;
            }
            if (l > maxLeft) {
                maxLeft = l;
            }
            if (l < minLeft) {
                minLeft = l;
            }
        }

        return {
            canvasTop: maxTop + minTop,
            canvasLeft: maxLeft + minLeft
        };
    },
    // 根据id移除节点以及关于节点的所有连线、端点，返回删除的路由线id数组
    removeNodeAndEdgesById: function (id) {
        let _base = myWORKFLOW._base;
        let deleteRouterIdArr = [];
        //debugger;
        $.each(_base.graph.nodeEdges(id), function () {
            let v = $(this)[0].v;
            let w = $(this)[0].w;
            let e = _base.graph.edge(v, w);
            deleteRouterIdArr.push(e.id);
            if (e.sourceEndPointId != undefined) {
                _base.plumb.deleteEndpoint(e.sourceEndPointId);
                _base.plumb.deleteEndpoint(e.targetEndPointId);
            }
            _base.graph.removeEdge($(this)[0].v, $(this)[0].w);
        });
        _base.graph.removeNode(id);
        _base.plumb.remove(id);
        return deleteRouterIdArr;
    },
    // 更新泳道对象
    updateLaneObjs: function (id) {
        let _base = myWORKFLOW._base;
        if (id.indexOf('#') != 0) {
            id = '#' + id;
        }
        let $this = $(id);
        if (id.indexOf('#') == 0) {
            id = id.substring(1);
        }
        let laneObj = _base.laneObjs[id];

        laneObj.locTop = $this.offset().top;
        laneObj.locLeft = $this.offset().left;
        laneObj.nodeHeight = $this.css('height');
        laneObj.nodeWidth = $this.css('width');
        laneObj.bgColor = $this.attr('bgColor-gradient');
    }
}

//流程属性列表工具类
let WorkflowPropertyUtil = {
    wfDefId: '',
    pgTableId: '',
    //流程节点对象
    wfNodeVM: function (id, wfDefId, code, type, nodeType, nodeName, nodeSetting, formFieldName, formFieldDisplayName, weightValue,
        executeOrgIds, executeOrgNames, executeRoleIds, executeRoleNames, executeUserIds,
        executeUserNames, exceptUserIds, exceptUserNames, notifyUserIds, notifyUserNames, rules, nodeLocTop, nodeLocLeft,
        preNodeId, preNodeCode, nextNodeId, nextNodeCode, returnNodeId, returnNodeCode) {
        return {
            id: id,
            workflowDefId: wfDefId,
            code: code,
            type: type != null ? type : 0,
            nodeType: nodeType != null ? nodeType : 0,
            name: nodeName,
            //审批设置
            executorSetting: nodeSetting && nodeSetting != null ? nodeSetting : 0,
            executorFormFieldName: formFieldName && formFieldName != null ? formFieldName : null,
            executorFormFieldDisplayName: formFieldDisplayName && formFieldDisplayName != null ? formFieldDisplayName : null,
            weightValue: weightValue && weightValue != null ? weightValue : 0,
            //审批人设置
            orgIds: executeOrgIds && executeOrgIds != null ? executeOrgIds : null,
            orgNames: executeOrgNames && executeOrgNames != null ? executeOrgNames : null,
            roleIds: executeRoleIds && executeRoleIds != null ? executeRoleIds : null,
            roleNames: executeRoleNames && executeRoleNames != null ? executeRoleNames : null,
            userIds: executeUserIds && executeUserIds != null ? executeUserIds : null,
            userNames: executeUserNames && executeUserNames != null ? executeUserNames : null,
            exceptUserIds: exceptUserIds && exceptUserIds != null ? exceptUserIds : null,
            exceptUserNames: exceptUserNames && exceptUserNames != null ? exceptUserNames : null,
            notifyUserIds: notifyUserIds && notifyUserIds != null ? notifyUserIds : null,
            notifyUserNames: notifyUserNames && notifyUserNames != null ? notifyUserNames : null,
            //流程节点设置
            locTop: nodeLocTop && nodeLocTop != null ? nodeLocTop : 300,
            locLeft: nodeLocLeft && nodeLocLeft != null ? nodeLocLeft : 300,
            preNodeId: preNodeId && preNodeId != null ? preNodeId : null,
            preNodeCode: preNodeCode && preNodeCode != null ? preNodeCode : null,
            nextNodeId: nextNodeId && nextNodeId != null ? nextNodeId : null,
            nextNodeCode: nextNodeCode && nextNodeCode != null ? nextNodeCode : null,
            returnNodeId: returnNodeId && returnNodeId != null ? returnNodeId : null,
            returnNodeCode: returnNodeCode && returnNodeCode != null ? returnNodeCode : null,
            //条件节点中的规则设置
            rules: rules && rules != null ? rules : []
        };
    },
    //流程审批处理人设置对象
    executorSettingNode: function (id, code, executorSetting, formFieldName, formFieldDisplayName, executeOrgIds, executeOrgNames, executeRoleIds, executeRoleNames, executeUserIds,
        executeUserNames, exceptUserIds, exceptUserNames) {
        return {
            id: id,
            code: code,
            executorSetting: executorSetting,
            executorFormFieldName: formFieldName ? formFieldName : '',
            executorFormFieldDisplayName: formFieldDisplayName ? formFieldDisplayName : '',
            //审批人设置
            orgIds: executeOrgIds ? executeOrgIds : '',
            orgNames: executeOrgNames ? executeOrgNames : '',
            roleIds: executeRoleIds ? executeRoleIds : '',
            roleNames: executeRoleNames ? executeRoleNames : '',
            userIds: executeUserIds ? executeUserIds : '',
            userNames: executeUserNames ? executeUserNames : '',
            exceptUserIds: exceptUserIds ? exceptUserIds : '',
            exceptUserNames: exceptUserNames ? exceptUserNames : '',
        };
    },
    //流程节点类型下拉数据
    getNodeTypeData: function () {
        return [{ "value": 0, "text": "开始" },
        { "value": 1, "text": "审批" },
        { "value": 2, "text": "条件" },
        { "value": 4, "text": "结束" }];
    },
    //流程属性列表格式：会签（或签）设置、节点类型、执行人设置、规则设置
    formatter: function (value, rowData, index) {
        let tdContext = value;
        switch (rowData.field) {
            case 'Type':
                tdContext = '或签(一个同意即可)';
                if (value != undefined && value != null && value !== 0) {
                    tdContext = '按同意(' + value + '%)通过率审核';
                }
                return tdContext;
            case 'NodeType':
                let data = WorkflowPropertyUtil.getNodeTypeData();
                for (var o in data) {
                    if (value == data[o].value) {
                        return data[o].text;
                    }
                }
                return value;
            case 'ExecutorSetting':
                let selectNode = rowData.value;
                let resContent = WorkflowPropertyUtil.getExecutorSettingContent(selectNode, '<br/>');
                return resContent;
            case 'Rules':
                let rules = rowData.value;
                let rulesContent = WorkflowPropertyUtil.getConditionRuleContent(rules, '<br/>');
                return rulesContent;
            default:
                return tdContext;
        }
    },
    //获取审批人的显示内容
    getExecutorSettingContent: function (selectNode, splitChar) {
        if (selectNode === undefined || selectNode === null) return '';

        let char = splitChar === undefined
            || splitChar === null
            || splitChar === ''
            ? '；' : splitChar;
        let resContent = '';

        let nodeSetting = selectNode.executorSetting;
        if ($.string.isNullOrEmpty(nodeSetting))
            return '';
        if (nodeSetting === 0 || nodeSetting === '0') {
            if (!$.string.isNullOrEmpty(selectNode.orgNames)) {
                if (resContent !== '') resContent += char;
                resContent += '已选组织：' + selectNode.orgNames;
            }
            if (!$.string.isNullOrEmpty(selectNode.roleNames)) {
                if (resContent !== '') resContent += char;
                resContent += '已选角色：' + selectNode.roleNames;
            }
            if (!$.string.isNullOrEmpty(selectNode.userNames)) {
                if (resContent !== '') resContent += char;
                resContent += '已选用户：' + selectNode.userNames;
            }
            if (!$.string.isNullOrEmpty(selectNode.exceptUserNames)) {
                if (resContent !== '') resContent += char;
                resContent += '已排除用户：' + selectNode.exceptUserNames;
            }
        } else {
            switch (nodeSetting) {
                case '10':
                case 10:
                    resContent = '流程发起人的主管';
                    break;
                case '11':
                case 11:
                    resContent = '流程发起人所在组织';
                    break;
                case '12':
                case 12:
                    resContent = '流程发起人所属角色';
                    break;
                case '13':
                case 13:
                    resContent = '流程发起人的上级主管';
                    break;

                case '20':
                case 20:
                    resContent = '提交审批人的主管';
                    break;
                case '21':
                case 21:
                    resContent = '提交审批人所在组织';
                    break;
                case '22':
                case 22:
                    resContent = '提交审批人所属角色';
                    break;
                case '23':
                case 23:
                    resContent = '提交审批人的上级主管';
                    break;

                case '30':
                case 30:
                    let fieldName = selectNode.executorFormFieldDisplayName
                        ? selectNode.executorFormFieldDisplayName
                        : selectNode.executorFormFieldName;
                    resContent = '设置表单字段【' + fieldName + '】为审批人';
                    break;
            }
        }

        return resContent;
    },
    //获取条件规则的显示内容
    getConditionRuleContent: function (rules, splitChar) {
        if (rules === undefined || rules === null) return '';

        let char = splitChar === undefined
            || splitChar === null
            || splitChar === ''
            ? '；' : splitChar;
        let resContent = '';
        if (rules.length > 0) {
            rules.forEach(function (row, index, arr) {
                let ruleType = '';
                switch (row.ruleType) {
                    case '0': //RuleType.None
                    case 0: //RuleType.None
                        break;
                    case '1': //RuleType.And
                    case 1: //RuleType.And
                        ruleType = '并且'
                        break;
                    case '2': //RuleType.Or
                    case 2: //RuleType.Or
                        ruleType = '或者'
                        break;
                }
                let operType = '';
                switch (row.operatorType) {
                    case 0: //RuleOperatorType.Equal--等于
                        operType = '等于';
                        break;
                    case 1: //RuleOperatorType.NotEqual--不等于
                        operType = '不等于';
                        break;
                    case 2: //RuleOperatorType.Contains--包含
                        operType = '包含';
                        break;

                    case 10: //RuleOperatorType.Greater--大于
                        operType = '大于';
                        break;
                    case 11: //RuleOperatorType.Less--小于
                        operType = '小于';
                        break;
                    case 12: //RuleOperatorType.GreaterThanAndEqual--大于等于
                        operType = '大于等于';
                        break;
                    case 13: //RuleOperatorType.LessThanAndEqual--小于等于
                        operType = '小于等于';
                        break;
                }

                let fieldName = row.fieldName;
                let fieldDisplayName = row.fieldDisplayName;
                let fieldValue = row.fieldValue;
                resContent += ruleType + " " + fieldDisplayName + " " + operType + " " + fieldValue + char;
            })
        }

        return resContent;
    },
    //加载节点数据
    loadPropertyNodeData: function (pgTableId, flowId, flowCode, flowName, flowVersion, nodeData, fieldData) {
        //debugger;
        var propertyData = {
            "total": 0,
            "rows": []
        };
        propertyData.total = propertyData.total + 1;
        propertyData.rows.push({
            "id": flowId,
            "code": flowCode,
            "field": "Code",
            "name": "流程编号",
            "value": flowCode,
            "group": "流程基本信息",
            "isNodeProperty": false,
            "editor": {
                type: 'validatebox textbox',
                options: {
                    readonly: true
                }
            }
        });
        propertyData.total = propertyData.total + 1;
        propertyData.rows.push({
            "id": flowId,
            "code": flowCode,
            "field": "Version",
            "name": "流程版本",
            "value": flowVersion,
            "group": "流程基本信息",
            "isNodeProperty": false,
            "editor": {
                type: 'validatebox textbox',
                options: {
                    readonly: true
                }
            }
        });
        propertyData.total = propertyData.total + 1;
        propertyData.rows.push({
            "id": flowId,
            "code": flowCode,
            "field": "Name",
            "name": "流程名称",
            "value": flowName,
            "group": "流程基本信息",
            "isNodeProperty": false,
            "editor": {
                type: 'validatebox',
                options: {
                    required: true,
                    readonly: false,
                }
            }
        });

        this._getWfPropertyDataByNodeData(nodeData, propertyData, fieldData);

        this.pgTableId = pgTableId;
        var pg = $(this.pgTableId);
        pg.propertygrid('loadData', propertyData);
    },
    // 根据后台返回的StartNode以及NodeFields，生成属性列表所需数据
    _getWfPropertyDataByNodeData: function (nodeData, propertyData, fieldData) {
        let rows = this.createPropertyRowsByNodeData(nodeData, fieldData);
        propertyData.total = propertyData.total + rows.length;
        propertyData.rows = propertyData.rows.concat(rows);
        //迭代
        if (nodeData.nextNode != null) {
            this._getWfPropertyDataByNodeData(nodeData.nextNode, propertyData, fieldData);
        }
    },
    //创建流程属性行数据
    createPropertyRowsByNodeData: function (nodeData, fieldData) {
        let id = nodeData.id;
        let wfDefId = nodeData.workflowDefId;
        let code = nodeData.code;
        let type = nodeData.type;
        let nodeType = nodeData.nodeType;
        let nodeName = nodeData.name;
        let nodeSetting = nodeData.executorSetting;
        let formFieldName = nodeData.executorFormFieldName;
        let formFieldDisplayName = nodeData.executorFormFieldDisplayName;
        let weightValue = nodeData.weightValue;
        let orgIds = nodeData.orgIds;
        let orgNames = nodeData.orgNames;
        let roleIds = nodeData.roleIds;
        let roleNames = nodeData.roleNames;
        let userIds = nodeData.userIds;
        let userNames = nodeData.userNames;
        let exceptUserIds = nodeData.exceptUserIds;
        let exceptUserNames = nodeData.exceptUserNames;
        let rules = nodeData.rules;

        let rows = [];
        //节点编号
        rows.push({
            "fid": CommonUtil.getGuid(),
            "id": id,
            "code": code,
            "wfDefId": wfDefId,
            "field": "Code", //属性字段
            "name": "节点编号",
            "value": code,
            "group": nodeName + "信息",
            "isNodeProperty": true,
            "editor": {
                type: 'validatebox textbox',
                options: {
                    readonly: true
                }
            }
        });
        //节点名称
        rows.push({
            "fid": CommonUtil.getGuid(),
            "id": id,
            "code": code,
            "wfDefId": wfDefId,
            "field": "Name", //属性字段
            "name": "节点名称",
            "value": nodeName,
            "group": nodeName + "信息",
            "isNodeProperty": true,
            "editor": {
                type: 'validatebox',
                options: {
                    required: true,
                    readonly: false
                }
            }
        });
        //节点类型
        rows.push({
            "fid": CommonUtil.getGuid(),
            "id": id,
            "code": code,
            "wfDefId": wfDefId,
            "field": "NodeType", //属性字段
            "name": "节点类型",
            "value": nodeType,
            "group": nodeName + "信息",
            "isNodeProperty": true,
            "editor": {
                type: "combobox",
                options: {
                    editable: false,
                    readonly: true,
                    multiple: false,
                    valueField: "value",
                    textField: "text",
                    panelHeight: "auto",
                    data: this.getNodeTypeData()
                }
            }
        });
        //节点为：Task，设置审批类型、审批人及权重
        if (nodeType == 1 || nodeType === 'task') //Task
        {
            //审批类型
            rows.push({
                "fid": CommonUtil.getGuid(),
                "id": id,
                "code": code,
                "wfDefId": wfDefId,
                "field": "Type", //属性字段
                "name": "审批设置",
                "value": weightValue,
                "group": nodeName + "信息",
                "isNodeProperty": true,
                "editor": {
                    type: 'wfAuditSetting',
                    options: {
                        value: weightValue,
                        labelPosition: 'after'
                    }
                }
            });

            //debugger;
            let selectNode = this.executorSettingNode(id, code, nodeSetting, formFieldName, formFieldDisplayName, orgIds, orgNames, roleIds, roleNames, userIds, userNames, exceptUserIds, exceptUserNames);
            //审批人设置
            rows.push({
                "fid": CommonUtil.getGuid(),
                "id": id,
                "code": code,
                "wfDefId": wfDefId,
                "field": "ExecutorSetting", //属性字段
                "name": "审批人设置",
                "value": selectNode,
                "group": nodeName + "信息",
                "isNodeProperty": true,
                "editor": {
                    type: "wfExecutorSetting",
                    options: {
                        multiline: true,
                        readonly: true,
                        height: "120px",
                        code: code
                    }
                }
            });
        }
        //节点为：Condition，设置规则
        if (nodeType == 2 || nodeType === 'condition') //Condition
        {
            //debugger;
            //条件设置
            rows.push({
                "fid": CommonUtil.getGuid(),
                "id": id,
                "code": code,
                "wfDefId": wfDefId,
                "field": "Rules", //属性字段
                "name": "条件设置",
                "value": rules,
                "group": nodeName + "信息",
                "isNodeProperty": true,
                "editor": {
                    type: "wfRuleSetting",
                    options: {
                        multiline: true,
                        readonly: true,
                        editable: false,
                        height: "120px",
                        wfNodeId: id,
                        wfNodeCode: code,
                        wfFields: fieldData
                    }
                }
            });
        }

        //当传入对象为：WorkflowProTask时，需要设置任务的相应属性
        let isProcessTask = nodeData.taskStatus ? true : false;
        if (isProcessTask) {
            //debugger;
            //任务状态
            rows.push({
                "fid": CommonUtil.getGuid(),
                "id": id,
                "code": code,
                "wfDefId": wfDefId,
                "field": "TaskStatusString", //属性字段
                "name": "任务状态",
                "value": nodeData.taskStatusString,
                "group": nodeName + "信息",
                "isNodeProperty": true,
                "editor": {
                    type: 'validatebox',
                    options: {
                        readonly: true
                    }
                }
            });
            //以下数据为流程实例的任务执行数据
            if (nodeData.taskExecutes && nodeData.taskExecutes.length > 0) {
                let lastExecute = $.array.last(nodeData.taskExecutes);
                //任务审核人
                rows.push({
                    "fid": CommonUtil.getGuid(),
                    "id": id,
                    "code": code,
                    "wfDefId": wfDefId,
                    "field": "ExecuteUserName", //属性字段
                    "name": "审核人",
                    "value": lastExecute.executeUserName,
                    "group": nodeName + "信息",
                    "isNodeProperty": true,
                    "editor": {
                        type: 'validatebox',
                        options: {
                            readonly: true
                        }
                    }
                });
                //任务审核时间
                rows.push({
                    "fid": CommonUtil.getGuid(),
                    "id": id,
                    "code": code,
                    "wfDefId": wfDefId,
                    "field": "ExecuteDateTime", //属性字段
                    "name": "审核时间",
                    "value": lastExecute.executeDateTime,
                    "group": nodeName + "信息",
                    "isNodeProperty": true,
                    "editor": {
                        type: 'validatebox',
                        options: {
                            readonly: true
                        }
                    }
                });
                //任务执行状态
                rows.push({
                    "fid": CommonUtil.getGuid(),
                    "id": id,
                    "code": code,
                    "wfDefId": wfDefId,
                    "field": "ExecuteStatusString", //属性字段
                    "name": "审核状态",
                    "value": lastExecute.executeStatusString,
                    "group": nodeName + "信息",
                    "isNodeProperty": true,
                    "editor": {
                        type: 'validatebox',
                        options: {
                            readonly: true
                        }
                    }
                });
                //任务执行备注
                rows.push({
                    "fid": CommonUtil.getGuid(),
                    "id": id,
                    "code": code,
                    "wfDefId": wfDefId,
                    "field": "ExecuteRemark", //属性字段
                    "name": "审核意见",
                    "value": lastExecute.executeRemark,
                    "group": nodeName + "信息",
                    "isNodeProperty": true,
                    "editor": {
                        type: 'validatebox',
                        options: {
                            readonly: true
                        }
                    }
                });
            }
        }

        return rows;
    },
    //更新流程属性中审批人的数据
    updateExecutorSettingRowById: function (nodeId, selectNode) {
        let pgAttrForm = $(this.pgTableId);
        let propertyData = pgAttrForm.propertygrid('getRows');
        //设置审批人数据
        propertyData.forEach(function (row, index, arr) {
            if (row.isNodeProperty
                && row.code === nodeId
                && row.field === 'ExecutorSetting') {
                //debugger;
                row['value'] = selectNode;
                //let index = pgAttrForm.propertygrid('getRowIndex', row);
                //pgAttrForm.propertygrid('updateRow', {
                //    index: index,
                //    row: row
                //});
            }
        })

        pgAttrForm.propertygrid('loadData', propertyData);
    },
    //更新流程属性中条件设置规则的数据
    updateConditionRulesRowById: function (nodeId, rules) {
        let pgAttrForm = $(this.pgTableId);
        let propertyData = pgAttrForm.propertygrid('getRows');
        //条件设置规则数据
        propertyData.forEach(function (row, index, arr) {
            if (row.isNodeProperty
                && row.code === nodeId
                && row.field === 'Rules') {
                //debugger;
                row['value'] = rules;
                //let index = pgAttrForm.propertygrid('getRowIndex', row);
                //pgAttrForm.propertygrid('updateRow', {
                //    index: index,
                //    row: row
                //});
            }
        })

        pgAttrForm.propertygrid('loadData', propertyData);
    },
    //删除流程属性行数据
    removePropertyRowById: function (nodeId) {
        var pgAttrForm = $(this.pgTableId);
        let propertyData = pgAttrForm.propertygrid('getRows');
        //删除行的Id
        propertyData.forEach(function (row, index, arr) {
            if (row.isNodeProperty
                && row.code === nodeId) {
                let i = pgAttrForm.propertygrid('getRowIndex', row);
                pgAttrForm.propertygrid("deleteRow", i);
            }
        })
    },

    //根据节点的Code，获取所归属的节点分类
    _getGroupIndexById: function (id) {
        let groupDiv = $('table.datagrid-btable:contains("' + id + '")').prev("div.datagrid-group");
        if (groupDiv.length > 0) {
            let index = groupDiv.attr('group-index');
            return index;
        }
        return null;
    },
    //隐藏当前分类项，同时显示其他分类项
    _hiddenRowByCode: function (nodeId) {
        let index = this._getGroupIndexById(nodeId);

        //隐藏收缩表头
        let view1 = $(this.pgTableId).prevAll('div.datagrid-view1');
        let body = view1.children('div.datagrid-body').children('div.datagrid-body-inner');
        let group1 = body.children('div.datagrid-group');
        group1.eq(index).hide(); //隐藏指定分类标题
        group1.not(":eq(" + index + ")").show();
        let table1 = body.children('table.datagrid-btable');
        table1.eq(index).hide(); //隐藏指定分类
        table1.not(":eq(" + index + ")").show();
        //隐藏分类
        let view2 = $(this.pgTableId).prevAll('div.datagrid-view2');
        let group2 = view2.children('div.datagrid-body').children('div.datagrid-group');
        group2.eq(index).hide(); //隐藏指定分类标题
        group2.not(":eq(" + index + ")").show();
        let table2 = view2.children('div.datagrid-body').children('table.datagrid-btable');
        table2.eq(index).hide(); //隐藏指定分类
        table2.not(":eq(" + index + ")").show();

    },
    //隐藏当前分类项，同时显示其他分类项
    showRowByCode: function (nodeId) {
        let index = this._getGroupIndexById(nodeId);

        //隐藏收缩表头
        let view1 = $(this.pgTableId).prevAll('div.datagrid-view1');
        let body = view1.children('div.datagrid-body').children('div.datagrid-body-inner');
        let group1 = body.children('div.datagrid-group');
        group1.eq(index).show(); //隐藏指定分类标题
        group1.not(":eq(" + index + ")").hide();
        let table1 = body.children('table.datagrid-btable');
        table1.eq(index).show(); //隐藏指定分类
        table1.not(":eq(" + index + ")").hide();
        //隐藏分类
        let view2 = $(this.pgTableId).prevAll('div.datagrid-view2');
        let group2 = view2.children('div.datagrid-body').children('div.datagrid-group');
        group2.eq(index).show(); //隐藏指定分类标题
        group2.not(":eq(" + index + ")").hide();
        let table2 = view2.children('div.datagrid-body').children('table.datagrid-btable');
        table2.eq(index).show(); //隐藏指定分类
        table2.not(":eq(" + index + ")").hide();

    },
    //隐藏当前分类项，同时显示其他分类项
    showAllRows: function () {
        //隐藏收缩表头
        let view1 = $(this.pgTableId).prevAll('div.datagrid-view1');
        let body = view1.children('div.datagrid-body').children('div.datagrid-body-inner');
        let group1 = body.children('div.datagrid-group');
        group1.show(); //隐藏指定分类标题
        let table1 = body.children('table.datagrid-btable');
        table1.show(); //隐藏指定分类
        //隐藏分类
        let view2 = $(this.pgTableId).prevAll('div.datagrid-view2');
        let group2 = view2.children('div.datagrid-body').children('div.datagrid-group');
        group2.show(); //隐藏指定分类标题
        let table2 = view2.children('div.datagrid-body').children('table.datagrid-btable');
        table2.show(); //隐藏指定分类
    },

    //根据属性列表，生成后台Node数据
    getWfNodeData: function () {
        var pgAttrForm = $(this.pgTableId);
        let propertyData = pgAttrForm.propertygrid('getRows');
        //根据节点进行分组，获取分组后的Node属性名及值
        let nodeData = propertyData.filter(function (value, index, array) {
            return value.isNodeProperty;
        });
        let groupPropeties = StatisticsUtil
            .groupBy(nodeData, (item) => {
                return [item.code];
            });

        //根据对象的属性列表设置相关属性
        let result = [];
        for (let i = 0; i < groupPropeties.length; i++) {
            let groupPropety = groupPropeties[i];
            let code = groupPropety.keyString;
            let nodeRow = {};

            //根据审批人控件，设置以下值
            let executeSetting = null;
            let formFieldName = null;
            let formFieldDisplayName = null;
            let executeOrgIds = null;
            let executeOrgNames = null;
            let executeRoleIds = null;
            let executeRoleNames = null;
            let executeUserIds = null;
            let executeUserNames = null;
            let exceptUserIds = null;
            let exceptUserNames = null;
            let notifyUserIds = null;
            let notifyUserNames = null;

            let propetyList = groupPropety.list;
            for (let j = 0; j < propetyList.length; j++) {
                let property = propetyList[j];
                let propertyId = property.id;
                let propertyWfDefId = property.wfDefId;
                nodeRow["Id"] = propertyId;
                nodeRow["wfDefId"] = propertyWfDefId;
                let propertyName = property.field;
                let propertyValue = property.value;
                //设置流程的审批设置属性值：Node.type、Node.weightValue
                if (propertyName === 'Type') {
                    if (propertyValue === undefined || propertyValue === null || propertyValue === '') {
                        nodeRow['type'] = 0;
                        nodeRow['weightValue'] = null;
                    } else {
                        nodeRow['type'] = 1;
                        nodeRow['weightValue'] = propertyValue;
                    }
                } else if (propertyName === 'ExecutorSetting') {
                    let selectNode = propertyValue;
                    executeSetting = selectNode.executorSetting;
                    formFieldName = selectNode.executorFormFieldName;
                    formFieldDisplayName = selectNode.executorFormFieldDisplayName;
                    executeOrgIds = selectNode.orgIds;
                    executeOrgNames = selectNode.orgNames;
                    executeRoleIds = selectNode.roleIds;
                    executeRoleNames = selectNode.roleNames;
                    executeUserIds = selectNode.userIds;
                    executeUserNames = selectNode.userNames;
                    exceptUserIds = selectNode.exceptUserIds;
                    exceptUserNames = selectNode.exceptUserNames;
                }
                else {
                    nodeRow[propertyName] = propertyValue;
                }
            }

            result.push(this.wfNodeVM(nodeRow.Id, nodeRow.wfDefId, nodeRow.Code, nodeRow.type, nodeRow.NodeType, nodeRow.Name, executeSetting, formFieldName, formFieldDisplayName, nodeRow.weightValue, executeOrgIds, executeOrgNames, executeRoleIds, executeRoleNames, executeUserIds, executeUserNames, exceptUserIds, exceptUserNames, notifyUserIds, notifyUserNames, nodeRow.Rules));
        }

        return result;
    },
    //根据节点属性数据（流程设置数据）和图形Node数据（流程数据）合成为后台所需的数据
    integratedNodeData: function (gNodeData, gLinkData) {
        let propertyData = this.getWfNodeData();
        //根据图形Node数据（流程数据）和节点属性数据（流程设置数据）合成为后台所需的数据
        gNodeData.forEach(function (gRow, gIndex, nodeArr) {
            let id = gRow.id;
            let code = gRow.key;
            let type = gRow.type;
            let text = gRow.text;
            let nodeType = gRow.nodeType;
            let locTop = gRow.locTop;
            let locLeft = gRow.locLeft;
            //获取当前节点的流程属性数据
            let currentRows = propertyData.filter(function (pNode, pIndex, proArr) {
                return pNode.code === code;
            });
            if (currentRows != undefined
                && currentRows != null
                && currentRows.length > 0) {
                let currentRow = currentRows[0];
                currentRow.locTop = locTop;
                currentRow.locLeft = locLeft;

                //根据连接线Link的数据，获取下一节点的流程数据，并设置流程中的：nextNodeCode、nextNodeId
                let nextLinkRows = gLinkData.filter(function (lRow, lIndex, linkArr) {
                    return lRow.from === currentRow.code;
                });
                if (nextLinkRows != undefined
                    && nextLinkRows != null
                    && nextLinkRows.length > 0) {
                    //当未Condition节点是，有两条连线，还需设置：returnNodeCode、returnNodeId
                    if (currentRow.nodeType == 2) {
                        nextLinkRows.forEach(function (r, rIndex, rArr) {
                            if (r.label === "真") {
                                let nextRows = propertyData.filter(function (nNode, nIndex, nextArr) {
                                    return nNode.code === r.to;
                                });
                                if (nextRows != undefined
                                    && nextRows != null
                                    && nextRows.length > 0) {
                                    let nextRow = nextRows[0];
                                    currentRow.nextNodeId = nextRow.id;
                                    currentRow.nextNodeCode = nextRow.code;
                                }
                            } else {
                                let returnRows = propertyData.filter(function (nNode, nIndex, nextArr) {
                                    return nNode.code === r.to;
                                });
                                if (returnRows != undefined
                                    && returnRows != null
                                    && returnRows.length > 0) {
                                    let returnRow = returnRows[0];
                                    currentRow.returnNodeId = returnRow.id;
                                    currentRow.returnNodeCode = returnRow.code;
                                }
                            }
                        })
                    } else {
                        let nextLinkRow = nextLinkRows[0];
                        let nextRows = propertyData.filter(function (nNode, nIndex, nextArr) {
                            return nNode.code === nextLinkRow.to;
                        });
                        if (nextRows != undefined
                            && nextRows != null
                            && nextRows.length > 0) {
                            let nextRow = nextRows[0];
                            currentRow.nextNodeId = nextRow.id;
                            currentRow.nextNodeCode = nextRow.code;
                        }
                    }
                }

                //根据连接线Link的数据，获取上一节点的流程数据，并设置流程中的：preNodeCode、preNodeId
                let preLinkRows = gLinkData.filter(function (lRow, lIndex, linkArr) {
                    return lRow.to === currentRow.code;
                });
                if (preLinkRows != undefined
                    && preLinkRows != null
                    && preLinkRows.length > 0) {
                    let preLinkRow = preLinkRows[0];
                    //根据Link的from的，以及不为condition的节点，获取前一节点
                    let preRows = propertyData.filter(function (pNode, pIndex, preArr) {
                        return pNode.code === preLinkRow.from && pNode.nodeType != 2;
                    });
                    if (preRows != undefined
                        && preRows != null
                        && preRows.length > 0) {
                        let preRow = preRows[0];
                        currentRow.preNodeId = preRow.id;
                        currentRow.preNodeCode = preRow.code;
                    }
                }
            }
        });

        return propertyData;
    }
}
