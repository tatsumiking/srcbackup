//

var m_svClrStroke;
var m_svClrFill;
var m_svThick;
var m_svFont;

function fncSaveImageData()
{
	var lystr;
	var no;
	var key;
	var rdo;

	m_svClrStroke = "";
	m_svClrFill = "";
	m_svThick = 0;
	m_svFon = "";
	lystr = "//,"+m_aryEdit.length+","+m_aryImg.length+",,,\r\n";
	lystr = lystr+"SIZE,"+m_nCAXSize+","+m_nCAYSize+",\r\n";
	lystr = lystr+"AREA,"+m_nCASX+","+m_nCASY+","+m_nCAEX+","+m_nCAEY+",\r\n";
	lystr = lystr + fncSaveAryObjFigu(m_aryObjFigu);

	key = "ObariLayout"+localStorage.getItem("ObariCrtNo");
	localStorage.setItem(key, lystr);

	if(m_rdoMen1.checked == true){
		rdo = 1;
	} else if(m_rdoMen2.checked == true){
		rdo = 2;
	} else if(m_rdoMen3.checked == true){
		rdo = 3;
	}
	key = "ObariMen"+localStorage.getItem("ObariCrtNo");
	localStorage.setItem(key, rdo);

	if(m_rdoWak1.checked == true){
		rdo = 1;
	} else if(m_rdoWak2.checked == true){
		rdo = 2;
	} else if(m_rdoMen3.checked == true){
		rdo = 3;
	}
	key = "ObariWaku"+localStorage.getItem("ObariCrtNo");
	localStorage.setItem(key, rdo);

	key = "ObariCount"+localStorage.getItem("ObariCrtNo");
	localStorage.setItem(key, "1");

	data = "com=layout.txt$"+lystr+"$$$";
	sendRequest("POST","savefile.php",data,false,fncSaveCallback);
}

function fncSaveCallback(xmlhttp)
{
	var data = xmlhttp.responseText;
}

function fncSaveAryObjFigu(aryObjFigu)
{
	var arystr;
	var max, idx;
	var objFigu;

	arystr = "";
	max = aryObjFigu.length;
	for(idx = 0; idx < max; idx++){
		objFigu = aryObjFigu[idx];
		if(objFigu.kind == OBJTEXT){
			arystr = arystr + fncSaveTextObj(objFigu);
		}else if(objFigu.kind == OBJPLGN){
			arystr = arystr + fncSavePlgnObj(objFigu);
		}else if(objFigu.kind == OBJBOXS){
			arystr = arystr + fncSaveBoxsObj(objFigu);
		}else if(objFigu.kind == OBJFILL){
			arystr = arystr + fncSaveFillObj(objFigu);
		}else if(objFigu.kind == OBJIMAGE){
			arystr = arystr + fncSaveImageObj(objFigu);
		}else if(objFigu.kind == OBJGROUP){
			arystr = arystr + fncSaveGroupObj(objFigu);
		}
	}
	return(arystr)
}

function fncSaveTextObj(objFigu)
{
	var objstr;
	var spp, epp;
	var cmdstr;
	var mimx;
	var title;
	var str;

	objstr = "";
	if(m_svFont != objFigu.font){
		m_svFont = objFigu.font
		objstr = objstr + "FONT,256,"+objFigu.font+",\r\n";
	}
	if(m_svClrStroke != objFigu.clrStroke || m_svThick != objFigu.thick){
		m_svClrStroke = objFigu.clrStroke;
		m_svThick = objFigu.thick;
		objstr = objstr + "CPEN,"+objFigu.clrStroke+","+objFigu.thick+",\r\n";
	}
	if(m_svClrFill != objFigu.clrFill){
		m_svClrFill = objFigu.clrFill;
		objstr = objstr + "BRUSH,"+objFigu.clrFill+",\r\n";
	}
	if(objFigu.yktt == YOKO){
		if(objFigu.mode == LEFT || objFigu.mode == TOP){
			cmdstr = "YLSTR";
		}else if(objFigu.mode == RIGHT || bjFigu.mode == BOTTOM){
			cmdstr = "YRSTR";
		}else if(objFigu.mode == CENTER){
			cmdstr = "YCSTR";
		}else{ //if(objFigu.mode == EQUAL){
			cmdstr = "YESTR";
		}
	}else{ // if(objFigu.yktt == TATE){
		if(objFigu.mode == LEFT || objFigu.mode == TOP){
			cmdstr = "TTSTR";
		}else if(objFigu.mode == RIGHT || objFigu.mode == BOTTOM){
			cmdstr = "TBSTR";
		}else if(objFigu.mode == CENTER){
			cmdstr = "TCSTR";
		}else{ //if(objFigu.mode == EQUAL){
			cmdstr = "TESTR";
		}
	}
	spp = objFigu.fncObjFiguGetAtrXY(0);
	epp = objFigu.fncObjFiguGetAtrXY(1);
	if(objFigu.idx == 0){
		title = "";
		str = objFigu.def;
	}else{
		title = m_aryTitle[objFigu.idx];
		str = m_aryEdit[objFigu.idx].value;
		str = str.replace("\n","|"); // inputarea
	}
	objstr = objstr + cmdstr + ","+objFigu.idx+","+title+",";
	objstr = objstr + spp.x + "," + spp.y + ",";
	objstr = objstr + epp.x + "," + epp.y + ",";
	objstr = objstr + str +",\r\n";
	return(objstr);
}

