var CONFIG = {
	"defaultConfig": {
		"resizableFlag": false,
		"smallMovePX": 1,
		"testPwd": "root",
		"language": "zh_cn"
	},
	"defaultStyle": {
		"selectedBoxShadow": "rgb(0, 0, 0) 0px 0px 20px 0px",
		"noSelectedBoxShadow": "",
		"dragOpacity": 0.8
	},
	"conn": {
		"stroke": "#2a2929",
		"strokeWidth": 3,
		"hoverConnStroke": "#d58512",
		"connectionType": "Flowchart",
		"connectionGap": 5,
		"connectionCornerRadius": 8,
		"connectionAlwaysRespectStubs": true,
		"connectionStub": 30,
		"isDetachable": false
	},
	"arrow": {
		"arrowWidth": 12,
		"arrowLength": 12,
		"arrowLocation": 1
	},
	"endPonit": {
		"fill": "#456",
		"stroke": "#2a2929",
		"strokeWidth": 1,
		"radius": 3,
		"hoverEndPointStroke": "#d58512"
	},
	"anchors": {
		"defaultAnchors": ["Bottom", "Right", "Top", "Left"],
		"sourceAnchors": [],
		"targetAnchors": [],
		"sameAnchorsFlag": true
	},
	"alignParam": {
		"levelDistance": 100,
		"verticalDistance": 100,
		"alignDuration": 500
	},
	"keyboardParam": {
		"multipleSelectedKey": 17,
		"deleteKey": 46,
		"upKey": 38,
		"downKey": 40,
		"leftKey": 37,
		"rightKey": 39,
		"undoKey": 90,
		"redoKey": 89,
		"selectedAllKey": 65,
		"saveKey": 83,
		"save2PhotoKey": 80,
		"clearKey": 68,
		"showGridKey": 81,
		"mouseToolKey": 81,
		"connectionToolKey": 82,
		"settingKey": 70
	},
	// 默认值
	"msg": {
		"noNode": "保存失败，流程图不存在任何节点",
		"noConn": "保存失败，流程图存在未连接的节点",
		"noSetNodeName": "保存失败，请设置节点名称",
		"hasAcyclic": "保存失败，流程图中存在循环路径",
		"noNodeBySave2Photo": "图中无节点，无法保存为图片",
		"deleteConn": "确定要删除连接吗？",
		"deleteLane": "删除成功",
		"deleteNode": "删除成功",
		"chooseNodeObjErr": "节点类型选择错误！",
		"saveSuccess": "流程保存成功",
		"saveObjErr": "发生未知错误，保存失败，请联系管理员",
		"clearConfirm": "确定要重新绘制吗？",
		"noFrontRoute": "无前继路径",
		"noBehindRoute": "无后续路径",
		"alignWayCheck": "请选择两个或两个以上的节点对齐",
		"repeatRouter": "重复路由",
		"closeFrame": "当前流程图还未保存，确认退出吗？",
		"repeatStartNode": "重复的开始节点",
		"repeatEndNode": "重复的结束节点",
		"currentProgress": "当前进度",
		"noFrontNode": "无前继节点",
		"noBehindNode": "无后续节点",
		"frontNode": "前继节点",
		"behindNode": "后续节点",
		"flowPublish": "流程已完成"
	}
}