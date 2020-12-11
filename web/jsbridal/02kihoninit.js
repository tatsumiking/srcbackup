
// 

function fncInitNengouComboBox()
{
	var data = "file=,,/list/nengou.txt";
	var fnc = fncNengouCallback;
	sendRequest("POST","php/readfile.php",data,false,fnc);
}
function fncNengouCallback(xmlhttp)
{
	var idx, setidx;
	var data = xmlhttp.responseText;
	var aryLine = data.split("\r\n");
	var max = aryLine.length;

	var cmbKyosikiNengo = document.getElementById("cmbKyosikiNengo");
	var cmbHirouenNengo = document.getElementById("cmbHirouenNengo");
	for(idx = 1, setidx = 0; idx < max; idx++){
		ary = aryLine[idx].split(",");
		if(2 <= ary.length){
			opt = new Option(ary[0], ary[1]);
			cmbKyosikiNengo.options[setidx] = opt;
			opt = new Option(ary[0], ary[1]);
			cmbHirouenNengo.options[setidx] = opt;
			setidx++;
		}
	}
	if(setidx == 0){
		cmbKyosikiNengo.options[0] = new Option("西暦", "0");
		cmbKyosikiNengo.options[1] = new Option("令和", "2019");

		cmbHirouenNengo.options[0] = new Option("西暦", "0");
		cmbHirouenNengo.options[1] = new Option("令和", "2019");
	}
}
function fncNengouToFullYear(nGG, sNengou)
{
	var idx, max;
	var cmbKyosikiNengo = document.getElementById("cmbKyosikiNengo");
	var nYear;
	var sYear;

	max = cmbKyosikiNengo.length;
	// 	西暦をスキップ
	for(idx = 1; idx < max; idx++){
		if(cmbKyosikiNengo.options[idx].text == sNengou){
			nYear = nGG + fnclibStringToInt(cmbKyosikiNengo.options[idx].value) - 1;
			sYear = nYear.toString();
			return(sYear);
		}
	}
	sYear = nGG.toString(); // 西暦
	return(sYear);
}
function fncFullYearToNengou(nYear, sNengou)
{
	var idx, max;
	var base;
	var cmbKyosikiNengo = document.getElementById("cmbKyosikiNengo");
	var nGG;
	var sGG;

	max = cmbKyosikiNengo.length;
	// 	西暦をスキップ
	for(idx = max - 1; idx > 0; idx--){
		base = fnclibStringToInt(cmbKyosikiNengo.options[idx].value);
		if(sNengou == "")
		{
			if(base <= nYear)
			{
				// 	年号は1から始まる
				nGG = nYear -  base + 1;
				sGG = fnclibStrnumToStrnum00(nGG);
				sNengou = cmbKyosikiNengo.options[idx].text;
				return[sGG, idx];
			}
		}else{
			if(cmbKyosikiNengo.options[idx].text == sNengou){
				// 	年号は1から始まる
				nGG = nYear -  base + 1;
				sGG = fnclibStrnumToStrnum00(nGG);
				return[sGG, idx];
			}
		}
	}
	sGG = nYear.toString(); // 西暦
	return[sGG, 0];
}
function fncInitSinroZokugaraComboBox()
{
	var data = "file=../list/sinrozokugara.txt";
	var fnc = fncSinroZokugaraCallback;
	sendRequest("POST","php/readfile.php",data,false,fnc);
}
function fncSinroZokugaraCallback(xmlhttp)
{
	var idx, setidx;
	var data = xmlhttp.responseText;
	var aryLine = data.split("\r\n");
	var max = aryLine.length;
	var cmbSinroZokugara = document.getElementById("cmbSinroZokugara");
	for(idx = 1, setidx = 0; idx < max; idx++){
		ary = aryLine[idx].split(",");
		if(2 <= ary.length){
			opt = new Option(ary[0], ary[1]);
			cmbSinroZokugara.options[setidx] = opt;
			setidx++;
		}
	}
	if(setidx == 0){
		cmbSinroZokugara.options[0] = new Option("長男", "1");
		cmbSinroZokugara.options[1] = new Option("次男", "2");
		cmbSinroZokugara.options[2] = new Option("三男", "3");
		cmbSinroZokugara.options[3] = new Option("四男", "4");
		cmbSinroZokugara.options[4] = new Option("五男", "5");
	}
}
function fncInitSinpuZokugaraComboBox()
{
	var data = "file=../list/sinpuzokugara.txt";
	var fnc = fncSinpuZokugaraCallback;
	sendRequest("POST","php/readfile.php",data,false,fnc);
}
function fncSinpuZokugaraCallback(xmlhttp)
{
	var idx, setidx;
	var data = xmlhttp.responseText;
	var aryLine = data.split("\r\n");
	var max = aryLine.length;
	var cmbSinpuZokugara = document.getElementById("cmbSinpuZokugara");
	for(idx = 1, setidx = 0; idx < max; idx++){
		ary = aryLine[idx].split(",");
		if(2 <= ary.length){
			opt = new Option(ary[0], ary[1]);
			cmbSinpuZokugara.options[setidx] = opt;
			setidx++;
		}
	}
	if(setidx == 0){
		cmbSinpuZokugara.options[0] = new Option("長女", "1");
		cmbSinpuZokugara.options[1] = new Option("次女", "2");
		cmbSinpuZokugara.options[2] = new Option("三女", "3");
		cmbSinpuZokugara.options[3] = new Option("四女", "4");
		cmbSinpuZokugara.options[4] = new Option("五女", "5");
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
	var dbnm = m_szHotelDB;
	var tble = "kaijyou";
	var fild = "id,name";
	var sort = "";
	var data = "dbnm="+dbnm+"&tble="+tble+"&fild="+fild+"&sort"+sort;
	var fnc = fncKaijyouCallback;
	sendRequest("POST","php/getlist.php",data,false,fnc);
}
function fncKaijyouCallback(xmlhttp)
{
	var idx;
	var data = xmlhttp.responseText;
	var ary = data.split(",");
	if(ary[0] == "0"){
		fnclibAlert(szTable+"テーブルリストを取得することが出来ませんでした");
		return;
	}
	var cmbKaijyou = document.getElementById("cmbKaijyou");
	var aryRec = data.split(";");
	var max = aryRec.length;
	for(idx = 0;idx < max; idx++){
		ary = aryRec[idx].split(",");
		cmbKaijyou.options[idx] = new Option(ary[1]);
	}
}
function fncInitKonreiListBox()
{
	var dbnm = m_szHotelDB;
	var tble = m_szKonreiTable;
	var fild = "id,username,sinroname1,sinpuname1,kyosiki";
	var data = "dbnm="+dbnm+"&tble="+tble+"&fild="+fild+"&sort="+m_strSortSql;
	var fnc = fncKonreiListCallback;
	sendRequest("POST","php/getlist.php",data,false,fnc);
}
function fncKonreiListCallback(xmlhttp)
{
	var data = xmlhttp.responseText;
	var ary = data.split(",");
	if(ary[0] == "0"){
		fnclibAlert("婚礼リストを取得することが出来ませんでした");
		return;
	}
	var lstKonrei = document.getElementById("lstKonrei");
	lstKonrei.options.length = 0;
	var cDate = new clsDate();
	var sData = "";
	var aryRec = data.split(";");
	var max = aryRec.length;
	for(idx = 0;idx < max; idx++){
		ary = aryRec[idx].split(",");
		if(ary.length >= 5){
			cDate.fncSetSimpleData(ary[4].toString());
			sData = ary[1] + " " + ary[2] + "家 " + ary[3] + "家 ";
			sData = sData + cDate.fncGetFormatData("西暦");
			lstKonrei.options[idx] = new Option(sData, ary[0]);
		}
	}
}
function fncGetKonreiData()
{
	var dbnm = m_szHotelDB;
	var tble = m_szKonreiTable;
	var fild = "id,username";
	fild = fild + ",kyosiki,hirouen,kaijyou,mukotori";
	fild = fild + ",sinrozoku,sinroname1,sinroname2";
	fild = fild + ",sinpuzoku,sinpuname1,sinpuname2";
	fild = fild + ",sinrodish,sinpudish,sinrosub,sinpusub";
	var where = "WHERE (id="+m_nKonreiId+") LIMIT 1";
	var data = "dbnm="+dbnm+"&tble="+tble+"&fild="+fild+"&where="+where;
	var fnc = fncGetKonreiDataCallback;
	sendRequest("POST","php/getdata.php",data,false,fnc);
}
function fncGetKonreiDataCallback(xmlhttp)
{
	var data = xmlhttp.responseText;
	var ary = data.split(",");
	if(ary[0] == "0"){
		txtKonreiId = document.getElementById("txtKonreiId");
		txtKonreiId.value = m_nKonreiId;
		txtKanriNo = document.getElementById("txtKanriNo");
		txtKanriNo.value = m_sKonreiNo;
		fncSetDefKonreiData();
		fncSaveCsvData();
		return;
	}
	if(ary[7] == "" && ary[11] == "" ){
		m_nKonreiId = ary[0];
		m_sKonreiNo = ary[1];
		txtKonreiId = document.getElementById("txtKonreiId");
		txtKonreiId.value = m_nKonreiId;
		txtKanriNo = document.getElementById("txtKanriNo");
		txtKanriNo.value = m_sKonreiNo;
		fncSetDefKonreiData();
		fncSaveCsvData();
		return;
	}
	txtKonreiId = document.getElementById("txtKonreiId");
	txtKonreiId.value = ary[0];
	txtKanriNo = document.getElementById("txtKanriNo");
	txtKanriNo.value = ary[1];
	
	cmbKyosikiNengo = document.getElementById("cmbKyosikiNengo");
	txtKyosikiGG = document.getElementById("txtKyosikiGG");
	var sYY = ary[2].substr(0,4);
	if(m_nDispNengo = 0)
	{
		cmbKyosikiNengo.selectedIndex = 0;
		txtKyosikiGG.value = sYY;
	}else{
		var aryNengou = fncFullYearToNengou(fnclibStringToInt(sYY), "");
		cmbKyosikiNengo.selectedIndex = fnclibStringToInt(aryNengou[1]);
		txtKyosikiGG.value = aryNengou[0];
	}
	txtKyosikiMM = document.getElementById("txtKyosikiMM");
	txtKyosikiMM.value = ary[2].substr(5,2);
	txtKyosikiDD = document.getElementById("txtKyosikiDD");
	txtKyosikiDD.value = ary[2].substr(8,2);
	txtKyosikiHH = document.getElementById("txtKyosikiHH");
	txtKyosikiHH.value = ary[2].substr(11,2);
	txtKyosikiMN = document.getElementById("txtKyosikiMN");
	txtKyosikiMN.value = ary[2].substr(14,2);
	
	cmbHirouenNengo = document.getElementById("cmbHirouenNengo");
	txtHirouenGG = document.getElementById("txtHirouenGG");
	var sYY = ary[3].substr(0,4);
	if(m_nDispNengo = 0)
	{
		cmbHirouenNengo.selectedIndex = 0;
		txtHirouenGG.value = sYY;
	}else{
		var aryNengou = fncFullYearToNengou(fnclibStringToInt(sYY), "");
		cmbHirouenNengo.selectedIndex = fnclibStringToInt(aryNengou[1]);
		txtHirouenGG.value = aryNengou[0];
	}
	txtHirouenMM = document.getElementById("txtHirouenMM");
	txtHirouenMM.value = ary[3].substr(5,2);
	txtHirouenDD = document.getElementById("txtHirouenDD");
	txtHirouenDD.value = ary[3].substr(8,2);
	txtHirouenHH = document.getElementById("txtHirouenHH");
	txtHirouenHH.value = ary[3].substr(11,2);
	txtHirouenMN = document.getElementById("txtHirouenMN");
	txtHirouenMN.value = ary[3].substr(14,2);

	cmbKaijyou = document.getElementById("cmbKaijyou");
	cmbKaijyou.value = ary[4];

	chkMukotori = document.getElementById("chkMukotori");
	if(ary[5] == "0"){
		chkMukotori.checked = false;
	}else{
		chkMukotori.checked = true;
	}

	cmbSinroZokugara = document.getElementById("cmbSinroZokugara");
	cmbSinroZokugara.value = ary[6];
	txtSinroMyouji = document.getElementById("txtSinroMyouji");
	txtSinroMyouji.value = ary[7];
	txtSinroNamae = document.getElementById("txtSinroNamae");
	txtSinroNamae.value = ary[8];
	
	cmbSinpuZokugara = document.getElementById("cmbSinpuZokugara");
	cmbSinpuZokugara.value = ary[9];
	txtSinpuMyouji = document.getElementById("txtSinpuMyouji");
	txtSinpuMyouji.value = ary[10];
	txtSinpuNamae = document.getElementById("txtSinpuNamae");
	txtSinpuNamae.value = ary[11];
	
	txtSinroRyouri = document.getElementById("txtSinroRyouri");
	txtSinroRyouri.value = ary[12];
	txtSinpuRyouri = document.getElementById("txtSinpuRyouri");
	txtSinpuRyouri.value = ary[13];
	txtSinroBikou = document.getElementById("txtSinroBikou");
	txtSinroBikou.value = ary[14];
	txtSinpuBikou = document.getElementById("txtSinpuBikou");
	txtSinpuBikou.value = ary[15];

	fncSaveCsvData();
}
function fncSetDefKonreiData()
{
	var now = new Date();
	var sYY = fncZeroPadding(now.getFullYear(), 4); // 年
	var sMM = fncZeroPadding(now.getMonth() + 1, 2); // 月
	var sDD = fncZeroPadding(now.getDate(), 2); // 日
	var sHH = "17";

	cmbKyosikiNengo = document.getElementById("cmbKyosikiNengo");
	txtKyosikiGG = document.getElementById("txtKyosikiGG");
	if(m_nDispNengo = 0)
	{
		cmbKyosikiNengo.selectedIndex = 0;
		txtKyosikiGG.value = sYY;
	}else{
		var aryNengou = fncFullYearToNengou(fnclibStringToInt(sYY), "");
		cmbKyosikiNengo.selectedIndex = fnclibStringToInt(aryNengou[1]);
		txtKyosikiGG.value = aryNengou[0];
	}
	txtKyosikiMM = document.getElementById("txtKyosikiMM");
	txtKyosikiMM.value = sMM;
	txtKyosikiDD = document.getElementById("txtKyosikiDD");
	txtKyosikiDD.value = sDD;
	txtKyosikiHH = document.getElementById("txtKyosikiHH");
	txtKyosikiHH.value = sHH;
	txtKyosikiMN = document.getElementById("txtKyosikiMN");
	txtKyosikiMN.value = "00";
	
	cmbHirouenNengo = document.getElementById("cmbHirouenNengo");
	txtHirouenGG = document.getElementById("txtHirouenGG");
	if(m_nDispNengo = 0)
	{
		cmbHirouenNengo.selectedIndex = 0;
		txtHirouenGG.value = sYY;
	}else{
		var aryNengou = fncFullYearToNengou(fnclibStringToInt(sYY), "");
		cmbHirouenNengo.selectedIndex = fnclibStringToInt(aryNengou[1]);
		txtHirouenGG.value = aryNengou[0];
	}
	txtHirouenMM = document.getElementById("txtHirouenMM");
	txtHirouenMM.value = sMM;
	txtHirouenDD = document.getElementById("txtHirouenDD");
	txtHirouenDD.value = sDD;
	txtHirouenHH = document.getElementById("txtHirouenHH");
	txtHirouenHH.value = sHH;
	txtHirouenMN = document.getElementById("txtHirouenMN");
	txtHirouenMN.value = "30";

	cmbKaijyou = document.getElementById("cmbKaijyou");
	cmbKaijyou.value = "";

	chkMukotori = document.getElementById("chkMukotori");
	chkMukotori.checked = false;

	cmbSinroZokugara = document.getElementById("cmbSinroZokugara");
	cmbSinroZokugara.value = "";
	txtSinroMyouji = document.getElementById("txtSinroMyouji");
	txtSinroMyouji.value = "";
	txtSinroNamae = document.getElementById("txtSinroNamae");
	txtSinroNamae.value = "";
	
	cmbSinpuZokugara = document.getElementById("cmbSinpuZokugara");
	cmbSinpuZokugara.value = "";
	txtSinpuMyouji = document.getElementById("txtSinpuMyouji");
	txtSinpuMyouji.value = "";
	txtSinpuNamae = document.getElementById("txtSinpuNamae");
	txtSinpuNamae.value = "";
	
	txtSinroRyouri = document.getElementById("txtSinroRyouri");
	txtSinroRyouri.value = "";
	txtSinpuRyouri = document.getElementById("txtSinpuRyouri");
	txtSinpuRyouri.value = "";
	txtSinroBikou = document.getElementById("txtSinroBikou");
	txtSinroBikou.value = "";
	txtSinpuBikou = document.getElementById("txtSinpuBikou");
	txtSinpuBikou.value = "";
}
