
var m_szHotelName;
var m_szHotelDB;

function fncInit()
{
	fncInitHotelCombobox();
	fncInitTableCombobox();

	m_szHotelName = localStorage.getItem("HotelName");
	m_szHotelDB = localStorage.getItem("HotelDB");

	var btnHotelNew = document.getElementById("btnHotelNew");
	btnHotelNew.onclick = fncOnClickInitHotel;
	var btnTableList = document.getElementById("btnTableList");
	btnTableList.onclick = fncOnClickTableList;
	var btnReturn = document.getElementById("btnReturn");
	btnReturn.onclick = fncOnClickReturn;
}
function fncOnClickInitHotel()
{
	var cmbHotelName = document.getElementById("cmbHotelName");
	var idx = cmbHotelName.selectedIndex;
	m_szHotelName = cmbHotelName.options[idx].text;
	m_szHotelDB = cmbHotelName.options[idx].value;
	fncCreateDB(m_szHotelDB);
	
}
function fncOnClickTableList()
{
	var cmbTableName = document.getElementById("cmbTableName");
	var idx = cmbTableName.selectedIndex;
	szTableName = cmbTableName.options[idx].text;
	szTable = cmbTableName.options[idx].value;
	fncListTable(szTable);
}
function fncOnClickReturn()
{
	var url = "../01menu.html";
	window.location.href = url;
}
function fncInitHotelCombobox()
{
	var data = "com=../list/hotel.txt";
	var fnc = fncInitHotelCallback; // hotel.js�Œ�'
	sendRequest("POST","../php/readfile.php",data,false,fnc);
}
function fncInitHotelCallback(xmlhttp)
{
	var idx, setidx;
	var data = xmlhttp.responseText;
	var aryLine = data.split("\r\n");
	var max = aryLine.length;

	var cmbHotelName = document.getElementById("cmbHotelName");
	for(idx = 1, setidx = 0; idx < max; idx++){
		ary = aryLine[idx].split(",");
		if(2 <= ary.length){
			opt = new Option(ary[0], ary[1]);
			cmbHotelName.options[setidx] = opt;
			setidx++;
		}
	}
	if(setidx == 0){
		cmbHotelName.options[0] = new Option("ホテルソア", "bridal");
	}
	max = cmbHotelName.options.length;
	for(idx = 0; idx < max; idx++){
		if(cmbHotelName.options[idx].text == m_szHotelName){
			cmbHotelName.selectedIndex = idx;
			break;	
		}
	}

}
function fncInitTableCombobox()
{
	var cmbTableName = document.getElementById("cmbTableName");

	cmbTableName.options[0] = new Option("", "");
	cmbTableName.options[1] = new Option("会場テーブル", "kaijyou");
	cmbTableName.options[2] = new Option("区分テーブル", "kubun");
	cmbTableName.options[3] = new Option("連名区分テーブル", "renmei");
	cmbTableName.options[4] = new Option("婚礼テーブル", "bridaluser");
}
function fncListTable(szTable)
{
	var fild;

	if(szTable == "kaijyou")
	{
		fild = "id,name";
	}
	else if(szTable == "kubun")
	{
		fild = "id,no,name";
	}
	else if(szTable == "renmei")
	{
		fild = "id,name";
	}
	else if(szTable == "bridaluser")
	{
		fild = "id,username,kyosiki,kaijyou,sinroname1,sinpuname1";
	}
	data = "dbnm="+m_szHotelDB;
	data = data+"&tble="+szTable+"&fild="+fild;
	var fnc = fncListTableCallback;
	sendRequest("POST","getlist.php",data,false,fnc);
}
function fncListTableCallback(xmlhttp)
{
	var idx;
	var retstr = xmlhttp.responseText;
	var ary = retstr.split(",");
	if(ary[0] == "0"){
		alert(szTable+"テーブルリストを取得することが出来ませんでした");
		return;
	}
	var lstTableData = document.getElementById("lstTableData");
	lstTableData.options.length = 0;
	ary = retstr.split(";");
	var max = ary.length;
	for(idx = 0;idx < max; idx++){
		lstTableData.options[idx] = new Option(ary[idx]);
	}
}


