
// CallBackに渡す引数を格納した構造体（CallBack構造体）
function StWebFont(no, str, argv1, argv2, fncCallBack)
{
	this.setno = no;	// CallBack構造体識別番号
	this.loadcnt = 0;	// WebFont文字呼び出し数
	this.lestcnt = str.length; // 未処理文字数
	this.argv1 = argv1; // ctx
	this.argv2 = argv2; // objFigu
	this.fncCallBack = fncCallBack;
}

var m_aryWebFontStruct; // CallBack構造体のArray構造
var m_nWebFontStLastNo; // CallBack構造体識別番号
var m_sWebFontFolder; // フォントフォルダー
var m_sWebFontName; // フォント名

//　初期化処理ページ処理の最初に呼ばれる
function fncWebFontInit(sFolder)
{
	m_aryWebFontStruct = new Array();
	m_nWebFontStLastNo = 0;
	m_sWebFontFolder = sFolder;
	m_sWebFontName = "eskaku"
}
function fncWebFontSetName(sName)
{
	m_sWebFontName = sName;
}
// 文字列に含まれる文字でキャッシングされてない文字をWebFontからロード
// コールバック関数に渡す引数２個をキー付の構造体に格納
// WebFontファイル読み込みようのPHPにキーを渡しPHPからのコールバックで受け取り
// この関数に渡されたコールバック関数と引数を復元し関数呼び出し
function fncWebFontCheckLoad(str, argv1, argv2, fncCallBack)
{
	var max, i;
	var sstr;
	var nCode;
	var sCode;
	var key;
	var sGryph;
	var stno;
	var cnt;

	stno = m_nWebFontStLastNo;
	stWF = new StWebFont(stno, str, argv1, argv2, fncCallBack);
	m_aryWebFontStruct.push(stWF);
	m_nWebFontStLastNo++;

	max = str.length;
	for(i = 0; i < max; i++){
		sstr = str.substr(i, 1);
		if(sstr != '\n' && sstr != '|'){
			nCode = sstr.charCodeAt(0);
			if(nCode < 0x100){
				sCode = "00"+nCode.toString(16).toUpperCase();
			}else{
				sCode = nCode.toString(16).toUpperCase();
			}
			key = m_sWebFontName+"_"+sCode;
			sGryph = localStorage.getItem(key);
			if(sGryph == null || sGryph == ""){
				cnt = stWF.loadcnt;
				cnt++;
				stWF.loadcnt = cnt;
				fncWebFontLoadCache(sCode, stno);
			}
		}
		cnt = stWF.lestcnt;
		cnt--;
		stWF.lestcnt = cnt;

	}
	if(stWF.loadcnt <= 0){
		stWF.fncCallBack(stWF.argv1, stWF.argv2);
	}
}

// 文字データをキャッシングするためにWebFont呼び出し
function fncWebFontLoadCache(sCode, stno)
{
	var hc = sCode.substr(0,2);
	var data = "com="+m_sWebFontFolder+"/"+m_sWebFontName+"/"+hc+"/"+sCode+".txt,"+sCode+","+stno+",,,";
	sendRequest("POST","readwebfont.php",data,false,fncWebFontCallbackLoadCache);
}
// 文字データが無いときの処理をPHPも含めいて検討する必要あり
// 文字データ呼び出しPHPからのコールバック
function fncWebFontCallbackLoadCache(xmlhttp)
{
	var cnt;
	var sGryph = xmlhttp.responseText;
	var aryLine = sGryph.split("\n");
	var aryClm = aryLine[0].split(",");
	var key = m_sWebFontName+"_"+aryClm[1];
	if(aryClm[0] == "0"){
		localStorage.setItem(key, "");
	}else{
		localStorage.setItem(key, sGryph);
	}
	var stno = parseInt(aryClm[2]);
	var idx = fncWebFontSrchStructIdx(stno);
	var stWF = m_aryWebFontStruct[idx];
	// m_aryWebFontStruct.RemoveAt(idx);
	cnt = stWF.loadcnt;
	cnt--;
	stWF.loadcnt = cnt;
	if(stWF.loadcnt <= 0 && stWF.lestcnt <= 0){
		stWF.fncCallBack(stWF.argv1, stWF.argv2);
	}
}
// キーから最初の処理に渡された引数とコールバック関数を格納した構造体のインデックスを取得
function fncWebFontSrchStructIdx(stno)
{
	var i, max;
	var stWF;

	max = m_aryWebFontStruct.length;
	for(i = 0; i < max; i++){
		stWF = m_aryWebFontStruct[i];
		if(stWF.setno == stno){
			return(i);
		}
	}
	return(-1);
}

