// クラスを定義
function ObjFigu()
{
	// プロパティ
	this.kind = 0; // OBJTEXT,OBJPLGN,OBJIMAGE,OBJGROUP
	this.aryObjFigu;
	this.font = "ＭＳ ゴシック"; // 書体(OBJTEXTのみ使用)
	this.yktt = YOKO; // 縦書き、横書き、OBJTEXTのみ使用
	this.mode = LEFT; // 右寄せ、左寄せ、中央寄せ、均等、上寄せ、下寄せ
	this.type = 0; // 0：１入力 1：複数行入力 2:空白入力
	this.deg = 0; // -360～360 回転角度
	this.hmirror = 1; // 水平鏡
	this.vmirror = 1; // 垂直鏡
	this.space = 0;
	this.img;
	this.arySelectLine; // イメージのデータがある部分を
	this.def = "";	// OBJTEXTは文字列を格納 OBJIMAGEはファイル名を格納
	this.aryAtrXY = Array(0); // PLGN以外は左上座標、右下座標を保持
	this.pcnt = 0;　 // PLGN以外は2固定
	this.arcr = 0; // 角Ｒ値
	this.thick = 1.0; // 線幅,サイズ単位mm 画像透過率　1.0～0.1
	this.clrStroke = "#000000"; // アウトラインの色
	this.clrFill = "#000000"; // 塗りつぶしの色
}
ObjFigu.prototype.fncHMirror = function()
{
	if(this.hmirror == 1){
		this.hmirror = -1;
	}else{
		this.hmirror = 1;
	}
}
ObjFigu.prototype.fncVMirror = function()
{
	if(this.vmirror == 1){
		this.vmirror = -1;
	}else{
		this.vmirror = 1;
	}
}
ObjFigu.prototype.fncObjFiguGetMiniMaxNoRound = function(mimx)
{
	var pp;
	var axy;

	axy = this.fncObjFiguGetAtrXY(0);
	mimx.sx = axy.x;
	mimx.sy = axy.y;
	mimx.ex = axy.x;
	mimx.ey = axy.y;
	for(pp = 1; pp < this.pcnt; pp++){
		axy = this.fncObjFiguGetAtrXY(pp);
		if(axy.x < mimx.sx){
			mimx.sx = axy.x;
		}
		if(axy.y < mimx.sy){
			mimx.sy = axy.y;
		}
		if(mimx.ex < axy.x){
			mimx.ex = axy.x;
		}
		if(mimx.ey < axy.y){
			mimx.ey = axy.y;
		}
	}
	return(mimx);
}
ObjFigu.prototype.fncObjFiguGetMiniMax = function(mimx)
{
	var pp, idx;
	var axy;
	var cx, cy;
	var sx, sy, ex, ey;
	var rag, tsin, tcos;
	var x, y, tx, ty;

	axy = this.fncObjFiguGetAtrXY(0);
	mimx.sx = axy.x;
	mimx.sy = axy.y;
	mimx.ex = axy.x;
	mimx.ey = axy.y;
	for(pp = 1; pp < this.pcnt; pp++){
		axy = this.fncObjFiguGetAtrXY(pp);
		if(axy.x < mimx.sx){
			mimx.sx = axy.x;
		}
		if(axy.y < mimx.sy){
			mimx.sy = axy.y;
		}
		if(mimx.ex < axy.x){
			mimx.ex = axy.x;
		}
		if(mimx.ey < axy.y){
			mimx.ey = axy.y;
		}
	}
	if(this.deg != 0){
		rag = this.deg * DEGTORAG;
		cx = (mimx.sx + mimx.ex) / 2.0;
		cy = (mimx.sy + mimx.ey) / 2.0;
		tsin = Math.sin(rag);
		tcos = Math.cos(rag);
		sx = mimx.sx; sy = mimx.sy;
		ex = mimx.ex; ey = mimx.ey;

		x = sx - cx; y = sy - cy;
		tx = x * tcos - y * tsin + cx;
		ty = x * tsin + y * tcos + cy;
		mimx.sx = tx;
		mimx.sy = ty;
		mimx.ex = tx;
		mimx.ey = ty;

		for(idx = 1; idx < 4; idx++){
			if(idx == 1){
				x = sx - cx; y = ey - cy;
			}else if(idx == 2){
				x = ex - cx; y = ey - cy;
			}else if(idx == 3){
				x = ex - cx; y = sy - cy;
			}
			tx = x * tcos - y * tsin + cx;
			ty = x * tsin + y * tcos + cy;
			if(tx < mimx.sx){
				mimx.sx = tx;
			}
			if(ty < mimx.sy){
				mimx.sy = ty;
			}
			if(mimx.ex < tx){
				mimx.ex = tx;
			}
			if(mimx.ey < ty){
				mimx.ey = ty;
			}
		}
	}
	return(mimx);
}
ObjFigu.prototype.fncCheckInUseArea = function(mimx, dx, dy)
{
	var xpos, ypos;
	if(this.kind == OBJIMAGE){
		if(mimx.sx < dx && dx < mimx.ex
		&& mimx.sy < dy && dy < mimx.ey){
			if(this.arySelectLine == null){
					return(true);
			}else{
				xpos = parseInt((dx - mimx.sx) / (mimx.ex - mimx.sx) * 100.0);
				ypos = parseInt((dy - mimx.sy) / (mimx.ey - mimx.sy) * 100.0);
				if(this.arySelectLine[ypos].substr(xpos, 1) == "o"){
					return(true);
				}
			}
		}
	}else{
		if(mimx.sx < dx && dx < mimx.ex
		&& mimx.sy < dy && dy < mimx.ey){
			return(true);
		}
	}
	return(false);
}
ObjFigu.prototype.fncTranslate = function(cx, cy, mx, my, tx, ty)
{
	switch(this.kind){
	case OBJGROUP:
		max = this.aryObjFigu.length;
		for(idx = 0; idx < max; idx++){
			this.aryObjFigu[idx].fncTransrate(cx, cy, mx, my, tx, ty);
		}
		this.fncObjFiguTranslateXY(0, cx, cy, mx, my, tx, ty);
		this.fncObjFiguTranslateXY(1, cx, cy, mx, my, tx, ty);
		break;
	case OBJPLGN:
		for(pp = 0; pp < this.pcnt; pp++){
			this.fncObjFiguTranslateXY(pp, cx, cy, mx, my, tx, ty);
		}
		break;
	case OBJIMAGE:
		for(pp = 0; pp < this.pcnt; pp++){
			this.fncObjFiguTranslateXY(pp, cx, cy, mx, my, tx, ty);
		}
		break;
	default:
		this.fncObjFiguTranslateXY(0, cx, cy, mx, my, tx, ty);
		this.fncObjFiguTranslateXY(1, cx, cy, mx, my, tx, ty);
		break;
	}
}
ObjFigu.prototype.fncResetArea = function(dmimx)
{
	var smimx;
	var ssubx, ssuby, dsubx, dsuby;
	var scx, scy, dcx, dcy;
	var timex, timey;
	var max, idx, pp;
	var axy;

	smimx = new StMiniMax;
	smimx = this.fncObjFiguGetMiniMax(smimx);
	scx = smimx.sx;
	scy = smimx.sy;
	dcx = dmimx.sx;
	dcy = dmimx.sy;
	ssubx = smimx.ex - smimx.sx;
	dsubx = dmimx.ex - dmimx.sx;
	ssuby = smimx.ey - smimx.sy;
	dsuby = dmimx.ey - dmimx.sy;

	if(dsubx == 0 && ssubx == 0){
		timex = 1.0;
	}else{
		timex = parseFloat(dsubx) / parseFloat(ssubx);
	}

	if(dsuby == 0 && ssuby == 0){
		timey = 1.0;
	}else{
		timey = parseFloat(dsuby) / parseFloat(ssuby);
	}

	switch(this.kind){
	case OBJGROUP:
		max = this.aryObjFigu.length;
		for(idx = 0; idx < max; idx++){
			smimx = this.aryObjFigu[idx].fncObjFiguGetMiniMax(smimx);
			smimx.sx = parseInt(parseFloat((smimx.sx - scx) * timex) + dcx);
			smimx.sy = parseInt(parseFloat((smimx.sy - scy) * timey) + dcy);
			smimx.ex = parseInt(parseFloat((smimx.ex - scx) * timex) + dcx);
			smimx.ey = parseInt(parseFloat((smimx.ey - scy) * timey) + dcy);
			this.aryObjFigu[idx].fncResetArea(smimx);
		}
		this.fncObjFiguSetAtrXY(0, 0, dmimx.sx, dmimx.sy);
		this.fncObjFiguSetAtrXY(1, 0, dmimx.ex, dmimx.ey);
		break;
	case OBJPLGN:
		for(pp = 0; pp < this.pcnt; pp++){
			axy = this.fncObjFiguGetAtrXY(pp);
			axy.x = parseInt(parseFloat((axy.x - scx) * timex) + dcx);
			axy.y = parseInt(parseFloat((axy.y - scy) * timey) + dcy);
			this.fncObjFiguSetAtrXY(pp, axy.atr, axy.x, axy.y);
		}
		break;
	case OBJIMAGE:
		for(pp = 0; pp < this.pcnt; pp++){
			axy = this.fncObjFiguGetAtrXY(pp);
			axy.x = parseInt(parseFloat((axy.x - scx) * timex) + dcx);
			axy.y = parseInt(parseFloat((axy.y - scy) * timey) + dcy);
			this.fncObjFiguSetAtrXY(pp, axy.atr, axy.x, axy.y);
		}
		break;
	default:
		this.fncObjFiguSetAtrXY(0, 0, dmimx.sx, dmimx.sy);
		this.fncObjFiguSetAtrXY(1, 0, dmimx.ex, dmimx.ey);
		break;
	}
}
ObjFigu.prototype.fncObjFiguTranslateXY = function(pp, cx, cy, mx, my, tx, ty)
{
	if(this.pcnt <= pp){
		return;
	}
	this.aryAtrXY[pp].x = (this.aryAtrXY[pp].x - cx) * tx + cx + mx;
	this.aryAtrXY[pp].y = (this.aryAtrXY[pp].y - cy) * ty + cy + my;
}
ObjFigu.prototype.fncObjFiguSetAtrXY = function(pp, atr, x, y)
{
	var axy;
	var sp, max;

	if(this.pcnt <= pp){
		if(this.aryAtrXY.length <= pp){
			max = pp+1;
			for(sp = this.aryAtrXY.length; sp < max; sp++){
				axy = new StAtrXY();
				axy.atr = -1; axy.x = -1; axy.y = -1;
				this.aryAtrXY.push(axy);
			}
		}else{
			axy = this.aryAtrXY[pp];
		}
		this.pcnt = pp + 1;
	}else{
		axy = this.aryAtrXY[pp];
	}		
	axy.atr = atr;
	axy.x = x;
	axy.y = y;
}
ObjFigu.prototype.fncObjFiguGetAtrXY = function(pp)
{
	var axy;

	if(this.pcnt <= pp){
		axy = new StAtrXY();
		axy.atr = -1;
		axy.x = -1;
		axy.y = -1;
		return(axy);
	}else{
		axy = this.aryAtrXY[pp];
		return(axy);
	}
}
