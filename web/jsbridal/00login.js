// 	</div>

function fncInit()
{
	var btnLogin = document.getElementById("btnLogin");
	btnLogin.onclick = fncLoginOnClick;
}
function fncLoginOnClick()
{
	var inputId = document.getElementById("userid");
	var inputPw = document.getElementById("password");
	var strID = inputId.value;
	var strPW = inputPw.value;
	fnclibAlert("ID->"+strID+"\r\nPW->"+strPW);
	var ret = funcCheckIDPW(strID, strPW);
	if(ret == 1){
		fnc01MenuCall(strID, strPW);
	}
}
function funcCheckIDPW(strID, strPW)
{
	return(1);
}
function fnc01MenuCall(strID, strPW)
{
	localStorage.setItem("BridalID", strID);
	localStorage.setItem("BridalPW", strPW);

	var url = "01menu.html";
	window.location.href = url;
}
