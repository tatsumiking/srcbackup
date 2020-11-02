var m_sfnceleImgPath = "img/";
var m_sfnceleBtnPath = "btn/";
var m_nfnceleFontSize = 20;

var getBrowser = function(){
    var ua = window.navigator.userAgent.toLowerCase();
    var ver = window.navigator.appVersion.toLowerCase();
    var name = 'unknown';

    if (ua.indexOf("msie") != -1){
        if (ver.indexOf("msie 6.") != -1){
            name = 'ie6';
        }else if (ver.indexOf("msie 7.") != -1){
            name = 'ie7';
        }else if (ver.indexOf("msie 8.") != -1){
            name = 'ie8';
        }else if (ver.indexOf("msie 9.") != -1){
            name = 'ie9';
        }else if (ver.indexOf("msie 10.") != -1){
            name = 'ie10';
        }else{
            name = 'ie';
        }
    }else if(ua.indexOf('trident/7') != -1){
        name = 'ie11';
    }else if (ua.indexOf('chrome') != -1){
        name = 'chrome';
    }else if (ua.indexOf('safari') != -1){
        name = 'safari';
    }else if (ua.indexOf('opera') != -1){
        name = 'opera';
    }else if (ua.indexOf('firefox') != -1){
        name = 'firefox';
    }
    return name;
};

function fnceleSetImgPath(path)
{
	m_sfnceleImgPath = path;
}

function fnceleSetFontSize(size)
{
	m_nfnceleFontSize = size;
}

function fnceleCreateArea(id, sx, sy, wd, hi)
{
	var area = document.createElement("div");
	area.id = id;
	if(sx != -1){
		area.style.left = sx+"px";
	}
	if(sy != -1){
		area.style.top = sy+"px";
	}
	if(wd != -1){
		area.style.width = wd+"px";
	}
	if(hi != -1){
		area.style.height = hi+"px";
	}
	area.style.position = "absolute";
	return(area);
}

function fnceleCreateCanvas(id, sx, sy, wd, hi)
{
	var canvas = document.createElement("CANVAS");
	canvas.id = id;
	canvas.style.left = sx+"px";
	canvas.style.top = sy+"px";
	canvas.style.width = wd+"px";
	canvas.style.height = hi+"px";
	canvas.style.position = "absolute";
	return(canvas);
}

function fnceleCreateCommboBoxs(id, size, area, sx, sy, fontsize)
{
	var cmb;
	var hi;

	hi = fontsize * 1.8;
	cmb = document.createElement("select");
	cmb.id = id;
	cmb.style.fontSize = fontsize+"px";
	cmb.style.width = size+"px";
	cmb.style.height = hi+"px";
	cmb.style.borderRadius="5px";
	fnceleSetSubElement(area, sx, sy, cmb);
	return(cmb);
}
function fnceleCreateTextRadio(area, name, sx, sy, str, fs, fnc)
{
	var rdo = fnceleCreateRadio(name, fs, fs);
	var div = fnceleSetSubElement(area, sx, sy, rdo);
	var txt = fnceleCreateRdoText(id, fs, str);
	rdo.onclick = fnc;
	txt.onclick = fnc;
	div.onmousedown = fnc;
	div.onclick = fnc;
	div.id = id;
	div.appendChild(txt);
	return(rdo);
}

function fnceleCreateRadio(name, fs, fs)
{
	var rdo = document.createElement("input");
	rdo.type = "radio";
	rdo.name = name;
	rdo.style.width = fs+"px";
	rdo.style.height = fs +"px";
	return(rdo);
}

