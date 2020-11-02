var m_dScrnTrnsTime = 1.0;
var m_dScrnRltvSX = 0.0;
var m_dScrnRltvSY = 0.0;
var m_dScrnBaseX = 0.0;
var m_dScrnBaseY = 0.0;

function fncScrnSetRltvTop(x, y)
{
	m_dScrnRltvSX = x;
	m_dScrnRltvSY = y;
}

function fncScrnSetBaseTop(x, y)
{
	m_dScrnBaseX = x;
	m_dScrnBaseY = y;
}
function fncScrnSetTrnsTime(time)
{
	m_dScrnTrnsTime = time;
}
function fncScrnTrnsPMX(pointx)
{
	var mmx;

	mmx = pointx * POINTTOMM * 10.0;
	return(mmx);
}
function fncScrnTrnsPMY(pointy)
{
	var mmy;

	mmy= pointy * POINTTOMM * 10.0;
	return(mmy);
}
function fncScrnTrnsPMLen(point)
{
	var mm;

	mm= point * POINTTOMM * 10.0;
	return(mm);
}
function fncScrnTrnsRMX(rltvx)
{
	var absx;
	var mmx;

	absx = (rltvx - m_dScrnRltvSX) / m_dScrnTrnsTime;
	mmx = absx - m_dScrnBaseX;
	return(mmx);
}
function fncScrnTrnsRMY(rltvy)
{
	var absy;
	var mmy;

	absy = (rltvy - m_dScrnRltvSY) / m_dScrnTrnsTime;
	mmy = absy - m_dScrnBaseY;
	return(mmy);
}
function fncScrnTrnsRMLen(rltv)
{
	var mm;

	mm = (rltv / m_dScrnTrnsTime);
	return(mm);
}
function fncScrnTrnsMRX(mmx)
{
	var absx;
	var rltvx;

	absx = mmx + m_dScrnBaseX;
	rltvx = (absx * m_dScrnTrnsTime) + m_dScrnRltvSX;
	return(rltvx);
}
function fncScrnTrnsMRY(mmy)
{
	var absx;
	var rltvy;

	absy = mmy + m_dScrnBaseY;
	rltvy = (absy * m_dScrnTrnsTime) + m_dScrnRltvSY;
	return(rltvy);
}
function fncScrnTrnsMRLen(mm)
{
	var rltv;

	rltv = (mm * m_dScrnTrnsTime);
	return(rltv);
}
