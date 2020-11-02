var m_szHotelName;
var m_szHotelDB;

function fncInit()
{
	fncInitHotelComboBox();
	fncInitNengouComboBox();

	fncInitSinrouZokugaraComboBox();
	fncInitSinpuZokugaraComboBox();
	var btnReturn = document.getElementById("btnReturn");
	btnReturn.onclick = fncOnClickReturn;
}
function fncOnClickNew()
{
	var fs = "14px";
	fncElementsSetFontSize(fs);
}
function fncOnClickUpdate()
{
	var fs = "16px";
	fncElementsSetFontSize(fs);
}
function fncOnClickBefore()
{
	var fs = "18px";
	fncElementsSetFontSize(fs);
}
function fncOnClickNext()
{
	var fs = "20px";
	fncElementsSetFontSize(fs);
}
function fncOnClickDelete()
{
	var sql = "SELECT * FROM `kaijyou`";
	var fld = "aa,bb,cc";
	var data = "com="+m_szHotelDB+"&sql="+sql+"&fld="+fld;
	var fnc = fncDeleteCallback;
	sendRequest("POST","php/test.php",data,false,fnc);
}
function fncDeleteCallback(xmlhttp)
{
	var data = xmlhttp.responseText;
}
function fncOnClickReturn()
{
	var url = "01menu.html";
	window.location.href = url;
}
// 
function fncInitHotelComboBox()
{
	var data = "com=../list/hotel.txt";
	var fnc = fncHotelCallback;
	sendRequest("POST","php/readfile.php",data,false,fnc);
}
function fncHotelCallback(xmlhttp)
{
	var idx, setidx;
	var data = xmlhttp.responseText;
	var aryLine = data.split("\r\n");
	var max = aryLine.length;
	var idHotelName = document.getElementById("idHotelName");
	for(idx = 1, setidx = 0; idx < max; idx++){
		ary = aryLine[idx].split(",");
		if(2 <= ary.length){
			opt = new Option(ary[0], ary[1]);
			idHotelName.options[setidx] = opt;
			setidx++;
		}
	}
	if(setidx == 0){
		idHotelName.options[0] = new Option("ホテル", "hotel");
	}
	idx = idHotelName.selectedIndex;
	m_szHotelName = idHotelName.options[idx].text;
	m_szHotelDB = idHotelName.options[idx].value;

	fncInitHotelElement();
}
function fncInitNengouComboBox()
{
	var data = "com=,,/list/nengou.txt";
	var fnc = fncNengouCallback;
	sendRequest("POST","php/readfile.php",data,false,fnc);
}
function fncNengouCallback(xmlhttp)
{
	var idx, setidx;
	var data = xmlhttp.responseText;
	var aryLine = data.split("\r\n");
	var max = aryLine.length;

	var idKyosikiNengo = document.getElementById("idKyosikiNengo");
	var idHirouenNengo = document.getElementById("idHirouenNengo");
	for(idx = 1, setidx = 0; idx < max; idx++){
		ary = aryLine[idx].split(",");
		if(2 <= ary.length){
			opt = new Option(ary[0], ary[1]);
			idKyosikiNengo.options[setidx] = opt;
			opt = new Option(ary[0], ary[1]);
			idHirouenNengo.options[setidx] = opt;
			setidx++;
		}
	}
	if(setidx == 0){
		idKyosikiNengo.options[0] = new Option("西暦", "0");
		idHirouenNengo.options[1] = new Option("西暦", "0");
		idKyosikiNengo.options[2] = new Option("令和", "2019");
		idHirouenNengo.options[3] = new Option("令和", "2019");
	}
}
function fncInitSinrouZokugaraComboBox()
{
	var data = "com=../list/sinrouzokugara.txt";
	var fnc = fncSinrouZokugaraCallback;
	sendRequest("POST","php/readfile.php",data,false,fnc);
}
function fncSinrouZokugaraCallback(xmlhttp)
{
	var idx, setidx;
	var data = xmlhttp.responseText;
	var aryLine = data.split("\r\n");
	var max = aryLine.length;
	var idSinrouZokugara = document.getElementById("idSinrouZokugara");
	for(idx = 1, setidx = 0; idx < max; idx++){
		ary = aryLine[idx].split(",");
		if(2 <= ary.length){
			opt = new Option(ary[0], ary[1]);
			idSinrouZokugara.options[setidx] = opt;
			setidx++;
		}
	}
	if(setidx == 0){
		idSinrouZokugara.options[0] = new Option("ホテル", "hotel");
	}
}
function fncInitSinpuZokugaraComboBox()
{
	var data = "com=../list/sinpuzokugara.txt";
	var fnc = fncSinpuZokugaraCallback;
	sendRequest("POST","php/readfile.php",data,false,fnc);
}
function fncSinpuZokugaraCallback(xmlhttp)
{
	var idx, setidx;
	var data = xmlhttp.responseText;
	var aryLine = data.split("\r\n");
	var max = aryLine.length;
	var idSinpuZokugara = document.getElementById("idSinpuZokugara");
	for(idx = 1, setidx = 0; idx < max; idx++){
		ary = aryLine[idx].split(",");
		if(2 <= ary.length){
			opt = new Option(ary[0], ary[1]);
			idSinpuZokugara.options[setidx] = opt;
			setidx++;
		}
	}
	if(setidx == 0){
		idSinpuZokugara.options[0] = new Option("ホテル", "hotel");
	}
}
function fncInitHotelElement()
{
	fncInitKaijyouComboBox();

	var idSortNoAsc = document.getElementById("idSortNoAsc");
	idSortNoAsc.checked = true;
	fncInitKonreiListBox();
}
function fncInitKaijyouComboBox()
{
	var com = m_szHotelDB;
	var sql = "SELECT * FROM `kaijyou`";
	var fld = "id,name";
	var data = "com="+com+"&sql="+sql+"&fld="+fld;
	var fnc = fncKaijyouCallback;
	//sendRequest("POST","php/getlistsql.php",data,false,fnc);
}
function fncKaijyouCallback()
{
}
function fncInitKonreiListBox()
{
}
function fncElementsSetFontSize(fs)
{
	var body = document.body;
	body.style.fontSize = fs;
	var nodes;
	nodes = document.getElementsByTagName("select");
	fncNodesSetFontSize(nodes, fs);
	nodes = document.getElementsByTagName("input");
	fncNodesSetFontSize(nodes, fs);
	nodes = document.getElementsByTagName("button");
	fncNodesSetFontSize(nodes, fs);
}
function fncNodesSetFontSize(nodes, fs)
{
	var idx;

	var max = nodes.length;
	for(idx = 0; idx < max; idx++){
		nodes[idx].style.fontSize = fs;
		fncChildNodesSetFontSize(nodes[idx].childnodes);
	}
}
function fncChildNodesSetFontSize(nodes, fs)
{
	var idx;

	var max = nodes.length;
	for(idx = 0; idx < max; idx++){
		nodes[idx].style.fontSize = fs;
		fncChildNodesSetFontSize(nodes[idx].childnodes);
	}
}