// 総文字サイズを取得
function fncWebFontYokoStrSize(str, fontsize, space)
{
	var size;
	var i, max;
	var sstr;

	size = 0;
	max = str.length;
	for(i = 0; i < max; i++){
		sstr = str.substr(i, 1);
		size = size + fncWebFontYokoMojiSize(sstr, fontsize);
		if(i != 0){
			size = size + space;
		}
	}
	return(size);
}

// 総文字サイズを取得
function fncWebFontTateStrSize(ary, fontsize, space)
{
	var size;
	var i, max;
	var sstr;

	size = 0;
	max = ary.length;
	for(i = 0; i < max; i++){
		sstr = ary[i];
		size = size + fncWebFontTateMojiSize(sstr, fontsize);
		if(i != 0){
			size = size + space;
		}
	}
	return(size);
}

function fncWebFontYokoMojiSize(str, fontsize)
{
	/*
	var time;
	var size;
	var stMiniMax;

	time = fontsize / 1024.0;
	stMiniMax = fncWebFontMojiArea(str);
	size = (stMiniMax.ex) * time;
	return(size);
	*/
	var c;

	c = str.charCodeAt(0);
	if(0x20 == c){ // 半角
		return(fontsize/2);
	}else{
		return(fontsize);
	}
}

function fncWebFontTateMojiSize(str, fontsize)
{
	/*
	var time;
	var size;
	var stMiniMax;

	time = fontsize / 1024.0;
	stMiniMax = fncWebFontMojiArea(str);
	size = (stMiniMax.ey) * time;
	return(size);
	*/
	var c;

	c = str.charCodeAt(0);
	if(0x20 == c){ // 半角
		return(fontsize/2);
	}else{
		return(fontsize);
	}
}

// 文字矩形座標取得
function fncWebFontMojiArea(sstr)
{
	var stMiniMax;
	var nCode;
	var sCode;
	var key;
	var sGryph;
	var aryLine;
	var aryClm;

	stMiniMax = new StMiniMax();
	nCode = sstr.charCodeAt(0);
	if(nCode < 0x100){
		sCode = "00"+nCode.toString(16).toUpperCase();
	}else{
		sCode = nCode.toString(16).toUpperCase();
	}
	if(sCode == "0020"){
		stMiniMax.sx = 0.0;
		stMiniMax.sy = -512.0;
		stMiniMax.ex = 512.0;
		stMiniMax.ey = 0.0;
		return(stMiniMax);
	}else if(sCode == "3000"){
		stMiniMax.sx = 0.0;
		stMiniMax.sy = -1024.0;
		stMiniMax.ex = 1024.0;
		stMiniMax.ey = 0.0;
		return(stMiniMax);
	}
	key = m_sWebFontName+"_"+sCode;
	sGryph = localStorage.getItem(key);
	if(sGryph == null || sGryph == ""){
		stMiniMax.sx = 0.0;
		stMiniMax.sy = -1024.0;
		stMiniMax.ex = 1024.0;
		stMiniMax.ey = 0.0;
		return(stMiniMax);
	}
	aryLine = sGryph.split("\n");
	aryClm = aryLine[0].split(",");
	stMiniMax.sx = parseFloat(aryClm[4]);
	stMiniMax.sy = parseFloat(aryClm[5]);
	stMiniMax.ex = parseFloat(aryClm[6]);
	stMiniMax.ey = parseFloat(aryClm[7]);
	return(stMiniMax);
}

