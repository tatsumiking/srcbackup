
var ctx_dFontSize;
var ctx_szFontName;
var ctx_sFillColor;
var ctx_sStrokeColor;
var ctx_nStrokeThick;
var ctx_dXTime = 1.0;
var ctx_dYTime = 1.0;

function fncCtxInit()
{
	ctx_dXTime = 1.0;
	ctx_dYTime = 1.0;
}
function fncCtxSetFont(ctx, fontsize, fontname)
{
	ctx.font = fontsize+"px"+"'"+fontname+"'";
	ctx_dFontSize = fontsize;
	ctx_szFontName = fontname;
}

function fncCtxSetBrush(ctx, color)
{
	ctx_sFillColor = color;
	ctx.fillStyle = ctx_sFillColor;
}

function fncCtxSetPen(ctx, color, thick)
{
	ctx_sStrokeColor = color
	ctx_nStrokeThick = thick;
	ctx.strokeStyle = ctx_sStrokeColor;
	ctx.lineWidth = ctx_nStrokeThick;
}

function fncCtxYokoString(ctx, sx, sy, str)
{
	var ey;

	ey = sy + ctx_dFontSize;
	fncCtxFillText(ctx, str, sx, ey, ctx_dFontSize, 0.0);
}
function fncCtxTateString(ctx, sx, sy, str)
{
	var	x, y;
	var	i, max;
	var	sstr;

	x = sx;
	y = sy;
	max = str.length;
	for(i = 0; i < max; i++){
		sstr = str.substr(i, 1);
		fncCtxFillSingleText(ctx, sstr, x, y);
		y = y + fncWebFontTateMojiSize(sstr, ctx_dFontSize);
	}
}

function fncCtxYokoLeftString(ctx, sx, sy, ex, ey, space, str)
{
	var wd, fs, size;
	var cnt;

	wd = ex - sx;
	fs = ey - sy;
	ctx_dFontSize = fs;
	cnt = str.length;
	ctx.font = fs +"px"+"'"+ctx_szFontName+"'";
	size = fncWebFontYokoStrSize(str, fs, space);
	if(size < wd){
		fncCtxFillText(ctx, str, sx, ey, fs, space);
	} else {
		fncCtxYokoTrnsString(ctx, sx, sy, ex, ey, space, str);
	}
}

function fncCtxYokoRightString(ctx, sx, sy, ex, ey, space, str)
{
	var wd, fs, size;
	var cnt;

	wd = ex - sx;
	fs = ey - sy;
	ctx_dFontSize = fs;
	cnt = str.length;
	ctx.font = fs +"px"+"'"+ctx_szFontName+"'";
	size = fncWebFontYokoStrSize(str, fs, space);
	if(size < wd){
		sx = ex - size;
		fncCtxFillText(ctx, str, sx, ey, fs, space);
	} else {
		fncCtxYokoTrnsString(ctx, sx, sy, ex, ey, space, str);
	}
}

function fncCtxYokoCenterString(ctx, sx, sy, ex, ey, space, str)
{
	var wd, fs, size;
	var cnt;

	wd = ex - sx;
	fs = ey - sy;
	ctx_dFontSize = fs;
	cnt = str.length;
	ctx.font = fs +"px"+"'"+ctx_szFontName+"'";
	size = fncWebFontYokoStrSize(str, fs, space);
	if(size < wd){
		sx = (sx+ex)/2 - size/2;
		fncCtxFillText(ctx, str, sx, ey, fs, space);
	} else {
		fncCtxYokoTrnsString(ctx, sx, sy, ex, ey, space, str);
	}
}

function fncCtxYokoEqualString(ctx, sx, sy, ex, ey, space, str)
{
	var wd, fs, size, add;
	var i, cnt;
	var sstr;

	wd = ex - sx;
	fs = ey - sy;
	ctx_dFontSize = fs;
	cnt = str.length;
	ctx.font = fs +"px"+"'"+ctx_szFontName+"'";
	size = fncWebFontYokoStrSize(str, fs, space);
	if(cnt == 1){
		add = (wd-size) / 2;
		fncCtxFillSingleText(ctx, str, sx+add, ey);
	} else if(size < wd){
		add = (wd - size) / (cnt-1);
		for(i = 0; i < cnt; i++){
			sstr = str.substr(i, 1);
			fncCtxFillSingleText(ctx, sstr, sx, ey);
			size = fncWebFontYokoMojiSize(sstr, fs);
			sx = sx+size+add+space;
		}
	} else {
		fncCtxYokoTrnsString(ctx, sx, sy, ex, ey, space, str);
	}
}

