

function fnclibAlert(msg)
{
	alert(msg);
}
function fnclibMessageWindow(title, msg)
{
	if(confirm(msg)){
		return("OK");
	}else{
		return("Cancel");
	}
}
function fnclibStrnumToStrnum00(str)
{
	var num;

	num = parseInt(str);
	if(num <= 9){
		str = "0"+num;
	}
	return(str);
}
function fnclibNumToStrnum00(num)
{
	if(num <= 9){
		str = "0"+num;
	}else{
		str = num.ToString();
	}
	return(str);
}

function fnclibPriceToTenprice(str)
{
	while(str != (str = str.replace(/^(-?\d+)(\d{3})/, "$1,$2")));
	return(str);
}
function fnclinHanToZen(hanstr)
{
	var ch, retStr;
	var idx, max;
	var cidx, cmax;
	var cc, ret;

	retStr = "";
	han = " !\"#$%&'()*+,-./0123456789;:<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
	zen = "　!＂＃＄％＆＇()＊＋,－.／０１２３４５６７８９：；＜＝＞？＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［＼］＾＿｀ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝～";
	cmax = han.length;;
	max = hanstr.length;
	for(idx = 0; idx < max; idx++){
		ch = hanstr.substr(idx, 1);
		ret = "";
		for(cidx = 0; cidx < cmax; cidx++){
			cc = han.substr(cidx, 1);
			if(cc == ch){
				ret = zen.substr(cidx, 1);
				break;
			}
		}
		if(ret == ""){
			retStr = retStr + ch;
		}else{
			retStr = retStr + ret;
		}
	}
	return(retStr);
}
function fnclibCheckTelNo(tel)
{
	var ary;
	var max, idx, len;

	ary = tel.split('-');
	max = ary.length;
	tel = "";
	for(idx = 0; idx < max; idx++){
		tel = tel + ary[idx];
	}
	len = tel.length;
	if(len == 10 || len == 11){
		return(true);
	}
	return(false);
}
function fnclibCheckEmailAddress(mail)
{
	var ary = mail.split('@');
	if(ary.length == 2){
		return(true);
	}
	return(false);
}
function fnclibCheckHankakuNum(str)
{
	var ret = str.match(/^[0-9-]+$/);
	if(ret == null){
		return(false);
	}
	return(true);
}
function fnclibCheckHankaku(str)
{
	var ret = str.match(/^[\x20-\x7E]+$/);
	if(ret == null){
		return(false);
	}
	return(true);
}
function fnclibCheckHankana(str)
{
	var ret = str.match(/[\uff61-\uff9f]/g);
	if(ret == null){
		return(false);
	}
	return(true);
}
function fnclibStrReplace(key1, key2, str)
{
	while ( str.indexOf(key1,0) != -1 )
	{
		str=str.replace(key1,key2);
	}
	return(str);
}