function fnceleCreateRdoText(id, fs, str)
{
	var text = document.createElement("text");
	text.type = "text";
	text.id = "s"+id;
	text.innerHTML = str;
	text.style.cursor = "pointer";
	text.style.font = fs+"px 'ÇlÇr ÉSÉVÉbÉN'";
	return(text);
}
function fnceleDivBtn(area, str, sx, sy, xsize, ysize, fs)
{
	divbtn = document.createElement("div");
	divbtn.textContent = str;
	divbtn.style.lineHeight = ysize + "px";
	divbtn.style.fontSize = fs+"px";
	divbtn.style.left = sx+"px";
	divbtn.style.top = sy+"px";
	divbtn.style.position = "absolute";
	divbtn.style.width = xsize+"px";
	divbtn.style.height = ysize+"px";
	area.appendChild(divbtn);
	return(divbtn);
}
function fnceleBtn(str, sx, sy, xsize, ysize, fs)
{
	btn = document.createElement("input");
	btn.type = "button";
	btn.value = str;
	btn.style.fontSize = fs+"px";
	btn.style.left = sx+"px";
	btn.style.top = sy+"px";
	btn.style.position = "absolute";
	btn.style.width = xsize+"px";
	btn.style.height = ysize+"px";
	btn.style.borderRadius="5px";
	return(btn);
}
function fnceleFileLblBtn(area, str, sx, sy, xsize, ysize, fncCB)
{
	var fs = m_nfnceleFontSize;
	var lbl = document.createElement("label");
	lbl.htmlFor="upload";
	lbl.innerHTML = str;
	lbl.style.fontSize = fs+"px";
	lbl.style.background = "#ccc";
	lbl.style.position = "absolute";
	lbl.style.width = xsize + "px";
	lbl.style.height = ysize + "px";
	lbl.style.left = sx+"px";
	lbl.style.top = sy+"px";
	var btn = document.createElement("input");
	btn.type = "file";
	btn.id = "upload";
	btn.style.display = "none";
	btn.onchange = fncCB;
	lbl.appendChild(btn);
	sidearea.appendChild(lbl);
	return(lbl);
}
function fnceleFileBtn(sx, sy, fontsize)
{
	btn = document.createElement("input");
	btn.type = "file";
	btn.name = "uploadfile";
	btn.id = "upload";
	btn.style.fontSize = fontsize+"px";
	btn.style.left = sx+"px";
	btn.style.top = sy+"px";
	btn.style.position = "absolute";
	btn.style.borderRadius="5px";
	return(btn);
}
function fnceleTextButton(area, sx, sy, wd, fs, str, fncClick)
{
	var txt = fnceleCreateText(str, fs, "#000000");
	txt.style.paddingLeft = '5px'; 
	txt.style.paddingRight = '5px'; 
	var div = fnceleSetSubElement(area, sx, sy, txt);
	div.onclick = fncClick;
	div.style.width = wd+'px';
	div.style.background = "#DDD";
	div.style.borderRight = "solid 2px #888";
	div.style.borderBottom = "solid 2px #888";
	return(div);
}
function fnceleText4Button(area, sx, sy, wd, fs, str, fncClick)
{
	var txt = fnceleCreateText(str, fs, "#000000");
	txt.style.paddingLeft = '-5px'; 
	txt.style.paddingRight = '-5px'; 
	var div = fnceleSetSubElement(area, sx, sy, txt);
	div.onclick = fncClick;
	div.style.width = wd+'px';
	div.style.background = "#DDD";
	div.style.borderRight = "solid 2px #888";
	div.style.borderBottom = "solid 2px #888";
	return(div);
}
// ï°êîçsì¸óÕóÃàÊ
function fnceleCreateKAreaInputBoxs(area, id, x, y, co, ln, fontsize)
{
	var area;

	ed = document.createElement("textarea");
	ed.id = id;
	ed.cols = co;
	ed.rows = ln;
	ed.style.borderRadius="5px";
	ed.style.border = "solid 2px #f00";
	ed.style.fontSize = fontsize+'px';
	ed.style.imeMode = "active";
	div = fnceleSetSubElement(area, x, y, ed);
	div.con
	return(ed);
}
function fnceleCreateKInputBoxs(area, id, size, x, y, fontsize)
{
	var ed;

	ed = fnceleCreateInputBoxs(id, size, fontsize);
	ed.style.imeMode = "active";
	fnceleSetSubElement(area, x, y, ed);
	return(ed);
}
function fnceleCreateInputBoxs(id, size, fontsize)
{
	var ed;

	ed = document.createElement("input");
	ed.type = "text";
	ed.id = id;
	ed.style.width = size+"px";
	ed.style.fontSize = fontsize;
	ed.style.borderRadius= '5px';
	ed.style.paddingLeft = '10px'; 
	ed.style.paddingTop = '4px'; 
	return(ed);
}

function fnceleColorText(area, fs, sx, sy, str, clr)
{
	var len = str.length+1;
	var width = fs * len;
	var text = fnceleCreateText(str, fs, clr);
	var div = fnceleSetSubElement(area, sx, sy, text);
	div.style.width = width+"px";
	return(text);
}
function fnceleText(area, fs, sx, sy, str)
{
	var len = str.length+1;
	var width = fs * len;
	var text = fnceleCreateText(str, fs, "#000000");
	var div = fnceleSetSubElement(area, sx, sy, text);
	div.style.width = width+"px";
	return(text);
}
function fnceleRightText(area, fs, ex, sy, str)
{
	var len = str.length+1;
	var width = fs * len;
	var text = fnceleCreateText(str, fs, "#000000");
	var div = fnceleSetSubElement(area, ex-width, sy, text);
	div.style.width = width+"px";
	return(text);
}
function fnceleImgBtn(id, safix)
{
	var btn;

	btn = document.createElement("input");
	btn.type = "image";
	btn.id = id;
	btn.src = m_sfnceleImgPath+id+"."+safix;
	return(btn);
}
function fnceleImage(area, name, safix, sx, sy, xsize, ysize)
{
	var img = document.createElement("img");
	img.src = m_sfnceleImgPath+name+"."+safix;
	img.style.width = xsize+"px";
	img.style.height = ysize+"px";
	var div = fnceleSetSubElement(area, sx, sy, img);
	div.style.padding = '0px';
	return(div);
}

