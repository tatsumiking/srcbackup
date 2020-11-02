
var m_aryAccordionGroup;
var m_aryAccordionButton;
var m_aryAccordionMenu;
var m_aryAccordionHeight;
var m_aryAccordionValue;
var m_aryAccordionCallBack;
var m_nAccordionCrtIndex;
var m_nAccordionWidth;
var m_nAccordionHeight;
var m_nAccordionFontSize;

// Arrayをまとめて下記クラスアレイにする必要なり
function clsAccordionGroup()
{
    this.sGroupName;
	this.nHeight;
	this.fncCallBack;
    this.value;
}
function fncAccordionInit(width, height, fontsize)
{
	m_aryAccordionGroupName = new Array();
	m_aryAccordionButton = new Array();
	m_aryAccordionMenu = new Array();
	m_aryAccordionHeight = new Array();
	m_aryAccordionValue = new Array();
	m_aryAccordionCallBack = new Array();
	m_nAccordionCrtIndex = -1;
	m_nAccordionWidth = width;
	m_nAccordionHeight = height;
	m_nAccordionFontSize = fontsize;
}
function fncAccordionSetCrtIndex(sGroupName)
{
	var max, idx;

	max = m_aryAccordionGroupName.length;
	for(idx = 0; idx < max; idx++){
		if(m_aryAccordionGroupName[idx] == sGroupName){
			m_nAccordionCrtIndex = idx;
		}
	}
}
function fncAccordionElement(area, sGroupName, sTitle, sItemList)
{
	var max, idx;
	var lbl;
	var inp;
	var br;

	var ary = sItemList.split(',');
	var btn = document.createElement("button");
	btn.id = "btn"+sGroupName;
	btn.className = "AccordTitle";
	btn.style.width = m_nAccordionWidth+"px";
	btn.style.height = m_nAccordionHeight+"px";
	btn.style.fontSize = m_nAccordionFontSize+"px";
	btn.innerText = sTitle;
	area.appendChild(btn);
	var div = document.createElement("div");
	div.id = "div"+sGroupName;
	div.className = "DefMenuArea AccordionMenu";
	div.style.width = m_nAccordionWidth+"px";
	area.appendChild(div);
	max = ary.length;
	for(idx = 0; idx < max; idx++){
		inp = document.createElement("input");
		inp.type = "radio";
		inp.id = sGroupName+idx;
		inp.name = sGroupName;
		inp.value = idx;
		inp.style.fontSize = m_nAccordionFontSize+"px";
		div.appendChild(inp);
		lbl = document.createElement("label");
		lbl.htmlFor = sGroupName+idx;;
		lbl.innerText = ary[idx];
		lbl.style.fontSize = m_nAccordionFontSize+"px";
		//lbl.style.width = m_nAccordionWidth+"px";
		//lbl.style.background = "#ffcccc";
		div.appendChild(lbl);
		br = document.createElement("BR");
		div.appendChild(br);
	}
}
function fncAccordionSetRedioButton(id)
{
	var radio;

	radio = document.getElementById(id);
	radio.checked = true;
	fncAccordionSetMenuTitle(radio);
}
function fncAccordionSetTitle(id)
{
	var radio;

	radio = document.getElementById(id);
	fncAccordionSetMenuTitle(radio);
}
function fncAccordionSet(sGroupName, callback, val="-1")
{
	var btnid, divid;
	var max, idx;
	var label;
	var radio;
	var title;

	btnid = "btn"+sGroupName;
	divid = "div"+sGroupName;
	var btn = document.getElementById(btnid);
	var menu = document.getElementById(divid);
	menu.style.height = 'auto';
	var height = menu.clientHeight;
	max = menu.children.length;
	for(idx = 0; idx < max; idx += 3){
		radio = menu.children[idx];
		radio.onclick = fncAccordionRadioClick;
		label = menu.children[idx+1];
		label.onclick = fncAccordionLabelClick;
		radio.label = label;
		if(radio.value == val){
			if(btnid == "btnKindSelect"){
				title = "";
			}else if(btnid == "btnSizeSelect"){
				title = "サイズ ";
			}else if(btnid == "btnKijiSelect"){
				title = "生地 ";
			}else if(btnid == "btnHatomeSelect"){
				title = "ハトメ ";
			}else if(btnid == "btnFurinjiSelect"){
				title = "フリンジ ";
			}
			btn.innerText = title+label.innerText;
			radio.checked = true;
		}
	}
	menu.style.height = '0px';
	btn.onclick = fncAccordionButtonClick;
	m_aryAccordionGroupName.push(sGroupName);
	m_aryAccordionButton.push(btn);
	m_aryAccordionMenu.push(menu);
	m_aryAccordionHeight.push(height);
	m_aryAccordionValue.push(val);
	m_aryAccordionCallBack.push(callback);

}
function fncAccordionButtonClick() 
{
	var max, idx;

	max = m_aryAccordionButton.length;
	for(idx = 0; idx < max; idx++){
		m_aryAccordionMenu[idx].style.height = '0px';
	}
	idx = fncAccordionGetIndex(this);
	lastH = m_aryAccordionMenu[idx].style.height;
	if(lastH == '0px'){
		var setH = m_aryAccordionHeight[idx] + 'px';
		m_aryAccordionMenu[idx].style.height = setH;
		m_nAccordionCrtIndex = idx;
	}else{
		m_aryAccordionMenu[idx].style.height = '0px';
	}
}
function fncAccordionRadioClick()
{
	fncAccordionSetMenuTitle(this);
}
function fncAccordionLabelClick()
{
	var radioid;
	var radio;

	radioid = this.htmlFor;
	radio = document.getElementById(radioid);
	fncAccordionSetMenuTitle(radio);
}
function fncAccordionSetMenuTitle(radio)
{
	var name;
	var label;
	var str;
	var val;
	var setStr;
	var fncCallBack;

	setStr = m_aryAccordionButton[m_nAccordionCrtIndex].innerText;
	name = radio.name;
	label = radio.label;
	str = label.innerText;
	val = radio.value;
	if(name == "KindSelect"){
		setStr = ""+str;
	}else if(name == "SizeSelect"){
		setStr = "サイズ "+str;
	}else if(name == "KijiSelect"){
		setStr = "生地 "+str;
	}else if(name == "HatomeSelect"){
		setStr = "ハトメ "+str;
	}else if(name == "FurinjiSelect"){
		setStr = "フリンジ "+str;
	}
	m_aryAccordionButton[m_nAccordionCrtIndex].innerText = setStr;
	m_aryAccordionMenu[m_nAccordionCrtIndex].style.height = '0px';
	fncCallBack = m_aryAccordionCallBack[m_nAccordionCrtIndex];
	fncCallBack(val);
}
function fncAccordionGetIndex(btn)
{
	var max, idx;
	max = m_aryAccordionButton.length;
	for(idx = 0; idx < max; idx++){
		if(m_aryAccordionButton[idx] == btn){
			return(idx);
		}
	}
	return(0);
}
