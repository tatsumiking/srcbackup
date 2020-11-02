//

var m_dDlgTrnsTime;
var m_nDlgSX;
var m_nDlgSY;
var m_nDlgXSize;
var m_nDlgYSize;

var m_areaDlgParent;
var m_areaDlgMain;
var m_areaDlgBar;
var m_areaDlgEdit;
var m_nDlgEditWidth;
var m_nDlgEditHeight;
var m_areaDlgBtn;
var m_nDlgAddBaseX;
var	m_nDlgAddBaseY;
var	m_nDlgMouseDown;
var m_ctxDlg; // 未使用

function fncDlgSetContext(ctx)
{
	m_ctxDlg = ctx;
}
function fncDlgInitMain(mainarea)
{
	var xsize = parseInt(1000 * m_dTrnsTime);
	var ysize = parseInt(600 * m_dTrnsTime);
	fncDlgInitMainSize(mainarea, xsize, ysize);
}
function fncDlgInitMainSize(mainarea, xsize, ysize)
{
	m_areaDlgParent = mainarea;
	var area = document.getElementById('dlgarea');
	if(area != null){
		fncDlgEditExit();
	}
	width = window.innerWidth; // ブラウザの左右のフチサイズ
	height = window.innerHeight;

	if(width < 1000){
		m_dDlgTrnsTime = width / 1000;
	}else{
		m_dDlgTrnsTime = 1.0;
	}
	m_nDlgXSize = xsize * m_dDlgTrnsTime;
	m_nDlgYSize = ysize * m_dDlgTrnsTime;
	m_nDlgSX = parseInt((width - m_nDlgXSize) / 2);
	m_nDlgSY = 50*m_dDlgTrnsTime;
	fncDlgInitMainCreate();
}
function fncDlgInitMainSxSySize(mainarea, sx, sy, xsize, ysize)
{
	m_areaDlgParent = mainarea;
	width = window.innerWidth; // ブラウザの左右のフチサイズ
	height = window.innerHeight;

	m_dDlgTrnsTime = 1.0;
	m_nDlgSX = sx * m_dDlgTrnsTime;
	m_nDlgSY = sy * m_dDlgTrnsTime;
	m_nDlgXSize = xsize * m_dDlgTrnsTime;
	m_nDlgYSize = ysize * m_dDlgTrnsTime;
	fncDlgInitMainCreate();
}
function fncDlgInitMainCreate()
{
	var sx, sy;
	var width, height;

	sx = m_nDlgSX;
	sy = m_nDlgSY;
	width = m_nDlgXSize;
	height = m_nDlgYSize;
	m_areaDlgMain = fnceleCreateArea("dlgarea", sx, sy, width, height);
	//m_areaDlgMain.style.borderRadius= '20px';

	fnceleSetImgPath("img/");
	fnceleImage(m_areaDlgMain, "dlgback", "jpg", 0, 0, width, height);
	m_areaDlgParent.appendChild(m_areaDlgMain);
	fncDlgEditAreaSetBorder(m_areaDlgMain);

	var canvas = fnceleCreateCanvas("dlgcanvas", 0, 0, width, height);
	ctx = canvas.getContext('2d');
	fncDlgSetContext(ctx);
}

