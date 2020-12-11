<?php

//$dbname = $_POST['dbnm'];
//$tblname = $_POST['tble'];
//$fildstr = $_POST['fild'];
//$typestr = $_POST['type'];
//$filename = $_POST['file'];
$dbname = "hotelsoa";
$tblname = "kubun";
$fildstr = "no,name";
$typestr = "20,48";
$filename = $dbname."/kubun.txt";
$filds = explode(",", $fildstr);
$types = explode(",", $typestr);

$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp); // コメントスキップ
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

$sql = "INSERT INTO ".$tblname." (";
$max = count($filds);
$sql = $sql.$filds[0];
for($idx = 1; $idx < $max; $idx++){
	$sql = $sql.",".$filds[$idx];
}
$sql = $sql.")";

$fp = fopen($filename, 'r');
echo "[".$fp."]\r\n";
// コメント行スキップ（BOM対応
fgets($fp);
// ２行目以降データはidカラムを含むため+1のカラムをセット
$count = 0;
while(TRUE){
	$datas = fgets($fp);
	if($datas == null || $datas == "")
	{
		break;
	}
	$ary = explode(",", $datas);
	if(count($ary) < $max){
		break;
	}
	if($count == 0){
		$sql = $sql." VALUES (";
	}else{
		$sql = $sql.",(";
	}
	if($types[0] == "0"){
		$sql = $sql.$ary[1];
	}else{
		$sql = $sql."'".$ary[1]."'";
	}
	for($idx = 1; $idx < $max; $idx++){
		if($types[idx] == "0"){
			$sql = $sql.",".$ary[$idx+1];
		}else{
			$sql = $sql.",'".$ary[$idx+1]."'";
		}
	}
	$sql = $sql.")";
	$count++;
}
fclose($fp);
$sql = $sql.";";
echo "[".$sql."]\r\n";
if($mysql = mysql_connect($server,$username,$password)){
	$ret = mysql_select_db($dbname, $mysql);
	if($ret == FALSE){
		echo "0,2";
		return;
	}
	$ret = mysql_query($sql, $mysql);
	if($ret == FALSE){
		echo "0,3";
		return;
	}
	$ret = mysql_close($mysql);
	if($ret == FALSE){
		echo "0,4";
		return;
	}
}else{
	echo "0,1";
	return;
}

$str = "1.0,".$dbname.",".$tblname.",".$count.",";
echo $str;

?>

