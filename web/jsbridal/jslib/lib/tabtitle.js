
var m_aryTabTitleGroup;

var m_nTabTitleWidth;
var m_nTabTitleHeight;
var m_nTabTitleFontSize;

function clsTabTitleGroup()
{
    this.sGroupName;
	this.fncClickCallBack;
    this.selectid;
}
function fncTabTitleInit()
{
    m_aryTabTitleGroup = new Array();
}
function fncTabTitleItemSize(width, height, fontsize)
{
	m_nTabTitleWidth = width;
	m_nTabTitleHeight = height;
	m_nTabTitleFontSize = fontsize;
}
function fcnTabTitleCreateElement(setarea, sGroupName, sIdList, sStrList)
{
	var idx, max;
	var item;

	var menudiv = document.createElement("div");
	menudiv.className = "TabTitleArea";
	setarea.appendChild(menudiv);

	var aryId = sIdList.split(',');
	var aryStr = sStrList.split(',');
	max = aryId.length;
	for(idx = 0; idx < max; idx++){
		item = document.createElement("div");
		item.className = "TabTitleItem";
		item.style.width = m_nTabTitleWidth+"px";
		item.style.height = m_nTabTitleHeight+"px";
		item.style.fontSize = m_nTabTitleFontSize+"px";
		item.id = aryId[idx];
		item.innerText = aryStr[idx];
		item.onclick = fncTabTitleClick;
		item.accessKey = sGroupName;
		menudiv.appendChild(item);
	}
}
function fncTabTitleNewGroup(name, fncTabCallBack, tabid)
{
    var cTabGrou = new clsTabGroup();
    cTabGrou.sGroupName = name;
	cTabGrou.fncClickCallBack = fncTabCallBack;
    cTabGrou.selectid = tabid;
    m_aryTabTitleGroup.push(cTabGrou);
    if(tabid != ""){
        var tab = document.getElementById(tabid);
        tab.className = 'TabTitleItem is-active';
		fncTabCallBack(tabid);
    }
}
function fncTabTitleClick()
{
    var idx, max;
    var tabid;

    max = m_aryTabTitleGroup.length;
    for(idx = 0; idx < max; idx++){
        if(this.accessKey == m_aryTabTitleGroup[idx].sGroupName)
        {
            tabid = m_aryTabTitleGroup[idx].selectid;
            if(tabid != "")
            {
                tab = document.getElementById(tabid);
                tab.className = 'TabTitleItem';
            }
            break;
        }
    }
	if(idx < max){
	    m_aryTabTitleGroup[idx].selectid = this.id;
    	this.className = 'TabTitleItem is-active';
		m_aryTabTitleGroup[idx].fncClickCallBack(this.id);
	}
}
