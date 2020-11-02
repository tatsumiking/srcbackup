//

function fncCreateDBCallback(xmlhttp)
{
	var retstr = xmlhttp.responseText;
	var ary = retstr.split(",");
	if(ary[0] == "0"){
		alert(m_szHotelDB+"データベース作成に失敗しました");
		return;
	}

	var data = "dbnm="+m_szHotelDB;
	data = data + "&tble=kubun";
	data = data + "&fild=no,name";
	data = data + "&type=20,48";
	var fnc = fncCreateKubunTblCallback;
	sendRequest("POST","inittbl.php",data,false,fnc);
}
function fncCreateKubunTblCallback(xmlhttp)
{
	var retstr = xmlhttp.responseText;
	var ary = retstr.split(",");
	if(ary[0] == "0"){
		alert("区分テーブル作成に失敗しました");
		return;
	}

	var data = "dbnm="+m_szHotelDB;
	data = data + "&tble=renmei";
	data = data + "&fild=no,name";
	data = data + "&type=20,48";
	var fnc = fncCreateRenmeiTblCallback;
	sendRequest("POST","inittbl.php",data,false,fnc);
}

function fncCreateRenmeiTblCallback(xmlhttp)
{
	var retstr = xmlhttp.responseText;
	var ary = retstr.split(",");
	if(ary[0] == "0"){
		alert("連名区分テーブル作成に失敗しました");
		return;
	}

	var data = "dbnm="+m_szHotelDB;
	data = data + "&tble=kaijyou";
	data = data + "&fild=name";
	data = data + "&type=96";
	var fnc = fncCreateKaijyouTblCallback;
	sendRequest("POST","inittbl.php",data,false,fnc);
}
function fncCreateKaijyouTblCallback(xmlhttp)
{
	var retstr = xmlhttp.responseText;
	var ary = retstr.split(",");
	if(ary[0] == "0"){
		alert("会場テーブル作成に失敗しました");
		return;
	}
	fild = "userno,tbllayout";
	type = "0,24";
	fild = fild + ",kihonlockflag,gestlockflag,tablelockflag,sitlockflag";
	type = type + ",0,0,0,0";
	fild = fild + ",kyosiki,hirouen,kaijyou,mukotori";
	type = type + ",32,32,96,0";
	fild = fild + ",sinrozoku,sinroname1,sinroname2";
	type = type + ",24,24,24";
	fild = fild + ",sinpuzoku,sinpuname1,sinpuname2";
	type = type + ",24,24,24";
	fild = fild + ",sinrodish,sinpudish,sinrosub,sinpusub";
	type = type + ",24,24,96,96";
	fild = fild + ",paperlocate,papersize,ryoukekind,tablekind";
	type = type + ",24,24,24,24";
	fild = fild + ",textsize,takasagokind,nametype";
	type = type + ",24,24,24";
	fild = fild + ",lefttext,righttext";
	type = type + ",96,96";
	fild = fild + ",tablelayout";
	type = type + ",255";
	var data = "dbnm="+m_szHotelDB;
	data = data + "&tble=konnrei&fild="+fild+"&type="+type;
	var fnc = fncCreateKihonTblCallback;
	sendRequest("POST","inittbl.php",data,false,fnc);
}
function fncCreateKihonTblCallback(xmlhttp)
{
	var retstr = xmlhttp.responseText;
	var ary = retstr.split(",");
	if(ary[0] == "0"){
		alert("婚礼基本テーブル作成に失敗しました");
		return;
	}

	var data = "dbnm="+m_szHotelDB;
	data = data + "&tble=kubun";
	data = data + "&fild=no,name";
	data = data + "&file=list/kubun.txt";
	var fnc = fncSetDataKubunTblCallback;
	sendRequest("POST","setdata.php",data,false,fnc);
}
function fncSetDataKubunTblCallback(xmlhttp)
{
	var retstr = xmlhttp.responseText;
	var ary = retstr.split(",");
	if(ary[0] == "0"){
		alert("区分テーブルへのデータ追加に失敗しました");
		return;
	}

	var data = "dbnm="+m_szHotelDB;
	data = data + "&tble=renmei";
	data = data + "&fild=no,name";
	data = data + "&file=list/renmei.txt";
	var fnc = fncSetDataRenmeiTblCallback;
	sendRequest("POST","setdata.php",data,false,fnc);
}
function fncSetDataRenmeiTblCallback(xmlhttp)
{
	var retstr = xmlhttp.responseText;
	var ary = retstr.split(",");
	if(ary[0] == "0"){
		alert("連名区分テーブルへのデータ追加に失敗しました");
		return;
	}

	var data = "dbnm="+m_szHotelDB;
	data = data + "&tble=kaijyou";
	data = data + "&fild=no,name";
	data = data + "&file=list/renmei.txt";
	var fnc = fncSetDataKaijyouTblCallback;
	sendRequest("POST","setdata.php",data,false,fnc);
}
function fncSetDataKaijyouTblCallback(xmlhttp)
{
	var retstr = xmlhttp.responseText;
	var ary = retstr.split(",");
	if(ary[0] == "0"){
		alert("会場テーブル作成に失敗しました");
		return;
	}
	alert("ホテルDB初期化しました");
}

