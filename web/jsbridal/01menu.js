// 

var m_strID;
var m_strPW;
var m_strUserKind;

function fncInit()
{
	m_strID = localStorage.getItem("BridalID");
	m_strPW = localStorage.getItem("BridalPW");
	m_strUserKind = localStorage.getItem("UserKind");

	var btnKihon = document.getElementById("btnKihon");
	btnKihon.onclick = fncOnClickKihon;
	var btnGest = document.getElementById("btnGest");
	btnGest.onclick = fncOnClickGest;
	var btnTableLayout = document.getElementById("btnTableLayout");
	btnTableLayout.onclick = fncOnClickTableLayout;
	var btnGestSit = document.getElementById("btnGestSit");
	btnGestSit.onclick = fncOnClickGestSit;
	var btnExit = document.getElementById("btnExit");
	btnExit.onclick = fncOnClickExit;

	var btnInit = document.getElementById("btnInit");
	if(m_strUserKind == "1"){
		btnInit.onclick = fncOnClickInit;
	}else{
		btnInit.style.display = 'none';
	}

 }
function fncOnClickKihon()
{
	var url = "02kihon.html";
	window.location.href = url;
}
function fncOnClickGest()
{
	var url = "03gest.html";
	window.location.href = url;
}
function fncOnClickTableLayout()
{
	var url = "04tablelayout.html";
	window.location.href = url;
}
function fncOnClickGestSit()
{
	var url = "05gestsit.html";
	window.location.href = url;
}
function fncOnClickExit()
{
	var url = "index.html";
	window.location.href = url;
}
function fncOnClickInit()
{
	var url = "util/index.html";
	window.location.href = url;
}
