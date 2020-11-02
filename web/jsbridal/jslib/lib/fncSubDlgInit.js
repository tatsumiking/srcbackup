//

var m_nSubDlgSX;
var m_nSubDlgSY;
var m_nSubDlgXSize;
var m_nSubDlgYSize;

var m_areaSubDlgParent;
var m_areaSubDlgMain;
var m_areaSubDlgBar;
var m_areaSubDlgEdit;
var m_nSubDlgEditWidth;
var m_nSubDlgEditHeight;
var m_areaSubDlgBtn;
var m_nSubDlgAddBaseX;
var	m_nSubDlgAddBaseY;
var	m_nSubDlgMouseDown;

var m_fncSubDlgCallBack;

function fncSubDlgInitMain(mainarea)
{
	fncSubDlgInitMainSize(mainarea, 900, 500);
}
function fncSubDlgInitMainSize(mainarea, xsize, ysize)
{
	m_areaSubDlgParent = mainarea;
	width = window.innerWidth; // ブラウザの左右のフチサイズ
	height = window.innerHeight;

	if(width < 1200){
		m_dDlgTrnsTime = width / 1200;
	}else{
		m_dDlgTrnsTime = 1.0;
	}
	m_nSubDlgXSize = xsize * m_dDlgTrnsTime;
	m_nSubDlgYSize = ysize * m_dDlgTrnsTime;
	m_nSubDlgSX = parseInt((width - m_nSubDlgXSize) / 2);
	m_nSubDlgSY = 50*m_dDlgTrnsTime;
	fncSubDlgInitMainCreate();
}
function fncSubDlgInitMainSxSySize(mainarea, sx, sy, xsize, ysize)
{
	m_areaSubDlgParent = mainarea;
	width = window.innerWidth; // ブラウザの左右のフチサイズ
	height = window.innerHeight;

	m_dDlgTrnsTime = 1.0;
	m_nSubDlgSX = sx * m_dDlgTrnsTime;
	m_nSubDlgSY = sy * m_dDlgTrnsTime;
	m_nSubDlgXSize = xsize * m_dDlgTrnsTime;
	m_nSubDlgYSize = ysize * m_dDlgTrnsTime;
	fncSubDlgInitMainCreate();
}
function fncSubDlgInitMainCreate()
{
	var sx, sy;
	var width, height;

	sx = m_nSubDlgSX;
	sy = m_nSubDlgSY;
	width = m_nSubDlgXSize;
	height = m_nSubDlgYSize;
	m_areaSubDlgMain = fnceleCreateArea("subdlgarea", sx, sy, width, height);
	//m_areaSubDlgMain.style.borderRadius= '20px';

	fnceleSetImgPath("img/");
	fnceleImage(m_areaSubDlgMain, "dlgback", "jpg", 0, 0, width, height);
	m_areaSubDlgParent.appendChild(m_areaSubDlgMain);
	fncSubDlgEditAreaSetBorder(m_areaSubDlgMain);
}