function fnceleCreateText(str, size, color)
{
	m_nfnceleFontSize = size;
	var text = document.createElement("text");
	text.type = "text";
	text.innerHTML = str;
	text.style.font = m_nfnceleFontSize+"px 'ÇlÇr ÉSÉVÉbÉN'";
	text.style.cursor = "default";
	text.style.color = color;
	return(text);
}

function fnceleCreateATag(str, size, color)
{
	m_nfnceleFontSize = size;
	var atag = document.createElement("a");
	atag.type = "text";
	atag.innerHTML = str;
	atag.style.font = m_nfnceleFontSize+"px 'ÇlÇr ÉSÉVÉbÉN'";
	atag.style.cursor = "default";
	atag.style.color = color;
	return(atag);
}

function fnceleCreateBtnText(str, size, color)
{
	m_nfnceleFontSize = size;
	var text = document.createElement("text");
	text.type = "text";
	text.innerHTML = str;
	text.style.font = m_nfnceleFontSize+"px 'ÇlÇr ÉSÉVÉbÉN'";
	text.style.cursor = "pointer";
	text.style.color = color;
	return(text);
}

function fnceleCreateBtnBoldText(str, size, color)
{
	m_nfnceleFontSize = size;
	var text = document.createElement("text");
	text.type = "text";
	text.innerHTML = str;
	text.style.font = m_nfnceleFontSize+"px 'ÇlÇr ÉSÉVÉbÉN'";
	text.style.fontWeight = "bold";
	text.style.cursor = "pointer";
	text.style.color = color;
	return(text);
}

function fnceleCreateImgButtonEx(area, id, safix, x, y, wd, hi, fnc)
{
	var btn;

	btn = document.createElement("input");
	btn.type = "image";
	btn.id = id;
	btn.style.width = wd+"px";
	btn.style.height = hi+"px";
	btn.src = m_sfnceleImgPath+id+"."+safix;
	btn.onclick = fnc;
	fnceleSetSubElement(area, x, y, btn);
	return(btn);
}

function fnceleCreateImgButton(area, id, safix, x, y, xsize, ysize, fnc)
{
	var btn;

	btn = document.createElement("input");
	btn.type = "image";
	btn.id = id;
	btn.src = m_sfnceleImgPath+id+"."+safix;
	btn.style.width = xsize+"px";
	btn.style.height = ysize+"px";
	btn.onclick = fnc;
	fnceleSetSubElement(area, x, y, btn);
	return(btn);
}

function fnceleCreateDiv(area, x, y, xsize, ysize)
{
	var div = document.createElement("div");
	div.style.left = x+"px";
	div.style.top = y+"px";
	div.style.position = "absolute";
	div.style.width = xsize + "px";
	div.style.height = ysize + "px";
	area.appendChild(div);
	return div;
}

function fnceleSetSubElement(area, x, y, subelement)
{
	var div = document.createElement("div");
	div.style.left = x+"px";
	div.style.top = y+"px";
	div.style.position = "absolute";
	area.appendChild(div);
	div.appendChild(subelement);
	return div;
}

function fnceleSetBtnDisabledFlag(name, safix, flag)
{
	var btn;

	btn = document.getElementById(name);
	if(flag == true){
		if(btn.disabled == false){
			btn.src = m_sfnceleBtnPath+name+"X."+safix;
			btn.disabled = true;
		}
	}else{
		if(btn.disabled == true){
			btn.src = m_sfnceleBtnPath+name+"."+safix;
			btn.disabled = false;
		}
	}
}

function fnceleSetBtnEvent(name, safix)
{
	var btn;

	btn = document.getElementById(name);
	btn.src = m_sfnceleBtnPath+name+"."+safix;

	btn.onmousedown = function() {
		btn.src = m_sfnceleBtnPath+name+"D."+safix;
	};
	btn.onmouseup = function() {
		btn.src = m_sfnceleBtnPath+name+"."+safix;
	};
	btn.onmouseover = function() {
		btn.src = m_sfnceleBtnPath+name+"O."+safix;
	};
	btn.onmouseleave = function() {
		btn.src = m_sfnceleBtnPath+name+"."+safix;
	};
}

