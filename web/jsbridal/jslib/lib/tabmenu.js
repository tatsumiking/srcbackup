
var m_aryTabMenuGroup;

var m_nTabItemWidth;
var m_nTabItemHeight;
var m_nTabItemFontSize;
var m_nTabSelectAreaWidth;
var m_nTabSelectAreaHeight;
var m_nTabButtonLeft;
var m_nTabButtonTop;
var m_nTabButtonWidth;
var m_nTabButtonHeight;
var m_nTabButtonFontSize;

class clsTabItem {
	constructor(sx, sy, wd, hi){
		this.sx = sx;
		this.sy = sy;
		this.wd = wd;
		this.hi = hi;
		this.name = "";
		this.img = new Image();
	}
}
function clsTabGroup()
{
    this.sGroupName;
	this.fncTabMenuClickCallBack;
	this.fncTabButtonClickCallBack;
    this.selectid;
}
function fncTabMenuInit()
{
    m_aryTabMenuGroup = new Array();
}
function fncTabItemSize(width, height, fontsize)
{
	m_nTabItemWidth = width;
	m_nTabItemHeight = height;
	m_nTabItemFontSize = fontsize;
}
function fncTabSelectAreaSize(width, height)
{
	m_nTabSelectAreaWidth = width;
	m_nTabSelectAreaHeight = height;
}
function fncTabButtonSize(left, top, width, height, fontsize)
{
	m_nTabButtonLeft = left;
	m_nTabButtonTop = top;
	m_nTabButtonWidth = width;
	m_nTabButtonHeight = height;
	m_nTabButtonFontSize = fontsize;
}
function fcnTabCreateElement(setarea, sGroupName, sIdList, sStrList, sBtnStr)
{
	var idx, max;
	var item;

	var menudiv = document.createElement("div");
	menudiv.className = "TabMenuArea";
	setarea.appendChild(menudiv);

	var aryId = sIdList.split(',');
	var aryStr = sStrList.split(',');
	max = aryId.length;
	for(idx = 0; idx < max; idx++){
		item = document.createElement("div");
		item.className = "TabItem";
		item.style.width = m_nTabItemWidth+"px";
		item.style.height = m_nTabItemHeight+"px";
		item.style.fontSize = m_nTabItemFontSize+"px";
		item.id = aryId[idx];
		item.innerText = aryStr[idx];
		item.onclick = fncTabMenuClick;
		item.accessKey = sGroupName;
		menudiv.appendChild(item);
	}
	var selectdiv = document.createElement("div");
	selectdiv.className = "TabSelectArea";
	selectdiv.id = "div"+sGroupName;
	selectdiv.style.width = m_nTabSelectAreaWidth+"px";
	selectdiv.style.height = m_nTabSelectAreaHeight+"px";
	//selectdiv.style.background = "#f88";
	setarea.appendChild(selectdiv);

	var selectcanvas = document.createElement("canvas");
	selectcanvas.className = "TabSelectCanvas";
	selectcanvas.id = "canvas"+sGroupName;
	selectcanvas.style.width = m_nTabSelectAreaWidth+"px";
	selectcanvas.style.height = m_nTabSelectAreaHeight+"px";
	//selectcanvas.style.background = "#8f8";
	selectdiv.appendChild(selectcanvas);

	var selectbtn = document.createElement("Button");
	selectbtn.className = "TabSelectButton";
	selectbtn.id = "btn"+sGroupName;
	selectbtn.style.position = "relative ";
	selectbtn.style.left = m_nTabButtonLeft+"px";
	selectbtn.style.top = m_nTabButtonTop+"px";
	selectbtn.style.width = m_nTabButtonWidth+"px";
	selectbtn.style.height = m_nTabButtonHeight+"px";
	selectbtn.style.fontSize = m_nTabButtonFontSize+"px";
	selectbtn.innerText = sBtnStr;
	selectbtn.accessKey = sGroupName;
	selectbtn.onclick = fncTabButtonClick;
	selectdiv.appendChild(selectbtn);

	return(selectcanvas);
}
function fncTabMenuNewGroup(name, fncTabCallBack, fncBtnCallBack, tabid = "")
{
    var cTabGrou = new clsTabGroup();
    cTabGrou.sGroupName = name;
	cTabGrou.fncTabMenuClickCallBack = fncTabCallBack;
	cTabGrou.fncTabButtonClickCallBack = fncBtnCallBack;
    cTabGrou.selectid = tabid;
    m_aryTabMenuGroup.push(cTabGrou);
    if(tabid != ""){
        var tab = document.getElementById(tabid);
        tab.className = 'TabItem is-active';
		fncTabCallBack(tabid);
    }
}
function fncTabMenuClick()
{
    var idx, max;
    var tabid;

    max = m_aryTabMenuGroup.length;
    for(idx = 0; idx < max; idx++){
        if(this.accessKey == m_aryTabMenuGroup[idx].sGroupName)
        {
            tabid = m_aryTabMenuGroup[idx].selectid;
            if(tabid != "")
            {
                tab = document.getElementById(tabid);
                tab.className = 'TabItem';
            }
            break;
        }
    }
	if(idx < max){
	    m_aryTabMenuGroup[idx].selectid = this.id;
    	this.className = 'TabItem is-active';
		m_aryTabMenuGroup[idx].fncTabMenuClickCallBack(this.id);
	}
}
function fncTabButtonClick()
{
    var idx, max;
    var tabid;
	var tab;

    max = m_aryTabMenuGroup.length;
    for(idx = 0; idx < max; idx++){
        if(this.accessKey == m_aryTabMenuGroup[idx].sGroupName)
        {
            break;
        }
    }
	if(idx < max){
		tabid = m_aryTabMenuGroup[idx].selectid;
        tab = document.getElementById(tabid);
		m_aryTabMenuGroup[idx].fncTabButtonClickCallBack(tab.id);
	}
}

