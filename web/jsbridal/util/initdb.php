<?php

$dbname = $_POST['dbnm'];
//$dbname = "hotelsoa";
$filename = "../env/dbenv.txt";

$fp = fopen($filename, 'r');

fgets($fp); // コメントスキップ
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

if($mysql = mysql_connect($server,$username,$password)){
	$sql = "DROP DATABASE `".$dbname."`;";
	$ret = mysql_query($sql, $mysql);

	$sql = "CREATE DATABASE ".$dbname." CHARACTER SET utf8;";
	$ret = mysql_query($sql, $mysql);
	if($ret == FALSE){
		echo "0,2,".$dbname." を作成に失敗しました"."<BR>";
		return;
	}
	mysql_close($mysql);
}else{
	echo "0,1,接続エラー"."<BR>";
	return;
}

$str = "1,".$dbname.",";
echo $str;

?>

