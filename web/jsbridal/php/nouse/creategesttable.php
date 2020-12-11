<?php
$dbname = $_POST['dbnm'];
$userno = $_POST['uno'];
$ary = explode(",", $argvs);
$strID = $ary[0];

// フォルダーの作成
$path="list/".$gesttblname;
umask(0);
$ret=mkdir($path, 0777, true);

// サーバー名等格納ファイル作成
$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp);
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

// 招待者テーブル作成
$sql = "CREATE TABLE `".$gesttblname."` ";
$sql = $sql."(`id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY";
$sql = $sql.",`flag` INT NOT NULL,`tno` INT NOT NULL,`sno` INT NOT NULL";
$sql = $sql.",`name1` VARCHAR(24),`name2` VARCHAR(24),`yomi` VARCHAR(48)";
$sql = $sql.",`sama` VARCHAR(15),`kind` VARCHAR(30),`skind` VARCHAR(30)";
$sql = $sql.",`adrsno` VARCHAR(24),`adrs1` VARCHAR(90),`adrs2` VARCHAR(90)";
$sql = $sql.",`adrs3` VARCHAR(90),`telno` VARCHAR(24),`kt1` VARCHAR(60)";
$sql = $sql.",`kt2` VARCHAR(60),`kt3` VARCHAR(60),`kt4` VARCHAR(60)";
$sql = $sql.",`gno` INT NOT NULL,`sub1` VARCHAR(96),`sub2` VARCHAR(96)";
$sql = $sql.",`gift` VARCHAR(30),`dish` VARCHAR(30));";

if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$mysql_result = mysql_query($sql, $mysql);
	mysql_close($mysql);
}

$str1 = "comp=".$gesttblname;
echo $str1;
?>