function fncSavePlgnObj(objFigu)
{
	var objstr;
	var pp;
	var axy;

	objstr = "";
	if(m_svClrStroke != objFigu.clrStroke || m_svThick != objFigu.thick){
		m_svClrStroke = objFigu.clrStroke;
		m_svThick = objFigu.thick
		objstr = objstr + "CPEN,"+objFigu.clrStroke+","+objFigu.thick+",\r\n";
	}
	if(objFigu.pcnt == 2){
		objstr = objstr + "LINE,";
		axy = objFigu.fncObjFiguGetAtrXY(0);
		objstr = objstr + axy.x + "," + axy.y + ","
		axy = objFigu.fncObjFiguGetAtrXY(1);
		objstr = objstr + axy.x + "," + axy.y + ",,"
	}else{
		objstr = objstr + "PLGN" + ","+objFigu.pcnt+",";
		for(pp = 0; pp < objFigu.pcnt; pp++){
			axy = objFigu.fncObjFiguGetAtrXY(pp);
			objstr = objstr + axy.atr + "," + axy.x + "," + axy.y + ","
		}
	}
	objstr = objstr +",\r\n";
	return(objstr);
}

function fncSaveBoxsObj(objFigu)
{
	var objstr;
	var spp, epp;

	objstr = "";
	if(m_svClrStroke != objFigu.clrStroke || m_svThick != objFigu.thick){
		m_svClrStroke = objFigu.clrStroke;
		m_svThick = objFigu.thick
		objstr = objstr + "CPEN,"+objFigu.clrStroke+","+objFigu.thick+",\r\n";
	}
	spp = objFigu.fncObjFiguGetAtrXY(0);
	epp = objFigu.fncObjFiguGetAtrXY(1);
	objstr = objstr + "BOXS,";
	objstr = objstr + spp.x + "," + spp.y + ",";
	objstr = objstr + epp.x + "," + epp.y + ",";
	objstr = objstr +",\r\n";
	return(objstr);
}

function fncSaveFillObj(objFigu)
{
	var objstr;
	var spp, epp;
	var 

	objstr = "";
	if(m_svClrStroke != objFigu.clrStroke || m_svThick != objFigu.thick){
		m_svClrStroke = objFigu.clrStroke;
		m_svThick = objFigu.thick
		objstr = objstr + "CPEN,"+objFigu.clrStroke+","+objFigu.thick+",\r\n";
	}
	if(m_svClrFill != objFigu.clrFill){
		m_svClrFill = objFigu.clrFill;
		objstr = objstr + "BRUSH,"+objFigu.clrFill+",\r\n";
	}
	spp = objFigu.fncObjFiguGetAtrXY(0);
	epp = objFigu.fncObjFiguGetAtrXY(1);
	r = objFigu.arcr;
	objstr = objstr + "FILL,";
	objstr = objstr + spp.x + "," + spp.y + ",";
	objstr = objstr + epp.x + "," + epp.y + ",";
	if(r == 0){
		objstr = objstr +",\r\n";
	}else{
		objstr = objstr + r +",\r\n";
	}
	return(objstr);
}

function fncSaveImageObj(objFigu)
{
	var objstr;
	var spp, epp;
	var scp, ecp;

	objstr = "";
	spp = objFigu.fncObjFiguGetAtrXY(0);
	epp = objFigu.fncObjFiguGetAtrXY(1);
	if(objFigu.pcnt > 2){
		scp = objFigu.fncObjFiguGetAtrXY(2);
		ecp = objFigu.fncObjFiguGetAtrXY(3);
	}
	objstr = objstr + "IMG,"+objFigu.idx+",";
	objstr = objstr + spp.x + "," + spp.y + ",";
	objstr = objstr + epp.x + "," + epp.y + ",";
	objstr = objstr +objFigu.def+","+objFigu.thick+",";
	if(objFigu.pcnt <= 2){
		objstr = objstr + "\r\n";
	}else{
		objstr = objstr + scp.x + "," + scp.y + ",";
		objstr = objstr + ecp.x + "," + ecp.y + ",\r\n";
	}
	return(objstr);
}

function fncSaveGroupObj(objFigu)
{
	var objstr;
	var spp, epp;

	objstr = "";
	spp = objFigu.fncObjFiguGetAtrXY(0);
	epp = objFigu.fncObjFiguGetAtrXY(1);
	objstr = objstr + "GROUP,";
	objstr = objstr + spp.x + "," + spp.y + ",";
	objstr = objstr + epp.x + "," + epp.y + ",,,";
	objstr = objstr +",\r\n";
	objstr = objstr +fncSaveAryObjFigu(objFigu.aryObjFigu);
	return(objstr);
}

