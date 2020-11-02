
function fncInit()
{
	var btnReturn = document.getElementById("btnReturn");
	btnReturn.onclick = fncOnClickReturn;
}
function fncOnClickReturn()
{
	var url = "01menu.html";
	window.location.href = url;
}