function fncSubDlgInitEdit(title)
{
	var titlesize;
	var centersize;
	var bottomsize;
	var sx, sy;
	var width, height;
	var name;
	var fs;

	titlesize = parseInt(30 * m_dDlgTrnsTime);
	bottomsize = parseInt(50 * m_dDlgTrnsTime);
	centersize = m_nSubDlgYSize - titlesize - bottomsize;
	sx = parseInt(0 * m_dDlgTrnsTime);
	sy = parseInt(0 * m_dDlgTrnsTime);
	width = m_nSubDlgXSize;
	height = titlesize;
	name = "dlgbararea";
	m_areaSubDlgBar = fnceleCreateArea(name, sx, sy, width, height);
	m_areaSubDlgBar.style.background = "#0064C8";
	fs = parseInt(height * 0.5);
	sx = height;
	sy = 0;
	fnceleColorText(m_areaSubDlgBar, fs, sx, sy, title, "#FFFFFF");
	m_areaSubDlgMain.appendChild(m_areaSubDlgBar);
	m_areaSubDlgBar.addEventListener("mousedown",fncSubDlgEditMouseDown);
	document.addEventListener("mousemove",fncSubDlgEditMouseMove);
	document.addEventListener("mouseup",fncSubDlgEditMouseUp);

	sx = parseInt(0 * m_dDlgTrnsTime);
	sy = titlesize;
	width = m_nSubDlgXSize;
	height = centersize;
	m_nSubDlgEditWidth = width;
	m_nSubDlgEditHeight = height;
	name = "dlgeditarea";
	m_areaSubDlgEdit = fnceleCreateArea(name, sx, sy, width, height);
	m_areaSubDlgMain.appendChild(m_areaSubDlgEdit);
	m_areaSubDlgEdit.style.overflowY = "auto";
	fncSubDlgEditAreaSetBorder(m_areaSubDlgEdit);

	sx = parseInt(0 * m_dDlgTrnsTime);
	sy = titlesize + centersize;
	width = m_nSubDlgXSize;
	height = bottomsize;
	name = "dlgbtnarea";
	m_areaSubDlgBtn = fnceleCreateArea(name, sx, sy, width, height);
	m_areaSubDlgMain.appendChild(m_areaSubDlgBtn);
}
function fncSubDlgAddRetButton()
{
	var sx, sy;
	var width, height;
	var fontsize;

	sx = m_areaSubDlgBtn.clientWidth - parseInt(100 * m_dDlgTrnsTime);
	sy = parseInt(7 * m_dTrnsTime);
	width = parseInt(90 * m_dDlgTrnsTime);
	height = parseInt(30 * m_dDlgTrnsTime);
	fontsize = height * 0.5;
	var btn = fnceleBtn("戻る", sx, sy, width, height, fontsize);
	m_areaSubDlgBtn.appendChild(btn);
	btn.onclick = fncSubDlgEditRet;
}
function fncSubDlgAddExitButton(fncSubDlgCallBack)
{
	var sx, sy;
	var width, height;
	var fontsize;

	sx = m_areaSubDlgBtn.clientWidth - parseInt(100 * m_dDlgTrnsTime);
	sy = parseInt(7 * m_dTrnsTime);
	width = parseInt(90 * m_dDlgTrnsTime);
	height = parseInt(30 * m_dDlgTrnsTime);
	fontsize = height * 0.5;
	var btn = fnceleBtn("設定", sx, sy, width, height, fontsize);
	m_areaSubDlgBtn.appendChild(btn);
	btn.onclick = fncSubDlgEditExit;
	m_fncSubDlgCallBack = fncSubDlgCallBack;
}
function fncSubDlgEditRet()
{
	m_areaSubDlgParent.removeChild(m_areaSubDlgMain);
}
function fncSubDlgEditExit()
{
	m_areaSubDlgParent.removeChild(m_areaSubDlgMain);
	m_fncSubDlgCallBack();
}
function fncSubDlgEditAreaSetBorder(area)
{
	area.style.borderStyle = "solid";
	area.style.borderWidth = "1px";
	area.style.borderColor = "#AAAAAA";
}
function fncSubDlgEditMouseDown(event)
{
	m_nSubDlgAddBaseX = event.clientX;
	m_nSubDlgAddBaseY = event.clientY;
	m_nSubDlgMouseDown = 1;
}
function fncSubDlgEditMouseMove(event)
{
	var btn = event.buttons;
	var x = event.clientX;
	var y = event.clientY;
	if(m_nSubDlgMouseDown == 1){
		crtx = m_nSubDlgSX + (x - m_nSubDlgAddBaseX);
		crty = m_nSubDlgSY + (y - m_nSubDlgAddBaseY);
		m_areaSubDlgMain.style.left = crtx+"px";
		m_areaSubDlgMain.style.top = crty+"px";
	}
}
function fncSubDlgEditMouseUp(event)
{
	var x = event.clientX;
	var y = event.clientY;
	if(m_nSubDlgMouseDown == 1){
		m_nSubDlgSX = m_nSubDlgSX + (x - m_nSubDlgAddBaseX);
		m_nSubDlgSY = m_nSubDlgSY + (y - m_nSubDlgAddBaseY);
		m_areaSubDlgMain.style.left = m_nSubDlgSX+"px";
		m_areaSubDlgMain.style.top = m_nSubDlgSY+"px";
	}
	m_nSubDlgMouseDown = 0;
}