function fncCtxYokoTrnsString(ctx, sx, sy, ex, ey, space, str)
{
	var wd, size;
	var time;

	wd = ex - sx;
	fs = ey - sy;
	ctx_dFontSize = fs;
	size = fncWebFontYokoStrSize(str, fs, space);
	time = wd / size;
	ctx_dXTime = time;
	ctx_dYTime = 1.0;
	// sx = sx / time;
	//ctx.save();
	// ctx.setTransform(time, 0, 0, 1, 0, 0);
	fncCtxFillText(ctx, str, sx, ey, fs, space);
	//ctx.restore();
	ctx_dXTime = 1.0;
	ctx_dYTime = 1.0;
}

function fncCtxTateTopString(ctx, sx, sy, ex, ey, space, str)
{
	var hi, fs, size;
	var x, y;
	var i, cnt;
	var ary;
	var sstr;
	var zenstr;
	var c;
	var tsy, tey;

	if(str == ""){
		return;
	}
	fs = ex - sx;
	hi = ey - sy;
	ctx_dFontSize = fs;
	ctx.font = fs +"px"+"'"+ctx_szFontName+"'";
	cnt = fncCtxGetTateMojiCount(str);
	ary = fncCtxGetStrAry(str);
	size = fncWebFontTateStrSize(ary, fs, space);
	if(size < hi){
		x = sx;
		y = sy + fncWebFontTateMojiSize(ary[0], fs);
		for(i = 0; i < cnt; i++){
			sstr = ary[i];
			c = sstr.charCodeAt(0);
			if(0x20 < c && c < 0x81){ // ”¼Šp
				tsy = y - fs;
				tey = y;

				zenstr = fnclinHanToZen(sstr);
				fncCtxYokoEqualString(ctx, sx, tsy, ex, tey, 0.0, zenstr);

				y = y + fs + space;
			}else{
				fncCtxFillSingleText(ctx, sstr, x, y);
				y = y + fncWebFontTateMojiSize(sstr, fs)+space;
			}
		}
	} else {
		fncCtxTateTrnsString(ctx, sx, sy, ex, ey, space, str);
	}
}

function fncCtxTateBottomString(ctx, sx, sy, ex, ey, space, str)
{
	var hi, fs, size;
	var x, y;
	var i, cnt;
	var ary;
	var sstr;
	var zenstr;
	var c;
	var tsy, tey;

	if(str == ""){
		return;
	}
	fs = ex - sx;
	hi = ey - sy;
	ctx_dFontSize = fs;
	ctx.font = fs +"px"+"'"+ctx_szFontName+"'";
	cnt = fncCtxGetTateMojiCount(str);
	ary = fncCtxGetStrAry(str);
	size = fncWebFontTateStrSize(ary, fs, space);
	if(size < hi){
		x = sx;
		y = ey-size+fncWebFontTateMojiSize(ary[0], fs);
		for(i = 0; i < cnt; i++){
			sstr = ary[i];
			c = sstr.charCodeAt(0);
			if(0x20 < c && c < 0x81){ // ”¼Šp
				tsy = y - fs;
				tey = y;
				zenstr = fnclinHanToZen(sstr);
				fncCtxYokoEqualString(ctx, sx, tsy, ex, tey, 0.0, zenstr);
				y = y + fs+space;
			}else{
				fncCtxFillSingleText(ctx, sstr, x, y);
				y = y + fncWebFontTateMojiSize(sstr, fs)+space;
			}
		}
	} else {
		fncCtxTateTrnsString(ctx, sx, sy, ex, ey, space, str);
	}
}

