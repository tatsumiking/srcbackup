function fncInit()
{
	data = "com=,,";
	sendRequest("POST","php/getip.php",data,false,fncGetIPCallback);
}
function fncGetIPCallback(xmlhttp)
{
	var divIp = document.getElementById("ipaddress");
	var str = xmlhttp.responseText;
	divIp.innerText = str;
}
function getCurrentIPv4Address_Success(ip){
	alert(ip);
}
function getCurrentIPv4Address_Error(err){
	alert(err.code);
}
function fncOnClick()
{
	var inputId = document.getElementById("userid");
	var inputPw = document.getElementById("password");
	var strID = inputId.value;
	var strPW = inputPw.value;
	fnclibAlert("ID->"+strID+"\r\nPW->"+strPW);
	if(strID == "000000" && strPW == "000000"){
		fncGotoKihonElement(strID, strPW);
	}
}
function fncKihonElement(strID, strPW)
{
	localStorage.setItem("BridalID", strID);
	localStorage.setItem("BridalPW", strPW);

	var url = "01kihon.html";
	window.location.href = url;
}
