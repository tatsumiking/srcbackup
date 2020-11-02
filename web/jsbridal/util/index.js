
var m_szHotelName;
var m_szHotelDB;

function fncInit()
{
	fncInitHotelCombobox();
	var btnHotelNew = document.getElementById("btnHotelNew");
	btnHotelNew.onclick = fncOnClickInitHotel;
}
function fncInitHotelCombobox()
{
	var data = "com=../list/hotel.txt";
	var fnc = fncHotelCallback;
	sendRequest("POST","../php/readfile.php",data,false,fnc);
}
function fncHotelCallback(xmlhttp)
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
		cmbHotelName.options[0] = new Option("ƒzƒeƒ‹", "hotel");
	}
	idx = cmbHotelName.selectedIndex;
	m_szHotelName = cmbHotelName.options[idx].text;
	m_szHotelDB = cmbHotelName.options[idx].value;
}
function fncOnClickInitHotel()
{
	var cmbHotelName = document.getElementById("cmbHotelName");
	var idx = cmbHotelName.selectedIndex;
	m_szHotelName = cmbHotelName.options[idx].text;
	m_szHotelDB = cmbHotelName.options[idx].value;
	
	var data = "dbnm="+m_szHotelDB;
	var fnc = fncCreateDBCallback;
	sendRequest("POST","initdb.php",data,false,fnc);
}
