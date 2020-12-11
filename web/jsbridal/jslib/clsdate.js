// 日付型クラス
function clsDate()
{
	var date = new Date();
	this.yy = date.getFullYear().toString().padStart(4, '0');
	this.MM = date.getMonth().toString().padStart(2, '0');
	this.dd = date.getDay().toString().padStart(2, '0');
	this.HH = date.getHours().toString().padStart(2, '0');
	this.mm = date.getMinutes().toString().padStart(2, '0');
}
clsDate.prototype.fncSetSimpleData = function(sSimpleDate)
{
	// 2020/12/01 09:00:00
	this.yy = sSimpleDate.substr(0, 4);
	this.MM = sSimpleDate.substr(5, 2);
	this.dd = sSimpleDate.substr(8, 2);
	this.HH = sSimpleDate.substr(11, 2);
	this.mm = sSimpleDate.substr(14, 2);
}
clsDate.prototype.fncGetSimpleData = function()
{
	var sSimpleDate;

	sSimpleDate = "";
	sSimpleDate = sSimpleDate + this.yy + "/";
	sSimpleDate = sSimpleDate + this.MM + "/";
	sSimpleDate = sSimpleDate + this.dd + " ";
	sSimpleDate = sSimpleDate + this.HH + ":";
	sSimpleDate = sSimpleDate + this.mm + ":00";
	return(sSimpleDate);
}
clsDate.prototype.fncSetData = function(sgg, syy, sMM, sdd, sHH, smm, sss)
{
	if(sgg == "西暦")
	{
		this.yy = syy;
	}
	else if(sgg == "令和")
	{
		if(syy == "元")
		{
			nyy = 2019;
		}else{
			nyy = parseInt(syy) + 2019 - 1;
		}
		this.yy = nyy.toString();
	}
	this.MM = fnclibStrnumToStrnum00(sMM);
	this.dd = fnclibStrnumToStrnum00(sdd);
	this.HH = fnclibStrnumToStrnum00(sHH);
	this.mm = fnclibStrnumToStrnum00(smm);
}
clsDate.prototype.fncGetData = function(sgg, syy, sMM, sdd, sHH, smm)
{
	if(sgg == "西暦")
	{
		syy = this.yy;
	}
	else
	{
		nyy = parseInt(this.yy);
		nyy = nyy - 2019 + 1;
		syy = fnclibNumToStrnum00(nyy);
		sgg = "令和";
	}
	sMM = this.MM;
	sdd = this.dd;
	sHH = this.HH;
	smm = this.mm;
}
clsDate.prototype.fncGetFormatData = function(sgg)
{
	var sFrmatDate;

	if(sgg == "西暦")
	{
		sFrmatDate = this.yy;
	}
	else
	{
		nyy = parseInt(this.yy);
		nyy = nyy - 2019 + 1;
		syy = fnclibNumToStrnum00(nyy);
		sFrmatDate = "令和" + syy;
	}
	sFrmatDate = sFrmatDate + "/" + this.MM;
	sFrmatDate = sFrmatDate + "/" + this.dd;
	sFrmatDate = sFrmatDate + " " + this.HH;
	sFrmatDate = sFrmatDate + ":" + this.mm;
	return(sFrmatDate);
}
