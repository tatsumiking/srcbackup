<?php

//$dbname = "bridal";
//$konreitable = "bridaluser";
//$recid = 7;
//$konreino = "012950";

$dbname = $_POST['dbnm'];
$konreitable = $_POST['krtbl'];
$recid = $_POST['recid'];
$konreino = $_POST['krno'];

$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp);
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $saveurl = $ary[0];
fclose($fp);

$sql = "SELECT * FROM ".$konreitable." WHERE (id=".$recid.");";
//echo $sql;
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$rows = mysql_query($sql, $mysql);
	while(1){
		$row = mysql_fetch_array($rows);
		if($row == NULL){
			break;
		}

		$gesttblname = $row[dbname];
		$str = $row[tablelayout].",";
		$str = $str.$row[kyosiki].",".$row[hirouen].",".$row[kaijyou].",";
		$str = $str.$row[mukotori].",".$row[sinrozoku].",".$row[sinroname1].",";
		$str = $str.$row[sinroname2].",".$row[sinpuzoku].",".$row[sinpuname1].",";
		$str = $str.$row[sinpuname2].",".$row[sinrodish].",".$row[sinpudish].",";
		$str = $str.$row[sinrosub].",".$row[sinpusub].",";
		$str = $str.$row[paperlocate].",".$row[papersize].",";
		$str = $str.$row[ryoukekind].",".$row[tablekind].",";
		$str = $str.$row[takasagokind].",".$row[nametype].",".$row[textsize].",,,\r\n";
		$lefttext = $row[lefttext];
		$righttext = $row[righttext];
		$tablelayout = $row[tableposition];
	}

	$filename = "../temp/download/ge".$konreino.".csv";
	$gesttable = "ge".$konreino;

	$fp = fopen($filename, 'w');
	//echo $str."<BR>";
	$str = mb_convert_encoding($str, 'sjis-win', 'UTF-8');
	fputs($fp, $str);

	$sql = "SELECT * FROM ".$gesttable.";";

	$rows = mysql_query($sql, $mysql);
	while(1){
		$row = mysql_fetch_array($rows);
		if($row == NULL){
			break;
		}

		$dish = substr($row[dish], 0, 1);
		$str = $row[name1].",".$row[name2];
		$str = $str.",".$row[yomi].",".$row[sama].",".$row[kind];
		$str = $str.",".$row[skind].",".$row[adrsno].",".$row[adrs1];
		$str = $str.",".$row[adrs2].",".$row[adrs3].",".$row[telno];
		$str = $str.",".$row[kt1].",".$row[kt2].",".$row[kt3].",".$row[kt4];
		$str = $str.",".$row[gno].",".$row[sub1];
		$str = $str.",".$row[gift].",".$dish;
		$str = $str.",".$row[tno].",".$row[sno].",,,\r\n";

		//echo $str."<BR>";
		$str = mb_convert_encoding($str, 'sjis-win', 'UTF-8');	
		fputs($fp, $str);
	}
	$mysql_result = mysql_close($mysql);

	$str = "tablelayout,start,,,,,,,,,,,,,,,,,,,,,,\r\n";
	//echo $str."<BR>";
	$str = mb_convert_encoding($str, 'sjis-win', 'UTF-8');	
	fputs($fp, $str);

	$arytbl = explode(":", $tablelayout);
	$tblmax = count($arytbl);
	for($idx = 0; $idx < $tblmax; $idx++){
		$xy = explode("x", $arytbl[$idx]);
		$str = $xy[0].",".$xy[1].",\r\n";
		//echo $str."<BR>";
		$str = mb_convert_encoding($str, 'sjis-win', 'UTF-8');	
		fputs($fp, $str);
	}
	$str = "tablelayout,end,\r\n";
	//echo $str."<BR>";
	$str = mb_convert_encoding($str, 'sjis-win', 'UTF-8');	
	fputs($fp, $str);

	$str = $leftstr.",".$rightstr.",\r\n";
	//echo $str."<BR>";
	$str = mb_convert_encoding($str, 'sjis-win', 'UTF-8');	
	fputs($fp, $str);

	fclose($fp);
}
$str = "1,".$saveurl.$gesttable.'.csv';
echo $str;
?>
