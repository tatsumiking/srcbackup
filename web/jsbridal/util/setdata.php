<?php

$dbname = $_POST['dbnm'];
$tblname = $_POST['tble'];
$fildstr = $_POST['fild'];
$filename = $_POST['file'];
$filds = explode(",", $fildstr);

$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp); // コメントスキップ
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

$sql = "INSERT INTO ".$tblname." (";
$max = $filds.length;
$sql = $sql.$filds[$idx];
for($idx = 1; $idx < $max; $idx++){
	$sql = $sql.",".$filds[$idx];
}
$fp = fopen($filename, 'r');
fgets($fp); // コメントスキップ
$count = 0;
while(TRUE){
	$datas = fgets($fp);
	$ary = explode(",", $datas);
	if($ary.length < $max){
		break;
	}
	if($count == 0){
		$sql = $sql.") VALUES (";
	}else{
		$sql = $sql.",(";
	}
	$sql = $sql.$ary[0];
	for($idx = 1; $idx < $max; $idx++){
		$sql = $sql.",".$ary[$idx];
	}
	$sql = $sql.")";
	$count++;
}
close($fp);
$sql = $sql.";";

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

