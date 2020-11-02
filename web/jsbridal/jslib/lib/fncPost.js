//
// —X•Ö”Ô†‚©‚çZŠ‚ðŽæ“¾‚·‚éŠÖ”
var m_sAdrsNo;
var m_fncAdrsCallBack;
		
function fncAdrsNoToAdres(path, adrsno, fncCallBack)
{
	var ary = adrsno.split('-');
	if(ary.length == 2){
		adrsno = ary[0]+ary[1];
	}
	if(adrsno.length != 7){
		return;
	}
	m_sAdrsNo = adrsno;
	m_fncAdrsCallBack = fncCallBack;
	var adrstbl = path + "/"+ adrsno.substr(0, 3) + ".js";
	var script = document.createElement("script");
	script.setAttribute("type", "text/javascript");
	script.setAttribute("charset", "UTF-8");
	script.setAttribute("src", adrstbl);
	document.getElementsByTagName("head").item(0).appendChild(script)
}
function $yubin(json) {
	var ary;
	var adrs1, adrs2, adrs3;
	var adrs;
	var baseno;

	ary = json[m_sAdrsNo];
	if(ary.length >= 3){
		adrs1 = ary[1];
		adrs2 = ary[2];
		if(ary.length == 4){
			adrs3 = ary[3];
		}else{
			adrs3 = "";
		}
		adrs = adrs1+","+adrs2+adrs3;
		m_fncAdrsCallBack(adrs);
	}else{
		baseno = m_sAdrsNo.substr(0, 3)+"0000";
		ary = json[m_sAdrsNo];
		adrs1 = ary[1];
		adrs2 = ary[2];
		adrs = adrs1+","+adrs2;
		m_fncAdrsCallBack(adrs);
	}
}

