<?php

//$dbname = "bridal";
//$tblname = "bridaluser";
//$fildstr = "id,username,kyosiki,hirouen,kaijyou,mukotori,sinrozoku,sinroname1,sinroname2,sinpuzoku,sinpuname1,sinpuname2,sinrodish,sinpudish,sinrosub,sinpusub";
//$wherestr = "WHERE (id=7) LIMIT 1";

$dbname = $_POST['dbnm'];
$tblname = $_POST['tble'];
$fildstr = $_POST['fild'];
$wherestr = $_POST['where'];

$a = explode(",", $fildstr);

$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp);
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

$sql = "SELECT * FROM ".$tblname." ".$wherestr.";";
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$rows = mysql_query($sql, $mysql);
	mysql_close($mysql);
	if($rows == NULL){
		$retstr = "0,2,"."<".$sql.">";
		echo $retstr;
		return;
	}
	$retstr = "";
	$row = mysql_fetch_array($rows);
	if($row == NULL){
		$retstr = "0,3,"."<".$sql.">";
		echo $retstr;
		return;
	}
	$max = count($a);
	$retstr = $retstr.$row[$a[0]];
	for($i = 1; $i < $max; $i++){
		if($a[$i] == ""){
			$retstr = $retstr.",0";
		}else{
			$retstr = $retstr.",".$row[$a[$i]];
		}
	}
}else{
	$retstr = "0,1,".$dbname.",".$tblname.",";
	echo $retstr;
	return;
}
echo $retstr;
?> 
