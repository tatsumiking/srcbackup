// 	</div>
var m_szHotelName;
var m_szHotelDB;
var m_szKonreiTable;
var m_strID;
var m_strNO;
var m_strPW;

function fncInit()
{
	m_szHotelName = "ホテルソア";
	m_szHotelDB = "bridal";
	m_szKonreiTable = "bridaluser";
	localStorage.setItem("HotelName", m_szHotelName);
	localStorage.setItem("HotelDB", m_szHotelDB);
	localStorage.setItem("KonreiTable", m_szKonreiTable);

	var inputNo = document.getElementById("userno");
	var inputPw = document.getElementById("password");
	//inputNo.value = "000001";
	//inputPw.value = "496721";
	inputNo.value = "0000";
	inputPw.value = "0000";
	var btnLogin = document.getElementById("btnLogin");
	btnLogin.onclick = fncLoginOnClick;
}
function fncLoginOnClick()
{
	var inputNo = document.getElementById("userno");
	var inputPw = document.getElementById("password");

	m_strID = 0;
	m_strNO = inputNo.value;
	m_strPW = inputPw.value;
	if(m_strNO == "0000" && m_strPW == "0000"){
		localStorage.setItem("UserKind", "1");
		fnc01MenuCall("0", m_strNO, m_strPW);
		return;
	}
	var data = "com="+m_strNO+","+m_strPW+",";
	var fnc = fncCheckNOPWCallback;
	sendRequest("POST","php/checkidpw.php",data,false,fnc);
}
function fncCheckNOPWCallback(xmlhttp)
{
	var data = xmlhttp.responseText;
	var ary = data.split(',');

	localStorage.setItem("UserKind", ary[1]);
	if(ary[0] == "1"){
		fncCheckKonrei(m_strNO, m_strPW);
	}else{
		fnclibAlert("パスワードが違います");
	}
}
function fncCheckKonrei(strNO, strPW)
{
	var dbnm = m_szHotelDB;
	var krtbl = m_szKonreiTable;
	var data = "dbnm="+dbnm+"&krtbl="+krtbl+"&uno="+strNO+"&upw="+strPW;
	var fnc = fncCheckKonreiCallBack;
	sendRequest("POST","php/initkonrei.php",data,false,fnc);
}
function fncCheckKonreiCallBack(xmlhttp)
{
	var data = xmlhttp.responseText;
	var ary = data.split(',');

	if(ary[0] == "1"){
		m_strID = ary[1];
		fnc01MenuCall(m_strID, m_strNO, m_strPW);
	}
}
function fnc01MenuCall(strID, strNO, strPW)
{
	localStorage.setItem("BridalID", strID);
	localStorage.setItem("BridalNO", strNO);
	localStorage.setItem("BridalPW", strPW);

	var url = "01menu.html";
	window.location.href = url;
}
