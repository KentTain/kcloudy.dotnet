
// 属性、样式设置工具类
var attrCfgUtil = {
	// 设置流程属性
	setCanvasAttr: function() {
		$('#attrForm').my('remove');
		let _base = FLOW._base;
		let fid = _base.flowId;
		let fcode = _base.flowCode;
		let fverson = _base.flowVerson;
		let fname = _base.flowName;
		let d = { flowId: fid, flowCode: fcode, flowVerson: fverson, flowName: fname};
		var manifest = {
			data: { flowId: '', flowCode: '', flowVerson: '', flowName: '' },
			init: function ($node, runtime) {
				let flowDiv = '<div class="panel datagrid panel-htop propertygrid" style="min-width: 10px; max-width: 10000px;"><div class="datagrid-wrap panel-body panel-body-noheader" title="" style="width: 307.2px; min-height: 10px; max-height: 10000px;"><div class="datagrid-view" style="width: 305.2px; height: 483px;"><div class="datagrid-view2" style="width: 285.2px;"><div class="datagrid-body" style="width: 285.2px; margin-top: 0px; min-height: 7px; max-height: 9997px; height: 482px;"><div group-index="0" class="datagrid-group" style="width: 285.6px;"><span class="datagrid-group-title" style="left: 0px;">流程基本信息</span></div>'
					+ '<table class="datagrid-btable" cellspacing="0" cellpadding="0" border="0"><tbody>'
					+ '<tr id="attrForm_datagrid-row-r1-2-0" datagrid-row-index="0" class="datagrid-row"><td field="name"><div style="height:auto;" class="datagrid-cell attrForm_datagrid-cell-c1-name">流程编号</div></td><td field="value"><div style="height:auto;" class="datagrid-cell attrForm_datagrid-cell-c1-value"><input id="txtFlowCode" type="text" name="title" value="' + fcode + '" autocomplete="off" class="easyui-textbox"></div></td></tr>'
					+ '<tr id="attrForm_datagrid-row-r1-2-1" datagrid-row-index="1" class="datagrid-row"><td field="name"><div style="height:auto;" class="datagrid-cell attrForm_datagrid-cell-c1-name">流程版本</div></td><td field="value"><div style="height:auto;" class="datagrid-cell attrForm_datagrid-cell-c1-value"><input id="txtFlowVersion" type="text" name="title" value="' + fverson + '" autocomplete="off" class="easyui-textbox"></div></td></tr>'
					+ '<tr id="attrForm_datagrid-row-r1-2-2" datagrid-row-index="2" class="datagrid-row"><td field="name"><div style="height:auto;" class="datagrid-cell attrForm_datagrid-cell-c1-name">流程名称</div></td><td field="value"><div style="height:auto;" class="datagrid-cell attrForm_datagrid-cell-c1-value"><input id="txtFlowName" type="text" name="title" value="' + fname + '" autocomplete="off" class="easyui-textbox"></div></td></tr>'
					+ '</tbody></table></div></div></div></div></div>';
				$node.html(flowDiv);
			},
			ui: {
				'#txtFlowCode': {
					bind: function (data, value, $control) {
						//debugger;
						let t = data.flowCode;
						if (value != undefined) {
							t = value;
							_base.flowCode = t;
						}
						return t;
					}
				},
				'#txtFlowVersion': {
					bind: function (data, value, $control) {
						//debugger;
						let t = data.flowVersion;
						if (value != undefined) {
							t = value;
							_base.flowVersion = t;
						}
						return t;
					}
				},
				'#txtFlowName': {
					bind: function (data, value, $control) {
						//debugger;
						let t = data.flowName;
						if (value != undefined) {
							t = value;
							_base.flowName = t;
						}
						return t;
					}
				}
			}
		};
		$('#attrForm').my( manifest, d );
	},
	// 设置节点属性、样式
	setNodeAttr: function(nodeId) {
		$('#attrForm').my('remove');
		var d = { textId: $('#' + nodeId).children(':first-child').text() };
		var manifest = {
			data: { textId: '' },
			init: function ($node, runtime) {
				$node.html(
			    	'<div class="layui-form-item">' + 
						'<label class="layui-form-label">名称：</label>' + 
						'<div class="layui-input-block">' + 
							'<input id="textId" type="text" name="title" lay-verify="title" autocomplete="off" placeholder="请输入文本信息" class="layui-input">' + 
						'</div>' + 
					'</div>'
				);
			},
			ui: {
				'#textId': {
					bind: function(data, value, $control) {
						var t = data.textId;
						if (value != undefined) {
							t = value;
							FLOW._base.graph.node(nodeId).text = t;
							$('#' + nodeId).children(':first-child').text(t);
						}
						return t;
					}
				}
			}
		};
		$('#attrForm').my( manifest, d );
	},
	// 设置连接线属性、样式
	setConnAttr: function(sourceId, targetId) {
		$('#attrForm').my('remove');
		var d = { connTextId: plumbUtil.getRouterLabel(sourceId, targetId)};
		var manifest = {
			data: { connTextId: '' },
			init: function ($node, runtime) {
				$node.html(
			    	'<div class="layui-form-item">' + 
						'<label class="layui-form-label">连线名称：</label>' + 
						'<div class="layui-input-block">' + 
							'<input id="connTextId" type="text" name="title" lay-verify="title" autocomplete="off" class="layui-input">' + 
						'</div>' + 
					'</div>'
				);
			},
			ui: {
				'#connTextId': {
					bind: function(data, value, $control) {
						var t = data.connTextId;
						if (value != undefined) {
							t = value;
							// 修改保存状态为未保存，将当前流程对象放入可撤销数组中
							$("#saveStatus").css('display', '');
							FLOW._base.undoStack.push(FLOW.getCurrentFlow());
							
							// 设置新文本
							plumbUtil.setRouterLabel(sourceId, targetId, t);
						}
						return t;
					}
				}
			}
		};
		$('#attrForm').my( manifest, d );
	},
	// 设置泳道属性、样式
	setLaneAttr: function(laneId, c) {
		$('#attrForm').my('remove');
		var laneObj = FLOW._base.laneObjs[laneId];
		var d = { laneNameId: laneObj.text };
		var manifest = {
			data: { laneNameId: '' },
			init: function ($node, runtime) {
				$node.html(
			    	'<div class="layui-form-item">' + 
						'<label class="layui-form-label">泳道名称：</label>' + 
						'<div class="layui-input-block">' + 
							'<input id="laneNameId" type="text" name="title" lay-verify="title" autocomplete="off" class="layui-input">' + 
						'</div>' + 
					'</div>'
				);
			},
			ui: {
				'#laneNameId': {
					bind: function(data, value, $control) {
						var t = data.laneNameId;
						if (value != undefined) {
							t = value;
							// 更新泳道对象
							laneObj.text = t;
							
							// 更新节点
							if (laneObj.nodeType == 'broadwiseLane') {
								let tempText = '', textArr = t.split('');
								for (i = 0; i < textArr.length; i++) {
									tempText += '<span style="display: block;">' + textArr[i] + '</span>';
								}
								$('#' + c).html(tempText);
								$('#' + c).css('line-height', ZFSN.getLaneLineHeight(t, $('#' + c).css('height')));
							} else {
								$('#' + c).html('<span>' + t + '</span>');
							}
						}
						return t;
					}
				}
			}
		};
		$('#attrForm').my( manifest, d );
	}
}
