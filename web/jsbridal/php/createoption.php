<?php

$argvs = $_POST['com'];
$argvs = mb_convert_encoding($argvs, 'SJIS', 'UTF-8');
$a = explode(",", $argvs);

$filename = "../env/dbenv.txt";
$fp = fopen($filename, 'r');
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $dbname = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $tblname = $ary[0];
fclose($fp);

$hotelno = intval($a[0]);
$locklogtblname = "locklog".sprintf("%04d",$hotelno);
$sql="CREATE TABLE `".$locklogtblname;
$sql=$sql."` (`id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY,";
$sql=$sql."`userno` INT NOT NULL,`kind` INT NOT NULL,`flag` INT NOT NULL,";
$sql=$sql."`who` INT NOT NULL,`date` DATETIME);";
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$mysql_result = mysql_query($sql, $mysql);
	$mysql_result = mysql_close($mysql);
}else{
	echo "<BR>connect fail<BR>";
	return;
}

// 料理マスターテーブルの作成
$dishtblname = "dhbs".sprintf("%04d",$hotelno);
$sql = "CREATE TABLE `".$dishtblname;
$sql = $sql."` (`id` INT AUTO_INCREMENT PRIMARY KEY,";
$sql = $sql."`key` VARCHAR(3),`name` VARCHAR(48),`price` INT NOT NULL);";
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$mysql_result = mysql_query($sql, $mysql);
	$mysql_result = mysql_close($mysql);
}else{
	echo "<BR>connect fail<BR>";
	return;
}
// 引出物マスターテーブルにデータをセット
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$fp = fopen('list/giftmaster.csv', 'r');
	if($fp == FALSE){
		$mysql_result = mysql_close($mysql);
		return;
	}
	$i = 0;
	while(1){
		if(feof($fp)){
			break;
		}
		$str = fgets($fp);
		if($str == ""){
			break;
		}
		$ar = split(',',$str);
		$sql = "INSERT INTO `".$gifttblname."` (`id`,`key`,`name`,`price`)";
		$sql = $sql." VALUES (NULL,'".$ar[0]."','".$ar[1]."','".$ar[2]."');";
		mb_convert_variables('UTF-8', 'sjis-win', $sql);
		$mysql_result = mysql_query($sql, $mysql);
	}
	fclose($fp);
	$mysql_result = mysql_close($mysql);
}

// 引出物マスターテーブルの作成
$gifttblname = "gfbs".sprintf("%04d",$hotelno);
$sql = "CREATE TABLE `".$dishtblname;
$sql = $sql."` (`id` INT AUTO_INCREMENT PRIMARY KEY,";
$sql = $sql."`key` VARCHAR(3),`name` VARCHAR(48),`price` INT NOT NULL);";
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$mysql_result = mysql_query($sql, $mysql);
	$mysql_result = mysql_close($mysql);
}else{
	echo "<BR>connect fail<BR>";
	return;
}
// 引出物マスターテーブルにデータをセット
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$fp = fopen('list/giftmaster.csv', 'r');
	if($fp == FALSE){
		$mysql_result = mysql_close($mysql);
		return;
	}
	$i = 0;
	while(1){
		if(feof($fp)){
			break;
		}
		$str = fgets($fp);
		if($str == ""){
			break;
		}
		$ar = split(',',$str);
		$sql = "INSERT INTO `".$gifttblname."` (`id`,`key`,`name`,`price`)";
		$sql = $sql." VALUES (NULL,'".$ar[0]."','".$ar[1]."','".$ar[2]."');";
		mb_convert_variables('UTF-8', 'sjis-win', $sql);
		$mysql_result = mysql_query($sql, $mysql);
	}
	fclose($fp);
	$mysql_result = mysql_close($mysql);
}

$str1 = "comp=".$dbname.",".$hotelno.",";
echo $str1;

?>