function fncCtxTateCenterString(ctx, sx, sy, ex, ey, space, str)
{
	var hi, fs, size;
	var x, y;
	var i, cnt;
	var ary;
	var sstr;
	var zenstr;
	var c;
	var tsy, tey;

	if(str == ""){
		return;
	}
	fs = ex - sx;
	hi = ey - sy;
	ctx_dFontSize = fs;
	ctx.font = fs +"px"+"'"+ctx_szFontName+"'";
	cnt = fncCtxGetTateMojiCount(str);
	ary = fncCtxGetStrAry(str);
	size = fncWebFontTateStrSize(ary, fs, space);
	if(size < hi){
		x = sx;
		y = sy + ((ey-sy)-size) / 2+fncWebFontTateMojiSize(ary[0], fs);
		for(i = 0; i < cnt; i++){
			sstr = ary[i];
			c = sstr.charCodeAt(0);
			if(0x20 < c && c < 0x81){ // ”¼Šp
				tsy = y - fs;
				tey = y;
				zenstr = fnclinHanToZen(sstr);
				fncCtxYokoEqualString(ctx, sx, tsy, ex, tey, 0.0, zenstr);
				y = y + fs+space;
			}else{
				fncCtxFillSingleText(ctx, sstr, x, y);
				y = y + fncWebFontTateMojiSize(sstr, fs)+space;
			}
		}
	} else {
		fncCtxTateTrnsString(ctx, sx, sy, ex, ey, space, str);
	}
}
function fncCtxTateEqualString(ctx, sx, sy, ex, ey, space, str)
{
	var hi, fs, size, add;
	var x, y;
	var i, cnt;
	var ary;
	var sstr;
	var zenstr;
	var c;
	var tsy, tey;

	if(str == ""){
		return;
	}
	fs = ex - sx;
	hi = ey - sy;
	ctx_dFontSize = fs;
	ctx.font = fs +"px"+"'"+ctx_szFontName+"'";
	cnt = fncCtxGetTateMojiCount(str);
	ary = fncCtxGetStrAry(str);
	size = fncWebFontTateStrSize(ary, fs, space);
	if(cnt == 1){
		add = (hi-size) / 2;
		x = sx;
		y = sy + add + fncWebFontTateMojiSize(ary[0], fs);
		sstr = ary[0];
		c = sstr.charCodeAt(0);
		if(0x20 < c && c < 0x81){ // ”¼Šp
			tsy = y - fs;
			tey = y;
			zenstr = fnclinHanToZen(sstr);
			fncCtxYokoEqualString(ctx, sx, tsy, ex, tey, 0.0, zenstr);
		}else{
			fncCtxFillSingleText(ctx, str, x, y);
		}
	} else if(size < hi){
		add = (hi - size) / (cnt-1);
		x = sx;
		y = sy + fncWebFontTateMojiSize(ary[0], fs);
		for(i = 0; i < cnt; i++){
			sstr = ary[i];
			c = sstr.charCodeAt(0);
			if(0x20 < c && c < 0x81){ // ”¼Šp
				tsy = y - fs;
				tey = y;
				zenstr = fnclinHanToZen(sstr);
				fncCtxYokoEqualString(ctx, sx, tsy, ex, tey, 0.0, zenstr);
				y = y+fs+add+space;
			}else{
				fncCtxFillSingleText(ctx, sstr, x, y);
				y = y + fncWebFontTateMojiSize(sstr, fs) + add + space;
			}
		}
	} else {
		fncCtxTateTrnsString(ctx, sx, sy, ex, ey, space, str);
	}
}

function fncCtxTateTrnsString(ctx, sx, sy, ex, ey, space, str)
{
	var fs, hi, size;
	var cnt;
	var time;
	var crty;
	var zenstr;

	if(str == ""){
		return;
	}
	fs = ex - sx;
	hi = ey - sy;
	ctx_dFontSize = fs;
	cnt = fncCtxGetTateMojiCount(str);
	ary = fncCtxGetStrAry(str);
	size = fncWebFontTateStrSize(ary, fs, space);

	time = hi / size;

	tsy = sy;
	for(i = 0; i < cnt; i++){
		sstr = ary[i];
		c = sstr.charCodeAt(0);
		if(0x20 < c && c < 0x81){ // ”¼Šp
			tey = tsy + fs * time + space;
			//fncCtxBoxs(ctx, sx, tsy, ex, tey);
			zenstr = fnclinHanToZen(sstr);
			fncCtxYokoEqualString(ctx, sx, tsy, ex, tey, 0.0, zenstr);
			tsy = tey + space;
		}else{
			ctx_dFontSize = fs;
			tey = tsy + fncWebFontTateMojiSize(sstr, fs)*time;
			//ctx.save();
			//ctx.setTransform(1, 0, 0, time, 0, 0);
			ctx_dXTime = 1.0;
			ctx_dYTime = time;
			//fncCtxBoxs(ctx, sx, tsy, ex, tey);
			fncCtxFillSingleText(ctx, sstr, sx, tey);
			ctx_dXTime = 1.0;
			ctx_dYTime = 1.0;
			//ctx.restore();
			tsy = tey + space;
		}
	}
}

function fncCtxImageDraw(ctx, img, sx, sy, wd, hi)
{
	ctx.drawImage(img, sx, sy, wd, hi);
}
function fncCtxImageDrawR180(ctx, img, sx, sy, wd, hi, cx, cy)
{
	ctx.drawImage(img, sx, sy, wd, hi);
}
function fncCtxImageClipDraw(ctx, img, x, y, wd, hi, sx, sy, ex, ey)
{
	ctx.save();
	ctx.beginPath();
	ctx.moveTo(sx,sy);
	ctx.lineTo(ex,sy);
	ctx.lineTo(ex,ey);
	ctx.lineTo(sx,ey);
	ctx.lineTo(sx,sy);
	ctx.clip();
	ctx.drawImage(img, x, y, wd, hi);
	ctx.restore();
}