function fncDlgInitEdit(title)
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
	centersize = m_nDlgYSize - titlesize - bottomsize;
	sx = parseInt(0 * m_dDlgTrnsTime);
	sy = parseInt(0 * m_dDlgTrnsTime);
	width = m_nDlgXSize;
	height = titlesize;
	name = "dlgbararea";
	m_areaDlgBar = fnceleCreateArea(name, sx, sy, width, height);
	m_areaDlgBar.style.background = "#0064C8";
	fs = parseInt(height * 0.5);
	sx = height;
	sy = 0;
	fnceleColorText(m_areaDlgBar, fs, sx, sy, title, "#FFFFFF");
	m_areaDlgMain.appendChild(m_areaDlgBar);
	m_areaDlgBar.addEventListener("mousedown",fncDlgEditMouseDown);
	document.addEventListener("mousemove",fncDlgEditMouseMove);
	document.addEventListener("mouseup",fncDlgEditMouseUp);

	sx = parseInt(0 * m_dDlgTrnsTime);
	sy = titlesize;
	width = m_nDlgXSize;
	height = centersize;
	m_nDlgEditWidth = width;
	m_nDlgEditHeight = height;
	name = "dlgeditarea";
	m_areaDlgEdit = fnceleCreateArea(name, sx, sy, width, height);
	m_areaDlgMain.appendChild(m_areaDlgEdit);
	m_areaDlgEdit.style.overflowY = "auto";
	fncDlgEditAreaSetBorder(m_areaDlgEdit);

	sx = parseInt(0 * m_dDlgTrnsTime);
	sy = titlesize + centersize;
	width = m_nDlgXSize;
	height = bottomsize;
	name = "dlgbtnarea";
	m_areaDlgBtn = fnceleCreateArea(name, sx, sy, width, height);
	m_areaDlgMain.appendChild(m_areaDlgBtn);
}
function fncDlgAddRetButton()
{
	var sx, sy;
	var width, height;
	var fontsize;

	sx = m_areaDlgBtn.clientWidth - parseInt(100 * m_dDlgTrnsTime);
	sy = parseInt(7 * m_dTrnsTime);
	width = parseInt(90 * m_dDlgTrnsTime);
	height = parseInt(30 * m_dDlgTrnsTime);
	fontsize = height * 0.5;
	var btn = fnceleBtn("戻る", sx, sy, width, height, fontsize);
	m_areaDlgBtn.appendChild(btn);
	btn.onclick = fncDlgEditExit;
}
function fncDlgAddExitButton()
{
	var sx, sy;
	var width, height;
	var fontsize;

	sx = m_areaDlgBtn.clientWidth - parseInt(100 * m_dDlgTrnsTime);
	sy = parseInt(7 * m_dTrnsTime);
	width = parseInt(90 * m_dDlgTrnsTime);
	height = parseInt(30 * m_dDlgTrnsTime);
	fontsize = height * 0.5;
	var btn = fnceleBtn("設定", sx, sy, width, height, fontsize);
	m_areaDlgBtn.appendChild(btn);
	btn.onclick = fncDlgEditExit;
}
function fncDlgRetExit()
{
	m_areaDlgParent.removeChild(m_areaDlgMain);
}
function fncDlgEditExit()
{
	m_areaDlgParent.removeChild(m_areaDlgMain);
}
function fncDlgEditAreaSetBorder(area)
{
	area.style.borderStyle = "solid";
	area.style.borderWidth = "1px";
	area.style.borderColor = "#AAAAAA";
}
function fncDlgEditMouseDown(event)
{
	m_nDlgAddBaseX = event.clientX;
	m_nDlgAddBaseY = event.clientY;
	m_nDlgMouseDown = 1;
}
function fncDlgEditMouseMove(event)
{
	var btn = event.buttons;
	var x = event.clientX;
	var y = event.clientY;
	if(m_nDlgMouseDown == 1){
		crtx = m_nDlgSX + (x - m_nDlgAddBaseX);
		crty = m_nDlgSY + (y - m_nDlgAddBaseY);
		m_areaDlgMain.style.left = crtx+"px";
		m_areaDlgMain.style.top = crty+"px";
	}
}
function fncDlgEditMouseUp(event)
{
	var x = event.clientX;
	var y = event.clientY;
	if(m_nDlgMouseDown == 1){
		m_nDlgSX = m_nDlgSX + (x - m_nDlgAddBaseX);
		m_nDlgSY = m_nDlgSY + (y - m_nDlgAddBaseY);
		m_areaDlgMain.style.left = m_nDlgSX+"px";
		m_areaDlgMain.style.top = m_nDlgSY+"px";
	}
	m_nDlgMouseDown = 0;
}

