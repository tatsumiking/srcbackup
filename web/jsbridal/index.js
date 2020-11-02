function fncInit()
{
	localStorage.setItem("BridalID", "");
	localStorage.setItem("BridalPW", "");

	var btnInitPage = document.getElementById("btnInitPage");
	btnInitPage.onclick = fncOnClickInitPage;
}
function fncOnClickInitPage()
{
	var url = "00login.html";
	//var url = "01menu.html";
	window.location.href = url;
}