function fncCtxRoundBoxs(ctx, sx, sy, ex, ey, r)
{
	var tsx, tsy, tex, tey;

	tsx = sx + r;
	tsy = sy + r;
	tex = ex - r;
	tey = ey - r;
	ctx.beginPath();
	ctx.moveTo(tsx,sy);
	ctx.lineTo(tex,sy);
	ctx.quadraticCurveTo(ex, sy, ex, tsy);
	ctx.lineTo(ex,tey);
	ctx.quadraticCurveTo(ex, ey, tex, ey);
	ctx.lineTo(tsx,ey);
	ctx.quadraticCurveTo(sx, ey, sx, tey);
	ctx.lineTo(sx,tsy);
	ctx.quadraticCurveTo(sx, sy, tsx, sy);
	ctx.closePath();
	ctx.stroke();
}

function fncCtxPointBoxs(ctx, cx, cy, add)
{
	var sx, sy, ex, ey;

	sx = cx-add;
	sy = cy-add;
	ex = cx+add;
	ey = cy+add;
	fncCtxBoxs(ctx, sx, sy, ex, ey);
}

function fncCtxBoxs(ctx, sx, sy, ex, ey)
{
	ctx.beginPath();
	ctx.moveTo(sx,sy);
	ctx.lineTo(ex,sy);
	ctx.lineTo(ex,ey);
	ctx.lineTo(sx,ey);
	ctx.lineTo(sx,sy);
	ctx.closePath();
	ctx.stroke();
}

function fncCtxRoundFill(ctx, sx, sy, ex, ey, r)
{
	var tsx, tsy, tex, tey;

	tsx = sx + r;
	tsy = sy + r;
	tex = ex - r;
	tey = ey - r;
	ctx.beginPath();
	ctx.moveTo(tsx,sy);
	ctx.lineTo(tex,sy);
	ctx.quadraticCurveTo(ex, sy, ex, tsy);
	ctx.lineTo(ex,tey);
	ctx.quadraticCurveTo(ex, ey, tex, ey);
	ctx.lineTo(tsx,ey);
	ctx.quadraticCurveTo(sx, ey, sx, tey);
	ctx.lineTo(sx,tsy);
	ctx.quadraticCurveTo(sx, sy, tsx, sy);
	ctx.closePath();
	ctx.fill();
}

function fncCtxFill(ctx, sx, sy, ex, ey)
{
	ctx.beginPath();
	ctx.moveTo(sx,sy);
	ctx.lineTo(ex,sy);
	ctx.lineTo(ex,ey);
	ctx.lineTo(sx,ey);
	ctx.lineTo(sx,sy);
	ctx.closePath();
	ctx.fill();
}

function fncCtxCrcl(ctx, x0, y0, r)
{
	ctx.beginPath();
	ctx.arc(x0, y0, r, 0, Math.PI*2, false);
	ctx.stroke();
}

function fncCtxLine(ctx, sx, sy, ex, ey)
{
	ctx.beginPath();
	ctx.moveTo(sx,sy);
	ctx.lineTo(ex,ey);
	ctx.stroke();
}

function fncCtxGetStrAry(str)
{
	var ary;
	var max, i;
	var c;
	var hanflag;
	var sstr;

	ary = new Array();
	hanflag = 0;
	crt = 0;
	max = str.length;
	for(i = 0; i < max; i++){
		sstr = str.substr(i, 1);
		c = str.charCodeAt(i);
		if(0x21 <= c && c < 0x81){ // ”¼Šp
			if(hanflag == 0){
				ary.push(sstr);
				crt = ary.length - 1;
				hanflag = 1;
			}else{
				ary[crt] = ary[crt] + sstr;
			}
		}else if(0x20 == c){
			if(hanflag == 0){
				ary.push(sstr);
			}else{
				ary[crt] = ary[crt] + sstr;
			}
		}else{ // ‘SŠp
			ary.push(sstr);
			hanflag = 0;
		}
	}
	return(ary);
}

function fncCtxGetTateMojiCount(str)
{
	var max, i;
	var c;
	var hanflag;

	hanflag = 0;
	cnt = 0;
	max = str.length;
	for(i = 0; i < max; i++){
		c = str.charCodeAt(i);
		if(0x20 < c && c < 0x81){ // ”¼Šp
			if(hanflag == 0){
				cnt++;
				hanflag = 1;
			}
		}else{ // ‘SŠp
			cnt++;
			hanflag = 0;
		}
	}
	return(cnt);
}

function fncCtxFillText(ctx, str, sx, ey, fs, space)
{
	var max, i;
	var size;
	var ex, sy;

	max = str.length;
	for(i = 0; i < max; i++){
		sstr = str.substr(i,1);
		fncCtxFillSingleText(ctx, sstr, sx, ey);
		size = fncWebFontYokoMojiSize(sstr, fs)*ctx_dXTime;
		ex = sx + size;
		sy = ey - fs;
		//fncCtxBoxs(ctx, sx, sy, ex, ey);
		sx = sx + size + space;
	}
}

var mlib_ctx;
var mlib_sx;
var mlib_ey;
var mlib_time;
var mlib_xtime, mlib_ytime;

function fncCtxFillSingleText(ctx, str, sx, ey)
{
	/*
	if(ctx_nStrokeThick != 0){
		ctx.strokeText(str, sx, ey);
	}
	ctx.fillText(str, sx, ey);
	*/
	var sCode;
	var nCode = str.charCodeAt(0);

	if(nCode < 0x100){
		sCode = "00"+nCode.toString(16).toUpperCase();
	}else{
		sCode = nCode.toString(16).toUpperCase();
	}
	var key = ctx_szFontName+"_"+sCode;
	var sGryph = localStorage.getItem(key);
	if(sGryph != null){
		mlib_ctx = ctx;
		mlib_sx = sx;
		mlib_ey = ey;
		mlib_time = ctx_dFontSize/1024.0;
		mlib_xtime = ctx_dXTime;
		mlib_ytime = ctx_dYTime;
		fncCtxDrawWebFont(sGryph);
	}
}

function fncCtxDrawWebFont(sGryph)
{
	
	var aryLine;
	var i, max;
	var aryClm;
	var x1, y1, x2, y2, x3, y3, x4, y4;
	var rsx, rsy, rex, rey;

	rsx = 0; rsy = 0;
	aryLine = sGryph.split("\n");
	max = aryLine.length
	mlib_ctx.beginPath();
    mlib_ctx.lineJoin  = "round"; // Ü‚êü‚ÌŠpŠÛ‚ß
	for(i = 1; i < max; i++){
		aryClm = aryLine[i].split(",");
		if(aryClm[0] == "m"){
			if(rsx != 0 && rsy != 0){
				mlib_ctx.lineTo(rsx, rsy);
				mlib_ctx.closePath(); // •¡”‚Ì}Œ`‚Ì•¡‡ŽžŽn“_ˆÙí‘Î‰ž
			}
			x1 = fncCanvasFontXTrns(parseFloat(aryClm[1]));
			y1 = fncCanvasFontYTrns(parseFloat(aryClm[2]));
			mlib_ctx.moveTo(x1, y1);
			rsx = x1; rsy = y1;
		}else if(aryClm[0] == "l"){
			x2 = fncCanvasFontXTrns(parseFloat(aryClm[1]));
			y2 = fncCanvasFontYTrns(parseFloat(aryClm[2]));
			mlib_ctx.lineTo(x2, y2);
			rex = x2; rey = y2;
		}else if(aryClm[0] == "c"){
			x2 = fncCanvasFontXTrns(parseFloat(aryClm[1]));
			y2 = fncCanvasFontYTrns(parseFloat(aryClm[2]));
			x3 = fncCanvasFontXTrns(parseFloat(aryClm[3]));
			y3 = fncCanvasFontYTrns(parseFloat(aryClm[4]));
			x4 = fncCanvasFontXTrns(parseFloat(aryClm[5]));
			y4 = fncCanvasFontYTrns(parseFloat(aryClm[6]));
			mlib_ctx.bezierCurveTo(x2, y2, x3, y3, x4, y4);
			rex = x4; rey = y4;
		}
	}
	mlib_ctx.lineTo(rsx, rsy);
	mlib_ctx.closePath();
	if(ctx_nStrokeThick != 0){
		mlib_ctx.stroke();
	}
	mlib_ctx.fill();
}

function fncCanvasFontXTrns(x)
{
	var retx;

	retx = (x)*mlib_time*mlib_xtime + mlib_sx;
	return(retx);
}

function fncCanvasFontYTrns(y)
{
	var rety;

	//rety = mlib_ey - (y+36)*mlib_time;
	rety = (y-114)*mlib_time*mlib_ytime + mlib_ey;
	return(rety);
}


